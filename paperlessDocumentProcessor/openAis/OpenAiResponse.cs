namespace paperlessDocumentProcessor.openAis;

public class OpenAiResponse
{
    public List<Choice> Choices { get; set; }
}

public class Choice
{
    public Message Message { get; set; }
}