using System;
using System.Collections.Generic;

namespace CartApp.Core.Entity
{
	public class Cart
	{
		public int Id { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		public Customer Customer { get; set; }
		public List<CartLine> CartLines { get; set; }
	}
}
