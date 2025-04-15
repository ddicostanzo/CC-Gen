using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace StructureBuilder
{
    internal static class StructureDictionaryService
    {
        internal static string ServerId;
        internal static List<StructureNameDictionaryOfStringListOfString> StructureDictionary;
        internal static void InitializeStructureDictionary()
        {
            string path = Path.Combine($"\\\\{ServerId}", "va_data$", "programdata", "vision", "visualscripting",
                "structurenamedictionary.xml");
            if (!File.Exists(path))
            {
                MessageBox.Show($"File path not found: {path}. Please check config file.");
                return;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<StructureNameDictionaryOfStringListOfString>));
            //StructureDictionary = new List<StructureNameDictionary>();
            using (StreamReader sr = new StreamReader(path))
            {
                StructureDictionary = (List<StructureNameDictionaryOfStringListOfString>)xmlSerializer.Deserialize(sr);
            }
        }
        internal static List<string> GetDictionaryValues(string keyValue)
        {
            List<string> matches = new List<string>();
            if (StructureDictionary != null)
            {
                if (StructureDictionary.Any(sd => sd.Key.Equals(keyValue, StringComparison.OrdinalIgnoreCase)))
                {
                    matches = StructureDictionary
                        .First(sd => sd.Key.Equals(keyValue, StringComparison.OrdinalIgnoreCase)).Value;
                }
            }
            return matches;
        }
    }
    public class StructureNameDictionaryOfStringListOfString
    {
        public string Key { get; set; }
        public List<string> Value { get; set; }
    }
}