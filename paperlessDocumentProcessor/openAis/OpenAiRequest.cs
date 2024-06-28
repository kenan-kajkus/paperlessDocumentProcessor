namespace paperlessDocumentProcessor.openAis;

public class OpenAiRequest
{
    private double _temperature;
    public string Model { get; set; }
    
    public IList<Message> Messages { get; set; }
    
    public double Temperature { 
        get => _temperature;
        set => _temperature = value is < 0d or > 1d ? throw new ArgumentException() : value; }
}

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
}