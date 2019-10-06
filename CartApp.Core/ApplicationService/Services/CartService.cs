using System;
using System.IO;
using CartApp.Core.DomainService;
using CartApp.Core.Entity;

namespace CartApp.Core.ApplicationService.Services
{
	public class CartService : ICartService
	{
		readonly ICartRepository _cartRepo;
		readonly ICustomerRepository _customerRepo;

		public CartService(ICartRepository cartRepo,
			ICustomerRepository customerRepository)
		{
			_cartRepo = cartRepo;
			_customerRepo = customerRepository;
		}

		public Cart New()
		{
			return new Cart();
		}

		public Cart CreateCart(Cart cart)
		{
			if (cart.Customer == null || cart.Customer.Id <= 0)
				throw new InvalidDataException("Sepet oluşturmak için müşteri tanımlanmalıdır!");
			if (_customerRepo.ReadById(cart.Customer.Id) == null)
				throw new InvalidDataException("Müşteri bulunamadı!");
			if (cart.OrderDate == null)
				throw new InvalidDataException("Sipariş tarihi gereklidir!");
			if (cart.DeliveryDate <= DateTime.MinValue)
				throw new InvalidDataException("Teslim tarihi gereklidir!");

			return _cartRepo.Create(cart);
		}

		public Cart FindCartById(int id)
		{
			return _cartRepo.ReadById(id);
		}

		public FilteredList<Cart> GetAll()
		{
			return _cartRepo.ReadAll();
		}

		public FilteredList<Cart> GetFilteredCarts(Filter filter)
		{
			if (filter.CurrentPage < 0 || filter.ItemsPerPage < 0)
			{
				throw new InvalidDataException("Aktif sayfa ve satır sayısı 0'dan büyük olmalıdır!");
			}
			if ((filter.CurrentPage - 1 * filter.ItemsPerPage) >= _cartRepo.Count())
			{
				throw new InvalidDataException("Aktif sayfa çok yüksek!");
			}

			if (filter == null || (filter.ItemsPerPage == 0 && filter.CurrentPage == 0))
			{
				return GetAll();
			}

			return _cartRepo.ReadAll(filter);
		}

		public Cart UpdateCart(Cart cartUpdate)
		{
			return _cartRepo.Update(cartUpdate);
		}

		public Cart DeleteCart(int id)
		{
			return _cartRepo.Delete(id);
		}
	}
}
