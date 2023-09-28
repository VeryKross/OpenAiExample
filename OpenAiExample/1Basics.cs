using Azure;
using Azure.AI.OpenAI;
using static OpenAiExample.KeyManager;

namespace OpenAiExample;

public class Basics
{
    public async Task<string> GoAsync()
    {
        OpenAIClient client;
        string model;

        UseAzure = true;
        if (UseAzure)
        {
            client = new OpenAIClient(new Uri(AzureOpenAIUrl), new AzureKeyCredential(SecretKey));
            model = AzureOpenAIModel;
        }
        else
        {
            client = new OpenAIClient(SecretKey);
            model = "gpt-3.5-turbo";
        }

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

        var response = await client.GetChatCompletionsAsync(model, options);

        var completions = response.Value;
        var content = completions.Choices[0].Message.Content;

        return content;
    }
}