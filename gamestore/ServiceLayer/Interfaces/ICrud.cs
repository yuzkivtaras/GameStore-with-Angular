
namespace ServiceLayer.Interfaces
{
    public interface ICrud<T> where T : class
    {
        Task<IEnumerable<T>> GetAllModelsAsync();
    }
}
