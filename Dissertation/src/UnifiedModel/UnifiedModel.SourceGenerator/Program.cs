using System;

namespace UnifiedModel.SourceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine("Invalid arguments passed!");
            }
            else
            {
                LexicalAnalyser lexicalAnalyser = new LexicalAnalyser(args[0]);
                lexicalAnalyser.Process(args[1]);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadLine();
        }
    }
}
