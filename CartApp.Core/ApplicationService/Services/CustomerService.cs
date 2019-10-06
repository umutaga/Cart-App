using CartApp.Core.DomainService;
using CartApp.Core.Entity;

namespace CartApp.Core.ApplicationService.Services
{
	public class CustomerService : ICustomerService
	{
		readonly ICustomerRepository _customerRepo;
		readonly ICartRepository _cartRepo;

		public CustomerService(ICustomerRepository customerRepository, ICartRepository cartRepository)
		{
			_customerRepo = customerRepository;
			_cartRepo = cartRepository;
		}

		public Customer NewCustomer(string firstName, string lastName, string address)
		{
			var cust = new Customer()
			{
				FirstName = firstName,
				LastName = lastName,
				Address = address
			};

			return cust;
		}

		public Customer CreateCustomer(Customer cust)
		{
			return _customerRepo.Create(cust);
		}

		public Customer FindCustomerById(int id)
		{
			return _customerRepo.ReadById(id);
		}

		public Customer FindCustomerByIdIncludeOrders(int id)
		{
			return _customerRepo.ReadByIdIncludeOrders(id);
		}

		public FilteredList<Customer> GetAllCustomers(Filter filter = null)
		{
			return _customerRepo.ReadAll(filter);
		}



		public Customer UpdateCustomer(Customer customerUpdate)
		{
			return _customerRepo.Update(customerUpdate);
		}

		public Customer DeleteCustomer(int id)
		{
			return _customerRepo.Delete(id);
		}

		public int Count()
		{
			return _customerRepo.Count();
		}
	}
}
