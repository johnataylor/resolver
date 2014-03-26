using Resolver.Metadata;
using Resolver.Resolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver
{
    class Program
    {
        static void Test0()
        {
            Gallery gallery = TestGallery.Create1();

            gallery.WriteTo(Console.Out);
        }

        static void Test1()
        {
            Debug.Assert(!SemanticVersionRange.Parse("[2.3.4]").Includes(SemanticVersion.Parse("2.3.3")));
            Debug.Assert(SemanticVersionRange.Parse("[2.3.4]").Includes(SemanticVersion.Parse("2.3.4")));
            Debug.Assert(!SemanticVersionRange.Parse("[2.3.4]").Includes(SemanticVersion.Parse("2.3.5")));

            Debug.Assert(SemanticVersionRange.Parse("[1.2.3,2.3.4]").Includes(SemanticVersion.Parse("2.0.0")));
            Debug.Assert(SemanticVersionRange.Parse("[1.2.3,2.3.4]").Includes(SemanticVersion.Parse("2.3.0")));
            Debug.Assert(!SemanticVersionRange.Parse("[1.2.3,2.3.4]").Includes(SemanticVersion.Parse("3.0.0")));

            Debug.Assert(SemanticVersionRange.Parse("2.0.0").Includes(SemanticVersion.Parse("2.0.0")));
            Debug.Assert(SemanticVersionRange.Parse("2.0.0").Includes(SemanticVersion.Parse("2.0.1")));
            Debug.Assert(!SemanticVersionRange.Parse("2.0.0").Includes(SemanticVersion.Parse("1.9.9")));
        }

        static void Test2()
        {
            SemanticVersion sv0 = SemanticVersion.Parse("1.0.4.3225");
            SemanticVersion sv1 = SemanticVersion.Parse("1.0.1-portableRC1");
            SemanticVersion sv2 = SemanticVersion.Parse("1.0.0.2473");
        }

        static async Task Test3()
        {
            Gallery gallery = TestGallery.Create4();
            PNode pnode = await MetadataTree.GetTree(new string[] { "D", "E", "F", "G", "H", "K" }, gallery, "all");
            //PNode pnode = MetadataTree.GetTree(new string[] { "D", "E", "F", "G", "H" }, gallery, "all");
            
            //pnode.WriteTo(Console.Out);

            List<PNode> independentTrees = TreeSplitter.FindIndependentTrees(pnode);

            IDictionary<string, SemanticVersion> solution = new Dictionary<string, SemanticVersion>();

            foreach (PNode tree in independentTrees)
            {
                List<Tuple<string, SemanticVersion>>[] lineup = Participants.Collect(tree);

                //Utils.PrintLineup(lineup);
                //Console.WriteLine("iterations = {0}", Utils.CountIterations(lineup));
                //Runner.Simulate(tree, lineup, true);

                // apply policy, where policy is strictly filtering and sorting on the lineup structiure

                // lineup registrations are ordered low to high version - reverse the order to find a solution using the latest
                foreach (List<Tuple<string, SemanticVersion>> registration in lineup)
                {
                    registration.Reverse();
                }

                IDictionary<string, SemanticVersion> partial = Runner.FindFirst(tree, lineup);

                if (partial == null)
                {
                    Console.Write("unable to find solution between: ");
                    Utils.PrintDistinctRegistrations(lineup);
                    Console.WriteLine();
                }
                else
                {
                    foreach (KeyValuePair<string, SemanticVersion> item in partial)
                    {
                        solution.Add(item);
                    }
                }
            }

            Utils.PrintPackages(solution);
        }

        static async Task Test4()
        {
            IGallery gallery = new RemoteGallery("http://nuget3.blob.core.windows.net/pub/");

            DateTime before = DateTime.Now;

            PNode pnode = await MetadataTree.GetTree(new string[] 
            { 
                "dotNetRdf",
                "WindowsAzure.Storage",
                "json-ld.net"
            }, 
            gallery, ".NETFramework4.0");

            DateTime after = DateTime.Now;

            Console.WriteLine("{0} seconds", (after - before).TotalSeconds);

            List<PNode> independentTrees = TreeSplitter.FindIndependentTrees(pnode);

            IDictionary<string, SemanticVersion> solution = new Dictionary<string, SemanticVersion>();

            foreach (PNode tree in independentTrees)
            {
                List<Tuple<string, SemanticVersion>>[] lineup = Participants.Collect(tree);

                foreach (List<Tuple<string, SemanticVersion>> registration in lineup)
                {
                    registration.Reverse();
                }

                IDictionary<string, SemanticVersion> partial = Runner.FindFirst(tree, lineup);

                if (partial == null)
                {
                    Console.Write("unable to find solution between: ");
                    Utils.PrintDistinctRegistrations(lineup);
                    Console.WriteLine();
                }
                else
                {
                    foreach (KeyValuePair<string, SemanticVersion> item in partial)
                    {
                        solution.Add(item);
                    }
                }
            }

            Console.WriteLine("solution:");
            Console.WriteLine();
            Utils.PrintPackages(solution);
            Console.WriteLine();
        }

        static void Test5()
        {
        }

        static Func<SemanticVersion, bool> Includes(SemanticVersion begin, SemanticVersionSpan span)
        {
            SemanticVersionRange range = new SemanticVersionRange(begin, span);

            Console.WriteLine("Adding range: {0}", range);

            return (version) => { return range.Includes(version); };
        }

        static void Print(IList<SemanticVersion> lineup)
        {
            foreach (SemanticVersion sv in lineup)
            {
                Console.WriteLine(sv);
            }
        }

        //TODO: Trim naturally DeDups too
        static void DeDup(List<SemanticVersion> original)
        {
            HashSet<SemanticVersion> hs = new HashSet<SemanticVersion>(original);
            original.Clear();
            original.AddRange(hs);
        }

        //TODO: Trim should be parameterized by what we want to trim by (minor, patch)
        static void Trim(List<SemanticVersion> original)
        {
            IDictionary<Tuple<int, int>, SemanticVersion> scratch = new Dictionary<Tuple<int,int>, SemanticVersion>();

            foreach (SemanticVersion semver in original)
            {
                Tuple<int, int> key = new Tuple<int, int>(semver.Major, semver.Minor);

                SemanticVersion current;
                if (scratch.TryGetValue(key, out current))
                {
                    if (SemanticVersionRange.DefaultComparer.Compare(semver, current) > 0)
                    {
                        scratch[key] = semver;
                    }
                }
                else
                {
                    scratch.Add(key, semver);
                }
            }

            original.Clear();
            original.AddRange(scratch.Values);
        }

        static void Test7()
        {
            IList<SemanticVersion> versions = new List<SemanticVersion>();

            versions.Add(SemanticVersion.Parse("1.0.0"));
            versions.Add(SemanticVersion.Parse("1.5.0"));
            versions.Add(SemanticVersion.Parse("1.8.0"));
            versions.Add(SemanticVersion.Parse("2.0.0"));
            versions.Add(SemanticVersion.Parse("2.2.0"));
            versions.Add(SemanticVersion.Parse("2.5.0"));
            versions.Add(SemanticVersion.Parse("2.5.1"));
            versions.Add(SemanticVersion.Parse("2.6.0"));
            versions.Add(SemanticVersion.Parse("2.6.7"));
            versions.Add(SemanticVersion.Parse("2.8.0"));
            versions.Add(SemanticVersion.Parse("2.8.1"));
            versions.Add(SemanticVersion.Parse("3.0.0"));
            versions.Add(SemanticVersion.Parse("3.5.0"));
            versions.Add(SemanticVersion.Parse("3.6.0"));
            versions.Add(SemanticVersion.Parse("3.7.0"));
            versions.Add(SemanticVersion.Parse("4.0.0"));
            versions.Add(SemanticVersion.Parse("4.1.0"));
            versions.Add(SemanticVersion.Parse("4.1.2"));
            versions.Add(SemanticVersion.Parse("4.5.0"));
            versions.Add(SemanticVersion.Parse("5.0.0"));

            SemanticVersion current = SemanticVersion.Parse("2.5.0");

            List<SemanticVersion> lineup = new List<SemanticVersion>();

            lineup.AddRange(versions.Where(Includes(current, SemanticVersionSpan.MaxMinor)));

            DeDup(lineup);

            Trim(lineup);
            
            lineup.Sort(SemanticVersionRange.DefaultComparer);

            Print(lineup);

            lineup.AddRange(versions.Where(Includes(current, SemanticVersionSpan.FromMajor(-1))));

            DeDup(lineup);

            Trim(lineup);
            
            lineup.Sort(SemanticVersionRange.DefaultComparer);

            Print(lineup);

            lineup.AddRange(versions.Where(Includes(current, SemanticVersionSpan.FromMajor(1))));

            DeDup(lineup);

            Trim(lineup);

            lineup.Sort(SemanticVersionRange.DefaultComparer);

            Print(lineup);
        }

        static void Main(string[] args)
        {
            try
            {
                //Test0();
                //Test1();
                //Test2();
                //Test3().Wait();
                Test4().Wait();
                //Test5();
                //Test6();
                //Test7();
            }
            //TODO: n-deep exception reporting
            catch (AggregateException g)
            {
                foreach (Exception e in g.InnerExceptions)
                {
                    Console.WriteLine(e.Message);
                    if (e.InnerException != null)
                    {
                        Console.WriteLine("\t{0}", e.InnerException.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("\t{0}", e.InnerException.Message);
                }
            }
        }
    }
}
