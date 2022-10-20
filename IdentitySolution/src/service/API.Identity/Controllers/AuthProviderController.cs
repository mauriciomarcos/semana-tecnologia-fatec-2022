using API.Identity.ApiServices.Interfaces;
using API.Identity.Configs.Models;
using API.Identity.Models;
using API.Identity.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace API.Identity.Controllers
{
    [ApiController]
    [Route("api/auth-provider")]
    public class AuthProviderController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtGenerator _jwtGenerator;

        public AuthProviderController(SignInManager<IdentityUser> signInManager, 
                                      UserManager<IdentityUser> userManager,
                                      IJwtGenerator jwtGenerator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
        }

        [HttpPost("new-user")]
        [SwaggerOperation(Description = "Endpoint responsável por prover o cadastramento de um novo usuário na aplicação.", Tags = new[] { SwaggerTags.AuthProvider })]
        [SwaggerResponse(201, "Criação de um novo usuário", Type = typeof(LoginUserResponse))]
        [SwaggerResponse(400, "Error: response status is Bad Request", typeof(string))]
        [SwaggerResponse(500, "Error: response status is Internal Server Error", typeof(string))]
        public async Task<IActionResult> NewUser([FromBody] NewUserViewModel newUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUser { UserName = newUser.Email, Email = newUser.Email, EmailConfirmed = true };

            var result = await _userManager.CreateAsync(user, newUser.Senha);
            if (!result.Succeeded)            
                return BadRequest(string.Join("\n", result.Errors.Select(erro => erro.Description)));
            
            var token = _jwtGenerator.TokenGenerator(newUser.Email);
            return Created(string.Empty ,token);
        }

        [HttpPost("authentication")]
        [SwaggerOperation(Description = "Endpoint responsável por gerar um Acess Token através das credencias de um usuário previamente cadastrado", Tags = new[] { SwaggerTags.AuthProvider })]
        [SwaggerResponse(200, "Login do usuário e geração do Access Token realizados com sucesso", Type = typeof(LoginUserResponse))]
        [SwaggerResponse(400, "Error: response status is Bad Request", typeof(string))]
        [SwaggerResponse(423, "Error: locked", typeof(string))]
        [SwaggerResponse(500, "Error: response status is Internal Server Error", typeof(string))]
        public async Task<ActionResult> Login([FromBody] UserAuthViewModel newUser)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = await _jwtGenerator.SignInManager.PasswordSignInAsync(newUser.Email, newUser.Senha, false, true);

            if (!result.Succeeded)
                return BadRequest("Usuário ou Senha incorretos.");
           
            if (result.IsLockedOut)
                return StatusCode((int)HttpStatusCode.Locked, "Usuário temporariamente bloqueado por tentativas inválidas.");

            return Ok(_jwtGenerator.TokenGenerator(newUser.Email));
        }
    }
} 