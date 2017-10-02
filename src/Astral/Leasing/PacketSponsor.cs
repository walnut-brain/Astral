using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Astral.Delivery
{
    public class PacketSponsor<TSposorId, TResource, TController, TControllerKey> : Sponsor<TSposorId, TResource>
        where TController : IPacketLeaseController<TSposorId, TResource>
    {
        private readonly Func<TController, TControllerKey> _keyExtractor;

        private readonly ConcurrentDictionary<TControllerKey, ConcurrentDictionary<TResource, ValueTuple>> _leases = 
            new ConcurrentDictionary<TControllerKey, ConcurrentDictionary<TResource, ValueTuple>>();
        
        public PacketSponsor(TSposorId sponsorId, TimeSpan leaseInterval, TimeSpan renewInterval, Func<TController, TControllerKey> keyExtractor) : base(sponsorId, leaseInterval, renewInterval)
        {
            _keyExtractor = keyExtractor;
        }

        protected override ILease CreateLease(TResource resource, ILeaseController<TSposorId, TResource> controller)
        {
            if (!(controller is TController plc))
                return base.CreateLease(resource, controller);

            ConcurrentDictionary<TResource, ValueTuple> created = null;
            var controllerKey = _keyExtractor(plc);
            var current = _leases.GetOrAdd(controllerKey, _ =>
            {
                created = new ConcurrentDictionary<TResource, ValueTuple>();
                return created;
            });

            if (current == created)
                Task.Run(() => Loop(plc, current));
            current.TryAdd(resource, default(ValueTuple));

            Func<Task> Renew(ConcurrentDictionary<TResource, ValueTuple> leases, TResource res)
              => () => leases.ContainsKey(res)
                  ? Task.CompletedTask
                  : Task.FromException(new OperationCanceledException("Lease cannot be taken"));

            Func<Exception, Task> Free(TController ctrl, TResource res)
                => ex => ctrl.FreeLease(SponsorId, res, ex); 
            
            return new DelegatedLease(Renew(current, resource), Free(plc, resource));
        }

        private async Task Loop(TController controller, ConcurrentDictionary<TResource, ValueTuple> leases)
        {
            while (true)
            {
                Finisher.Token.ThrowIfCancellationRequested();
                var current = leases.Keys;
                var taked = await controller.RenewLeases(SponsorId, LeaseInterval);
                foreach (var resource in current.Except(taked))
                {
                    leases.TryRemove(resource, out var _);
                }
                
                await Task.Delay(RenewInterval, Finisher.Token);
            }
        }
    }
}