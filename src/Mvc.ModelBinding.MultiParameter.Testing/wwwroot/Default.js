(function ()
{
	PageEvents();
	Init();
})();

var result;

function PageEvents()
{
	document.addEventListener("click", function (e)
	{
		if (typeof window[e.target.id] === "function")
			window[e.target.id].call(e, e);
	});

	$id("UploadExample").on("change", function (e)
	{
		StartUpload(e);
	});
}

function Init()
{
	result = $id('Result');
	netproxy("./api/HelloWorld"); // Makes session (and cookie)
}

var r;
var result1,result2;

function C(s, jscript)
{
	if (eval(jscript))
		result.innerHTML += "<div class='ok'>" + s + " :: " + jscript + '</div>\n';
	else
		result.innerHTML += "<div class='error'>" + s + " :: " + jscript + '</div>\n';
}

async function UnitTest()
{
	result.innerText = '';

	var user = { user: 'alphons' };

	r = await netproxyasync("./api/SimpleString1", user);
	C("SimpleString1", "r.user == 'alphons'");
	r = await netproxyasync("./api/SimpleString2", user);
	C("SimpleString2", "r.user == 'alphons'");
	r = await netproxyasync("./api/SimpleString3", { model: { user: 'alphons' } });
	C("SimpleString3", "r.user == 'alphons'");
	r = await netproxyasync("./api/SimpleString4", { model: { user: 'alphons' } });
	C("SimpleString4", "r.user == 'alphons'");

	var users = { "users": ["admins", "editors", null, "sisters"] };

	r = await netproxyasync("./api/ArrayOfStrings1", users);
	C("ArrayOfStrings1", "r.users && r.users.length == 4");
	r = await netproxyasync("./api/ArrayOfStrings2", users);
	C("ArrayOfStrings2", "r.users && r.users.length == 4");
	r = await netproxyasync("./api/ArrayOfStrings3", { model: { "users": ["admins", "editors", null, "sisters"] } });
	C("ArrayOfStrings3", "r.users && r.users.length == 4");
	r = await netproxyasync("./api/ArrayOfStrings4", { model: { "users": ["admins", "editors", null, "sisters"] } });
	C("ArrayOfStrings4", "r.users && r.users.length == 4");

	var arrayofarrayofstrings = { model: { "users": [["admins", "0"], ["editors", "1"], null, ["sisters", "3"]] } };
	r = await netproxyasync("./api/ArrayOfArrayOfStrings1", arrayofarrayofstrings);
	C("ArrayOfArrayOfStrings1", "r.users && r.users.length == 4");
	r = await netproxyasync("./api/ArrayOfArrayOfStrings2", arrayofarrayofstrings);
	C("ArrayOfArrayOfStrings2", "r.users && r.users.length == 4");

	r = await netproxyasync("./api/TwoParameters1", { a: 'aa aa', b: 'bbb bbb' });
	C("TwoParameters1", "r.a =='aa aa' && r.b =='bbb bbb'");
	r = await netproxyasync("./api/TwoParameters2", { a: 'aa aa', b: 'bbb bbb' });
	C("TwoParameters2", "r.a =='aa aa' && r.b =='bbb bbb'");
	r = await netproxyasync("./api/TwoParameters3", { a: { user: 'aa aa' }, b: { user: 'bbb bbb' } });
	C("TwoParameters3", "r.a =='aa aa' && r.b =='bbb bbb'");

	r = await netproxyasync("./api/ListOfDoubles", { list: [1.2, 3.4, 5.6, null, 7.8] });
	C("ListOfDoubles", "r.list[0] == 1.2");
	C("ListOfDoubles", "r.list[1] == 3.4");
	C("ListOfDoubles", "r.list[2] == 5.6");
	C("ListOfDoubles", "r.list[3] == null");
	C("ListOfDoubles", "r.list[4] == 7.8");

	//r = await netproxyasync("./api/ComplexArrayArray",
	//	{
	//		Group: "groupy",
	//		List:
	//		{
	//			Name: "My Name is",
	//			"Users":
	//				[
	//					["admins", "0"],
	//					["editors", "1"],
	//					null,
	//					["sisters", "3"]
	//				]
	//		}
	//	});

	r = await netproxyasync("./api/ComplexDouble", { F: 123.456 });
	C("ComplexDouble", "r.F == 123.456");

	r = await netproxyasync("./api/ComplexDouble", { F: '123.456' }); // Needs JsonNumberHandling.AllowReadingFromString
	C("ComplexDouble", "r.F == 123.456");

	r = await netproxyasync("./api/ComplexSingleObject", { AA: { a: 'aaa', b: 'bbb' } });
	C("ComplexSingleObject", "r.AA.a== 'aaa'");
	C("ComplexSingleObject", "r.AA.b== 'bbb'");

	r = await netproxyasync("./api/ComplexString", { Name: 'This is a test' });
	C("ComplexString", "r.Name == 'This is a test'");

	r = await netproxyasync("./api/ComplexStringInt", { Name: 'This is a test', A : 123 });
	C("ComplexStringInt", "r.Name == 'This is a test'");
	C("ComplexStringInt", "r.A == 123");

	r = await netproxyasync("./api/ComplexListOfStrings", { ListOfStrings: ['a', '', 'b', null, 'c'] });
	C("ComplexList", "r.ListOfStrings[0]== 'a'");
	C("ComplexList", "r.ListOfStrings[1]== ''");
	C("ComplexList", "r.ListOfStrings[2]== 'b'");
	C("ComplexList", "r.ListOfStrings[3]== null");
	C("ComplexList", "r.ListOfStrings[4]== 'c'");

	r = await netproxyasync("./api/ComplexListOfInts", { ListOfInts: [ 0, 1 , 2, null , 4] });
	C("ComplexListInt", "r.ListOfInts[0]== 0");
	C("ComplexListInt", "r.ListOfInts[1]== 1");
	C("ComplexListInt", "r.ListOfInts[2]== 2");
	C("ComplexListInt", "r.ListOfInts[3]== null");
	C("ComplexListInt", "r.ListOfInts[4]== 4");
	
	r = await netproxyasync("./api/ComplexArray", { list: [{ a: 'a', b: null }, null, { a: 'c', b: 'd' }] });
	C("ComplexArray", "r.list.length == 3");
	if (r.list.length == 3)
	{
		C("ComplexArray", "r.list[0].a== 'a'")
		C("ComplexArray", "r.list[0].b== null");
		C("ComplexArray", "r.list[1]== null");
		C("ComplexArray", "r.list[2].a== 'c'");
		C("ComplexArray", "r.list[2].b== 'd'");
	}

	r = await netproxyasync("./api/ComplexArrayArray",
	{
		Group: "groupy",
		List:
		{
			Name: "My Name is",
			"Users":
				[
					["admins", "0"],
					["editors", "1"],
					null,
					["sisters", "3"]
				]
		}
	});
	C("ComplexArrayArray", "r.Group== 'groupy'");
	C("ComplexArrayArray", "r.List.Name== 'My Name is'");
	C("ComplexArrayArray", "r.List.Users.length== 4");
	C("ComplexArrayArray", "r.List.Users[0][0]= 'admins'");
	C("ComplexArrayArray", "r.List.Users[0][1]= '0'");
	C("ComplexArrayArray", "r.List.Users[1][0]= 'editors'");
	C("ComplexArrayArray", "r.List.Users[1][1]= '1'");
	C("ComplexArrayArray", "r.List.Users[2]== null");
	if (r.List.Users.length == 4)
	{
		C("ComplexArrayArray", "r.List.Users[3][0]== 'sisters'");
		C("ComplexArrayArray", "r.List.Users[3][1]== '3'");
	}

	r = await netproxyasync("./api/ComplexArrayArrayClass",
		{
			Testing: true,
			Relaxed: false,
			Group: "Nice Group",
			GroupInfo:
			{
				Name: "My Name is",
				"Users":
					[
						[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
						[{ Name: "User10" }, { Name: "User11" }],
						[{ Name: "User20" }, { Name: "User21" }]
					]
			}
		});
	C("ComplexArrayArrayClass", "r.Testing== true");
	C("ComplexArrayArrayClass", "r.Relaxed== false");
	C("ComplexArrayArrayClass", "r.Group== 'Nice Group'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Name== 'My Name is'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users.length== 3");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[0][0].Name== 'User00'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[0][0].Alias.length== 3");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[0][1].Name== 'User01'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[0][1].Alias== null");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[1][0].Name== 'User10'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[1][0].Alias== null");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[1][1].Name== 'User11'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[1][1].Alias== null");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[2][0].Name== 'User20'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[2][0].Alias== null");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[2][1].Name== 'User21'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[2][1].Alias== null");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[0][0].Alias[0]== 'aliasa'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[0][0].Alias[1]== 'aliasb'");
	C("ComplexArrayArrayClass", "r.GroupInfo.Users[0][0].Alias[2]== 'aliasc'");

	// Checking, run till end

	netproxy("./api/GetEnums", null, function ()
	{
		var data = this;
	});

	var product = { StatusType: 1, AddressType: 2 };

	netproxy("./api/PostEnums", { product: product }, function ()
	{
		var data = this;
	});

	netproxy("./api/PostEnum", { status: 1 }, function ()
	{
		var data = this;
	});

	C("ready", "true == false");
}


	//netproxy("./api/ComplexDouble", { F: null });
	//netproxy("./api/ComplexDouble", { F: 123.456 });
	//netproxy("./api/ComplexString", { Name: null });
	//netproxy("./api/ComplexString", { Name: 12.34 });
	//netproxy("./api/ComplexString", { Name: 'abc def' });
	//netproxy("./api/ComplexStringInt", { Name: 'abc def', A : 123 });
	//netproxy("./api/ComplexList", { list: [ "a", "b" , "", null, "d"] });
	//netproxy("./api/ComplexList", { list: ["a", "", "b", null, "c"] }); // "a" null "b" "b" "c" WTF?
	//netproxy("./api/ComplexListNullableDouble", { list: [1.2, 3.4, 5.6, null, 7.8] });
	//netproxy("./api/ComplexListObjecs", { list: [1.2, null, 3 ] }); // numbers to string, using CultureInfo!
	//netproxy("./api/ComplexStringList", { Name: 'abc def', list: [ 'a', 'b' , 'c'] });
	//netproxy("./api/ComplexObjectArray", { objB: { Name: 'Alphons', List: [{ a: 'a', b: 'b' }, { a: 'c', b: 'd' }] } });



