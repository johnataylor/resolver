
using System.Collections.Generic;

namespace Resolver.Metadata
{
    public class Package
    {
        public string Id { get; private set; }
        public SemanticVersion Version { get; private set; }
        public ICollection<Dependency> Dependencies { get; private set; }

        public Package(string id, SemanticVersion version)
        {
            Id = id;
            Version = version;
            Dependencies = new List<Dependency>();
        }

        public Package(string id, string version, IDictionary<string, string> dependencies = null)
            : this(id, SemanticVersion.Parse(version))
        {
            if (dependencies != null)
            {
                foreach (KeyValuePair<string, string> dependency in dependencies)
                {
                    Dependencies.Add(new Dependency(dependency.Key, dependency.Value));
                }
            }
        }
    }
}
