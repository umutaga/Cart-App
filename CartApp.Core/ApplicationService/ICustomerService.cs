using CartApp.Core.Entity;

namespace CartApp.Core.ApplicationService
{
	public interface ICustomerService
	{
		Customer NewCustomer(string firstName, string lastName, string address);
		Customer CreateCustomer(Customer cust);
		Customer FindCustomerById(int id);
		Customer FindCustomerByIdIncludeOrders(int id);
		FilteredList<Customer> GetAllCustomers(Filter filter);
		int Count();
		Customer UpdateCustomer(Customer customerUpdate);

		Customer DeleteCustomer(int id);
	}
}
