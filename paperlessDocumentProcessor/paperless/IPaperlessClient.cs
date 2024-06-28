namespace paperlessDocumentProcessor.paperless;

public interface IPaperlessClient
{
    Task<PaperlessDocument> GetPaperlessDocument(int documentId);
    Task<bool> PatchTitle(int documentId, string openAiResponseContent);
}

public class PaperlessClient(
    ILogger<PaperlessClient> logger,
    HttpClient httpClient) : IPaperlessClient
{
    public async Task<PaperlessDocument> GetPaperlessDocument(int documentId)
    {
        logger.LogInformation("Document ID {DocumentId}: Reading document details from paperless", documentId);
        var documentUrl = $"/api/documents/{documentId}/";
        var httpResponse = await httpClient.GetAsync(documentUrl);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new NoDocumentFoundException($"Document ID {documentId}: Document does not exist!");
        }

        return await httpResponse.Content.ReadFromJsonAsync<PaperlessDocument>() ?? throw new InvalidOperationException();
    }

    public async Task<bool> PatchTitle(int documentId, string title)
    {   
        var documentUrl = $"/api/documents/{documentId}/";
        var patchData = new PatchData()
        {
            Title = title
        };
        var httpResponse = await httpClient.PatchAsJsonAsync(documentUrl, patchData);
        if (httpResponse.IsSuccessStatusCode)
        {
            logger.LogInformation("Document ID {DocumentId}: Successfully updated title", documentId);
        }
        else
        {
            logger.LogError("Document ID {DocumentId}: Error updating the document! Status code {UpdateResponseStatusCode}", documentId, httpResponse.StatusCode);
        }
        return httpResponse.IsSuccessStatusCode;
    }
}

public class NoDocumentFoundException(string s) : Exception(s);