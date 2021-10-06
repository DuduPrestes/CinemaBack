using Cine.Context;
using Cine.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Cine.Controllers
{
    public class SessaoController : Controller
    {
        private CinemaContext _banco;

        public SessaoController(CinemaContext context)
        {
            _banco = context;
        }

        [Authorize]
        [Route("Sessao/BuscarTodas")]
        [HttpGet]
        public IActionResult BuscarTodas()
        {
            try
            {
                var sessoes = (from s in _banco.Sessao
                               join f in _banco.Filme on s.IdFilme equals f.Id
                               join sa in _banco.Sala on s.IdSala equals sa.Id
                               select new Sessao()
                               {
                                   Id = s.Id,
                                   DataHora = s.DataHora,
                                   Data = s.DataHora.ToShortDateString(),
                                   Hora = s.DataHora.ToShortTimeString(),
                                   HoraFim = s.DataHora.AddMinutes(f.Duracao).ToShortTimeString(),
                                   ValorIngresso = s.ValorIngresso,
                                   IdFilme = s.IdFilme,
                                   Filme = f.Titulo,
                                   IdSala = s.IdSala,
                                   Sala = sa.Nome,
                                   Animacao = s.Animacao,
                                   Audio = s.Audio
                               }).ToList();

                var salas = _banco.Sala.ToList();
                var filmes = _banco.Filme.ToList();

                return Json(new { resultado = 1, sessoes, filmes, salas });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }

        [Route("Sessao/AdicionarArquivos")]
        [HttpPost]
        public IActionResult AdicionarArquivos([FromForm] IFormFile animacao, IFormFile audio, int idSessao)
        {
            try
            {
                var sessao = _banco.Sessao.Find(idSessao);

                if (animacao != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        animacao.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        sessao.Animacao = fileBytes;
                    }
                }
                if (audio != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        audio.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        sessao.Audio = fileBytes;
                    }
                }
                _banco.SaveChanges();

                return Json(new { resultado = 1 });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    resultado = 0,
                    mensagem = ex.Message
                });
            }
        }

        [Authorize]
        [Route("Sessao/Adicionar")]
        [HttpPost]
        public IActionResult Adicionar([FromBody] Sessao sessao, bool teste = false)
        {
            try
            {
                using (var transaction = _banco.Database.BeginTransaction())
                {
                    integridadeSala(sessao);
                    _banco.Sessao.Add(sessao);
                    _banco.SaveChanges();

                    var Filme = _banco.Filme.Find(sessao.IdFilme);

                    sessao.Data = sessao.DataHora.ToShortDateString();
                    sessao.Hora = sessao.DataHora.ToShortTimeString();
                    sessao.HoraFim = sessao.DataHora.AddMinutes(Filme.Duracao).ToShortTimeString();
                    sessao.Filme = Filme.Titulo;
                    sessao.Sala = _banco.Sala.Find(sessao.IdSala).Nome;

                    if (!teste)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    transaction.Dispose();
                }

                return Json(new { resultado = 1, sessao });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }

        [Authorize]
        [Route("Sessao/Editar")]
        [HttpPut]
        public IActionResult Editar([FromBody] Sessao sessaoEditada, bool teste = false)
        {
            try
            {
                using (var transaction = _banco.Database.BeginTransaction())
                {
                    integridadeSala(sessaoEditada);
                    var sessao = _banco.Sessao.Find(sessaoEditada.Id);
                    sessao.DataHora = sessaoEditada.DataHora;
                    sessao.ValorIngresso = sessaoEditada.ValorIngresso;
                    sessao.IdFilme = sessaoEditada.IdFilme;
                    sessao.IdSala = sessaoEditada.IdSala;
                    _banco.SaveChanges();

                    var Filme = _banco.Filme.Find(sessao.IdFilme);

                    sessao.Data = sessao.DataHora.ToShortDateString();
                    sessao.Hora = sessao.DataHora.ToShortTimeString();
                    sessao.HoraFim = sessao.DataHora.AddMinutes(Filme.Duracao).ToShortTimeString();
                    sessao.Filme = Filme.Titulo;
                    sessao.Sala = _banco.Sala.Find(sessao.IdSala).Nome;

                    if (!teste)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    transaction.Dispose();

                    return Json(new { resultado = 1, sessao });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }

        }

        [Authorize]
        [Route("Sessao/Excluir")]
        [HttpGet]
        public IActionResult Excluir(int id, bool teste = false)
        {
            try
            {
                using (var transaction = _banco.Database.BeginTransaction())
                {
                    var sessao = _banco.Sessao.Find(id);
                    integridadeSalaExclusao(sessao);
                    _banco.Remove(sessao);
                    _banco.SaveChanges();

                    if (!teste)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    return Json(new { resultado = 1 });

                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }

        public void integridadeSala(Sessao sessao)
        {
            var dataHoraInicio = sessao.DataHora;
            var dataHoraFim = sessao.DataHora.AddMinutes(_banco.Filme.Find(sessao.IdFilme).Duracao);

            var sessoes = (from s in _banco.Sessao
                           join f in _banco.Filme on s.IdFilme equals f.Id
                           where s.Id != sessao.Id
                           select new
                           {
                               DataHoraInicio = s.DataHora,
                               DataHoraFim = s.DataHora.AddMinutes(f.Duracao),
                           }).ToList();

            if (sessoes.Where(c => c.DataHoraInicio <= dataHoraFim && c.DataHoraFim >= dataHoraInicio).Any())
                throw new Exception("Sala selecionada está ocupada neste horário!");

        }

        public void integridadeSalaExclusao(Sessao sessao)
        {
            if (DateTime.Today.AddDays(10) > sessao.DataHora.Date)
            {
                throw new Exception("Sessão só pode ser excluída com 10 dias ou mais de antecedência!");
            }
        }
    }
}
