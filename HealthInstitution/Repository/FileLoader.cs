using HealthInstitution.Class;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace HealthInstitution.Repository
{
    internal class FileLoader
    {
        public static void Serialize<T>(object obj, string filePath)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                serializer.Serialize(writer, obj);
            }
        }

        public static object Deserialize<T>(string path)
        {
            List<T> items;

            using (StreamReader r = new StreamReader(Path.Combine(Environment.CurrentDirectory, path)))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<T>>(json);
            }

            if (items == null) items = new List<T>();

            return items;
        }

    }
}
