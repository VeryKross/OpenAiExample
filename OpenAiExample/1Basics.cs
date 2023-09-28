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
            Temperature = 1.5f,               // The higher the temperature, the more "creative" the text
            MaxTokens = 800,                  // The maximum number of tokens to generate in the completion
            //NucleusSamplingFactor = 0.95f,    // How much of the previous tokens to sample from (.1 = 10%)
            FrequencyPenalty = 0f,            // The higher the value, the less likely the AI will repeat words
            PresencePenalty = 0f              // The higher the value, the less likely the AI will repeat statements
        };

        var prompt = 
            "Write a one paragraph LinkedIn post about a cat who can program computers.";

        Console.WriteLine($"P: {prompt}");
        Console.WriteLine();

        options.Messages.Add(new ChatMessage(ChatRole.User, prompt));

        var response = await client.GetChatCompletionsAsync(model, options);

        var completions = response.Value;
        var content = completions.Choices[0].Message.Content;

        return content;
    }
}