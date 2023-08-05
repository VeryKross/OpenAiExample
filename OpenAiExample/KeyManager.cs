using Microsoft.Extensions.Configuration;

namespace OpenAiExample;

// Keep it secret, keep it safe. It is precious to us.
public static class KeyManager
{
    private static readonly IConfigurationBuilder ConfigurationBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
    private static readonly IConfiguration Config = ConfigurationBuilder.Build();

    // This is the key that you get from the OpenAI website.
    // It's a secret, so we don't want to check it into source control.
    // Instead, we'll use the .NET user secrets feature to store it locally.
    public static string SecretKey = Config["APIKey"];
}