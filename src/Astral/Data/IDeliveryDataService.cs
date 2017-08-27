using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;

namespace Astral.Data
{
    public interface IWhereBuilder<T, TKey>
    {
        IWhereBuilder<T, TKey> Where(Expression<Func<T, bool>> predicate);
        Task<int> Delete();
        IUpdateBuilder<T, TKey> Set<TValue>(Expression<Func<T, TValue>> prop, TValue value);
        IUpdateBuilder<T, TKey> Set<TValue>(Expression<Func<T, TValue>> prop, Expression<Func<T, TValue>> getter);

    }

    public interface IUpdateBuilder<T, TKey>
    {
        IUpdateBuilder<T, TKey> Where(Expression<Func<T, bool>> predicate);
        IUpdateBuilder<T, TKey> Set<TValue>(Expression<Func<T, TValue>> prop, TValue value);
        IUpdateBuilder<T, TKey> Set<TValue>(Expression<Func<T, TValue>> prop, Expression<Func<T, TValue>> getter);
        Task<int> Update();
        Task<IEnumerable<TKey>> UpdatedKeys();
        Task<IEnumerable<T>> Updated();
    }

    public interface IDeliveryDataService<T> : IDataService<T>, IWhereBuilder<DeliveryRecord, Guid>
        where T : IStore<T>
    {
        Task<DeliveryRecord> Insert(DeliveryRecord insert);

        

        /*
        void DeleteAll(string serviceName, string endpointName, string messageKey);

        void Create(DeliveryRecord delivery);


        /// <summary>
        ///     Update conrete lease
        ///     UPDATE DeliveryRecords SET Leased = @leasedTo, Sponsor = @sponsor WHERE DeliveryId = @deliveryId AND (Sponsor =
        ///     @sponsor OR (Sponsor is null))
        ///     must update single record  - then true
        /// </summary>
        /// <param name="sponsor">sponsor</param>
        /// <param name="deliveryId">id</param>
        /// <param name="leasedTo">leased to</param>
        /// <returns>success</returns>
        Task<bool> UpdateLease(string sponsor, Guid deliveryId, DateTimeOffset leasedTo);


        void SetDelivered(Guid deliveryId, DateTimeOffset archiveTo);
        void Delete(Guid deliveryId);


        IEnumerable<DeliveryRecord> GetAwaitedDelivery(string serviceName, string endpointName, string sponsorName,
            DateTimeOffset leaseTo, uint? maxCount);

        void CleanLateDeliveries();

        /// <summary>
        ///     Remove all leases by sponsor:
        ///     UPDATE DeliveryRecords SET Leased = DateTime.Now, Sponsor = NULL WHERE Sponsor = @sponsor
        /// </summary>
        /// <param name="sponsor">sponsor</param>
        Task RemoveLeases(string sponsor);

        /// <summary>
        ///     Remove concrete lease
        ///     UPDATE DeliveryRecords SET Leased = DateTime.Now, Sponsor = NULL WHERE Sponsor = @sponsor AND DeliveryId =
        ///     @deliveryId
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <param name="sponsorId"></param>
        Task RemoveLease(Guid deliveryId, string sponsorId);

        /// <summary>
        ///     Renew all sponsor leases
        ///     UPDATE DeliveryRecords SET Leased = DateTime.Now + @leasePeriod WHERE Sponsor = @sponsor
        ///     SELECT DeliveryId FROM DeliveryRecords WHERE Sponsor = @sponsor
        /// </summary>
        /// <param name="sponsor">sponsor</param>
        /// <param name="leasePeriod">leasePeriod</param>
        /// <returns>leased deliveries</returns>
        Task<IEnumerable<Guid>> RenewLeases(string sponsor, TimeSpan leasePeriod);

        /// <summary>
        /// Set delivery exception
        /// UPDATE DeliveryRecords SET LastError = @exception WHERE DeliveryId = @deliveryId
        /// </summary>
        /// <param name="recordDeliveryId"></param>
        /// <param name="objException"></param>
        void SetException(Guid recordDeliveryId, Exception objException);

        /// <summary>
        /// Mark delivery as delivered and TTL to archiveTo
        /// UPDATE DeliveryRecords SET Delivered = true, Ttl = archiveTo WHERE DeliveryId = @deliveryId
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <param name="archiveTo"></param>
        void Archive(Guid deliveryId, DateTimeOffset archiveTo);
        */
    }
}