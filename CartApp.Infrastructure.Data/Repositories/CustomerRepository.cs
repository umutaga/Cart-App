using System.Linq;
using CartApp.Core.DomainService;
using CartApp.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace CartApp.Infrastructure.Data.Repositories
{
	public class CustomerRepository : ICustomerRepository
	{
		readonly CartAppContext _ctx;

		public CustomerRepository(CartAppContext ctx)
		{
			_ctx = ctx;
		}

		public Customer Create(Customer customer)
		{
			var customerSaved = _ctx.Customers.Add(customer).Entity;
			_ctx.SaveChanges();
			return customerSaved;
		}

		public Customer ReadById(int id)
		{
			return _ctx.Customers
				.FirstOrDefault(c => c.Id == id);
		}


		public Customer ReadByIdIncludeOrders(int id)
		{
			return _ctx.Customers
				.Include(c => c.Carts)
				.FirstOrDefault(c => c.Id == id);
		}

		public FilteredList<Customer> ReadAll(Filter filter)
		{
			var filteredList = new FilteredList<Customer>();

			if (filter != null && filter.ItemsPerPage > 0 && filter.CurrentPage > 0)
			{
				filteredList.List = _ctx.Customers
					.Skip((filter.CurrentPage - 1) * filter.ItemsPerPage)
					.Take(filter.ItemsPerPage);
				filteredList.Count = _ctx.Customers.Count();
				return filteredList;
			}
			else
			{
				filteredList.List = _ctx.Customers;
				filteredList.Count = filteredList.List.Count();
				return filteredList;
			}
		}

		public Customer Update(Customer customerUpdate)
		{
			_ctx.Attach(customerUpdate).State = EntityState.Modified;
			_ctx.Entry(customerUpdate).Collection(c => c.Carts).IsModified = true;
			var carts = _ctx.Cart.Where(o => o.Customer.Id == customerUpdate.Id
								   && !customerUpdate.Carts.Exists(co => co.Id == o.Id));
			foreach (var cart in carts)
			{
				cart.Customer = null;
				_ctx.Entry(cart).Reference(o => o.Customer)
					.IsModified = true;
			}
			_ctx.SaveChanges();
			return customerUpdate;
		}

		public Customer Delete(int id)
		{
			var custRemoved = _ctx.Remove(new Customer { Id = id }).Entity;
			_ctx.SaveChanges();
			return custRemoved;
		}

		public int Count()
		{
			return _ctx.Customers.Count();
		}
	}
}
