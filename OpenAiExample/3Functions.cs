using System.Text.Json;
using Azure.AI.OpenAI;
using static OpenAiExample.KeyManager;

namespace OpenAiExample;

// This example shows how to use the new Functions feature in OpenAI to create a chatbot that can call
// external functions to get additional data to use in its responses. This is a very simple example that
// calls a function to get the current weather in a given location, but it could be extended to call
// any external API or database to get data to use in its responses.
//
// Reminder: the "-0613" suffix is needed to enable the OpenAI Function feature, whether you're using the
// using gpt-3.5-turbo-0613 or gpt-4-0613 model.
public class Functions 
{
    private readonly string _model;

    public Functions(string model)
    {
        _model = model;
    }

    public async Task<string> GoAsync()
    {
        var client = new OpenAIClient(SecretKey);

        // Set the options for the chat completion
        var options = new ChatCompletionsOptions()
        {
            Temperature = (float)0.5,
            MaxTokens = 800,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0
        };

        // Create a list of messages. This acts as the conversation state/history for the AI.
        // This approach makes it easy to control the conversation flow and to add additional
        // context for the AI to use when generating its response (like function results).
        var conversationMessages = new List<ChatMessage>()
        {
            new(ChatRole.User, "What is the weather like in Atlanta?"),
        };

        // Add the messages to the options Messages collection for the chat completion.
        foreach (var message in conversationMessages)
        {
            options.Messages.Add(message);
            Console.WriteLine($"P: {message.Role} - {message.Content}");
        }
        Console.WriteLine("===============================");

        // This creates the function definition that ChatGPT will use to identify
        // the available function and its parameters. The parameters define the
        // JSON schema for the results that will come back from ChatGPT.
        var getWeatherFunctionDefinition = new FunctionDefinition()
        {
            Name = "get_current_weather",
            Description = "Get the current weather in a given location",
            Parameters = BinaryData.FromObjectAsJson(
                new
                {
                    Type = "object",
                    Properties = new
                    {
                        Location = new
                        {
                            Type = "string",
                            Description = "The city and state, e.g. San Francisco, CA",
                        },
                        Unit = new
                        {
                            Type = "string",
                            Enum = new[] { "celsius", "fahrenheit" },
                        }
                    },
                    Required = new[] { "location" },
                },
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
        };

        // Register the function definition with the API (note: not done via options.Messages)
        options.Functions.Add(getWeatherFunctionDefinition);

        // Get the response from the OpenAI API
        var response = await client.GetChatCompletionsAsync(_model, options);

        var responseChoice = response.Value.Choices[0];

        if (responseChoice.FinishReason == CompletionsFinishReason.FunctionCall)
        {
            // Include the FunctionCall message in the conversation history
            conversationMessages.Add(responseChoice.Message);

            if (responseChoice.Message.FunctionCall.Name == "get_current_weather")
            {
                // Validate and process the JSON arguments for the function call
                var unvalidatedArguments = responseChoice.Message.FunctionCall.Arguments;

                // This is where we call our function, passing it the JSON arguments identified by the AI.
                // The function could be a simple C# method, or a call to an external API, database, etc.
                var functionResultData = GetYourFunctionResultData(unvalidatedArguments);

                // Serialize the result data from the function into a new chat message with the 'Function' role,
                // then add it to the messages after the first User message and initial response FunctionCall.
                // Note that the schema for functionResultData doesn't exactly match the schema for the function
                // but does provide additional data the the AI can draw on when creating its final answer.
                var functionResponseMessage = new ChatMessage(
                    ChatRole.Function,
                    JsonSerializer.Serialize(functionResultData,
                        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                    ) {Name = "get_current_weather"}; // <<= The Name is required but can't be set in the constructor

                // Add the function response message to the conversation history
                conversationMessages.Add(functionResponseMessage);
            }
        }

        // Now reconstruct the options.Messages collection with the new messages, including the function response,
        // and send it back to the API. This creates a "clean" conversation history for the AI to use
        // that only contains the messages that we want it to consider when generating its final response.
        options.Messages.Clear();

        foreach (var message in conversationMessages)
        {
            options.Messages.Add(message);
            Console.WriteLine($"P: {message.Role} - {message.Content}");
        }
        Console.WriteLine("-------------------------------");

        // Get the final response from the OpenAI API. Note that in more complex examples, particularly those that
        // may register multiple functions, you would probably want to loop through the responses until you get
        // a response with a FinishReason of CompletionsFinishReason.Stop or CompletionsFinishReason.MaxTokens since
        // the AI may need to call multiple functions before it can generate its final response.
        response = await client.GetChatCompletionsAsync(_model, options);
        responseChoice = response.Value.Choices[0];

        return responseChoice.Message.Content;
    }

    // This is represents our super complex function for integrating with our corporate systems, external APIs
    // or anything else we need to pull data from. It takes in the JSON arguments identified by the AI and
    // returns an object that will be serialized into a JSON response for the AI to use in its final answer.
    private FuncReturnData GetYourFunctionResultData(string unvalidatedArguments)
    {
        // Map the response JSON to a matching C# class
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
        var aiResponseData = JsonSerializer.Deserialize<AiResponseData>(unvalidatedArguments, jsonOptions);

        var funcReturn = new FuncReturnData()
        {
            Name = aiResponseData.Location,
            Temperature = aiResponseData.Unit == "celsius" ? 31 : 95,
            Unit = aiResponseData.Unit,
            Season = "Summer",
            CloudCover = "Partially Cloudy"
        };

        return funcReturn;
    }
}

// Simple example POCO to act as a deserialization destination
// for the JSON response from the function
public class AiResponseData
{
    public string Location { get; set; }
    public string Unit { get; set; }
}

// Simple POCO to act as the return value from the function. Note
// that it doesn't have to match the input schema and so can include
// additional data that the AI can use in its final answer.
public class FuncReturnData
{
    public string Name { get; set; }
    public int Temperature { get; set; }
    public string Unit { get; set; }
    public string Season { get; set; }
    public string CloudCover { get; set; }
}
