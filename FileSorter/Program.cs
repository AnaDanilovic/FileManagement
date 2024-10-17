using System.Diagnostics;

namespace FileSorter;

public static class Program
{
    const long MaxMemorySize = 100 * 1024 * 1024; // 100MB chunks

    //const long MaxMemorySize = 100;

    public static async Task Main(string[] args)
    {
        try
        {
            Stopwatch sw = Stopwatch.StartNew();

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string inputFilePath = Path.Combine(docPath, "TextFile.txt");
            string outputFilePath = Path.Combine(docPath, "SortedTextFile.txt"); ;
            string tempDir = Path.Combine(docPath, "temp");

            List<string> tempFiles = SortFileInChunks(inputFilePath, tempDir);

            MergeSortedFiles(tempFiles, outputFilePath);

            CleanupTempFiles(tempFiles);
            sw.Stop();
            Console.WriteLine($"Time elapsed: {sw.Elapsed}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}, stack trace: {ex.StackTrace}");
        }
    }

    private static List<string> SortFileInChunks(string inputFilePath, string tempDir)
    {
        var tempFiles = new List<string>();
        var lines = new List<string>();
        long currentSize = 0;
        int chunkIndex = 0;

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
                currentSize += line.Length;

                if (currentSize > MaxMemorySize)
                {
                    var sortedChunk = SortChunk(lines);
                    var tempFile = WriteChunk(sortedChunk, tempDir, chunkIndex++);
                    tempFiles.Add(tempFile);
                    lines.Clear();
                    currentSize = 0;
                }
            }

            if (lines.Count > 0)
            {
                var sortedChunk = SortChunk(lines);
                var tempFile = WriteChunk(sortedChunk, tempDir, chunkIndex++);

                tempFiles.Add(tempFile);
            }
        }

        return tempFiles;
    }

    private static List<string> SortChunk(List<string> lines)
    {
        var extractedLines = new (string Original, int Number, string Text)[lines.Count];

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];

            try
            {
                int dotIndex = line.IndexOf('.');
                int number = int.Parse(line.AsSpan(0, dotIndex));
                string text = line.Substring(dotIndex + 2);

                extractedLines[i] = (Original: line, Number: number, Text: text);
            }
            catch (Exception ex) 
                {
                Console.WriteLine(line);
                throw;
                }
        }

        Array.Sort(extractedLines, (x, y) =>
        {
            int textComparison = string.Compare(x.Text, y.Text, StringComparison.Ordinal);
            if (textComparison != 0) return textComparison;
            return x.Number.CompareTo(y.Number);
        });

        return extractedLines.Select(x => x.Original).ToList();
    }



    private static string WriteChunk(List<string> sortedLines, string tempDir, int chunkIndex)
    {
        string tempFilePath = Path.Combine(tempDir, $"chunk_{chunkIndex}.txt");
        File.WriteAllLines(tempFilePath, sortedLines);

        return tempFilePath;
    }

    private static void MergeSortedFiles(List<string> tempFiles, string outputFilePath)
    {
        var readers = tempFiles.Select(file => new StreamReader(file)).ToList();
        var priorityQueue = new PriorityQueue<(string Text, int Number, int Index), (string Text, int Number)>();
        int index = 0;

        foreach (var reader in readers)
        {
            if (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                var key = ParseLine(line, index);
                priorityQueue.Enqueue((line, key.Number, index), (key.Text, key.Number));
                index++;
            }
        }

        using (var writer = new StreamWriter(outputFilePath))
        {
            while (priorityQueue.Count > 0)
            {
                var (line, number, idx) = priorityQueue.Dequeue();

                writer.WriteLine(line);

                var reader = readers[idx];
                if (!reader.EndOfStream)
                {
                    string nextLine = reader.ReadLine();
                    var nextKey = ParseLine(nextLine, idx);
                    priorityQueue.Enqueue((nextLine, nextKey.Number, idx), (nextKey.Text, nextKey.Number));
                }
            }
        }

        // Clean up readers
        foreach (var reader in readers)
        {
            reader.Close();
        }
    }

    private static (string Text, int Number) ParseLine(string line, int index)
    {
        int dotIndex = line.IndexOf('.');
        int number = int.Parse(line.AsSpan(0, dotIndex));
        string text = line.Substring(dotIndex + 2);
        return (text, number);
    }


    private static void CleanupTempFiles(List<string> tempFiles)
    {
        foreach (var tempFile in tempFiles)
        {
            File.Delete(tempFile);
        }
    }


}