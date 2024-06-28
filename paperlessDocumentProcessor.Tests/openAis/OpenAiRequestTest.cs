using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using paperlessDocumentProcessor.openAis;
using Xunit;

namespace paperlessDocumentProcessor.Tests.openAis;

public class OpenAiRequestTest
{

    [Fact]
    public void Serialization()
    {
        var request = new OpenAiRequest()
        {
            Model = "openaiModel",
            Messages = new List<Message>()
            {
                new(){ Role = "system", Content = "systemRoleMessage"},
                new(){ Role = "user", Content = "originalContent"}
            },
            Temperature = 0.7
        };

        // Serialize the request to JSON
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
        
        var root = JsonSerializer.Serialize(request, options);

        Assert.NotNull(root);
        Assert.Equal("{\"Model\":\"openaiModel\",\"Messages\":[{\"Role\":\"system\",\"Content\":\"systemRoleMessage\"},{\"Role\":\"user\",\"Content\":\"originalContent\"}],\"Temperature\":0.7}",root);
    }
}