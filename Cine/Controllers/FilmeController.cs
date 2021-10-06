using Cine.Context;
using Cine.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Cine.Controllers
{
    public class FilmeController : Controller
    {
        private CinemaContext _banco;

        public FilmeController(CinemaContext context)
        {
            _banco = context;
        }

        [Authorize]
        [Route("Filme/BuscarTodos")]
        [HttpGet]
        public IActionResult BuscarTodos()
        {
            try
            {
                var filmes = _banco.Filme.ToList();

                return Json(new { resultado = 1, filmes });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }

        [Route("Filme/AdicionarImagem")]
        [HttpPost]
        public IActionResult AdicionarImagem([FromForm] IFormFile arquivo, int idFilme)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    arquivo.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var filme = _banco.Filme.Find(idFilme);
                    filme.Imagem = fileBytes;
                    _banco.SaveChanges();
                }

                return Json(new { resultado = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }

        [Authorize]
        [Route("Filme/Adicionar")]
        [HttpPost]
        public IActionResult Adicionar([FromBody] Filme filme, bool teste = false)
        {
            try
            {
                using (var transaction = _banco.Database.BeginTransaction())
                {
                    integridadeFilme(filme);
                    _banco.Filme.Add(filme);
                    _banco.SaveChanges();

                    if (!teste)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    transaction.Dispose();

                    return Json(new { resultado = 1, filme });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }

        [Authorize]
        [Route("Filme/Editar")]
        [HttpPost]
        public IActionResult Editar([FromBody] Filme filmeEditado, bool teste= false)
        {
            try
            {
                using (var transaction = _banco.Database.BeginTransaction())
                {
                    integridadeFilme(filmeEditado);
                    var filme = _banco.Filme.Find(filmeEditado.Id);
                    filme.Titulo = filmeEditado.Titulo;
                    filme.Descricao = filmeEditado.Descricao;
                    filme.Duracao = filmeEditado.Duracao;
                    _banco.SaveChanges();

                    if (!teste)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    transaction.Dispose();

                    return Json(new { resultado = 1, filme });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }

        }

        [Authorize]
        [Route("Filme/Excluir")]
        [HttpGet]
        public IActionResult Excluir(int id, bool teste = false)
        {
            try
            {
                using (var transaction = _banco.Database.BeginTransaction())
                {
                    integridadeFilmeExclusao(id);
                    var filme = _banco.Filme.Find(id);
                    _banco.Remove(filme);
                    _banco.SaveChanges();

                    if (!teste)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    transaction.Dispose();

                    return Json(new { resultado = 1 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }

        public void integridadeFilme(Filme filme)
        {
            if (_banco.Filme.Where(c => c.Titulo == filme.Titulo && c.Id != filme.Id).Any())
                throw new Exception("Este título já está cadastrado!");
        }
        public void integridadeFilmeExclusao(int idFilme)
        {
            if (_banco.Sessao.Where(c => c.IdFilme == idFilme).Any())
                throw new Exception("Filme não pode ser excluído por existir sessões vinculadas a ele!");
        }
    }
}
