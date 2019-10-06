using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CartApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly IConfiguration _conf;
		public TokenController(IConfiguration configuration)
		{
			_conf = configuration;
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Login([FromBody]LoginRequest request)
		{
			if (ModelState.IsValid)
			{
				//Normal şartlarda, requestten gelen kullanıcı ve şifre ile user servisinden kullanıcının sorgulanması gerekir.
				//Ancak burada dummy olarak geçiyorum.
				var user = "dummyuser";
				if (user == null)
				{
					return Unauthorized();
				}

				var claims = new[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, request.Username),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				};

				var token = new JwtSecurityToken
				(
					issuer: _conf["Issuer"],
					audience: _conf["Audience"],
					claims: claims,
					expires: DateTime.UtcNow.AddDays(30),
					notBefore: DateTime.UtcNow,
					signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["SigningKey"])),
							SecurityAlgorithms.HmacSha256)
				);
				return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
			}
			return BadRequest();
		}


		public class LoginRequest
		{
			public string Username { get; set; }
			public string Password { get; set; }

		}
	}
}