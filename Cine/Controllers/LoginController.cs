using Cine.Context;
using Cine.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Cine.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private CinemaContext _banco;

        public LoginController(IConfiguration configuration, CinemaContext context)
        {
            _configuration = configuration;
            _banco = context;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] Usuario usuario)
        {
            try
            {
                string strToken;
                Usuario usuarioLogado = null;

                var usuarioBanco = _banco.Usuario.Where(u => u.Nome == usuario.Nome).SingleOrDefault();

                if (usuarioBanco != null)
                {
                    var senha = GerarHashMd5(usuario.Senha);

                    if (usuarioBanco.Senha == senha)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, "Eduardo")
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));

                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: "Eduardo",
                            audience: "Eduardo",
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(30),
                            signingCredentials: creds);


                        strToken = new JwtSecurityTokenHandler().WriteToken(token);
                        usuarioLogado = new Usuario()
                        {
                            Id = 1,
                            Nome = "Admin",
                            Nivel = 1,
                            Senha = "",
                            Token = strToken
                        };

                        return Json(new { resultado = 1, usuarioLogado });
                    }
                }

                return Json(new { resultado = 0, mensagem = "Credenciais inválidas" });

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
        [HttpGet]
        [Route("ValidarToken")]
        public IActionResult ValidarToken()
        {
            return Json(1);
        }

        public static string GerarHashMd5(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
