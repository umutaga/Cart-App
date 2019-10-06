using CartApp.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace CartApp.Infrastructure.Data
{
	public class CartAppContext : DbContext
	{
		public CartAppContext(DbContextOptions<CartAppContext> opt) : base(opt) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Cart>()
				.HasOne(o => o.Customer)
				.WithMany(c => c.Carts)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<CartLine>()
				.HasKey(ol => new { ol.ProductId, ol.CartId });

			modelBuilder.Entity<CartLine>()
				.HasOne(ol => ol.Cart)
				.WithMany(o => o.CartLines)
				.HasForeignKey(ol => ol.CartId);

		}
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Cart> Cart { get; set; }
		public DbSet<CartLine> CartLines { get; set; }

	}
}
