using System.ComponentModel.DataAnnotations;

namespace API.Identity.Models.ViewModels
{
    public class NewUserViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O campo {0} possui um formato incorreto de e-mail!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(10, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres!", MinimumLength = 6)]
        public string Senha { get; set; }

        [Compare("Senha", ErrorMessage = "As senhas não conferem!")]
        public string SenhaConfirmacao { get; set; }
    }
}