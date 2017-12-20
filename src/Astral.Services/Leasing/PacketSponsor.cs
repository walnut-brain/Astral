using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public class PacketSponsor<TResource, TController> : Sponsor<TResource, TController>
        where TController : IPacketLeaseController<TResource>
    {
        private readonly ConcurrentDictionary<TResource, ValueTuple> _leases = 
            new ConcurrentDictionary<TResource, ValueTuple>();

        public PacketSponsor(TController controller, string sponsor, TimeSpan leaseInterval, TimeSpan renewInterval) :
            base(controller, sponsor, leaseInterval, renewInterval)
        {
            Task.Run(Loop);
        }

        protected override Lease CreateLease(TResource resource)
        {
            _leases.TryAdd(resource, default(ValueTuple));

            Func<Task> Renew(TResource res)
              => () => _leases.ContainsKey(res)
                  ? Task.CompletedTask
                  : Task.FromException(new OperationCanceledException("Lease cannot be taken"));

            Func<Exception, Task> Free(TResource res)
                => ex => Controller.FreeLease(SponsorName, res, ex); 
            
            return new Lease(Renew(resource), Free(resource));
        }

        private async Task Loop()
        {
            while (true)
            {
                Finisher.Token.ThrowIfCancellationRequested();
                var current = _leases.Keys;
                var taked = await Controller.RenewLeases(SponsorName, LeaseInterval);
                foreach (var resource in current.Except(taked))
                {
                    _leases.TryRemove(resource, out var _);
                }
                
                await Task.Delay(RenewInterval, Finisher.Token);
            }
        }
    }
}