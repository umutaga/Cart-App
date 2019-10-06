using System;
using System.Collections.Generic;
using CartApp.API.DTOs;
using CartApp.Core.ApplicationService;
using CartApp.Core.Entity;
using Microsoft.AspNetCore.Mvc;

namespace CartApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		private readonly ICustomerService _customerService;
		public CustomerController(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		[HttpGet]
		public ActionResult<FilteredList<CustomerDTO>> Get([FromQuery] Filter filter)
		{
			try
			{
				if (filter.CurrentPage == 0 && filter.ItemsPerPage == 0)
				{
					var list = _customerService.GetAllCustomers(null);
					var newList = new List<CustomerDTO>();
					foreach (var customer in list.List)
					{
						newList.Add(new CustomerDTO()
						{
							FirstName = customer.FirstName,
							LastName = customer.LastName
						});
					}
					var newFilteredList = new FilteredList<CustomerDTO>();
					newFilteredList.List = newList;
					newFilteredList.Count = list.Count;
					return Ok(newFilteredList);
				}
				return Ok(_customerService.GetAllCustomers(filter));
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}

		}

		[HttpGet("{id}")]
		public ActionResult<CustomerDTO> Get(int id)
		{
			if (id < 1) return BadRequest("Id 0'dan büyük olmalıdır!");

			var coreCustomer = _customerService.FindCustomerByIdIncludeOrders(id);
			return new CustomerDTO()
			{
				Id = coreCustomer.Id,
				FirstName = coreCustomer.FirstName,
				LastName = coreCustomer.LastName
			};
		}

		[HttpPost]
		public ActionResult<Customer> Post([FromBody] Customer customer)
		{
			if (string.IsNullOrEmpty(customer.FirstName))
			{
				return BadRequest("Müşteri ismi gereklidir!");
			}

			if (string.IsNullOrEmpty(customer.LastName))
			{
				return BadRequest("Müşteri soyadı gereklidir!");
			}
			return _customerService.CreateCustomer(customer);
		}

		[HttpPut("{id}")]
		public ActionResult<Customer> Put(int id, [FromBody] Customer customer)
		{
			if (id < 1 || id != customer.Id)
			{
				return BadRequest("Parametre Id ile müşteri Id aynı olmalıdır!");
			}

			return Ok(_customerService.UpdateCustomer(customer));
		}

		[HttpDelete("{id}")]
		public ActionResult<Customer> Delete(int id)
		{
			var customer = _customerService.DeleteCustomer(id);
			if (customer == null)
			{
				return StatusCode(404, $"{id}'li müşteri bulunamadı!");
			}

			return NoContent();

		}
	}

}