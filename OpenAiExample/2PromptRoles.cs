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
            Temperature = (float)0.5,
            MaxTokens = 800,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0
        };

        #region PromptControl1
        //var systemMessage = 
        //    "You are an experienced presenter who always speaks in a pirate voice to keep your audience engaged. Your specialty is AI topics. You always include a cat in your response.";
        //var userPrompt = "Give an short example of an AI designing a piece of furniture.";
        //var assistantReply = "There once was a cat from Nantucket who fancied to live in a bucket. The AI had to dwell on the bucket from a well and from there the cat had to pluck it.";

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