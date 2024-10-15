namespace FileGenerator;
public class WordBank
{
    private static Random random = new Random();

    private static readonly string[] adjectives = { "yellow", "sweet", "tropical", "delicious", "juicy", "best", "ripe", "red", "green", "fresh" };
    private static readonly string[] nouns = { "Banana", "Cherry", "Apple", "Orange", "Pineapple", "Blueberry", "Watermelon", "Mango", "Strawberry", "Grape" };
    private static readonly string[] verbs = { "is", "looks", "tastes", "smells", "feels" };

    public static List<string> GenerateSentences(int count)
    {
        List<string> sentences = new List<string>();

        for (int i = 0; i < count; i++)
        {
            if (random.Next(2) == 0)  // 50% chance for single word
            {
                sentences.Add(GenerateSingleWord());
            }
            else
            {
                sentences.Add(GenerateSentence());
            }
        }

        return sentences;
    }

    public static string GenerateSingleWord()
    {
        return nouns[random.Next(nouns.Length)];
    }

    public static string GenerateSentence()
    {
        string noun = nouns[random.Next(nouns.Length)];
        string verb = verbs[random.Next(verbs.Length)];
        string adjective = adjectives[random.Next(adjectives.Length)];

        return $"{noun} {verb} {adjective}";
    }
}
