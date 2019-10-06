using CartApp.Core.Entity;

namespace CartApp.Core.DomainService
{
	public interface ICartRepository
	{
		Cart Create(Cart Cart);
		Cart ReadById(int id);
		FilteredList<Cart> ReadAll(Filter filter = null);
		int Count();
		Cart Update(Cart CartToUpdate);
		Cart Delete(int id);
		
	}
}
