using System;
using System.IO;
using System.Text.RegularExpressions;

using WDict = System.Collections.Generic.Dictionary<string, uint>;
using TDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, uint>>;

namespace Markov
{
    class Program
    {
        static public Random r { get; private set; }
        static void Main(string[] args)
        {
            r = new Random();
            int limit, size;
            bool exact = false;

            if (args.Length < 4) { printUsage(); return; }
            string  inFile = args[0];
            string outFile = args[1];
            if (!int.TryParse(args[2], out limit)) { printUsage(); return; }
            if (!int.TryParse(args[3], out size)) { printUsage(); return; }
            if (args.Length == 5) exact = true;

            if (!File.Exists(inFile)) { Console.WriteLine("Input file doesn't exist"); return; }

            string s = Regex.Replace(File.ReadAllText(inFile), @"\s+", " ").TrimEnd(' ');
            TDict t = MarkovHelper.BuildTDict(s, size);
            File.WriteAllText( outFile, MarkovHelper.BuildString(t, limit, exact).TrimEnd(' ') );
        }

        static void printUsage()
        {
            Console.WriteLine("Usage: Markov.exe infile outfile wordlimit size [exact]");
        }

    }
}
