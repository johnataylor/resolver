using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Resolver
{
    class TreeSplitter
    {
        public static List<PNode> FindIndependentTrees(PNode original)
        {
            List<Subtree> subtrees = new List<Subtree>();

            PVNode parent = original.Children.First();

            foreach (PNode pnode in parent.Children)
            {
                //Console.Write("{0}: ", pnode.Id);

                HashSet<string> participants = new HashSet<string>();
                participants.Add(pnode.Id);

                GetParticipants(pnode, participants);

                bool newSubtreeNeeded = true;
                foreach (Subtree subtree in subtrees)
                {
                    if (subtree.HasOverlap(participants))
                    {
                        subtree.Roots.Add(pnode.Id);
                        newSubtreeNeeded = false;
                        break;
                    }
                }

                if (newSubtreeNeeded)
                {
                    Subtree newSubtree = new Subtree();
                    newSubtree.Roots.Add(pnode.Id);

                    foreach (string s in participants)
                    {
                        newSubtree.Participants.Add(s);
                    }

                    subtrees.Add(newSubtree);
                }

                //foreach (string participant in participants)
                //{
                //    Console.Write("{0} ", participant);
                //}
                //Console.WriteLine();
            }

            foreach (Subtree subtree in subtrees)
            {
                subtree.Print();
            }

            return null;
        }

        class Subtree
        {
            public List<string> Roots = new List<string>();
            public HashSet<string> Participants = new HashSet<string>();

            public bool HasOverlap(HashSet<string> p)
            {
                foreach (string s in p)
                {
                    if (Participants.Contains(s))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Print()
            {
                foreach (string s in Roots)
                {
                    Console.Write("{0} ", s);
                }
                Console.WriteLine();
            }
        }

        public static void GetParticipants(PVNode pvnode, HashSet<string> participants)
        {
            foreach (PNode child in pvnode.Children)
            {
                GetParticipants(child, participants);
            }
        }

        private static void GetParticipants(PNode pnode, HashSet<string> participants)
        {
            participants.Add(pnode.Id);

            foreach (PVNode child in pnode.Children)
            {
                GetParticipants(child, participants);
            }
        }
    }
}
