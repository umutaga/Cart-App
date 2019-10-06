using System;
using System.Collections.Generic;
using System.Linq;
using CartApp.Core.DomainService;
using CartApp.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace CartApp.Infrastructure.Data.Repositories
{
	public class CartRepository : ICartRepository
	{
		readonly CartAppContext _ctx;

		public CartRepository(CartAppContext ctx)
		{
			_ctx = ctx;
		}

		public Cart Create(Core.Entity.Cart cart)
		{
			int noStockProduct = IsStockAvailable(cart.CartLines);
			if (noStockProduct > 0)
				throw new Exception($"{noStockProduct} idli ürün stokta yok!");

			_ctx.Attach(cart).State = EntityState.Added;
			_ctx.SaveChanges();
			return cart;
		}

		public Cart ReadById(int id)
		{
			var ab = _ctx.Cart.Include(o => o.Customer)
			   .Include(o => o.CartLines)
			   .FirstOrDefault(o => o.Id == id);
			return ab;
		}

		public FilteredList<Core.Entity.Cart> ReadAll(Filter filter)
		{
			if (filter == null)
			{
				return new FilteredList<Core.Entity.Cart>() { List = _ctx.Cart.ToList(), Count = _ctx.Cart.Count() };
			}

			var items = _ctx.Cart.Include(o => o.CartLines)
				.Include(o => o.Customer)
				.Skip((filter.CurrentPage - 1) * filter.ItemsPerPage)
				.Take(filter.ItemsPerPage)
				.ToList();
			return new FilteredList<Core.Entity.Cart>() { List = items, Count = Count() };

		}

		public int Count()
		{
			return _ctx.Cart.Count();
		}

		public Cart Update(Core.Entity.Cart cartUpdate)
		{
			int noStockProduct = IsStockAvailable(cartUpdate.CartLines);
			if (noStockProduct > 0)
				throw new Exception($"{noStockProduct} idli ürün stokta yok!");

			var newCartLines = new List<CartLine>(cartUpdate.CartLines);
			_ctx.Attach(cartUpdate).State = EntityState.Modified;
			_ctx.CartLines.RemoveRange(
				_ctx.CartLines.Where(ol => ol.CartId == cartUpdate.Id)
			);
			foreach (var ol in newCartLines)
			{
				_ctx.Entry(ol).State = EntityState.Added;
			}
			_ctx.Entry(cartUpdate).Reference(o => o.Customer).IsModified = true;
			_ctx.SaveChanges();
			return cartUpdate;
		}

		public Cart Delete(int id)
		{
			var removed = _ctx.Remove(new Cart { Id = id }).Entity;
			_ctx.SaveChanges();
			return removed;
		}


		public int IsStockAvailable(List<CartLine> cartLine)
		{
			foreach (var product in cartLine)
			{
				int productId = product.ProductId;
				//Burada DB'den stok sorgulaması yapılır.Ancak test amaçlı random olarak üretiliyor.
				//Eklenen ürünlerden herhangi biri stokta yok ise ürün Id'si geri döndürülür.
				Random rnd = new Random();
				bool isInStock = rnd.Next(100) <= 5 ? false : true; //Sonuç random olarak %5 ihtimalle stokta yok gelecek.
				if (!isInStock)
				{
					return productId;
				}
			}
			return 0;
		}
	}
}
