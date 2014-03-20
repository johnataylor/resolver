using Resolver.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Resolver
{
    public class Runner
    {
        public static void Simulate(PNode pnode, List<Tuple<string, SemanticVersion>>[] lineup, bool verbose)
        {
            int good = 0;
            int bad = 0;

            DateTime before = DateTime.Now;

            Permutations.Run(lineup, (candidate) =>
            {
                if (MetadataTree.Satisfy(pnode, candidate))
                {
                    if (verbose)
                    {
                        Console.Write("GOOD: ");
                        Print(candidate);
                        Console.WriteLine();
                    }
                    good++;
                }
                else
                {
                    if (verbose)
                    {
                        Console.Write("BAD: ");
                        Print(candidate);
                        Console.WriteLine();
                    }
                    bad++;
                }
                return false;
            });

            DateTime after = DateTime.Now;
            Console.WriteLine("duration: {0} seconds", (after - before).TotalSeconds);
            Console.WriteLine("good: {0} bad: {1}", good, bad);
        }

        public static List<Tuple<string, SemanticVersion>> FindFirst(PNode pnode, List<Tuple<string, SemanticVersion>>[] lineup)
        {
            List<Tuple<string, SemanticVersion>> solution = null;
            Permutations.Run(lineup, (candidate) =>
            {
                if (MetadataTree.Satisfy(pnode, candidate))
                {
                    solution = candidate;
                    return true;
                }
                return false;
            });
            return solution;
        }

        static void Print(List<Tuple<string, SemanticVersion>> list)
        {
            foreach (Tuple<string, SemanticVersion> item in list)
            {
                Console.Write("{0}/{1} ", item.Item1, item.Item2);
            }
        }
    }
}
