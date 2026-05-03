namespace AirDreams.API.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(string id);
    }
}