using Resolver.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Resolver
{
    public static class MetadataTree
    {
        //  Metadata tree building

        public static PNode GetTree(string[] packageIds, IGallery gallery)
        {
            PNode root = new PNode("$");
            PVNode rootVersion = new PVNode(SemanticVersion.Min);
            root.Children.Add(rootVersion);

            foreach (string packageId in packageIds)
            {
                Registration registration = gallery.GetRegistration(packageId);

                PNode pnode = new PNode(registration.Id);
                rootVersion.Children.Add(pnode);

                foreach (Package package in registration.Packages)
                {
                    InnerGetTree(package, gallery, pnode);
                }
            }

            return root;
        }

        static PNode GetTree(Package package, IGallery gallery)
        {
            PNode root = new PNode(package.Id);
            InnerGetTree(package, gallery, root);
            return root;
        }

        static void InnerGetTree(Package package, IGallery gallery, PNode parent)
        {
            PVNode pvnode = new PVNode(package.Version);
            parent.Children.Add(pvnode);

            foreach (Dependency dependency in package.Dependencies)
            {
                Registration registration = gallery.GetRegistration(dependency.Id);

                PNode pnode = new PNode(dependency.Id);
                pvnode.Children.Add(pnode);

                foreach (Package nextPackage in registration.Packages)
                {
                    if (dependency.Range.Includes(nextPackage.Version))
                    {
                        InnerGetTree(nextPackage, gallery, pnode);
                    }
                }
            }
        }

        //  Testing a candidate solution against a tree

        public static bool Satisfy(PNode pnode, List<Tuple<string, SemanticVersion>> candidate)
        {
            IDictionary<string, SemanticVersion> dictionary = new Dictionary<string, SemanticVersion>();
            foreach (Tuple<string, SemanticVersion> item in candidate)
            {
                dictionary.Add(item.Item1, item.Item2);
            }
            dictionary.Add("$", new SemanticVersion(0));

            return Satisfy(pnode, dictionary);
        }

        static bool Satisfy(PNode pnode, IDictionary<string, SemanticVersion> dictionary)
        {
            if (pnode.Children.Count == 0)
            {
                return true;
            }

            // for a package ANY child can satisfy

            foreach (PVNode child in pnode.Children)
            {
                if (Satisfy(child, pnode.Id, dictionary))
                {
                    return true;
                }
            }

            return false;
        }

        static bool Satisfy(PVNode pvnode, string id, IDictionary<string, SemanticVersion> dictionary)
        {
            if (dictionary.Contains(new KeyValuePair<string, SemanticVersion>(id, pvnode.Version), new KeySemantciVersionEqualityComparer()))
            {
                if (pvnode.Children.Count == 0)
                {
                    return true;
                }

                // for a particular version of a package ALL children must satisfy

                foreach (PNode child in pvnode.Children)
                {
                    if (!Satisfy(child, dictionary))
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        class KeySemantciVersionEqualityComparer : EqualityComparer<KeyValuePair<string, SemanticVersion>>
        {
            public override bool Equals(KeyValuePair<string, SemanticVersion> x, KeyValuePair<string, SemanticVersion> y)
            {
                return (x.Key == y.Key) && (SemanticVersionRange.DefaultComparer.Compare(x.Value, y.Value) == 0);
            }
            public override int GetHashCode(KeyValuePair<string, SemanticVersion> obj)
            {
                return obj.GetHashCode(); 
            }
        }
    }
}
