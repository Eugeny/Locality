using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locality
{
    public static class DataStore
    {
        private static string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Locality");

        static DataStore()
        {
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
        }

        public static void CreatePath(string path)
        {
            if (!Directory.Exists(GetPath(path)))
            Directory.CreateDirectory(GetPath(path));
        }

        public static string GetPath(string file)
        {
            return Path.Combine(root, file);
        }

        public static string GetCurrentSpacePath(string file)
        {
            var path = Path.Combine("Spaces", App.Instance.ActiveSpace.Name);
            CreatePath(path);
            return Path.Combine(GetPath(path), file);
        }
    }
}
