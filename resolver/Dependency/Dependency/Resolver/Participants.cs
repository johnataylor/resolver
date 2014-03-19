using Resolver.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Resolver
{
    class Participants
    {
        public static IDictionary<string, ISet<SemanticVersion>> GetParticipants(PNode pnode)
        {
            IDictionary<string, ISet<SemanticVersion>> participants = new Dictionary<string, ISet<SemanticVersion>>();
            foreach (PVNode child in pnode.Children)
            {
                GetParticipants(child, participants);
            }
            return participants;
        }

        public static void GetParticipants(PVNode pvnode, IDictionary<string, ISet<SemanticVersion>> participants)
        {
            foreach (PNode child in pvnode.Children)
            {
                GetParticipants(child, participants);
            }
        }

        private static void GetParticipants(PNode pnode, IDictionary<string, ISet<SemanticVersion>> participants)
        {
            foreach (PVNode child in pnode.Children)
            {
                GetParticipants(child, pnode.Id, participants);
            }
        }

        private static void GetParticipants(PVNode pvnode, string id, IDictionary<string, ISet<SemanticVersion>> participants)
        {
            ISet<SemanticVersion> versions;
            if (!participants.TryGetValue(id, out versions))
            {
                versions = new HashSet<SemanticVersion>();
                participants.Add(id, versions);
            }
            versions.Add(pvnode.Version);

            foreach (PNode child in pvnode.Children)
            {
                GetParticipants(child, participants);
            }
        }
    }
}
