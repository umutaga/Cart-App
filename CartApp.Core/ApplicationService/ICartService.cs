using CartApp.Core.Entity;

namespace CartApp.Core.ApplicationService
{
	public interface ICartService
	{
		Cart New();
		Cart CreateCart(Cart cart);
		Cart FindCartById(int id);
		FilteredList<Cart> GetAll();
		FilteredList<Cart> GetFilteredCarts(Filter filter);
		Cart UpdateCart(Cart cartUpdate);
		Cart DeleteCart(int id);
	}
}
