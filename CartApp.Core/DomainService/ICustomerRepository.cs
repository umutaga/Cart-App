using CartApp.Core.Entity;

namespace CartApp.Core.DomainService
{
	public interface ICustomerRepository
	{
		Customer Create(Customer customer);
		Customer ReadById(int id);
		Customer ReadByIdIncludeOrders(int id);

		FilteredList<Customer> ReadAll(Filter filter);
		int Count();
		Customer Update(Customer customerUpdate);
		Customer Delete(int id);
	}
}
