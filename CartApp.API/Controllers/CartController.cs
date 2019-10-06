using System;
using System.Collections.Generic;
using CartApp.Core.ApplicationService;
using CartApp.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		public CartController(ICartService cartService)
		{
			_cartService = cartService;
		}

		[HttpGet]
		public ActionResult<IEnumerable<Core.Entity.Cart>> Get([FromQuery] Filter filter)
		{
			try
			{
				return Ok(_cartService.GetFilteredCarts(filter));
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}

		[HttpGet("{id}")]
		public ActionResult<Core.Entity.Cart> Get(int id)
		{
			if (id < 1) return BadRequest("Id 0'dan büyük olmalıdır!");

			return Ok(_cartService.FindCartById(id));
		}

		[HttpPost]
		public ActionResult<Core.Entity.Cart> Post([FromBody] Core.Entity.Cart order)
		{
			try
			{
				return Ok(_cartService.CreateCart(order));
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}

		}

		[HttpPut("{id}")]
		public ActionResult<Core.Entity.Cart> Put(int id, [FromBody] Core.Entity.Cart cart)
		{
			if (id < 1 || id != cart.Id)
			{
				return BadRequest("Parametre Id ile sepet Id aynı olmalıdır!");
			}

			try
			{
				return Ok(_cartService.UpdateCart(cart));
			}
			catch (Exception e)
			{
				return StatusCode(403, e.Message);
			}

		}

		[HttpDelete("{id}")]
		public ActionResult<Core.Entity.Cart> Delete(int id)
		{
			return Ok($"{id}'li sepet silindi.");
		}
	}
}