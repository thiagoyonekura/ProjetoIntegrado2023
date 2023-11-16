using System.ComponentModel.DataAnnotations;

namespace api.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Informe o e-Mail para realizar o login!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Informe a senha para realizar o login!")]
        public string Senha { get; set; }
    }
}
