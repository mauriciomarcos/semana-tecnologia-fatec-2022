using API.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Identity.ApiServices.Interfaces
{
    public interface IJwtGenerator
   {
        SignInManager<IdentityUser> SignInManager { get; set; }

        Task<LoginUserResponse> TokenGenerator(string email);        
    }
}