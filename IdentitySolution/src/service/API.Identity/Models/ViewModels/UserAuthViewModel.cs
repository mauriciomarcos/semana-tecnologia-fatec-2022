using System.ComponentModel.DataAnnotations;

namespace API.Identity.Models.ViewModels
{
    public class UserAuthViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O campo {0} possui um formato incorreto de e-mail!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        public string Senha { get; set; }
    }
}