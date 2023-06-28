using System;
using System.Collections.Generic;
using System.Windows;
using VMS.TPS.Common.Model.API;
using OSU_Helpers;
using OSUStructureGenerator;
using System.Text.RegularExpressions;
using System.Configuration;

[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    internal partial class Script
    {
        public Script()
        {
        }

        /// <summary>
        /// Generic ESAPI script execution as a plugin.
        /// </summary>
        /// <param name="context">The provided script context by Eclipse.</param>
        /// <exception cref="ArgumentNullException">If the script context is null, then this expection is raised.</exception>
        public void Execute(ScriptContext context/*, Window window*/)
        {
            context.Patient.BeginModifications();

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FileIOHelper.AddExtension("txt", "Text File");

            FileIOHelper.Open_Files(ConfigurationManager.AppSettings["DefaultPath"], false, "Open a Template");

            if (FileIOHelper.files.Count == 0) return;

            List<string> file_contents = FileIOHelper.ReadFile(FileIOHelper.files[0]);

            foreach (string line in file_contents)
            {
                if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
                string _line = Regex.Replace(line, @"\s+", "");
                try
                {
                    StructureGenerationFromText structure = new StructureGenerationFromText(_line, context.StructureSet);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failure creating structure from:\r\n" + line + "\r\n" + e.Message);
                }
            }
        }
    }
}