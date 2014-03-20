using Resolver.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resolver.Resolver
{
    static class Utils
    {
        public static string Indent(int indent)
        {
            string s = string.Empty;
            while (indent-- > 0)
            {
                s += " ";
            }
            return s;
        }

        public static int CountIterations(List<Tuple<string, SemanticVersion>>[] lineup)
        {
            int iterations = 1;

            foreach (List<Tuple<string, SemanticVersion>> registration in lineup)
            {
                iterations *= registration.Count;
            }

            return iterations;
        }

        public static void PrintLineup(List<Tuple<string, SemanticVersion>>[] lineup)
        {
            foreach (List<Tuple<string, SemanticVersion>> registration in lineup)
            {
                PrintPackages(registration);
                Console.WriteLine();
            }
        }

        public static void PrintPackages(List<Tuple<string, SemanticVersion>> packages)
        {
            foreach (Tuple<string, SemanticVersion> package in packages)
            {
                Console.Write("{0}/{1} ", package.Item1, package.Item2);
            }
        }

        public static void PrintDistinctRegistrations(List<Tuple<string, SemanticVersion>>[] lineup)
        {
            foreach (List<Tuple<string, SemanticVersion>> registration in lineup)
            {
                Console.Write("{0} ", registration.First().Item1);
            }
        }
    }
}
