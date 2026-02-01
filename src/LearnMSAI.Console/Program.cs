using System.ClientModel;
using OpenAI.Chat;
using OpenAI;
using System.Text;
using LearnMSAI.Console;


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
Console.WriteLine("Команды: /file [путь], /clear, exit");

while (true)
{
    Console.Write("You: ");

    string Input = Console.ReadLine()?.Trim() ?? "";

    //Проверка
    if (Input.ToLower() is "exit") break;

    if (string.IsNullOrEmpty(Input)) continue;

    if (Input.ToLower() is "clear")
    {
        history.Clear();
        Console.WriteLine("[Система]: История очищена.");
        continue;
    }

    string finalPrompt = Input;

    //Work with files
    if (Input.StartsWith("/file"))
    {
        try
        {
            if (Input.Length <= 6) throw new Exception("Укажите путь к файлу после команды /file");
            string path = Input.Substring(6).Trim();
            string content = FileService.GetFileContent(path);

            finalPrompt = $"Проанализируй файл {Path.GetFileName(path)}: \n\n``` {content}```";
            Console.WriteLine($"[Система]: Файл загружен. (Text/PDF)");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Ошибка]: {ex.Message}");
            continue;
        }
    }
    //Ответ ИИ
    history.Add(new UserChatMessage(finalPrompt));
    Console.Write("Assistant: ");
    StringBuilder assistantResponse = new();

    try
    {

        var completionUpdates = client.CompleteChatStreamingAsync(history);




        await foreach (StreamingChatCompletionUpdate update in completionUpdates)
        {
            if (update.ContentUpdate.Count > 0)
            {
                string text = update.ContentUpdate[0].Text;
                Console.Write(text);
                assistantResponse.Append(text);

            }

        }
        Console.WriteLine();
        history.Add(new AssistantChatMessage(assistantResponse.ToString()));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[Ошибка API]: Убедитесь, что Ollama запущена. {ex.Message}");
        if (history.Count > 0) history.RemoveAt(history.Count - 1);
    }
    
    
}
;

//ChatCompletion completion = await client.CompleteChatAsync(userMessage);
//Console.WriteLine(completion.Content[0].Text);