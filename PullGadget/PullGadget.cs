using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

public class PullCardGadget
{
    static readonly string rootFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\";
    static Random random = new Random();

    public static void Main(string[] labels)
    {
        Console.Write("\n");
        List<string> files = new List<string>();
        int repeat = 1;
        for (int readPos = 0; readPos < labels.Length; readPos++)
        {
            switch (labels[readPos])
            {
                case "/repeat":
                    repeat = int.Parse(labels[++readPos]);
                    break;
                default:
                    string file = rootFolder + labels[readPos] + ".txt";
                    if (File.Exists(file)) files.Add(file);
                    else Console.WriteLine($"file named {labels[readPos]} not exists!");
                    break;
            }
        }

        for (; repeat > 0; repeat--)
        {
            foreach (string file in files) Console.Write(Reader(File.ReadAllText(file)) + " ");
            Console.Write("\n");
        }
    }

    static string Reader(string input)
    {
        string[] lines = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<(string name, int proportion)> cards = new List<(string, int)>();

        foreach (string line in lines)
        {
            string[] labels = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            cards.Add((labels[0], labels.Length > 1 ? int.Parse(labels[1]) : 1));
        }

        int totalRange = 0;
        cards.ForEach(x => totalRange += x.proportion);

        int pullResult = random.Next(1, totalRange + 1);

        foreach ((string name, int proportion) card in cards)
        {
            pullResult -= card.proportion;
            if (pullResult < 1) return card.name;
        }
        return "unknow error";
    }
}