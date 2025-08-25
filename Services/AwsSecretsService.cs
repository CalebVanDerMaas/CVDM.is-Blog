using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

namespace CVDMBlog.Services
{
    public interface IAwsSecretsService
    {
        Task<Dictionary<string, string>> GetSecretsAsync();
    }

    public class AwsSecretsService : IAwsSecretsService
    {
        private readonly IConfiguration _configuration;

        public AwsSecretsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Dictionary<string, string>> GetSecretsAsync()
        {
            string secretName = "prod/CVDMBlog";
            string region = "us-east-2";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                // For a list of the exceptions thrown, see
                // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
                throw e;
            }

            string secret = response.SecretString;

            return JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);
            
        }
    }
}

