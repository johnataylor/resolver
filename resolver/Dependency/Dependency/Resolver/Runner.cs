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
        public static void Run(PNode pnode)
        {
            IDictionary<string, ISet<SemanticVersion>> participants = Participants.GetParticipants(pnode);

            List<Tuple<string, SemanticVersion>>[] run = Permutations.SortParticipants(participants);

            Console.WriteLine("iterations = {0}", Permutations.CountIterations(run));

            int good = 0;
            int bad = 0;

            DateTime before = DateTime.Now;

            Permutations.Run(run, (xl) =>
            {
                if (MetadataTree.Satisfy(pnode, xl))
                {
                    //Console.Write("GOOD: ");
                    //Print(xl);
                    //Console.WriteLine();

                    good++;
                }
                else
                {
                    //Console.Write("BAD: ");
                    //Print(xl);
                    //Console.WriteLine();

                    bad++;
                }
            });

            DateTime after = DateTime.Now;

            Console.WriteLine("duration: {0} seconds", (after - before).TotalSeconds);

            Console.WriteLine("good: {0} bad: {1}", good, bad);
        }
    }
}
