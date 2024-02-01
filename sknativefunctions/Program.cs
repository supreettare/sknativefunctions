using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;
using System.Runtime;
using sknativefunctions.plugins;




const string azureOpenAIDeploymentName = "<DeploymentName>";
const string azureOpenAIEndpoint = "<EndpointName>";
const string azureOpenAIAPIKey = "<AzureOPenAIKey>;
const string azureOPenAIModelId = "gpt-35-turbo";









//1. Create builder 
var builder = Kernel.CreateBuilder();

//2. Configure it with the AI services 
builder.Services.AddAzureOpenAIChatCompletion(
     azureOpenAIDeploymentName,
     azureOpenAIEndpoint,
     azureOpenAIAPIKey
    );


//3. Create the kernel using the provided settings 
var kernel = builder.Build();



//4. Create Plugins 

//Symantic Functions 
//CityDetectionPlugin / FindCity /
//WriterPlugin / ShortPoem

//Native Functions 
//CityInfo
//WeatherService


//5. Load the plugin 
var cityDetectionPluginPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "..", "plugins", "CityDetectionPlugin");
var writerPluginPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "..", "plugins", "WriterPlugin");


var detectCityNameFunction = kernel.ImportPluginFromPromptDirectory(cityDetectionPluginPath);
var summaryPlugin = kernel.ImportPluginFromPromptDirectory(writerPluginPath);
var weatherPlugin = kernel.ImportPluginFromType<WeatherService>();
var cityNamePlugin = kernel.ImportPluginFromType<CityInfo>();

//6. Construct the arguments 
var arguments = new KernelArguments();

//7. Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

//8. Create planner 
#pragma warning disable SKEXP0003, SKEXP0011, SKEXP0052, SKEXP0060
var planner = new HandlebarsPlanner();

//9. Start a chat 
while (true)
{
    //7. Start with a basic chat    
    var readUserInput = Console.ReadLine();

    Func<string, Task> Chat = async (string input) =>
    {
        arguments["input"] = input;

        var originalPlan = await planner.CreatePlanAsync(kernel, input);

        Console.WriteLine("Original plan:\n");
        Console.WriteLine(originalPlan);


        //7. Execute the Original plan 
        var originalPlanResult = await originalPlan.InvokeAsync(kernel, new KernelArguments(openAIPromptExecutionSettings));

        Console.WriteLine("Original Plan results:\n");
        Console.WriteLine(originalPlanResult.ToString());
    };

    await Chat(readUserInput);
}
