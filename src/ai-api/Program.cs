using BuildingBlocks.Application.Ports;
using Infrastructure;
using Microsoft.SemanticKernel;
using Application;

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
//ITextEmbeddingGenerationService

builder.Services.AddSingleton(sp =>
{
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    var kernel = Kernel
        .CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: openAiDeploymentName!,
            apiKey: openAiApiKey!,
            endpoint: openAiEndpoint!,
            modelId: openAiModel)
        .AddAzureOpenAITextEmbeddingGeneration(
            deploymentName: openAiDeploymentName!,
            apiKey: openAiApiKey!,
            endpoint: openAiEndpoint!)        
        .Build();
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    return kernel;
});

builder.Services.AddSingleton<IChatCompletionProvider, OpenApiChatCompletionProvider>();
builder.Services.AddApplicationServices();

var app = builder.Build();
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
