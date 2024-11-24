const output = $id('Output');

const input = {
	"SomeParameter4":
	{
		Name: "four",
		"Users":
			[
				[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
				[{ Name: "User10" }, { Name: "User11" }],
				[{ Name: "User20" }, { Name: "User21" }]
			]
	},
	"SomeParameter5": 5.5 // double binder
};


onReady(() =>
{
	PageEvents();
	Init();
});

function PageEvents()
{
	document.on("click", async function (e)
	{
		if (e.target.id && typeof window[e.target.id] === "function")
		{
			const func = window[e.target.id].call(e, e);
			if (func instanceof Promise)
				await func;
		}
	});
}

function Init()
{
	netproxy("./api/HelloWorld"); // Makes session (and cookie)
}

async function SpeedTest()
{
	output.innerText = '';

	const tasks = Array.from({ length: 50 }, () => MultiBinderTest());
	await Promise.all(tasks);
	console.log("All tasks completed");
}

async function MultiBinderTest()
{
	const result = await netproxyasync("./api/Speedtest/two?SomeParameter3=three&SomeParameter6=6", input);

}
