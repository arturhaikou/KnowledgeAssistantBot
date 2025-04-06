using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
//var aiKey = builder.AddParameter("AI-KEY");
var kernelMemory = builder.AddContainer("kernel-memory", "kernelmemory/service")
    .WithEnvironment("KernelMemory__TextGeneratorType", "OpenAI")
    .WithEnvironment("KernelMemory__DataIngestion__EmbeddingGeneratorTypes__0", "OpenAI")
    .WithEnvironment("KernelMemory__Retrieval__EmbeddingGeneratorType", "OpenAI")
    .WithEnvironment("KernelMemory__Services__OpenAI__APIKey", "xxx")
    .WithEnvironment("KernelMemory__Services__OpenAI__Endpoint", "https://models.inference.ai.azure.com")
    .WithEnvironment("KernelMemory__Services__OpenAI__TextModel", "gpt-4o-mini")
    .WithEnvironment("KernelMemory__Services__OpenAI__EmbeddingModel", "text-embedding-3-small")
    .WithHttpEndpoint(port: 9001, targetPort: 9001, name: "http");

var endpoint = kernelMemory.GetEndpoint("http");

builder.AddProject<Projects.BotsDemo_KnowledgeAssistant>("botsdemo-knowledgeassistant")
    .WithEnvironment("KernelMemory__Host", endpoint)
    .WaitFor(kernelMemory);

builder.Build().Run();
