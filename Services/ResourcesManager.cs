using System.IO;
using System.Threading.Tasks;

namespace Mashawi.Resources
{
    public static class AppResourcesManager
    {
        private static readonly string AssemblyName = "mashawi.Resources.";

        public static Stream GetStream(string name)
        {
            var assembly = typeof(AppResourcesManager).Assembly;
            return assembly.GetManifestResourceStream(AssemblyName + name)!;
        }

        public static async Task<string> GetText(string name)
        {
            using var reader = new StreamReader(GetStream(name));
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}