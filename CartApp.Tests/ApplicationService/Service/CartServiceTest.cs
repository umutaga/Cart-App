using System;
using System.IO;
using CartApp.Core.ApplicationService;
using CartApp.Core.ApplicationService.Services;
using CartApp.Core.DomainService;
using CartApp.Core.Entity;
using Moq;
using Xunit;

namespace CartApp.Tests
{
	public class CartServiceTest
	{
		public CartServiceTest()
		{

		}


		[Fact]
		public void CreateCartWithCustomerMissingThrowsException()
		{
			var custRepo = new Mock<ICustomerRepository>();
			var orderRepo = new Mock<ICartRepository>();
			ICartService service = new CartService(orderRepo.Object, custRepo.Object);
			var cart = new Cart()
			{
				DeliveryDate = DateTime.Now,
				OrderDate = DateTime.Now
			};
			Exception ex = Assert.Throws<InvalidDataException>(() => service.CreateCart(cart));
			Assert.Equal("Sepet oluþturmak için müþteri tanýmlanmalýdýr!", ex.Message);
		}

		[Fact]
		public void CreateCartDeliveryDateMissingThrowsException()
		{
			var custRepo = new Mock<ICustomerRepository>();
			custRepo.Setup(x => x.ReadById(It.IsAny<int>())).Returns(new Customer() { Id = 1 });
			var cartRepo = new Mock<ICartRepository>();
			ICartService service = new CartService(cartRepo.Object, custRepo.Object);
			var cart = new Cart()
			{
				Customer = new Customer() { Id = 1 },
				OrderDate = DateTime.Now
			};
			Exception ex = Assert.Throws<InvalidDataException>(() => service.CreateCart(cart));
			Assert.Equal("Teslim tarihi gereklidir!", ex.Message);
		}

		[Fact]
		public void CreateCartShouldCallCartRepoCreateCartOnce()
		{
			var custRepo = new Mock<ICustomerRepository>();
			custRepo.Setup(x => x.ReadById(It.IsAny<int>())).Returns(new Customer() { Id = 1 });

			var cartRepo = new Mock<ICartRepository>();
			ICartService service = new CartService(cartRepo.Object, custRepo.Object);
			var cart = new Cart()
			{
				Customer = new Customer() { Id = 1 },
				DeliveryDate = DateTime.Now,
				OrderDate = DateTime.Now
			};
			service.CreateCart(cart);
			cartRepo.Verify(x => x.Create(It.IsAny<Cart>()), Times.Once);
			custRepo.Verify(x => x.ReadById(It.IsAny<int>()), Times.Once);

		}


	}
}
