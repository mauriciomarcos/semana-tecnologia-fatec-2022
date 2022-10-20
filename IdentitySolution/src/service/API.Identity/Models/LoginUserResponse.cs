namespace API.Identity.Models
{
    public class LoginUserResponse
    {
        public string AccessToken { get; set; }
        public double ExpireIn { get; set; }
        public UserToken UsuarioToken { get; set; }
    }
}