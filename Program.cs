using System.ClientModel;
using OpenAI.Chat;
using OpenAI;
using System.Text;

//Ollama
ChatClient client = new(
    model: "llama3.2:latest",
    credential: new ApiKeyCredential("Ollama"),
    options: new OpenAIClientOptions()
    {
        Endpoint = new Uri("http://localhost:11434/v1")
    }
);
//OPenAI
//string apikey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
//ChatClient client = new (model:"gpt-4o-mini", apikey);

List<ChatMessage> history = new();
Console.WriteLine("--- Chat started (Type 'exit' or 'quit' to stop) ---");

while (true)
{

    string rawInput = Console.ReadLine();
    string userMessage;

    if (rawInput != null)
    {
        userMessage = rawInput.Trim();
    }
    else
    {
        userMessage = "";
    }

    if (string.IsNullOrEmpty(userMessage)) continue;
    if (userMessage.ToLower() is "exit" or "quit") break;

    history.Add(new UserChatMessage(userMessage));
    var completionUpdates = client.CompleteChatStreamingAsync(history);

    StringBuilder assistantResponce = new();


    await foreach (StreamingChatCompletionUpdate update in completionUpdates)
    {
        if (update.ContentUpdate.Count > 0)
        {
            string text = update.ContentUpdate[0].Text;
            Console.Write(text);
            assistantResponce.Append(text);

        }
        
    }
    Console.WriteLine();
    history.Add(new AssistantChatMessage(assistantResponce.ToString()));
}
;

//ChatCompletion completion = await client.CompleteChatAsync(userMessage);
//Console.WriteLine(completion.Content[0].Text);