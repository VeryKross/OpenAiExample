using Microsoft.Extensions.Configuration;

namespace OpenAiExample;

// Keep it secret, keep it safe. It is precious to us.
public static class KeyManager
{
    private static readonly IConfigurationBuilder ConfigurationBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
    private static readonly IConfiguration Config = ConfigurationBuilder.Build();

    public static bool UseAzure { get; set; } = true;

    // This is the key that you get from the OpenAI or Azure websites.
    // It's a secret, so we don't want to check it into source control.
    // Instead, we'll use the .NET user secrets feature to store it locally.
    public static string SecretKey => UseAzure ? Config["AzureOpenAIKey"] : Config["OpenAIKey"];

    // These are the URLs and model names for the Azure OpenAI service. They are different from the OpenAI service
    // and are only needed if you're using the Azure service because they'll be unique to your Azure deployment.
    public static string AzureOpenAIUrl => Config["AzureOpenAIUrl"];
    public static string AzureOpenAIModel => Config["AzureOpenAIModel"];
}