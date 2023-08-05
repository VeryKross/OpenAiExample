using Azure.AI.OpenAI;
using static OpenAiExample.KeyManager;

namespace OpenAiExample;

public class Basics
{
    private readonly string _model;

    public Basics(string model)
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

        var prompt = 
            "Write a one paragraph LinkedIn post about a cat who can program computers.";

        Console.WriteLine($"P: {prompt}");

        options.Messages.Add(new ChatMessage(ChatRole.User, prompt));

        var response = await client.GetChatCompletionsAsync(_model, options);

        var completions = response.Value;
        var content = completions.Choices[0].Message.Content;

        return content;
    }
}