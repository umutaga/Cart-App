namespace CartApp.Core.Entity
{
	public class CartLine
	{
		public int ProductId { get; set; }

		public int CartId { get; set; }
		public Cart Cart { get; set; }

		public int Quantity { get; set; }
		public double Price { get; set; }
	}
}
