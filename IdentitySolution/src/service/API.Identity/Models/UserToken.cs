namespace API.Identity.Models
{
    public class UserToken
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public IEnumerable<ClaimUser> Claims { get; set; }
    }
}