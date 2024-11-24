using Microsoft.AspNetCore.Mvc;


namespace Mvc.ModelBinding.MultiParameter.Testing.Controllers;

[Route("~/api")]
public class SpeedController : ControllerBase
{
	public class Usert
	{
		public string Name { get; set; } = string.Empty;

		public List<string> Alias { get; set; } = [];
	}

	public class ApiModel
	{
		public string Name { get; set; } = string.Empty;
		public List<List<Usert>> Users { get; set; } = [];

		public override string ToString() => Name;
	}

	[HttpPost("~/api/Speedtest/{SomeParameter2}")]
	public IActionResult Speedtest(
	string Referer,
	string SomeParameter2,
	string SomeParameter3,
	ApiModel SomeParameter4,
	double SomeParameter5,
	int SomeParameter6)
	{
		return Ok(new
		{
			Referer,
			SomeParameter2,
			SomeParameter3,
			SomeParameter4,
			SomeParameter5,
			SomeParameter6
		});
	}


	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Address
	{
		public string street { get; set; }
		public string city { get; set; }
		public string zipcode { get; set; }
	}

	public class Metadata
	{
		public int totalCount { get; set; }
		public DateTime generatedAt { get; set; }
	}

	public class Preferences
	{
		public string theme { get; set; }
		public bool notifications { get; set; }
	}

	public class LargeObject
	{
		public List<User> users { get; set; }
		public Metadata metadata { get; set; }
	}

	public class User
	{
		public int id { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public Address address { get; set; }
		public Preferences preferences { get; set; }
	}


	[HttpPost("~/api/LargeJson")]
	public IActionResult LargeJson(LargeObject largeJson)
	{
		return Ok();
	}

}
