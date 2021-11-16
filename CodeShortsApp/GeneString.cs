// ----------------------------------------------------------------------------------------------------------------
// ---------------------------- CodeShorts ----------------------------
// ---------------------------- CodeShortsApp ----------------------------
// ----------------------------------------------------------------------------------------------------------------
// 
// File: GeneString.cs
// Created: 10/11/2021
// By: ASED
// 
//    HackerRank Bear and Steady problem 
// 
// ----------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CodeShortsApp
{
    public class GeneString
    {

        private const char A = 'A';
        private const char C = 'C';
        private const char T = 'T';
        private const char G = 'G';
        // Not complete
        public static int steadyGene(string gene)
        {
            // count chars
            int a = 0, c = 0, t = 0, g = 0;
            int n = gene.Length;
            foreach (var cha in gene)
            {
                switch (cha)
                {
                    case A:
                        a++;
                        break;
                    case C:
                        c++;
                        break;
                    case T:
                        t++;
                        break;
                    case G:
                        g++;
                        break;
                }
            }

            int k = n / 4;
            a -= k;
            c -= k;
            g -= k;
            t -= k;
            var queue = new Queue<(char, int)>();
            var subFound = false;
            for (var index = 0; index < gene.Length; index++)
            {
                var cha = gene[index];
                if (!subFound)
                {
                    switch (cha)
                    {
                        case A when a > 0:
                            a--;
                            queue.Enqueue((A, index));
                            break;
                        case C when c > 0:
                            c--;
                            queue.Enqueue((C, index));
                            break;
                        case G when g > 0:
                            g--;
                            queue.Enqueue((G, index));
                            break;
                        case T when t > 0:
                            t--;
                            queue.Enqueue((T, index));
                            break;
                    }

                    if (a <= 0 && c <= 0 && g <= 0 && t <= 0)
                    {
                        subFound = true;
                    }
                }
            }

            return 0;
        }
    }
}