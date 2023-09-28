using OpenAiExample;

// Uncomment ONE of the following lines to run the example.

var example = new Basics();
//var example = new PromptRoles();
//var example = new Functions();


var content = await example.GoAsync();

Console.WriteLine($"C: {content}");

Console.WriteLine("");
Console.WriteLine("--------------------------------");
