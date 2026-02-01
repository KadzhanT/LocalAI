using OpenAI.Chat;
using LearnMSAI.Console;
using OpenAI;
using System.ClientModel;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton(sp => new ChatClient(
    model: "llama3.2:latest",
    credential: new ApiKeyCredential("Ollama"),
    options: new OpenAIClientOptions()
    {
        Endpoint = new Uri("http://localhost:11434/v1")
    }
));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/hello", () => "Сервер работает! ");

app.MapPost("/chat", async (ChatRequest request, ChatClient client, HttpContext context) =>
    {
        var history = new List<ChatMessage> { new UserChatMessage(request.Message) };
        context.Response.ContentType = "text/plain";

        var updates = client.CompleteChatStreamingAsync(history);

        await foreach (var update in updates)
        {
            if (update.ContentUpdate.Count > 0)
            {
                await context.Response.WriteAsync(update.ContentUpdate[0].Text);
                await context.Response.Body.FlushAsync();
            }
        }


    })
.WithName("PostChat");



app.Run();
public record ChatRequest(string Message);
