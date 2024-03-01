namespace Music_Club.Repository
{
    public interface IRepository<T>
    {

        Task<List<T>> GetList();
        Task<T> GetObject(int? id);
        Task Create(T item);
        void Update(T item);
        Task Delete(int? id);
        Task Save();
    }
}
