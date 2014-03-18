using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Resolver
{
    class Utils
    {
        public static string Indent(int indent)
        {
            string s = string.Empty;
            while (indent-- > 0)
            {
                s += " ";
            }
            return s;
        }
    }
}
