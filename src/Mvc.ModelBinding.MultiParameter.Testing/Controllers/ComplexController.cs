
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable disable

namespace CoreBasic.Web.LogicControllers
{
	public class ComplexController : ControllerBase
	{
		[HttpGet]
		[Route("~/api/index")]

		// 1.0
		public async Task<IActionResult> Index(List<string> list)
		{
			await Task.Yield();

			// Using SomeValueProviderFactory
			// Returned list "a", b", "b",  "c" <- repeat-bug
			// Expected list "a", b", null, "c"

			return Ok();
		}

		// 2.1

		[HttpPost]
		[Route("~/api/SimpleString1")]
		public async Task<IActionResult> SimpleString1(string user)
		{
			await Task.Yield();
			return Ok(new
			{
				user
			});
		}

		[HttpPost]
		[Route("~/api/SimpleString2")]
		public async Task<IActionResult> SimpleString2([FromBody] string user)
		{
			await Task.Yield();
			return Ok(new
			{
				user
			});
		}

		public class ModelOfString
		{
			public string user { get; set; }
		}

		[HttpPost]
		[Route("~/api/SimpleString3")]
		public async Task<IActionResult> SimpleString3(ModelOfString model)
		{
			await Task.Yield();
			return Ok(new
			{
				model?.user
			});
		}

		[HttpPost]
		[Route("~/api/SimpleString4")]
		public async Task<IActionResult> SimpleString4([FromBody] ModelOfString model)
		{
			await Task.Yield();
			return Ok(new
			{
				model?.user
			});
		}

		// 2.2
		[HttpPost]
		[Route("~/api/ArrayOfStrings1")]
		public async Task<IActionResult> ArrayOfStrings1(List<string> users)
		{
			await Task.Yield();
			return Ok(new
			{
				users
			});
		}

		[HttpPost]
		[Route("~/api/ArrayOfStrings2")]
		public async Task<IActionResult> ArrayOfStrings2([FromBody] List<string> users)
		{
			await Task.Yield();
			return Ok(new
			{
				users
			});
		}

		public class ModelArrayOfStrings
		{
			public List<string> users { get; set; }
		}

		[HttpPost]
		[Route("~/api/ArrayOfStrings3")]
		public async Task<IActionResult> ArrayOfStrings3(ModelArrayOfStrings model)
		{
			await Task.Yield();
			return Ok(new
			{
				model?.users
			});
		}

		[HttpPost]
		[Route("~/api/ArrayOfStrings4")]
		public async Task<IActionResult> ArrayOfStrings4([FromBody] ModelArrayOfStrings model)
		{
			await Task.Yield();
			return Ok(new
			{
				model?.users
			});
		}

		// 2.3

		public class ModelArrayOfArrayOfStrings
		{
			public List<List<string>> users { get; set; }
		}

		[HttpPost]
		[Route("~/api/ArrayOfArrayOfStrings1")]
		public async Task<IActionResult> ArrayOfArrayOfStrings1(ModelArrayOfArrayOfStrings model)
		{
			await Task.Yield();
			return Ok(new
			{
				model?.users
			});
		}

		[HttpPost]
		[Route("~/api/ArrayOfArrayOfStrings2")]
		public async Task<IActionResult> ArrayOfArrayOfStrings2([FromBody] ModelArrayOfArrayOfStrings model)
		{
			await Task.Yield();
			return Ok(new
			{
				model?.users
			});
		}

		[HttpPost]
		[Route("~/api/TwoParameters1")]
		public async Task<IActionResult> TwoParameters1(string a, string b)
		{
			await Task.Yield();
			return Ok(new
			{
				a,
				b
			});
		}

		[HttpPost]
		[Route("~/api/TwoParameters2")]
		public async Task<IActionResult> TwoParameters2([FromBody] string a, string b)
		{
			await Task.Yield();
			return Ok(new
			{
				a,b
			});
		}