function ProgressHandler(event)
{
	var percent = 0;
	var position = event.loaded || event.position;
	var total = event.total;
	if (event.lengthComputable)
		percent = Math.ceil(position / total * 100);
	$id("Result").innerText = "Uploading " + percent + "%";
}

function StartUpload(e)
{
	var file = e.target.files[0];

	$id("Result").innerText = 'Uploading';

	var formData = new FormData();

 	formData.append("file", file, file.name);
	formData.append("Form1", "Value1"); // some extra Form data

	netproxy("/api/Upload", formData, function ()
	{
		$id("Result").innerText = 'Ready Length:' + this.Length + " ExtraValue:" + this.Form1;
	}, window.NetProxyErrorHandler, ProgressHandler);
}

async function MultiBinderTest()
{
	$id("Result").innerText = '';
	result1 = await netproxyasync("./api/DemoProposal/two?SomeParameter3=three&SomeParameter6=six",
	{
		"SomeParameter4": // Now the beast has a name
		{
			Name: "four",
			"Users":
			[
				[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
				[{ Name: "User10" }, { Name: "User11" }],
				[{ Name: "User20" }, { Name: "User21" }]
			]
		},
		"SomeParameter5": "five" // double binder
	});

	$id("Result").innerText = 'some alias: ' + result1.SomeParameter4.Users[0][0].Alias[1];

	result2 = await netproxyasync("./api/DemoProposal2/two?SomeParameter3=three&SomeParameter6=six",
	{
		"SomeParameter4": // Now the beast has a name
		{
			Name: "four",
			"Users":
			[
				[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
				[{ Name: "User10" }, { Name: "User11" }],
				[{ Name: "User20" }, { Name: "User21" }]
			]
		},
		"SomeParameter5": "five" // double binder
	});

	$id("Result").innerText += ' other alias: ' + result2.SomeParameter4.Users[0][0].Alias[2];

}
