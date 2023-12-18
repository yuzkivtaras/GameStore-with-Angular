namespace DataAccessLayer.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
    }
}
