namespace FileGenerator;

public static class FileGenerator
{
    private const long ChunkSize = 1_000_000;
    public static Dictionary<long, (long, string)> GenerateFile(long count, long startValue)
    {
        List<string> multipleWords = StringGenerator.GenerateMultipleWordsOrPhrases(count);
        List<int> numbers = NumberGenerator.GenerateNumbers(count);
        Dictionary<long, (long, string)> dictionary = new Dictionary<long, (long, string)>();

        for (int i = 0; i < count; i++)
        {
            dictionary.Add(startValue + i, (numbers[i], multipleWords[i]));
        }

        return dictionary;
    }

    public static async Task WriteFile(string basePath, long totalLineCount)
    {
        string filePath = Path.Combine(basePath, "TextFile.txt");

        using (StreamWriter outputFile = new StreamWriter(filePath, append: true))
        {
            long writtenLines = 0;

            while (writtenLines < totalLineCount)
            {
                long linesToWrite = Math.Min(ChunkSize, totalLineCount - writtenLines);
                var fileItems = GenerateFile(linesToWrite, writtenLines);
                foreach (var item in fileItems)
                {
                    await outputFile.WriteLineAsync($"{item.Value.Item1}. {item.Value.Item2}");
                }

                writtenLines += linesToWrite;
                Console.WriteLine($"Written {writtenLines} lines out of {totalLineCount}");
            }
        }
    }
}
