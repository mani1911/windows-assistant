using DotNetEnv;
using Helpers;
using Microsoft.CognitiveServices.Speech;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics;
using WinAI.Plugins;
public class Program
{
    public static async Task Main()
    {
        // get env path
        var basePath = AppContext.BaseDirectory;
        var envPath = Path.Combine(basePath, "..", "..", "..", ".env");
        Env.Load(envPath);

        var speechRecognition = new SpeechRecognition(Environment.GetEnvironmentVariable("SPEECH_API_KEY"), Environment.GetEnvironmentVariable("SPEECH_REGION"));

        // get the services
        var recognizer = speechRecognition.GetSpeechRecognizer();
        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
            deploymentName: Environment.GetEnvironmentVariable("DEPLOYMENT_NAME"),
            endpoint: Environment.GetEnvironmentVariable("BASE_URL"),
            apiKey: Environment.GetEnvironmentVariable("API_KEY"),
            apiVersion: Environment.GetEnvironmentVariable("API_VERSION"),
            serviceId: Environment.GetEnvironmentVariable("SERVICE_NAME")

        );

        // adding all the plugins here
        builder.Plugins.AddFromType<SpotifyPlugin>();
        builder.Plugins.AddFromType<SnippingToolPlugin>();
        builder.Plugins.AddFromType<VolumePlugin>();
        var kernel = builder.Build();


        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        ChatHistory chatMessages = new ChatHistory();

        chatMessages.AddSystemMessage($"You are a helpful AI Windows Assistant, named {Environment.GetEnvironmentVariable("SERVICE_NAME")} that can perform some tasks like playing music from spotify and doing basic windows actions.");

        bool isListeningForCommand = false;

        recognizer.Recognized += async (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                string input = e.Result.Text.Trim().ToLowerInvariant();
                Debug.WriteLine($"Recognized: {input}");
                if (!isListeningForCommand)
                {
                    if (input.StartsWith("alexa"))
                    {
                        Debug.WriteLine("Wake word detected: entering command mode...");
                        isListeningForCommand = true;
                        Console.WriteLine("Listening for your command...");
                    }
                }
                else
                {
                    Debug.WriteLine($"Command received: {input}");
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

                    // Reset back to wake word mode after completing a command
                    isListeningForCommand = false;
                    Console.WriteLine("Say Alexa to activate again.");
                }
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine("No speech recognized.");
            }
        };

        await speechRecognition.StartSpeechRecognition();


        return;
    }
}
