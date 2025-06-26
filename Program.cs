using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using WinAI.Plugins;
using DotNetEnv;
public class Program
{
    public static async Task Main()
    {
        //SpeechRecognitionApp.SpeechRecognition.Run();
        //return;

        // get env path
        var basePath = AppContext.BaseDirectory;
        var envPath = Path.Combine(basePath, "..", "..", "..", ".env");

        Env.Load(envPath);
        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
            deploymentName: Environment.GetEnvironmentVariable("DEPLOYMENT_NAME"),
            endpoint: Environment.GetEnvironmentVariable("BASE_URL"),
            apiKey: Environment.GetEnvironmentVariable("API_KEY"),
            apiVersion: Environment.GetEnvironmentVariable("API_VERSION"),
            serviceId: Environment.GetEnvironmentVariable("SERVICE_NAME")
        );

        builder.Plugins.AddFromType<SpotifyPlugin>();
        var kernel = builder.Build();


        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        ChatHistory chatMessages = new ChatHistory();


        //var spot = new SpotifyPlugin();
        //Console.WriteLine(spot.OpenSpotify());

        //Thread.Sleep(2000);

        //Console.WriteLine(spot.PlayMusic());

        //Thread.Sleep(2000);

        //Console.WriteLine(spot.PlayNextMusic());

        //Thread.Sleep(2000);

        //Console.WriteLine(spot.PlayPreviousMusic());

        //Thread.Sleep(2000);

        //Console.WriteLine(spot.CloseSpotify());

        //Thread.Sleep(2000);

        //Console.WriteLine(spot.CloseSpotify());

        while (true)
        {
            Console.Write("User:> ");
            var input = Console.ReadLine();

            chatMessages.AddUserMessage(input);

            var completion = chatService.GetStreamingChatMessageContentsAsync(
                chatHistory: chatMessages,
                executionSettings: new OpenAIPromptExecutionSettings
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                },
                kernel: kernel);

            string fullMessage = "";
            await foreach (var message in completion)
            {
                Console.Write(message.Content);
                fullMessage += message.Content;
            }

            chatMessages.AddAssistantMessage(fullMessage);
            Console.WriteLine();
        }
    }
}
