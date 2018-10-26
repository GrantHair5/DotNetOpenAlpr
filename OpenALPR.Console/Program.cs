using OpenALPR.Lib;
using OpenALPR.Lib.Data;

namespace OpenALPR.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: OpenALPR.Console.exe imagePath");
                return;
            }

            var lib = new OpenAlprLib();
            var res = lib.GetBestMatch(Country.GB, args[0]);
            System.Console.WriteLine("Best match: " + (res ?? "No Reg Found"));
            System.Console.WriteLine();
        }
    }
}