		[HttpPost]
		[Route("~/api/TwoParameters3")]
		public async Task<IActionResult> TwoParameters3([FromBody] ModelOfString a, ModelOfString b)
		{
			await Task.Yield();
			return Ok(new
			{
				a = a.user,
				b = b.user
			});
		}

		// Solution 1, problem of the repeater bug

		[HttpPost]
		[Route("~/api/ListOfDoubles")]
		public async Task<IActionResult> ListOfDoubles(List<double?> list)
		{
			await Task.Yield();
			return Ok(new
			{
				list
			});
		}

		public class ApiModel
		{
			public string Name { get; set; }
			public List<List<Usert>> Users { get; set; }

			public override string ToString() => Name;
		}


		[HttpGet]
		[Route("~/api/HelloWorld")]
		public async Task<IActionResult> HelloWorld()
		{
			// Forces a session cooky
			HttpContext.Session.SetString("Hello", "World");
			await HttpContext.Session.CommitAsync();

			return Ok();
		}

		[HttpPost]
		[Route("~/api/ComplexTest")]
		public async Task<IActionResult> ComplexTest(
			ApiModel SomeParameter4,
			string SomeParameter5)
		{
			await Task.Yield();

			return Ok(new
			{

			});
		}


		[HttpPost]
		[Route("~/api/DemoProposal/{SomeParameter2}")]
		public async Task<IActionResult> DemoProposal(
			[FromCooky(Name = ".AspNetCore.Session")] string SomeParameter0,
			[FromHeader(Name = "Referer")] string SomeParameter1,
			[FromRoute] string SomeParameter2,
			[FromQuery] string SomeParameter3,
			[FromBody] ApiModel SomeParameter4,
			[FromBody] string SomeParameter5,
			[FromQuery]string SomeParameter6)
		{
			await Task.Yield();

			return Ok(new
			{

			});
		}

		[HttpPost]
		[Route("~/api/DemoProposal2/{SomeParameter2}")]
		public async Task<IActionResult> DemoProposal(
			string Referer,
			string SomeParameter2,
			string SomeParameter3,
			ApiModel SomeParameter4,
			string SomeParameter5,
			string SomeParameter6)
		{
			await Task.Yield();

			return Ok(new
			{

			});
		}


		[HttpPost]
		[Route("~/api/ComplexString")]
		public async Task<IActionResult> ComplexString(string Name)
		{
			await Task.Yield();

			return Ok(new
			{
				Name
			});
		}

		[HttpPost]
		[Route("~/api/ComplexDouble")]
		public async Task<IActionResult> ComplexDouble(double? F)
		{
			await Task.Yield();

			return Ok(new
			{
				F
			});
		}

		[HttpPost]
		[Route("~/api/ComplexStringInt")]
		public async Task<IActionResult> ComplexStringInt(string Name, int A)
		{
			await Task.Yield();

			return Ok(new
			{
				Name,
				A
			});
		}

		[HttpPost]
		[Route("~/api/ComplexListOfStrings")]
		public async Task<IActionResult> ComplexListOfStrings(List<string> ListOfStrings)
		{
			await Task.Yield();

			return Ok(new
			{
				ListOfStrings
			});
		}

		[HttpPost]
		[Route("~/api/ComplexListOfInts")]
		public async Task<IActionResult> ComplexListOfInts(List<int?> ListOfInts)
		{
			await Task.Yield();

			return Ok(new
			{
				ListOfInts
			});
		}

		[HttpPost]
		[Route("~/api/ComplexListNullableDouble")]
		public async Task<IActionResult> ComplexListNullableDouble(List<double?> list)
		{
			await Task.Yield();

			return Ok(new
			{
				list
			});
		}


		[HttpPost]
		[Route("~/api/ComplexListObjecs")]
		public async Task<IActionResult> ComplexListObjecs(List<string> list)
		{
			await Task.Yield();

			return Ok(new
			{
				list
			});
		}

