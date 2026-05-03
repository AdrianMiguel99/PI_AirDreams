namespace AirDreams.API.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public RegisterUserModel Register(RegisterModel model)
        {
            var user = new RegisterUserModel
            {
                NombreCompleto = model.NombreCompleto,
                TipoUsuario = model.TipoUsuario,
                Correo = model.Correo,
                Cedula = model.Cedula,
                emailConfirmed = false
            };

            var CreatedUserId = _userRepository.Create(user);
            return user;
        }

        public RegisterUserModel GetUserByEmail(string email)
        {
            return _userRepository.GetByEmail(email);
        }
    }
}