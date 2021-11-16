using System;

namespace CodeShortsApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pathSum = new PathSum();
            pathSum.LoadData();
            Console.WriteLine(pathSum.Solve());
        }
    }
}