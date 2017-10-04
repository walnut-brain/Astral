using System;
using System.Threading.Tasks;

namespace Astral.Delivery
{
    public interface ILease
    {
        Task Renew();
        Task Free(Exception error = null);
    }
}