using paperlessDocumentProcessor.openAis;
using paperlessDocumentProcessor.paperless;

namespace paperlessDocumentProcessor;

public class DocumentProcessor(
    ILogger<DocumentProcessor> logger,
    IPaperlessClient paperlessClient,
    IOpenAiClient openAiClient)
{
    public async Task Process(int documentId)
    {
        logger.LogInformation("Document ID {DocumentId}: Processing document",documentId);

        var documentJson = await paperlessClient.GetPaperlessDocument(documentId);

        var originalContent = documentJson.Content;

        var suggestedTitle = await openAiClient.GetTitleSuggestion(originalContent);
        
        logger.LogInformation("Document ID {DocumentId}: OpenAI title suggestion: {SuggestedTitle}", documentId,
            suggestedTitle);

        await paperlessClient.PatchTitle(documentId, suggestedTitle);
    }
}