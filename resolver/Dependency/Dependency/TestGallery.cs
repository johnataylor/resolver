using Resolver.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver
{
    class TestGallery
    {
        public static Gallery Create1()
        {
            Gallery gallery = new Gallery();

            gallery.AddPackage(new Package("A", "1.0.0"));
            gallery.AddPackage(new Package("A", "1.5.0"));
            gallery.AddPackage(new Package("A", "2.0.0"));
            gallery.AddPackage(new Package("A", "2.5.0"));
            gallery.AddPackage(new Package("A", "3.0.0"));
            gallery.AddPackage(new Package("A", "3.5.0"));
            gallery.AddPackage(new Package("A", "4.0.0"));

            gallery.AddPackage(new Package("B", "1.0.0"));
            gallery.AddPackage(new Package("B", "1.5.0"));
            gallery.AddPackage(new Package("B", "2.0.0"));
            gallery.AddPackage(new Package("B", "2.5.0"));
            gallery.AddPackage(new Package("B", "3.0.0"));
            gallery.AddPackage(new Package("B", "3.5.0", new Dictionary<string, string> { { "A", "4.0.0" } }));
            gallery.AddPackage(new Package("B", "4.0.0"));

            gallery.AddPackage(new Package("C", "1.0.0", new Dictionary<string, string> { { "A", "2.0.0" }, { "B", "[1.0.0,2.0.0)" } }));
            gallery.AddPackage(new Package("C", "2.0.0", new Dictionary<string, string> { { "A", "2.0.0" }, { "B", "[2.0.0]" } }));
            gallery.AddPackage(new Package("C", "2.5.0", new Dictionary<string, string> { { "A", "2.0.0" }, { "B", "[2.0.0,4.0.0)" } }));

            return gallery;
        }

        public static Gallery Create2()
        {
            Gallery gallery = new Gallery();

            gallery.AddPackage(new Package("A", "1.0.0"));
            gallery.AddPackage(new Package("A", "1.5.0"));
            gallery.AddPackage(new Package("A", "2.0.0"));
            gallery.AddPackage(new Package("A", "3.0.0"));
            gallery.AddPackage(new Package("A", "4.0.0"));
            gallery.AddPackage(new Package("A", "5.0.0"));
            gallery.AddPackage(new Package("A", "6.0.0"));
            gallery.AddPackage(new Package("A", "7.0.0"));
            gallery.AddPackage(new Package("A", "8.0.0"));
            gallery.AddPackage(new Package("A", "9.0.0"));

            gallery.AddPackage(new Package("B", "1.0.0"));
            gallery.AddPackage(new Package("B", "1.5.0"));
            gallery.AddPackage(new Package("B", "2.0.0"));
            gallery.AddPackage(new Package("B", "3.0.0", new Dictionary<string, string> { { "A", "[1.5.0,2.0.0)" } }));

            gallery.AddPackage(new Package("C", "1.0.0", new Dictionary<string, string> { { "B", "[2.0.0,3.0.0]" } }));

            gallery.AddPackage(new Package("D", "1.0.0", new Dictionary<string, string> { { "A", "[1.0.0,1.5.0]" } }));
            gallery.AddPackage(new Package("D", "2.0.0", new Dictionary<string, string> { { "A", "2.0.0" } }));

            return gallery;
        }

        public static Gallery Create3()
        {
            Gallery gallery = new Gallery();

            gallery.AddPackage(new Package("A", "1.0.0"));
            gallery.AddPackage(new Package("A", "2.0.0"));
            gallery.AddPackage(new Package("D", "2.0.0", new Dictionary<string, string> { { "A", "2.0.0" } }));

            return gallery;
        }
    }
}
