using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var openAiApiKey = builder.Configuration["AzureOpenAI:ApiKey"];
var openAiEndpoint = builder.Configuration["AzureOpenAI:Endpoint"];
var openAiModel = builder.Configuration["AzureOpenAI:Model"];
var openAiDeploymentName = builder.Configuration["AzureOpenAI:DeploymentName"];

ValidateConfiguration(openAiApiKey, nameof(openAiApiKey));
ValidateConfiguration(openAiEndpoint, nameof(openAiEndpoint));
ValidateConfiguration(openAiModel, nameof(openAiModel));
ValidateConfiguration(openAiDeploymentName, nameof(openAiDeploymentName));

builder.Services.AddSingleton(sp =>
{
    var kernel = Kernel
        .CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: "NAME_OF_YOUR_DEPLOYMENT",
            apiKey: "YOUR_API_KEY",
            endpoint: "YOUR_AZURE_ENDPOINT",
            modelId: "gpt-4")
        .Build();
    return kernel;
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthy");
app.MapHealthChecks("/ready");
app.Run();

void ValidateConfiguration(string? value, string name)
{
    if (string.IsNullOrEmpty(value))
    {
        throw new ArgumentNullException(name, $"{name} is missing.");
    }
}
