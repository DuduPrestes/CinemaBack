using System.ComponentModel.DataAnnotations.Schema;

namespace Cine.ViewModels
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public int Nivel { get; set; }
        [NotMapped]
        public string Token { get; set; }
    }
}
