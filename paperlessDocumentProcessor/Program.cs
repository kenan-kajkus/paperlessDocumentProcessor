using System.Net.Http.Headers;
using paperlessDocumentProcessor;
using paperlessDocumentProcessor.openAis;
using paperlessDocumentProcessor.paperless;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.AddConfig<OpenAiConfig>();
builder.AddConfig<PaperlessConfig>();
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddOptions()
    .AddScoped<DocumentProcessor>()
    .AddScoped<IPaperlessClient, PaperlessClient>()
    .AddScoped<IOpenAiClient, OpenAiClient>()
    .AddHttpClient<IPaperlessClient,PaperlessClient>(
        httpClient =>
        {
            var config = builder.GetConfig<PaperlessConfig>();
            httpClient.BaseAddress = new Uri(config.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", config.ApiKey);
        });

builder.Services.AddHttpClient<IOpenAiClient,OpenAiClient>(
    httpClient =>
    {

        var openAiConfig = builder.GetConfig<OpenAiConfig>();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", openAiConfig.ApiKey);
        httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", openAiConfig.Organization);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/process/{documentId:int}", async (int documentId, DocumentProcessor documentProcessor) =>
    {
        try
        {
            await documentProcessor.Process(documentId);
        }
        catch (NoDocumentFoundException e)
        {
            return Results.NotFound(e.Message);
        }
        
        return Results.Ok();
    })
    .WithName("process")
    .WithOpenApi();

app.Run();