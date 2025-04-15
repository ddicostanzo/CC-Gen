using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using OSU_Helpers;
using System.Windows;
using System.Collections;

namespace OSUStructureGenerator
{
    /// <summary>
    /// Generates a structure from a line of text in a configuration file.
    /// </summary>
    public class StructureGenerationFromText
    {
        /// <summary>
        /// The generated structure that will end up in the Eclipse Structure Set.
        /// </summary>
        public Structure GeneratedStructure { get; private set; }
        /// <summary>
        /// The Structure Set to use as part of the structure generation.
        /// </summary>
        public StructureSet BaseStructureSet { get; private set; }
        /// <summary>
        /// The flag as to whether to overwrite a structure or create a new one.
        /// </summary>
        private bool OverwriteExisting { get; set; } = false;
        /// <summary>
        /// The DICOM type of the structure if it is being created.
        /// </summary>
        private string dicomtype;
        /// <summary>
        /// The ID of the strucutre to be overwritten or created.
        /// </summary>
        private string id;

        /// <summary>
        /// The constructor for this class that takes both the line from the text file and the structure set to work on.
        /// A generic catch is used to determine if the structure provided for overwriting is present. If not, it will be created.
        /// </summary>
        /// <param name="line">The line for the text file to create or overwrite a structure. Please see documentation for format.</param>
        /// <param name="structureSet">The ESAPI StructureSet to use for this operation.</param>
        /// <exception cref="ArgumentException">Will throw an argument exception if no DICOM type is provided when creating a new structure.</exception>
        public StructureGenerationFromText(string line, StructureSet structureSet)
        {
            if (line.ToCharArray()[0] == '!' || line.ToCharArray()[0] == '~')
                OverwriteExisting = true;

            BaseStructureSet = structureSet;
            //_Opti,AVOIDANCE = PTV.Ring(5,20).CropOut(Body,3)
            string[] target_and_operation = GetTargetAndOps(line);

            //[0]_Opti     [1]PTV.Ring(5,20).CropOut(Body,3)
            string[] operations = target_and_operation[1].Split('.');
            //operations => [0]PTV     [1]Ring(5,20)    [2]CropOut(Body,3)

            //_Opti,AVOIDANCE
            //check if color was provided prior to getting ID and type.
            string id_part = target_and_operation[0];
            string colorPart = String.Empty;
            if (id_part.Contains("|"))
            {
                id_part = target_and_operation[0].Split('|').First();
                colorPart = target_and_operation[0].Split('|').Last();
            }
            string[] id_and_type = id_part.Split(',');

            id = ValidateStructureID(id_and_type[0]);

            if (!OverwriteExisting)
            {
                if (id_and_type.Length != 2)
                    throw new ArgumentException("No DICOM type was supplied for a new structure. Please supply.\r\n" + line);

                dicomtype = ValidateDICOMType(id_and_type[1]);
                GeneratedStructure = BaseStructureSet.AddStructure(dicomtype, id);
                //GeneratedStructure.SegmentVolume = GetSegmentVolumeFromBase(operations[0]);
            }
            else
            {
                try
                {
                    GeneratedStructure = BaseStructureSet.Structures.First(a => a.Id.ToUpper() == id.ToUpper());
                }
                catch
                {
                    MessageBox.Show("Cannot find structure with ID = " + id + ". \r\nGenerating a new structure with CONTROL as the DICOM Type.",
                        "New Structure", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                    //GeneratedStructure = BaseStructureSet.AddStructure("CONTROL", id);

                }
            }

            if (!String.IsNullOrEmpty(colorPart))
            {
                string[] colors = colorPart.Split(',');
                if (colors.Length != 3)
                {
                    MessageBox.Show($"Color provided but incorrect format: {colorPart}");
                }
                else
                {
                    GeneratedStructure.Color = System.Windows.Media.Color.FromRgb(
                        Convert.ToByte(Convert.ToInt16(colors.First())),
                        Convert.ToByte(Convert.ToInt16(colors.ElementAt(1))),
                        Convert.ToByte(Convert.ToInt16(colors.Last())));
                }
            }

            GeneratedStructure.SegmentVolume = GetSegmentVolumeFromBase(operations[0]);
            ApplyOperations(operations, line);
        }

        /// <summary>
        /// The private method to apply the operations from the text file to create the structure.
        /// </summary>
        /// <param name="operations">A string array that includes the operations provided in the text file.</param>
        /// <param name="line">The full line of text for this operation. Only used if an error is encountered to provide visibility to the line causing issues.</param>
        /// <exception cref="ArgumentException">Thrown in the arguments provided to an operation are not valid.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown in no structures in the structure set are found with provided search string.</exception>
        private void ApplyOperations(string[] operations, string line)
        {
            //[1]Ring(5,20)    [2]CropOut(Body,3)
            for (int i = 1; i < operations.Length; i++)
            {
                //Ring(5,20)
                string[] splitops = operations[i].Replace(")", string.Empty).Split('(');
                //[0]Ring     [1]5,20
                string operation = splitops[0];
                //operation = Ring
                string[] arguments = splitops[1].Split(',');
                //arguments => [0]5 [1]20

                if (operation.ToUpper() == "RING" && (arguments.Length == 2 || arguments.Length == 3))
                {
                    int start = int.Parse(arguments[0]);
                    int end = int.Parse(arguments[1]);
                    if (arguments.Length == 2)
                    {
                        //GeneratedStructure = GeneratedStructure.GenerateRing(BaseStructureSet, start, end);
                        GeneratedStructure.SegmentVolume = BaseStructureSet.GenerateRing(GeneratedStructure, start, end);
                    }
                    else
                    {
                        //GeneratedStructure = GeneratedStructure.GenerateRing(BaseStructureSet, start, end, true);
                        GeneratedStructure.SegmentVolume = BaseStructureSet.GenerateRing(GeneratedStructure, start, end, true);
                    }
                }
                else if (operation.ToUpper() == "OR")
                {
                    List<Structure> structures = new List<Structure>();
                    //structures.Add(GeneratedStructure);
                    foreach (string s in arguments)
                    {
                        structures.Add(GetStructureFromBase(s));
                    }
                    GeneratedStructure.SegmentVolume = BaseStructureSet.UnionStructures(structures, GeneratedStructure);
                }
                else if (operation.ToUpper() == "AND")
                {
                    List<Structure> structures = new List<Structure>();

                    foreach (string s in arguments)
                    {
                        structures.Add(GetStructureFromBase(s));
                    }
                    GeneratedStructure.SegmentVolume = BaseStructureSet.IntersectionOfStructures(GeneratedStructure, structures);
                }
                else if (operation.ToUpper() == "SUB")
                {
                    List<Structure> structures = new List<Structure>();

                    foreach (string s in arguments)
                    {
                        structures.Add(GetStructureFromBase(s));
                    }
                    GeneratedStructure.SegmentVolume = BaseStructureSet.SubStructures(GeneratedStructure, structures);
                }
                else if (operation.ToUpper() == "NOT")
                {
                    List<Structure> structures = new List<Structure>();

                    foreach (string s in arguments)
                    {
                        structures.Add(GetStructureFromBase(s));
                    }
                    GeneratedStructure.SegmentVolume = BaseStructureSet.NonOverlapStructure(GeneratedStructure, structures);
                }
                else if (operation.ToUpper() == "CROPOUT" && arguments.Length == 2)
                {
                    Structure CropFromStructure = GetStructureFromBase(arguments[0]);

                    GeneratedStructure.SegmentVolume = BaseStructureSet.CropExtendingOutside(GeneratedStructure, CropFromStructure, int.Parse(arguments[1]));
                }
                else if (operation.ToUpper() == "CROPIN" && arguments.Length == 2)
                {
                    Structure CropFromStructure = GetStructureFromBase(arguments[0]);

                    GeneratedStructure.SegmentVolume = BaseStructureSet.CropExtendingInside(GeneratedStructure, CropFromStructure, int.Parse(arguments[1]));
                }
                else if (operation.ToUpper() == "MARGIN" && arguments.Length == 1)
                {
                    GeneratedStructure.SegmentVolume = GeneratedStructure.SegmentVolume.MarginGreaterThan50mm(int.Parse(arguments[0]));
                }
                else if (operation.ToUpper() == "PTVALL")
                {
                    List<Structure> ptvs = new List<Structure>();
                    foreach (Structure s in BaseStructureSet.Structures)
                    {
                        if (s.Id.ToUpper().StartsWith("PTV"))
                            ptvs.Add(s);
                    }

                    GeneratedStructure.SegmentVolume = ptvs.TotalSegmentVolume(BaseStructureSet);
                }
                else if (operation.ToUpper() == "HIGHRES")
                {
                    if (GeneratedStructure.CanConvertToHighResolution())
                        GeneratedStructure.ConvertToHighResolution();
                }
                /// Ant, Post, Left, Right, Sup, Inf
                else if (operation.ToUpper() == "ASYMMARGIN" && arguments.Length >= 6)
                {
                    //Margins = -x, -y, -z, x, y, z
                    //Margins = Right, Ant, Inf, Left, Post, Sup
                    int[] margins = new int[6];
                    margins[0] = int.Parse(arguments[3]);
                    margins[1] = int.Parse(arguments[0]);
                    margins[2] = int.Parse(arguments[5]);
                    margins[3] = int.Parse(arguments[2]);
                    margins[4] = int.Parse(arguments[1]);
                    margins[5] = int.Parse(arguments[4]);

                    StructureMarginGeometry margingeometry = StructureMarginGeometry.Outer;
                    if (arguments.Length > 6)
                        Enum.TryParse(arguments[6], true, out margingeometry);

                    SegmentVolume sv = GeneratedStructure.SegmentVolume;

                    if (margins.All(a => a <= 50))
                    {
                        var axisAligned = new AxisAlignedMargins(margingeometry, margins[0], margins[1], margins[2], margins[3], margins[4], margins[5]);
                        sv = sv.AsymmetricMargin(axisAligned);
                    }
                    else
                    {
                        sv = sv.AsymMarginGreaterThan50mm(margins, margingeometry);
                    }

                    GeneratedStructure.SegmentVolume = sv;

                }
                else if (operation.ToUpper() == "ALL" && arguments.Length == 2)
                {
                    string search_text = arguments[0].ToUpper();
                    string op = arguments[1].ToUpper();
                    SegmentVolume sv = GeneratedStructure.SegmentVolume;

                    List<Structure> structures = new List<Structure>();
                    if (op == "STARTS")
                    {
                        structures = BaseStructureSet.Structures.Where(a => a.Id.ToUpper().StartsWith(search_text) && !a.IsEmpty).ToList();
                    }
                    else if (op == "IN")
                    {
                        structures = BaseStructureSet.Structures.Where(a => a.Id.ToUpper().Contains(search_text) && !a.IsEmpty).ToList();

                    }
                    else if (op == "ENDS")
                    {
                        structures = BaseStructureSet.Structures.Where(a => a.Id.ToUpper().EndsWith(search_text) && !a.IsEmpty).ToList();

                    }
                    else { throw new ArgumentException("The specified operation does not exist. \r\nStarts, In, and Ends are the only valid search parameters:\r\n" + line); }

                    if (structures.Count > 0)
                    {
                        GeneratedStructure.SegmentVolume = structures.TotalSegmentVolume(BaseStructureSet);
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("There were no structures found in the structure set with the specified search string.\r\n" + line);
                    }
                }
                else if (operation.ToUpper() == "SIMTPTV")
                {
                    var strs = BaseStructureSet.Structures.ToArray();
                    var c = strs.Length;

                    for (int l = 0; l < c; l++)
                    {
                        if (strs[l].Id.ToUpper().StartsWith("PTV"))
                        {
                            string ptv_id = string.Empty;
                            if (strs[l].Id.Length == 4)
                            {
                                ptv_id = strs[l].Id;
                            }
                            else if (strs[l].Id.Length == 5)
                            {
                                ptv_id = strs[l].Id;
                            }
                            else if (strs[l].Id.Contains("_"))
                            {
                                ptv_id = strs[l].Id.Split('_')[0];
                            }
                            else
                            {
                                ptv_id = strs[l].Id.Substring(0, 5);
                                double t = -1;
                                if (!double.TryParse(ptv_id[ptv_id.Length - 1].ToString(), out t))
                                {
                                    ptv_id.Remove(4, 1);
                                }
                            }


                            GeneratedStructure = BaseStructureSet.AddStructure("PTV", ("z_" + ptv_id + "_10mm"));
                            GeneratedStructure.SegmentVolume = strs[l].SegmentVolume;
                            GeneratedStructure.SegmentVolume = GeneratedStructure.SegmentVolume.MarginGreaterThan50mm(10);

                        }
                    }
                }
                else if (operation.ToUpper() == "LIST" && arguments.Length == 2)
                {
                    string search_text = arguments[0].ToUpper();
                    string op = arguments[1].ToUpper();

                    string[] individual_operations = new string[operations.Length - 1];
                    individual_operations[0] = string.Empty;

                    for (int z = 2; z < operations.Length; z++)
                    {
                        individual_operations[z - 1] = operations[z];
                    }

                    if (CheckForBaseStructureExistence())
                        throw new ArgumentException("The base structure id ("+ id + ") appended with a number already exists in the structure set. Please remove or rename before continuing:\r\n" + line);


                    List<Structure> structures = new List<Structure>();
                    if (op == "STARTS")
                    {
                        structures = BaseStructureSet.Structures.Where(a => a.Id.ToUpper().StartsWith(search_text) && !a.IsEmpty).ToList();
                        //structures = OrderedStructures(structures, search_text);
                    }
                    else if (op == "IN")
                    {
                        structures = BaseStructureSet.Structures.Where(a => a.Id.ToUpper().Contains(search_text) && !a.IsEmpty).ToList();

                    }
                    else if (op == "ENDS")
                    {
                        structures = BaseStructureSet.Structures.Where(a => a.Id.ToUpper().EndsWith(search_text) && !a.IsEmpty).ToList();

                    }
                    else { throw new ArgumentException("The specified operation does not exist. \r\nStarts, In, and Ends are the only valid search parameters:\r\n" + line); }

                    if (structures.Count > 0)
                    {
                        for (int z = 0; z < structures.Count; z++)
                        {
                            GeneratedStructure = BaseStructureSet.AddStructure(dicomtype, id + (z + 1).ToString());
                            GeneratedStructure.SegmentVolume = structures[z].SegmentVolume;
                            ApplyOperations(individual_operations, line);
                        }
                        BaseStructureSet.RemoveStructure(BaseStructureSet.Structures.First(a => a.Id == id));
                        break;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("There were no structures found in the structure set with the specified search string.\r\n" + line);
                    }
                }
                else { throw new ArgumentException("There was an issue with the structure generation associated with:\r\n" + line); }
            }
        }

        /// <summary>
        /// Checks the structure set for the presence of a structure.
        /// </summary>
        /// <returns></returns>
        private bool CheckForBaseStructureExistence()
        {
            List<string> possible_ids = new List<string>();
            for (int i = 1; i < 100; i++)
            {
                possible_ids.Add((id + i.ToString()));
            }

            foreach(string s in BaseStructureSet.Structures.Select(a => a.Id))
            {
                if (possible_ids.Contains(s))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a specific structure from the structure set using the structure ID.
        /// </summary>
        /// <param name="s">Structure ID to search for.</param>
        /// <returns>ESAPI <see cref="Structure"/> found.</returns>
        private Structure GetStructureFromBase(string s)
        {
            return BaseStructureSet.Structures.First(a => a.Id.ToUpper() == s.ToUpper());
        }

        /// <summary>
        /// Gets a specific segment volume from a structure searched for in the structure set using the structure ID.
        /// </summary>
        /// <param name="s">Structure ID to search for.</param>
        /// <returns>ESAPI <see cref="SegmentVolume"/> found.</returns>
        private SegmentVolume GetSegmentVolumeFromBase(string v)
        {
            return BaseStructureSet.Structures.First(a => a.Id.ToUpper() == v.ToUpper()).SegmentVolume;
        }

        /// <summary>
        /// Splits the line from the text file into the target structure and the operations to create it.
        /// </summary>
        /// <param name="line">String from the text file to parse.</param>
        /// <returns>String array containing two elements, target structure with optional DICOM type and ID and the operations delimited by periods.</returns>
        private string[] GetTargetAndOps(string line)
        {
            string[] _ret = line.Split('=');
            for (int i = 0; i < _ret.Length; i++)
            {
                _ret[i] = _ret[i].Trim();
            }
            return _ret;
        }

        /// <summary>
        /// Validation of the provided DICOM type.
        /// </summary>
        /// <param name="v">The provided DICOM type to use for structure creation.</param>
        /// <returns>Either the provided DICOM type or the the DICOM Type of NONE if the provided one is not in the list below.</returns>
        private string ValidateDICOMType(string v)
        {
            v = v.ToUpper();
            if (v == "PTV" || v == "CTV" || v == "GTV" || v == "AVOIDANCE" || v == "CONTROL" || v == "ORGAN")
            {
                return v;
            }
            else
            {
                return "NONE";
            }
        }

        /// <summary>
        /// Validation of the structure ID. If the structure ID is present and the Overwrite Flag is not set, then a unique structure ID will be created.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private string ValidateStructureID(string v)
        {
            if (OverwriteExisting)
                v = v.Substring(1, v.Length - 1);

            string id_return = v.Trim();

            if (id_return.Length > 16)
            {
                id_return = id_return.Substring(0, 16);
            }
            StringBuilder sb = new StringBuilder();
            char[] id_array = id_return.ToCharArray();
            for (int i = 0; i < id_array.Length; i++)
            {
                if (char.IsLetterOrDigit(id_array[i]) || id_array[i] == '^' || id_array[i] == '_' || id_array[i] == '-')
                    sb.Append(id_array[i]);
                else
                    sb.Append('_');
            }

            if (OverwriteExisting)
                return sb.ToString();
            else
                return sb.ToString().UniqueStructureId(BaseStructureSet);
        }

    }

}