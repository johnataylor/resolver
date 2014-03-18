using Resolver.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Resolver
{
    public class MetadataTree
    {
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

        public static PNode GetTree(Package package, IGallery gallery)
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
    }
}
