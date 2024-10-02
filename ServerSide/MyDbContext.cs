using Microsoft.EntityFrameworkCore;

namespace ServerSide;

internal class MyDbContext:DbContext
{

	public DbSet<User> Users { get; set; }
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-0P1DC60\SQLEXPRESS;Initial Catalog=UserContext;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

}
