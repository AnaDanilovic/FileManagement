namespace FileGenerator;

public static class StringGenerator
{  
    private static readonly List<string> wordBank = WordBank.GenerateSentences(100);

    private static Random random = new Random();
    public static string GenerateRandomWordOrPhrase()
    {
        int randomIndex = random.Next(wordBank.Count);
        return wordBank[randomIndex];
    }
    public static List<string> GenerateMultipleWordsOrPhrases(long count)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < count; i++)
        {
            result.Add(GenerateRandomWordOrPhrase());
        }

        return result;
    }


}
