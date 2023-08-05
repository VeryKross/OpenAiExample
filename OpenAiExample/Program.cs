using OpenAiExample;

var example = new Basics("gpt-3.5-turbo");
//var example = new PromptRoles("gpt-3.5-turbo");
//var example = new Functions("gpt-3.5-turbo-0613"); // <- Note the different model name here: the "-0613" suffix is needed to enable the function feature.


var content = await example.GoAsync();

Console.WriteLine($"C: {content}");

Console.WriteLine("");
Console.WriteLine("--------------------------------");
