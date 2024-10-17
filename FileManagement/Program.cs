using Microsoft.Extensions.Configuration;

namespace FileGenerator;
public static class Program
{
    
    public static async Task Main()
    {
        try
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            string basePath = configuration["basePath"];
            Directory.CreateDirectory(basePath);

            long result = PromptForNumber();
            await  FileGenerator.WriteFile(basePath, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ ex.Message.ToString()}. Stack trace: {ex.StackTrace}");
        }
    }

    public static long PromptForNumber()
    {
        Console.WriteLine("Please enter the desired number of lines: ");
        string input = Console.ReadLine();
        long number;

        if (!string.IsNullOrEmpty(input) && Int64.TryParse(input, out number))
        {
            return number;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid integer number.");
            return PromptForNumber();  
        }
    }
}
    
