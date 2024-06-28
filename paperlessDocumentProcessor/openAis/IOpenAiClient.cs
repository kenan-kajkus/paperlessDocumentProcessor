using Microsoft.Extensions.Options;

namespace paperlessDocumentProcessor.openAis;

public interface IOpenAiClient
{
    Task<string> GetTitleSuggestion(string originalContent);
}

public class OpenAiClient(ILogger<OpenAiClient> logger, HttpClient httpClient, IOptions<OpenAiConfig> options)
    : IOpenAiClient
{
    private readonly OpenAiConfig _openAiConfig = options.Value;

    public async Task<string> GetTitleSuggestion(string originalContent)
    {
        var openaiModel = _openAiConfig.Model;
        var openaiLanguage = _openAiConfig.Language;
        var systemRoleMessage = "You are an expert in analyzing texts. Your task is to create a title for " +
                                "the text provided by the user. Be aware that the text may result from an OCR " +
                                "process and contain imprecise segments. Avoid mentioning dates, any form of " +
                                "monetary values or specific names (such as individuals or organizations) in the " +
                                "title. Ensure the title does never exceed 128 characters." +
                                "Do not add or insert any special characters at the beginning and end of the title." +
                                "A bad example is 'title' or '/title/'." +
                                "Most importantly, generate the title in " + openaiLanguage + ".";
        
        var request = new OpenAiRequest()
        {
            Model = openaiModel,
            Messages = new List<Message>()
            {
                new(){ Role = "system", Content = systemRoleMessage},
                new(){ Role = "user", Content = originalContent}
            },
            Temperature = 0.7
        };
        
        var httpResponse = await httpClient.PostAsJsonAsync(_openAiConfig.BaseUrl, request);
        
        var openAiResponse = await httpResponse.Content.ReadFromJsonAsync<OpenAiResponse>();

        return openAiResponse?.Choices[0].Message.Content ?? throw new InvalidOperationException();
    }
}