		[HttpPost]
		[Route("~/api/ComplexStringList")]
		public async Task<IActionResult> ComplexStringList(string Name, List<string> list)
		{
			await Task.Yield();

			return Ok(new
			{
				Name,
				list
			});
		}

		public class ObjectB
		{
			public List<ObjectA> List { get; set; }

			public string Name { get;set;}
		}

		public class ObjectA
		{
			public string a { get; set; }
			public string b { get; set; }
		}

		[HttpPost]
		[Route("~/api/ComplexSingleObject")]
		public async Task<IActionResult> ComplexSingleObject(ObjectA AA)
		{
			await Task.Yield();

			return Ok(new
			{
				AA
			});
		}


		[HttpPost]
		[Route("~/api/ComplexArray")]
		public async Task<IActionResult> ComplexArray(ObjectA[] list)
		{
			await Task.Yield();

			return Ok(new
			{
				list
			});
		}

		[HttpPost]
		[Route("~/api/ComplexObjectArray")]
		public async Task<IActionResult> ComplexObjectArray(ObjectB objB)
		{
			await Task.Yield();

			return Ok(new
			{
				objB
			});
		}

		public class ObjectC
		{
			public string Name { get; set; }
			public List<List<string>> Users { get; set; }
		}

		[HttpPost]
		[Route("~/api/ComplexArrayArray")]
		public async Task<IActionResult> ComplexArrayArray(string Group, ObjectC List)
		{
			await Task.Yield();

			return Ok(new
			{
				Group,
				List
			});
		}

		public class Usert
		{
			public string Name { get; set; }

			public List<string> Alias { get; set; }
		}

		public class ObjectD
		{
			public string Name { get; set; }
			public List<List<Usert>> Users { get; set; }
		}

		[HttpPost]
		[Route("~/api/ComplexArrayArrayClass")]
		public async Task<IActionResult> ComplexArrayArrayClass(bool Testing, bool Relaxed, string Group, ObjectD GroupInfo)
		{
			await Task.Yield();

			return Ok(new
			{
				Testing,
				Relaxed,
				Group,
				GroupInfo
			});
		}


		/// <summary>
		/// Uploads have default a maximum of 30MByte presenting upload example of 2.5GB
		///
		/// For IIS Limit maxAllowedContentLength in Web.config (in the root of the app, not in wwwroot content folder!)
		/// 
		/// </summary>
		/// <param name="formFile"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("~/api/Upload")]
		[RequestSizeLimit(2_500_000_000)]
		[RequestFormLimits(MultipartBodyLengthLimit = 2_500_000_000)]
		public async Task<IActionResult> Upload(IFormFile file, string Form1)
		{
			if (file.Length > 0)
			{
				using var ms = new MemoryStream();
				await file.CopyToAsync(ms); // some dummy operation
			}
			return Ok(new 
			{ 
				file.Length,
				Form1
			});
		}

		public enum AddressEnum
		{
			Unknown,
			Living,
			Working
		}

		public enum StatusEnum
		{
			Unknown,
			Active,
			Passive
		}

		[HttpGet]
		[Route("~/api/GetEnums")]
		public async Task<IActionResult> GetEnums()
		{
			await Task.Yield();
			return Ok(new
			{
				StatusEnum.Active,
				AddressEnum.Working
			});
		}

		public class Product
		{
			public StatusEnum StatusType { get; set; }
			public AddressEnum AddressType { get; set; }
		}

		[HttpPost]
		[Route("~/api/PostEnum")]
		public async Task<IActionResult> PostEnum(StatusEnum status)
		{
			await Task.Yield();
			return Ok();
		}

		[HttpPost]
		[Route("~/api/PostEnums")]
		public async Task<IActionResult> PostEnums(Product product)
		{
			await Task.Yield();
			return Ok();
		}


	}
}
