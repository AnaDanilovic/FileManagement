namespace FileGenerator;

public static class NumberGenerator
{    
    public static List<int> GenerateNumbers(long count)
    {
        Random random = new Random();
        List<int> numbers = new List<int>();

        for (int i = 0; i < count; i++)
        {
            numbers.Add(random.Next(0, (int)(count / 2)));
        }

        return numbers;
    }
}
