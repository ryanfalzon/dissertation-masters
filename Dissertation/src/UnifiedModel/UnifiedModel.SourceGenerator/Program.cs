using System;

namespace UnifiedModel.SourceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Invalid parameters!");
            }
            else
            {
                LexicalAnalyser lexicalAnalyser = new LexicalAnalyser(args[0]);
                lexicalAnalyser.Process();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadLine();
        }
    }
}
