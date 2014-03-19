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
            Gallery gallery = TestGallery.Create2();
            PNode node = MetadataTree.GetTree(new string[] { "A" }, gallery);
            node.WriteTo(Console.Out);
        }

        static void Test3()
        {
            Gallery gallery = TestGallery.Create4();
            PNode pnode = MetadataTree.GetTree(new string[] { "D", "E", "F", "G", "H", "K" }, gallery);
            pnode.WriteTo(Console.Out);

            List<PNode> independentTrees = TreeSplitter.FindIndependentTrees(pnode);

            //Runner.Run(pnode);
        }

        static void Test4()
        {
            Gallery gallery = TestGallery.Create2();
            PNode node = MetadataTree.GetTree(new string[] { "C", "D" }, gallery);
            node.WriteTo(Console.Out);
        }

        static void Test5()
        {
            Gallery gallery = TestGallery.Create3();
            PNode node = MetadataTree.GetTree(new string[] { "D" }, gallery);
            node.WriteTo(Console.Out);
        }

        static void Test6()
        {
            Gallery gallery = TestGallery.Create2();

            Package application = new Package("$", "1.0.0",
                new Dictionary<string, string> { { "A", "2.0.0" }, { "C", "1.0.0" } });
            
            PNode node = MetadataTree.GetTree(application, gallery);
            node.WriteTo(Console.Out);
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
                Test3();
                //Test4();
                //Test5();
                //Test6();
                //Test7();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
