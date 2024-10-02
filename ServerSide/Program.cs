using Bogus;
using ServerSide;
using System.Reflection.Metadata.Ecma335;

var fakeUser = new Faker<User>()
	.RuleFor(u => u.Id,() => { Random rnd = new Random(); return rnd.Next(1000); })
	.RuleFor(u => u.FirstName, f => f.Person.FirstName)
	.RuleFor(u => u.LastName, f => f.Person.LastName)
	.RuleFor(u => u.Age, ()=> { Random rnd = new Random(); return rnd.Next(18, 40); });

var Users= new List<User>(fakeUser.Generate(50));

new WebHost(27001,Users).Run();


