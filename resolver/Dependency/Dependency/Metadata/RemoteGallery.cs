using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Metadata
{
    public class RemoteGallery : IGallery
    {
        string _source;

        IDictionary<string, Registration> _cache;

        public RemoteGallery(string source)
        {
            _source = source;
            _cache = new Dictionary<string, Registration>();
        }

        public async Task<Registration> GetRegistration(string registrationId)
        {
            Registration registration;
            if (!_cache.TryGetValue(registrationId, out registration))
            {
                registration = await InnerGetRegistration(registrationId);
                _cache.Add(registrationId, registration);
            }
            return registration;
        }

        public async Task<Registration> InnerGetRegistration(string registrationId)
        {
            string address = (_source.Trim('/') + '/' + registrationId).ToLowerInvariant();

            Console.WriteLine(address);

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string json = await response.Content.ReadAsStringAsync();

                JObject jobjectRegistration = JObject.Parse(json);
                string id = jobjectRegistration["id"].ToObject<string>();

                Registration registration = new Registration();
                registration.Id = id;

                JArray jarrayPackages = (JArray)jobjectRegistration["package"];

                foreach (JObject jobjectPackage in jarrayPackages)
                {
                    string version = jobjectPackage["version"].ToObject<string>();

                    Package package = new Package(id, version);
                    registration.Packages.Add(package);

                    JToken jtokenDependencies;
                    if (jobjectPackage.TryGetValue("dependencies", out jtokenDependencies))
                    {
                        JObject dependencies = (JObject)jtokenDependencies;

                        JArray jarrayGroups = (JArray)dependencies["group"];

                        foreach (JObject jobjectGroup in jarrayGroups)
                        {
                            string groupTargetFramework = "all";
                            JToken jtokenTargetFramework;
                            if (jobjectGroup.TryGetValue("targetFramework", out jtokenTargetFramework))
                            {
                                groupTargetFramework = jtokenTargetFramework.ToObject<string>();
                            }

                            Group group = new Group();

                            package.DependencyGroups.Add(groupTargetFramework, group);

                            JArray jarrayDependency = (JArray)jobjectGroup["dependency"];

                            foreach (JObject jobjectDependencyPart in jarrayDependency)
                            {
                                string dependencyId = jobjectDependencyPart["id"].ToObject<string>();

                                string dependencyRange = "LATEST";
                                JToken jtokenRange;
                                if (jobjectDependencyPart.TryGetValue("range", out jtokenRange))
                                {
                                    dependencyRange = jtokenRange.ToObject<string>();
                                }

                                group.Dependencies.Add(new Dependency(dependencyId, dependencyRange));
                            }
                        }
                    }
                }

                return registration;
            }

            throw new Exception(string.Format("unable to load: {0}", registrationId));
        }
    }
}
