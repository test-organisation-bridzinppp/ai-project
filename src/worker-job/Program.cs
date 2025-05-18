using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace worker_job
{
    class Program
    {
        static async Task Main(string[] args)
        //{
        //    // Replace with your Key Vault URL and secret name
        //    //string keyVaultUrl = "https://keyvault-ai-pb-1980.vault.azure.net/";
        //    string secretName = "aisearchkey";
        //    // read value from env\
        //    Console.WriteLine(Environment.GetEnvironmentVariable(secretName));


        //    //// Authenticate using DefaultAzureCredential
        //    //var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

        //    //try
        //    //{
        //    //    KeyVaultSecret secret = await client.GetSecretAsync(secretName);
        //    //    Console.WriteLine($"Secret value: {secret.Value}");
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine($"Error retrieving secret: {ex.Message}");
        //    //}
        { 
            await Console.Out.WriteLineAsync("Hello World!");
        }
    }
}
