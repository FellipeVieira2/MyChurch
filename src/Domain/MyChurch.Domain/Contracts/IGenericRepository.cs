namespace MyChurch.Domain.Contracts
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<TEntity> ById(Guid id);
        Task<IEnumerable<TEntity>> List();
        IQueryable<TEntity> Query();
    }
}
