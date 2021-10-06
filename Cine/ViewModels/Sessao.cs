using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine.ViewModels
{
    public class Sessao
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        [NotMapped]
        public string Data { get; set; }
        [NotMapped]
        public string Hora { get; set; }
        [NotMapped]
        public string HoraFim { get; set; }
        public decimal ValorIngresso { get; set; }
        public int IdFilme { get; set; }
        [NotMapped]
        public string Filme { get; set; }
        public int IdSala { get; set; }
        [NotMapped]
        public string Sala { get; set; }
        public byte[] Animacao { get; set; }
        public byte[] Audio { get; set; }
    }
}
