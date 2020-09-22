using System;

namespace CW_Emirp
{
    class Program
    {
        static void Main(string[] args)
        {
            var checkRange = 200000;
            var emirps = Emirps.FindEmirp(checkRange);

            Console.WriteLine($"For given range: 1-{checkRange}, there is {emirps[0]} emirps found. " +
                $"Max emirp is {emirps[1]} and sum of all found emirps is {emirps[2]}.");
        }

    }
}
