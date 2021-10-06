namespace Cine.ViewModels
{
    public class Filme
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; } 
        public int Duracao { get; set; }
        public byte[] Imagem { get; set; }
    }
}
