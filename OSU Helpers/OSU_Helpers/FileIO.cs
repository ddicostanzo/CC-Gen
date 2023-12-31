﻿using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OSU_Helpers
{
    /// <summary>
    /// File IO helper that provides interfaces for reading and writing typical types of files.
    /// </summary>
    public static class FileIOHelper
    {
        /// <summary>
        /// Output directory for writing data to. Not currently implemented.
        /// </summary>
        private static string directory { get; set; }
        /// <summary>
        /// List of files selected by the "Open_Files" method
        /// </summary>
        public static List<string> files { get; set; } = new List<string>();
        /// <summary>
        /// List of extensions to be used when opening files. Uses "FileType" class rather than strings.
        /// </summary>
        public static List<FileType> extensions { get; set; } = new List<FileType>();

        /// <summary>
        /// This method adds an extension to the FileIO user interface
        /// </summary>
        /// <param name="ext">File extension for the specified file type, without a period or astrisk</param>
        /// <param name="lbl">The label displayed in the user interface.</param>
        public static void AddExtension(string ext, string lbl)
        {
            extensions.Add(new FileType(ext, lbl));
        }

        /// <summary>
        /// Method to open the files. Stores the results in the files variable of this class.
        /// </summary>
        /// <param name="dir">The initial directory for the user interface to open with</param>
        /// <param name="multi_select">Defines if multiple select is allowed</param>
        /// <param name="title">Title of the open file window. Defaults to "Open File(s)"</param>
        public static void Open_Files(string dir, bool multi_select, string title = "Open File(s)")
        {
            if (Directory.Exists(dir))
            {
                directory = dir;
            }
            List<string> list_return = new List<string>();

            OpenFileDialog open_file = new OpenFileDialog();

            open_file.Filter = AddExtensions();

            open_file.InitialDirectory = dir;
            open_file.Multiselect = multi_select;
            open_file.Title = title;

            MessageBoxResult cancel = new MessageBoxResult();
            do
            {
                var result = open_file.ShowDialog();

                if (!(bool)result)
                {
                    cancel = MessageBox.Show("Are you sure you want to exit?", "Exit?", MessageBoxButton.YesNo);
                }
            }
            while (cancel.Equals(MessageBoxResult.No) && open_file.FileNames.Length == 0);

            if (open_file.FileNames.Length == 0) { return; }
            else
            {
                dir = Path.GetDirectoryName(open_file.FileNames[0]);
                for (int i = 0; i < open_file.FileNames.Length; i++)
                {
                    list_return.Add(open_file.FileNames[i]);
                }

                files = list_return;
            }

        }

        /// <summary>
        /// Private method to add extensions to UI when creating window.
        /// </summary>
        /// <returns>String in format required for extensions of OpenFileDialog</returns>
        private static string AddExtensions()
        {
            string string_of_extensions = "";

            if (extensions.Count != 0)
            {
                foreach (FileType ext in extensions)
                {
                    if (ext == extensions.First())
                    {
                        string_of_extensions += string.Format("{0} ({1})|{1}", ext.label, ext.file_open_filter_extension);
                    }
                    else
                    {
                        string_of_extensions += string.Format("|{0} ({1})|{1}", ext.label, ext.file_open_filter_extension);
                    }
                }
                string_of_extensions += "|All files (*.*)|*.*";
            }
            else
            {
                string_of_extensions = "All files (*.*)|*.*";
            }

            return string_of_extensions;
        }

        /// <summary>
        /// Currently not in implementation. Goal is to allow creation of format mask so CSV can be autogenerated.
        /// </summary>
        /// <param name="fileType">FileType of output file.</param>
        /// <param name="file_name">File name of output file.</param>
        private static void write_output(FileType fileType, string file_name)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = directory;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                using (StreamWriter wr = new StreamWriter(Path.Combine(directory, (file_name + "."), fileType.file_extension)))
                {


                }

                //Console.WriteLine(string.Format("Successfully written analysis to: {0}", Path.Combine(directory, "results.csv")));
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("You have exited out of the program by failing to select a write directory.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Read a specified file
        /// </summary>
        /// <param name="filename">File to be read</param>
        /// <returns>List of strings where each item is a line of the source</returns>
        public static List<string> ReadFile(string filename)
        {
            List<string> file_lines = new List<string>();
            using (StreamReader rdr = new StreamReader(filename))
            {
                while (!rdr.EndOfStream)
                {
                    file_lines.Add(rdr.ReadLine());
                }
            }
            return file_lines;
        }

        /// <summary>
        /// Read a specified file that is delimited by a specific character
        /// </summary>
        /// <param name="filename">File to be read</param>
        /// <param name="delimiter">Character to be used to split each line</param>
        /// <returns>Returns a list of string arrays where each item in list is a line of the source split by specific character</returns>
        public static List<string[]> ReadDelimitedFile(string filename, char delimiter)
        {
            List<string[]> file_lines = new List<string[]>();
            using (StreamReader rdr = new StreamReader(filename))
            {
                while (!rdr.EndOfStream)
                {
                    file_lines.Add(rdr.ReadLine().Split(delimiter));
                }
            }
            return file_lines;
        }

        /// <summary>
        /// Read a specified stream of data, to be used with embedded resources
        /// </summary>
        /// <param name="stream">Stream to be read</param>
        /// <returns>List of strings where each item is a line of the source</returns>
        public static List<string> ReadStream(Stream stream)
        {
            List<string> file_lines = new List<string>();
            using (StreamReader rdr = new StreamReader(stream))
            {
                while (!rdr.EndOfStream)
                {
                    file_lines.Add(rdr.ReadLine());
                }
            }
            return file_lines;
        }

        /// <summary>
        /// Read a specified stream of data, to be used with embedded resources
        /// </summary>
        /// <param name="stream">Stream to be read</param>
        /// <param name="delimiter">Delimiter of data on each line</param>
        /// <returns>List of string arrays where each item is a line of the source split by the delimiter</returns>
        public static List<string[]> ReadDelimitedStream(Stream stream, char delimiter)
        {
            List<string[]> file_lines = new List<string[]>();
            using (StreamReader rdr = new StreamReader(stream))
            {
                while (!rdr.EndOfStream)
                {
                    file_lines.Add(rdr.ReadLine().Split(delimiter));
                }
            }
            return file_lines;
        }

        /// <summary>
        /// Write data to a file line by line
        /// </summary>
        /// <param name="filename">Output filename</param>
        /// <param name="append">Append data to the file specified or overwrite file</param>
        /// <param name="contents">List of strings for the output files. Each item should be a line.</param>
        /// <returns>Boolean to alert to successful or unsuccessful write of file</returns>
        public static bool WriteLineByLine(string filename, bool append, List<string> contents)
        {
            try
            {
                using (StreamWriter wr = new StreamWriter(filename, append))
                {
                    foreach (string line in contents)
                    {
                        wr.WriteLine(line);
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                return false;
            }
        }

        /// <summary>
        /// Write CSV from list of strings
        /// </summary>
        /// <param name="filename">Output filename</param>
        /// <param name="append">Append data to the file specified or overwrite file</param>
        /// <param name="contents">List of strings for the output files. Each item should be a line that is separated into a specified number of columns in array format</param>
        /// <param name="cols">Number of columns to write per line</param>
        /// <returns>Boolean to alert to successful or unsuccessful write of file</returns>
        public static bool WriteCSVFromList(string filename, bool append, List<string[]> contents, int cols)
        {
            try
            {
                int counter = 0;
                using (StreamWriter wr = new StreamWriter(filename, append))
                {
                    for (int i = counter; i < (cols + counter); i++)
                    {
                        wr.Write(contents[i] + ",");
                    }
                    counter += cols;
                    wr.Write(wr.NewLine);

                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                return false;
            }
        }

        /// <summary>
        /// Makes a given file name valid by removing invalid characters
        /// </summary>
        /// <param name="s">Input file name to check for invalid characters</param>
        /// <returns>Valid file name with underscores replacing each invalid character.</returns>
        public static string MakeFilenameValid(string s)
        {
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char ch in invalidChars)
            {
                s = s.Replace(ch, '_');
            }
            return s;
        }

        /// <summary>
        /// Returns a datatable object from a specific file that is delimited.
        /// </summary>
        /// <param name="pathName">The file to be parsed.</param>
        /// <param name="delimiter">The delimiter used in the file.</param>
        /// <returns>The datatable object generated from the input file.</returns>
        public static DataTable DelimitedFiletoDataTable(string pathName, string delimiter)
        {
            DataTable tbl = new DataTable();

            using (var reader = new StreamReader(pathName))
            {
                var line = reader.ReadLine();
                string[] values = line.Split(delimiter.ToCharArray());
                //define the columns
                for (int col = 0; col < values.Count(); col++)
                    tbl.Columns.Add(new DataColumn(values[col]));
                int ii = 1;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    values = line.Split(delimiter.ToCharArray());

                    //if (values[0] != "")
                    //{
                    DataRow dr = tbl.NewRow();
                    for (int cIndex = 0; cIndex < values.Count(); cIndex++)
                    {
                        dr[cIndex] = values[cIndex];
                    }
                    tbl.Rows.Add(dr);
                    //}
                    ii++;
                }
            }
            return tbl;
        }

        /// <summary>
        /// Returns a datatable object from a specific spreadsheet in an Excel Workbook
        /// </summary>
        /// <param name="pathName">The workbook to be read.</param>
        /// <param name="sheetName">The specific sheet in the workbook. Defaults to Sheet1 if no sheet name is given.</param>
        /// <returns>Returns a datatable object generated from the input workbook and specific spreadsheet.</returns>
        public static DataTable ExcelToDataTable(string pathName, string sheetName = "Sheet1")
        {
            DataTable tbContainer = new DataTable();
            string strConn = string.Empty;
            if (string.IsNullOrEmpty(sheetName)) { sheetName = "Sheet1"; }
            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("Error, file doesn't exists!"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheetName), cnnxls);
            DataSet ds = new DataSet();
            oda.Fill(tbContainer);
            return tbContainer;
        }

        /// <summary>
        /// Opens a folder picker dialog to select the location of the output and returns that directory.
        /// </summary>
        /// <param name="Title">Title of the folder picker dialog</param>
        /// <returns></returns>
        public static string OutputFolder(string Title)
        {
            var dialog = new CommonOpenFileDialog();
            if (!string.IsNullOrEmpty(directory))
                dialog.DefaultDirectory = directory;

            dialog.Title = Title;
            dialog.EnsurePathExists = true;
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Cancel)
            {
                MessageBox.Show("No output path selected.", "No Write Path", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return string.Empty;
            }

            return dialog.FileName;

        }
    }

}
