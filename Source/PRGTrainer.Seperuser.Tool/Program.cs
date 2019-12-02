namespace PRGTrainer.SuperUser.Tool
{
    using System;
    using System.Linq;
    using TokenProcessing;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var tokenGenerator = new TokenGenerator();
            if (args.Contains(@"-token"))
                Console.WriteLine(
                    tokenGenerator.Generate(
                        args.Contains(@"-uStat") || args.Contains(@"-all"), 
                        args.Contains(@"-qStat") || args.Contains(@"-all")));

            Console.ReadKey();
        }
    }
}
