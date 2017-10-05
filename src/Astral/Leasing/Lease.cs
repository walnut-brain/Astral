using System;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public class Lease
    {
        private readonly Func<Task> _renew;
        private readonly Func<Exception, Task> _free;

        public Lease(Func<Task> renew, Func<Exception, Task> free)
        {
            _renew = renew;
            _free = free;
        }

        public Task Renew() => _renew();

        public Task Free(Exception error = null) => _free(error);
            
    }
}