namespace NewLibrary.Application.Commands.UserCommands
{
    public class UserRegisterCommand
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string Email { get; set; }
    }
}
