using Cine.Context;
using Cine.Controllers;
using Cine.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace TestesUnitarios
{
    public class TesteFilme
    {
        FilmeController _controller;
        CinemaContext _banco; 

        public TesteFilme()
        {
            var contextOptions = new DbContextOptionsBuilder<CinemaContext>().
                UseSqlServer("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Cinema;Data Source=.\\SQLEXPRESS").Options;
            _banco = new CinemaContext(contextOptions);
            _controller = new FilmeController(_banco);
        }

        [Fact]
        public void BuscarTodos()
        {
            var retorno = _controller.BuscarTodos() as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }

        [Fact]
        public void Adicionar()
        {
            Filme filme = new Filme()
            {
                Titulo = "Teste3",
                Descricao = "Teste",
                Duracao = 100,
            };

            var retorno = _controller.Adicionar(filme, true) as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }

        [Fact]
        public void Editar()
        {
            Filme filme = new Filme()
            {
                Id = 12,
                Titulo = "Teste",
                Descricao = "Teste",
                Duracao = 100,
            };

            var retorno = _controller.Editar(filme, true) as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }

        [Fact]
        public void Excluir()
        {
            var retorno = _controller.Excluir(12, true) as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }
    }
}
