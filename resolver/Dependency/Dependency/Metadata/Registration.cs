﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Resolver.Metadata
{
    public class Registration
    {
        public string Id { get; set; }

        public ICollection<Package> Packages { get; private set; }

        public Registration()
        {
            Packages = new List<Package>();
        }

        public void WriteTo(TextWriter writer)
        {
            writer.WriteLine("{0}", Id);
            foreach (Package package in Packages)
            {
                writer.WriteLine("\t{0}", package.Version);
                foreach (Dependency dependency in package.Dependencies)
                {
                    writer.WriteLine("\t\t{0} {1}", dependency.Id, dependency.Range);
                }
            }
        }
    }
}
