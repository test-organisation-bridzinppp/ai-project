using BuildingBlocks.Application.Ports;
using Infrastructure;
using Microsoft.SemanticKernel;
using Application;
using Azure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var openAiApiKey = builder.Configuration["AzureOpenAI:ApiKey"];
var openAiEndpoint = builder.Configuration["AzureOpenAI:Endpoint"];
var openAiModel = builder.Configuration["AzureOpenAI:Model"];
var openAiDeploymentName = builder.Configuration["AzureOpenAI:DeploymentName"];

var openAiEmbeddingsApiKey = builder.Configuration["AzureOpenAIEmbeddings:ApiKey"];
var openAiEmbeddingsEndpoint = builder.Configuration["AzureOpenAIEmbeddings:Endpoint"];
var openAiEmbeddingsModel = builder.Configuration["AzureOpenAIEmbeddings:Model"];
var openAiEmbeddingsDeploymentName = builder.Configuration["AzureOpenAIEmbeddings:DeploymentName"];

var documentAnalysisApiKey = builder.Configuration["AzureAIDocumentAnalysis:ApiKey"];
var documentAnalysisEndpoint = builder.Configuration["AzureAIDocumentAnalysis:Endpoint"];

var azureAISearchApiKey = builder.Configuration["AzureAISearch:ApiKey"];
var azureAISearchEndpoint = builder.Configuration["AzureAISearch:Endpoint"];


ValidateConfiguration(openAiApiKey, nameof(openAiApiKey));
ValidateConfiguration(openAiEndpoint, nameof(openAiEndpoint));
ValidateConfiguration(openAiModel, nameof(openAiModel));
ValidateConfiguration(openAiDeploymentName, nameof(openAiDeploymentName));
ValidateConfiguration(openAiEmbeddingsApiKey, nameof(openAiEmbeddingsApiKey));
ValidateConfiguration(openAiEmbeddingsEndpoint, nameof(openAiEmbeddingsEndpoint));
ValidateConfiguration(azureAISearchEndpoint, nameof(azureAISearchEndpoint));
ValidateConfiguration(azureAISearchApiKey, nameof(azureAISearchApiKey));

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
            deploymentName: openAiEmbeddingsDeploymentName!,
            apiKey: openAiEmbeddingsApiKey!,
            endpoint: openAiEmbeddingsEndpoint!)
        .Build();
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    return kernel;
});

builder.Services.AddSingleton<IChatCompletionProvider, OpenApiChatCompletionProvider>();
builder.Services.AddTransient<ITextEmbeddingsProvider, OpenAITextEmbeddingProvider>();
builder.Services.AddSingleton<IPdfRecognizer>(sp => new PdfRecognizer(documentAnalysisEndpoint!, documentAnalysisApiKey!));
builder.Services.AddSingleton<IVectorDatabase>(sp =>
{
    var endpoint = new Uri(azureAISearchEndpoint!);
    var key = new AzureKeyCredential(azureAISearchApiKey!);
    return new VectorDatabase(endpoint, key);
});

builder.Services.AddTransient<IStorage, Storage>();
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
