using System;

namespace OSU_Helpers
{
    /// <summary>
    /// FileType class that allows generation and passing of labels and extensions.
    /// </summary>
    public class FileType
    {
        /// <summary>
        /// Label of the FileType that can be displayed in the UI
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// File Extension of the specified FileType, does not include the .
        /// </summary>
        public string file_extension { get; set; }

        /// <summary>
        /// FileType constructor. Checks if a period is included, if it is tries to split string.
        /// </summary>
        /// <param name="extension">Only the extensions, exclude period and astrisk</param>
        /// <param name="type_label">Label of file type such as "Text File"</param>
        public FileType(string extension, string type_label)
        {
            if (extension.Contains("."))
            {
                try
                {
                    extension = extension.Split('.')[1];
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
            label = type_label;
            file_extension = extension;
        }

        /// <summary>
        /// Specific property to include the astrisk and period for proper format of open filter dialogs
        /// </summary>
        public string file_open_filter_extension => string.Format("*.{0}", file_extension);
    }
}
