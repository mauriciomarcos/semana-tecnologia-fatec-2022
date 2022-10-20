using API.Identity.ApiServices.Interfaces;
using API.Identity.Configs.Models;
using API.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Identity.ApiServices.Implementations
{
    public class JwtGeneratorProvider : IJwtGenerator
    {
        #region | CAMPOS PRIVADOS |
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        #endregion

        #region | PROPRIEDADES IMPLEMENTADAS INTEFACES |
        public SignInManager<IdentityUser> SignInManager { get; set; }
        #endregion

        #region | CONSTRUTOR |
        public JwtGeneratorProvider(UserManager<IdentityUser> userManager,                   
                   IOptions<AppSettings> appSettings,
                   SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            SignInManager = signInManager;

        }
        #endregion

        public async Task<LoginUserResponse> TokenGenerator(string email)
        {
            var user = await _userManager.FindByEmailAsync(email: email);
            var claims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await GetUserClaim(claims, user);
            var encodedToken = EncodingToken(identityClaims);

            return GetResponseToken(encodedToken, user, claims);
        }

        #region | MÉTODOS PRIVADOS - AUXILIARES |

        private async Task<ClaimsIdentity> GetUserClaim(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            // Adicionando Claims específicas para o JWT definidas na RFC (especificação do JWT) - não obrigatório
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            // Informação de quando o Token será expirado.
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.Now).ToString()));

            // informação de quando ele foi emitido.
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            /*
             * Adicionando as roles do usuário como se fossem Claims. Com isso essas Claims terão as mesmas
             * representatividades nesse contexto, serão tratadadas da mesma maneira aqui.
             */
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(type: "role", value: userRole));
            }

            // Adicionando as Claims dentro do Identity Claims.
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        private string EncodingToken(ClaimsIdentity identityClaims)
        {
            // Gerando o manipulador do Token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            // Gerando o token
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,

                // Dados do usuário que está solicitando login - regras do perfil (roles e claims)
                Subject = identityClaims,

                // Importante utilizar o DateTime.Utc.Now
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private LoginUserResponse GetResponseToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
        {
            return new LoginUserResponse
            {
                AccessToken = encodedToken,
                ExpireIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UserToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(claim => new ClaimUser { Type = claim.Type, Value = claim.Value })
                }
            };
        }

        // Método responsável por recuperar a data no forma Universal (UNIX - Offset)
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero)).TotalSeconds);

        #endregion
    }
}