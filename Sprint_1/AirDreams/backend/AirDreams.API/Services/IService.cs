namespace AirDreams.API.Services.Interfaces
{
    public interface IService<T> where T : class
    {
        List<T> GetAll();
        T GetById(string id);
    }
}