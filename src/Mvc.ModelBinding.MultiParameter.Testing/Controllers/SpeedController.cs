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


}
