using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OSU_Helpers
{
    /// <summary>
    /// Access embedded resources in assemblies
    /// </summary>
    public static class EmbeddedResource
    {
        /// <summary>
        /// Will return the stream of a specified embedded resource
        /// </summary>
        /// <param name="name">Name of the embedded resource including file extension</param>
        /// <returns>Stream of embedded resource or null MemoryStream if resource does not exist.</returns>
        public static Stream EmbeddedStream(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string resourceName = string.Empty;

            resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));
            if (resourceName == string.Empty || resourceName == null)
            {
                Console.WriteLine("No available resource of name " + name);
                Console.WriteLine("Not attempting to get embedded resource since cannot find by name.");
                return new MemoryStream();
            }

            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
