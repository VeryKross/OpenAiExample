using Microsoft.Extensions.Configuration;

namespace OpenAiExample;

// Keep it secret, keep it safe. It is precious to us.

// This class is used to retrieve your secrets from the .NET user secrets store or from environment variables.
// If running from Visual Studio, you can add these keys to the user secrets store by right-clicking on the project
// in the Solution Explorer, selecting Manage User Secrets, and adding the following JSON:
// {
//  "OpenAIKey": "<your OpenAI.com key value>",
//  "AzureOpenAIKey": "<your Azure OpenAI key value>",
//  "AzureOpenAIUrl": "<your Azure OpenAI deployment URL>",
//  "AzureOpenAIModel": "<the name of your deployed Azure OpenAI GPT model>"
// }

public static class KeyManager
{
    private static readonly IConfigurationBuilder ConfigurationBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
    private static readonly IConfiguration Config = ConfigurationBuilder.Build();

    public static bool UseAzure { get; set; } = true;

    // This is the key that you get from the OpenAI or Azure websites, depending the value of UseAzure.
    public static string SecretKey => UseAzure ? Config["AzureOpenAIKey"] : Config["OpenAIKey"];

    // These are the URLs and model names for the Azure OpenAI service. They are different from the OpenAI service
    // and are only needed if you're using the Azure service because they'll be unique to your Azure deployment.
    public static string AzureOpenAIUrl => Config["AzureOpenAIUrl"];
    public static string AzureOpenAIModel => Config["AzureOpenAIModel"];
}