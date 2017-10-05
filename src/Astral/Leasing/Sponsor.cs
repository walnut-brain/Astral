using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Disposables;

namespace Astral.Leasing
{
    public class Sponsor<TResource, TController> : ISponsor<TResource>, IDisposable 
        where TController : ILeaseController<TResource>
    {
        protected TController Controller { get; }
        public string SponsorName { get; }
        protected TimeSpan LeaseInterval { get; }
        protected TimeSpan RenewInterval { get; }
        protected CancellationDisposable Finisher { get; }

        public Sponsor(TController controller, string sponsor, TimeSpan leaseInterval, TimeSpan renewInterval)
        {
            Controller = controller;
            SponsorName = sponsor;
            LeaseInterval = leaseInterval;
            RenewInterval = renewInterval;
            Finisher = new CancellationDisposable();
        }

        protected virtual Lease CreateLease(TResource resource)
        {
            return new Lease(
                () => Controller.RenewLease(SponsorName, resource, LeaseInterval), 
                ex => Controller.FreeLease(SponsorName, resource, ex));
        }
        
        public Func<Task> Prepare(TResource resource, Func<CancellationToken, Task> work)
        {
            if(Finisher.IsDisposed) throw new ObjectDisposedException($"{nameof(Sponsor<TResource, TController>)}");
            var lease = CreateLease(resource);
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