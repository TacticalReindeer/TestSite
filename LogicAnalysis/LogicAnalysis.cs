using System;
using System.Collections.Generic;

public class Program
{
    enum Logic
    {
        and, or, xor
    }
    static void Main(string[] args)
    {
        Console.WriteLine("\nlogicAnalysis:");

        string input = "";

        for (; ; )
        {
            string str = Console.ReadLine();
            if (str == null) break;
            input += str;
        }

        List<string> variables = new List<string>();
        List<int> varPositions = new List<int>();

        Console.CursorTop++;

        // print header row
        {
            string[] labels;
            labels = input.Split(new[] { ' ', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int readPos = 0; readPos != labels.Length; readPos++)
            {
                switch (labels[readPos])
                {
                    case "(":
                    case ")":
                    case "or":
                    case "and":
                    case "not":
                    case "xor":
                        break;
                    default:
                        Console.Write(" " + labels[readPos]);
                        variables.Add(labels[readPos]);
                        varPositions.Add(Console.CursorLeft - 1);
                        break;
                }
            }

            varPositions.Add(Console.CursorLeft + 2);
            Console.WriteLine("  Q\n");
        }

        // print truth table
        for (int i = 0, max = (int)Math.Pow(2, variables.Count); i < max; i++)
        {
            string binary = ToBinary(i);
            int binaryPos = 0;

            Console.WriteLine(GetResult(input) ? "1" : "0");

            string ToBinary(int x)
            {
                char[] buff = new char[32];

                for (int i = 31; i >= 0; i--)
                {
                    int mask = 1 << i;
                    buff[31 - i] = (x & mask) != 0 ? '1' : '0';
                }

                return new string(buff);
            }
            bool GetResult(string arg)
            {
                string[] labels = arg.Split(new[] { ' ', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

                int packLayer = 0;
                string pack = "";
                bool result = false, not = false, isPacking = false;
                Logic logic = Logic.or;

                for (int readPos = 0; readPos != labels.Length; readPos++)
                    switch (labels[readPos])
                    {
                        case "(":
                            if (isPacking) pack += " " + labels[readPos];
                            else isPacking = true;
                            packLayer++;
                            break;
                        case ")":
                            if (isPacking && packLayer > 1)
                            {
                                pack += " " + labels[readPos];
                                break;
                            }
                            else if (packLayer < 1) Console.WriteLine("Error: bracket ) not matched!");
                            isPacking = false;
                            packLayer--;

                            LogicGate(GetResult(pack) ^ not);

                            pack = "";
                            not = false;
                            break;

                        case "and":
                            if (isPacking) pack += " " + labels[readPos];
                            else logic = Logic.and;
                            break;
                        case "or":
                            if (isPacking) pack += " " + labels[readPos];
                            else logic = Logic.or;
                            break;
                        case "xor":
                            if (isPacking) pack += " " + labels[readPos];
                            else logic = Logic.xor;
                            break;
                        case "not":
                            if (isPacking) pack += " " + labels[readPos];
                            else not = true;
                            break;

                        default:
                            if (isPacking)
                            {
                                pack += " " + labels[readPos];
                                break;
                            }
                            char bin = binary[(binary.Length - variables.Count) + binaryPos];
                            Console.CursorLeft = varPositions[binaryPos];
                            Console.Write(bin.ToString());

                            LogicGate((bin == '1') ^ not);

                            not = false;
                            binaryPos++;
                            break;
                    }
                if (packLayer != 0) Console.WriteLine("Error: bracket ( not matched!");

                Console.CursorLeft = varPositions[varPositions.Count - 1];
                return result;

                void LogicGate(bool input)
                {
                    switch (logic)
                    {
                        case Logic.and:
                            result &= input;
                            break;
                        case Logic.or:
                            result |= input;
                            break;
                        case Logic.xor:
                            result ^= input;
                            break;
                    }
                }
            }
        }

        Main(null);
    }
}