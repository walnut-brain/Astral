using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Disposables;

namespace Astral.Delivery
{
    public class Sponsor<TSposorId, TResource> : ISponsor<TSposorId, TResource>, IDisposable
    {
        public TSposorId SponsorId { get; }
        protected TimeSpan LeaseInterval { get; }
        protected TimeSpan RenewInterval { get; }
        protected CancellationDisposable Finisher { get; }

        public Sponsor(TSposorId sponsorId, TimeSpan leaseInterval, TimeSpan renewInterval)
        {
            SponsorId = sponsorId;
            LeaseInterval = leaseInterval;
            RenewInterval = renewInterval;
            Finisher = new CancellationDisposable();
        }

        protected class DelegatedLease : ILease
        {
            private readonly Func<Task> _renew;
            private readonly Func<Exception, Task> _free;

            public DelegatedLease(Func<Task> renew, Func<Exception, Task> free)
            {
                _renew = renew;
                _free = free;
            }

            public Task Renew() => _renew();

            public Task Free(Exception error = null) => _free(error);
            
        }
        
        protected virtual ILease CreateLease(TResource resource, ILeaseController<TSposorId, TResource> controller)
        {
            return new DelegatedLease(
                () => controller.RenewLease(SponsorId, resource, LeaseInterval), 
                ex => controller.FreeLease(SponsorId, resource, ex));
        }
        
        public Func<Task> Prepare(TResource resource, Func<CancellationToken, Task> work, ILeaseController<TSposorId, TResource> controller)
        {
            if(Finisher.IsDisposed) throw new ObjectDisposedException($"{nameof(Sponsor<TSposorId, TResource>)}");
            var lease = CreateLease(resource, controller);
            var workCancellation = new CancellationDisposable();
            var leaseCancellation = new CancellationDisposable();
            var combinedWorkSource = CancellationTokenSource.CreateLinkedTokenSource(workCancellation.Token, Finisher.Token);
            var combinedLeaseSource =
                CancellationTokenSource.CreateLinkedTokenSource(leaseCancellation.Token, Finisher.Token);

            IDisposable CreateFinisher(IDisposable cancellation, IDisposable combination)
                => Disposable.Create(() =>
                {
                    cancellation.Dispose();
                    combination.Dispose();
                });
            

            var workDisposable = CreateFinisher(workCancellation, combinedWorkSource);
            var leaseDisposable = CreateFinisher(leaseCancellation, combinedLeaseSource);

            // ReSharper disable once ImplicitlyCapturedClosure
            async Task LeaseTask(CancellationToken cancellation)
            {
                try
                {
                    while (true)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        await lease.Renew();
                        await Task.Delay(RenewInterval, cancellation);
                    }
                }
                catch 
                {
                    workCancellation.Dispose();
                }
                finally
                {
                    leaseDisposable.Dispose();                    
                }
            }
            
            // ReSharper disable once ImplicitlyCapturedClosure
            async Task ControlledWork(CancellationToken token)
            {
                try
                {
                    await work(token);
                    await lease.Free();
                    leaseCancellation.Dispose();
                }
                catch (Exception ex)
                {
                    await lease.Free(ex);
                    leaseCancellation.Dispose();
                    throw;
                }
                finally
                {
                    workDisposable.Dispose();
                }
            }

            Func<Task> Combo(CancellationToken leaseToken, CancellationToken workToken)
                => async () =>
                {
                    var leaseTask = LeaseTask(leaseToken);
                    var workTask = ControlledWork(workToken);
                    try
                    {
                        await Task.WhenAll(leaseTask, workTask);
                    }
                    catch 
                    {
                        if(workTask.Status == TaskStatus.RanToCompletion)
                            return;
                        throw;
                    }
                };
            
            return Combo(combinedLeaseSource.Token, combinedWorkSource.Token);
        }

        public void Dispose() => Finisher.Dispose();
    }
}