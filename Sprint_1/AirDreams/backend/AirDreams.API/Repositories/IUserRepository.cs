namespace AirDreams.API.Repositories
{
    public interface IUserRepository
    {
        int Create(RegisterUserModel user);
        RegisterUserModel GetByEmail(string email); 
    }
}