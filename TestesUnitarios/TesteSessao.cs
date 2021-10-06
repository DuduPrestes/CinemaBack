using Cine.Context;
using Cine.Controllers;
using Cine.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace TestesUnitarios
{
    public class TesteSessao
    {
        SessaoController _controller;
        CinemaContext _banco;

        public TesteSessao()
        {
            var contextOptions = new DbContextOptionsBuilder<CinemaContext>().
                UseSqlServer("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Cinema;Data Source=.\\SQLEXPRESS").Options;
            _banco = new CinemaContext(contextOptions);
            _controller = new SessaoController(_banco);
        }

        [Fact]
        public void BuscarTodos()
        {
            var retorno = _controller.BuscarTodas() as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }

        [Fact]
        public void Adicionar()
        {
            Sessao sessao = new Sessao()
            {
                DataHora = DateTime.Today,
                ValorIngresso = 50,
                IdFilme = 12,
                IdSala = 3
            };

            var retorno = _controller.Adicionar(sessao, true) as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }

        [Fact]
        public void Editar()
        {
            Sessao sessao = new Sessao()
            {
                Id= 3,
                DataHora = DateTime.Today,
                ValorIngresso = 50,
                IdFilme = 12,
                IdSala = 3
            };

            var retorno = _controller.Editar(sessao, true) as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }

        [Fact]
        public void Excluir()
        {
            var retorno = _controller.Excluir(6, true) as JsonResult;
            Assert.Equal(1, retorno.Value.GetType().GetProperty("resultado").GetValue(retorno.Value));
        }
    }
}
