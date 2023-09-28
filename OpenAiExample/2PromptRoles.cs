using Azure;
using Azure.AI.OpenAI;
using static OpenAiExample.KeyManager;

namespace OpenAiExample;

// This examples shows you how to use different roles in the prompt to control the AI's response.
// This is useful for training the AI to respond in a specific way to a specific type of input.
public class PromptRoles
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
            //Temperature = 1.5f,               // The higher the temperature, the more "creative" the text
            MaxTokens = 800,                  // The maximum number of tokens to generate in the completion
            NucleusSamplingFactor = 0.95f,    // How much of the previous tokens to sample from (.1 = 10%)
            FrequencyPenalty = 0f,            // The higher the value, the less likely the AI will repeat words
            PresencePenalty = 0f              // The higher the value, the less likely the AI will repeat statements
        };

        #region PromptControl1
        //var systemMessage =
        //    "You are an experienced presenter who always speaks in a pirate voice to keep your audience engaged. Your specialty is AI topics. You always include a cat in your response.";
        //var userPrompt = "Give an short example of an AI designing a piece of furniture.";
        //var assistantReply = "Arr matey! Imagine a smart AI, ye see, sketchin’ a chair with a fluffy cushion, perfect for a cat nap. It considers comfort, style, and a special spot for our feline friend. Shiver me timbers, that’s innovation!";

        //options.Messages.Add(new ChatMessage(ChatRole.System, systemMessage));
        //options.Messages.Add(new ChatMessage(ChatRole.User, userPrompt));
        //options.Messages.Add(new ChatMessage(ChatRole.Assistant, assistantReply));
        #endregion

        #region PromptControl2
        //var systemMessage =
        //    @"The Assistant is an AI chatbot that helps users convert a C# class into a T-SQL Create Table script. 
        //    After users input the C# class, it will provide the TSQL script. 
        //    If the class does not contain an Id property, the assistant will still create an Id field in the TSQL script. 
        //    Id fields should be of type Int and be marked as Identity. 
        //    Any properties that appear to be foreign key ids should be scripted as Int data types and should have a foreign key constraint applied.";
        //var userPrompt =
        //    @"// My really big class!
        //    public class MySample 
        //    {
        //        public string? Description { get; set; }
        //        public string? ParentId {get; set; }
        //        public RoleType? Type {get;set }
        //        public DateTime? DateCreated {get; set; }
        //    }";
        //var assistantReply =
        //    @"CREATE TABLE [dbo].[MySample]
        //    (
        //    [Id] INT NOT NULL identity constraint PK_Inject PRIMARY KEY,
        //    [Description] NVARCHAR(MAX),
        //    [ParentId] INT NULL constraint FK_Inject_Game references Game(Id)
        //    [Type] INT NULL constraint FK_Inject_RoleType references RoleType(Id)
        //    [DateCreated] DATETIME2 NULL DEFAULT GETUTCDATE()
        //    )";

        //options.Messages.Add(new ChatMessage(ChatRole.System, systemMessage));
        //options.Messages.Add(new ChatMessage(ChatRole.User, userPrompt));
        //options.Messages.Add(new ChatMessage(ChatRole.Assistant, assistantReply));

        //var prompt =
        //    @"public class MyStuff 
        //    {
        //        public string? Name { get; set; } // 50 chars
        //        public string? Description {get; set; } // 200 chars
        //        public int? TypeId {get;set }
        //        public int? Quantity {get; set; }
        //        public float? TotalCost {get; set; }
        //        public int? StoreId {get; set; }
        //        public DateTime? DateCreated {get; set; }
        //    }";

        // Remember to comment out the prompt below if you uncomment the code above.
        #endregion

        var prompt = "Provide a one paragraph design for a car.";
        
        options.Messages.Add(new ChatMessage(ChatRole.User, prompt));

        Console.WriteLine($"P: {prompt}");

        var response = await client.GetChatCompletionsAsync(model, options);

        var completions = response.Value;
        var content = completions.Choices[0].Message.Content;

        return content;
    }
}