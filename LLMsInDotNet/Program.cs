using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

///https://www.milanjovanovic.tech/blog/working-with-llms-in-dotnet-using-microsoft-extensions-ai
///To run ollama docker image in a non GPU machine use the command below
///docker run -d -v ollama_data:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
///Note that the response is slow when there is no GPU

var builder = Host.CreateApplicationBuilder();
builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), "llama3"));

var app = builder.Build();

var chatHistory = new List<ChatMessage>();

var chatClient = app.Services.GetRequiredService<IChatClient>();

Console.WriteLine("Conntected to LLM");

while (true)
{
    Console.Write("Your prompt: ");
    var userPrompt = Console.ReadLine();
    if(userPrompt.Trim().ToUpper() == "EXIT")
    {
        break;
    }
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    Console.Write("AI: ");
    var chatResponse = "";
    await foreach(var item in chatClient.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        chatResponse += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
    Console.WriteLine();
}

//var chatCompletion = await chatClient.GetResponseAsync("What is Docker?  Explain in 100 words");

//Console.WriteLine(chatCompletion.Message.Text);

