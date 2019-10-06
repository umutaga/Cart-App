﻿using System.Collections.Generic;

namespace CartApp.Core.Entity
{
	public class Customer
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public List<Cart> Carts { get; set; }
	}
}
