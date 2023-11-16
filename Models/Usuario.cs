using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace api.Models
{
    public class Usuario
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome para realizar o cadastro!")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o CPF para realizar o cadastro!")]
        [StringLength(11, ErrorMessage = "O CPF deve ter 11 caracteres.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Informe a data de nascimento para realizar o cadastro")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Informe o e-Mail para realizar o cadastro!")]
        [EmailAddress(ErrorMessage = "O formato do e-mail está inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o telefone para realizar o cadastro!")]
        [Phone(ErrorMessage = "O formato do telefone está inválido.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Informe a senha para realizar o cadastro!")]
        public string Senha { get; set; }

        public Usuario()
        {
        }

    }
}