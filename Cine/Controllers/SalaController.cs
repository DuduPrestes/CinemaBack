using Cine.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Cine.Controllers
{
    [Authorize]
    public class SalaController : Controller
    {
        private CinemaContext _banco;

        public SalaController(CinemaContext context)
        {
            _banco = context;
        }

        [Route("Sala/BuscarTodas")]
        [HttpGet]
        public IActionResult BuscarTodas()
        {
            try
            {
                var salas = _banco.Sala.ToList();

                return Json(new { resultado = 1, salas });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = 0, mensagem = ex.Message });
            }
        }
    }
}
