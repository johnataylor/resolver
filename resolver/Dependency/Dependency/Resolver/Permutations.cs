using Resolver.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Resolver
{
    class Permutations
    {
        static void Loop(List<Tuple<string, SemanticVersion>>[] x, int i, Stack<Tuple<string, SemanticVersion>> candidate, Action<List<Tuple<string, SemanticVersion>>> test)
        {
            if (i == x.Length)
            {
                List<Tuple<string, SemanticVersion>> list = new List<Tuple<string, SemanticVersion>>(candidate);
                test(list);
                return;
            }

            foreach (Tuple<string, SemanticVersion> s in x[i])
            {
                candidate.Push(s);
                Loop(x, i + 1, candidate, test);
                candidate.Pop();
            }
        }

        public static void Run(List<Tuple<string, SemanticVersion>>[] x, Action<List<Tuple<string, SemanticVersion>>> test)
        {
            Stack<Tuple<string, SemanticVersion>> candidate = new Stack<Tuple<string, SemanticVersion>>();
            Loop(x, 0, candidate, test);
        }

        public static List<Tuple<string, SemanticVersion>>[] SortParticipants(IDictionary<string, ISet<SemanticVersion>> participants)
        {
            List<Tuple<string, SemanticVersion>>[] result = new List<Tuple<string, SemanticVersion>>[participants.Count];

            int i = 0;

            foreach (KeyValuePair<string, ISet<SemanticVersion>> participant in participants)
            {
                List<SemanticVersion> versions = new List<SemanticVersion>(participant.Value);
                versions.Sort(SemanticVersionRange.DefaultComparer);

                List<Tuple<string, SemanticVersion>> candidates = new List<Tuple<string, SemanticVersion>>();
                foreach (SemanticVersion version in versions)
                {
                    candidates.Add(new Tuple<string, SemanticVersion>(participant.Key, version));
                }

                result[i++] = candidates;
            }

            return result;
        }

        public static int CountIterations(List<Tuple<string, SemanticVersion>>[] run)
        {
            int iterations = 1;

            foreach (List<Tuple<string, SemanticVersion>> packageRegistration in run)
            {
                iterations *= packageRegistration.Count;
            }

            return iterations;
        }
    }
}
