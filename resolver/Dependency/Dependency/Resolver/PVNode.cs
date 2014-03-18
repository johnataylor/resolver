using Resolver.Metadata;
using System.Collections.Generic;
using System.IO;

namespace Resolver.Resolver
{
    public class PVNode
    {
        public SemanticVersion Version { get; private set; }

        public List<PNode> Children { get; private set; }

        public PVNode(SemanticVersion version)
        {
            Version = version;
            Children = new List<PNode>();
        }

        public void WriteTo(TextWriter writer, int indent = 0)
        {
            writer.WriteLine("{0}{1}", Utils.Indent(indent), Version);
            foreach (PNode pnode in Children)
            {
                pnode.WriteTo(writer, indent + 1);
            }
        }
    }
}
