using System;
using System.Linq;
using System.Collections.Generic;

namespace Algorithms
{
    public class Program
    {

        public static void Main(string[] args)
        {
            List<String[]>  data = new List<String[]>{ new[] {"Beer",   "Eggs" }};
            Console.WriteLine("a");
            List<String[]> sara = Apriori.GenerateNextCandidates(data).ToList();

            Console.WriteLine(sara.Count());
            //Console.WriteLine(sara.Count());
            Console.WriteLine("b");
            Console.ReadLine();
        }  
    }
}
