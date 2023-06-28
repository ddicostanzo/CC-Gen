using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using static OSU_Helpers.Enumerables;

namespace OSU_Helpers
{
    /// <summary>
    /// Class to hold extension methods for the ESAPI <c>Structure</c> class
    /// </summary>
    public static class StructureHelpers
    {
        /// <summary>
        /// Generates a unique structure ID for the base structure by appending the next digit.
        /// </summary>
        /// <param name="BaseIdString">Base structure ID to append the digit to</param>
        /// <param name="structureSet">The structure set where the structures reside</param>
        /// <returns>A structure ID in the form of a string that has a unique ID for the provided structure set.</returns>
        public static string UniqueStructureId(this string BaseIdString, StructureSet structureSet)
        {
            if (structureSet.Structures.Where(a => a.Id == BaseIdString).Count() > 0)
            {
                string tmp_id_r1 = BaseIdString;
                int i = 0;
                while (structureSet.Structures.Where(a => a.Id == tmp_id_r1).Count() > 0 && tmp_id_r1.Length < 16)
                {
                    tmp_id_r1 = BaseIdString + "_" + i.ToString();
                    i++;
                }
                return tmp_id_r1;
            }
            else
            {
                return BaseIdString;
            }
        }

        /// <summary>
        /// Generates the ring based upon a particular structure, adds to the given structure set, using the distances provided
        /// </summary>
        /// <param name="_ResultantRing">Base structure to generate the ring from</param>
        /// <param name="BaseStructureSet"></param>
        /// <param name="StartDistance_mm"></param>
        /// <param name="EndDistance_mm"></param>
        /// <param name="HighResFlag"></param>
        /// <returns>The ring as a structure type.</returns>
        public static Structure GenerateRing(this Structure _ResultantRing, StructureSet BaseStructureSet, int StartDistance_mm, int EndDistance_mm, bool HighResFlag = false)
        {
            if (HighResFlag && !_ResultantRing.IsHighResolution)
                _ResultantRing.ConvertToHighResolution();

            string id_r1 = "_tempr1";
            string id_r2 = "_tempr2";

            id_r1 = id_r1.UniqueStructureId(BaseStructureSet);
            id_r2 = id_r2.UniqueStructureId(BaseStructureSet);

            Structure temp_start = BaseStructureSet.AddStructure("AVOIDANCE", id_r1);
            if (HighResFlag && !temp_start.IsHighResolution) temp_start.ConvertToHighResolution();
            Structure temp_end = BaseStructureSet.AddStructure("AVOIDANCE", id_r2);
            if (HighResFlag && !temp_end.IsHighResolution) temp_end.ConvertToHighResolution();

            temp_start.SegmentVolume = _ResultantRing.SegmentVolume.MarginGreaterThan50mm(StartDistance_mm);
            temp_end.SegmentVolume = _ResultantRing.SegmentVolume.MarginGreaterThan50mm(EndDistance_mm);

            _ResultantRing.SegmentVolume = temp_end.Sub(temp_start);

            BaseStructureSet.RemoveStructure(temp_start);
            BaseStructureSet.RemoveStructure(temp_end);

            return _ResultantRing;
        }

        ///// <summary>
        ///// Generates a ring from a base structure
        ///// </summary>
        ///// <param name="structure">The base structure</param>
        ///// <param name="ss">The structure set to use during calculation</param>
        ///// <param name="StartDistance_mm">The distance the ring should start from the base structure in milimeters</param>
        ///// <param name="EndDistance_mm">The distance the ring should end from the base structure in milimeters</param>
        ///// <returns>Returns a structure that is the ring created at the specified distances from the base structure.</returns>
        //public static Structure GenerateRing(this Structure structure, StructureSet ss, double StartDistance_mm, double EndDistance_mm)
        //{
        //    Structure exp1 = ss.AddStructure("CONTROL", "Exp1");
        //    Structure exp2 = ss.AddStructure("CONTROL", "Exp2");

        //    exp1.SegmentVolume = structure.Margin(StartDistance_mm);
        //    exp2.SegmentVolume = structure.Margin(EndDistance_mm);

        //    structure.SegmentVolume = exp2.Sub(exp1.SegmentVolume);

        //    ss.RemoveStructure(exp1);
        //    ss.RemoveStructure(exp2);

        //    return structure;
        //}

        /// <summary>
        /// Generates a ring <c>SegmentVolume</c> from a collection of base structures that have been added together
        /// </summary>
        /// <param name="structure">The base structure collection</param>
        /// <param name="ss">The structure set to use during calculation</param>
        /// <param name="StartDistance_mm">The distance the ring should start from the base structure in milimeters</param>
        /// <param name="EndDistance_mm">The distance the ring should end from the base structure in milimeters</param>
        /// <returns>Returns a <c>SegmentVolume</c> that is the ring created at the specified distances from the base structure.</returns>
        public static SegmentVolume GenerateRing(this ICollection<Structure> structure, StructureSet ss, double StartDistance_mm, double EndDistance_mm)
        {
            SegmentVolume sv = structure.TotalSegmentVolume(ss);

            Structure exp1 = ss.AddStructure("CONTROL", "Exp1");
            Structure exp2 = ss.AddStructure("CONTROL", "Exp2");

            exp1.SegmentVolume = sv.Margin(StartDistance_mm);
            exp2.SegmentVolume = sv.Margin(EndDistance_mm);

            sv = exp2.Sub(exp1.SegmentVolume);

            ss.RemoveStructure(exp1);
            ss.RemoveStructure(exp2);

            return sv;
        }


        /// <summary>
        /// Generates a segment volume that is the union of all structures within the collection. Does not protect against approved structures.
        /// </summary>
        /// <param name="structures">The base collection of structures</param>
        /// <returns>Returns a segment volume that is the union of all structures within the collection</returns>
        public static SegmentVolume TotalSegmentVolume(this IEnumerable<Structure> structures)
        {
            int st_count = structures.Count();
            bool high_res = structures.Any(a => a.IsHighResolution);

            SegmentVolume sv;
            if (high_res)
            {

                if (!structures.ElementAt(0).IsHighResolution)
                {
                    try
                    {
                        structures.ElementAt(0).ConvertToHighResolution();
                    }
                    catch
                    {

                    }
                }
                sv = structures.ElementAt(0).SegmentVolume;
                for (int i = 1; i < st_count; i++)
                {
                    if (!structures.ElementAt(i).IsHighResolution)
                    {
                        structures.ElementAt(i).ConvertToHighResolution();
                    }
                    sv = sv.Or(structures.ElementAt(i).SegmentVolume);
                }

            }
            else
            {
                sv = structures.ElementAt(0).SegmentVolume;
                for (int i = 1; i < st_count; i++)
                {
                    sv = sv.Or(structures.ElementAt(i).SegmentVolume);
                }

            }
            return sv;
        }

        /// <summary>
        /// Generates a segment volume that is the union of all structures within the collection. Protects against approved structures and high resolution structures.
        /// </summary>
        /// <param name="structures">The base collection of structures</param>
        /// <param name="BaseStructureSet">The structure set that is the parent of the structures being joined together</param>
        /// <param name="HighResFlag">Optional parameter for high resolution. Defaults to false.</param>
        /// <returns>Returns a segment volume that is the union of all structures within the collection</returns>
        public static SegmentVolume TotalSegmentVolume(this IEnumerable<Structure> structures, StructureSet BaseStructureSet, bool HighResFlag = false)
        {
            int st_count = structures.Count();
            bool high_res = HighResFlag;
            if(structures.Any(a => a.IsHighResolution))
                high_res = true;

            SegmentVolume sv = structures.ElementAt(0).SegmentVolume;
            if (high_res)
            {
                Structure structureAtZeroIndex = structures.ElementAt(0);
                if (!structureAtZeroIndex.IsHighResolution)
                {
                    try
                    {
                        structureAtZeroIndex.ConvertToHighResolution();
                        sv = structureAtZeroIndex.SegmentVolume;
                    }
                    catch
                    {
                        string dicomtype = (structureAtZeroIndex.DicomType == "EXTERNAL" || structureAtZeroIndex.DicomType == "SUPPORT" || string.IsNullOrEmpty(structureAtZeroIndex.DicomType)) ? "CONTROL" : structureAtZeroIndex.DicomType;
                        structureAtZeroIndex = BaseStructureSet.AddStructure(dicomtype, structureAtZeroIndex.Id.UniqueStructureId(BaseStructureSet));
                        structureAtZeroIndex.ConvertToHighResolution();
                        sv = structureAtZeroIndex.SegmentVolume;
                        BaseStructureSet.RemoveStructure(structureAtZeroIndex);
                    }

                }

                for (int i = 1; i < st_count; i++)
                {
                    Structure structureAtIndex = structures.ElementAt(i);
                    if (!structureAtIndex.IsHighResolution)
                    {
                        try
                        {
                            structureAtIndex.ConvertToHighResolution();
                            sv = sv.Or(structureAtIndex.SegmentVolume);
                        }
                        catch
                        {
                            string dicomtype = (structureAtIndex.DicomType == "EXTERNAL" || structureAtIndex.DicomType == "SUPPORT" || string.IsNullOrEmpty(structureAtIndex.DicomType)) ?  "CONTROL" : structureAtIndex.DicomType;
                            structureAtIndex = BaseStructureSet.AddStructure(dicomtype, structureAtIndex.Id.UniqueStructureId(BaseStructureSet));
                            structureAtIndex.ConvertToHighResolution();
                            sv = sv.Or(structureAtIndex.SegmentVolume);
                            BaseStructureSet.RemoveStructure(structureAtIndex);
                        }
                    }
                    else
                    {
                        sv = sv.Or(structureAtIndex.SegmentVolume);
                    }
                }

            }
            else
            {
                sv = structures.ElementAt(0).SegmentVolume;
                for (int i = 1; i < st_count; i++)
                {
                    sv = sv.Or(structures.ElementAt(i).SegmentVolume);
                }

            }
            return sv;
        }

        ///// <summary>
        ///// Returns a new segment volume that is contained by both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">Second structure to create union with extended structure</param>
        ///// <returns>A segment volume that is the union of both the extended structure and the parameter structure</returns>
        //public static SegmentVolume UnionStructures(this Structure s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.Or(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is contained by both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">A collection of structures that will be added together to create union with extended structure</param>
        ///// <returns>A segment volume that is the union of both the extended structure and the parameter structure collection</returns>
        //public static SegmentVolume UnionStructures(this Structure s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.Or(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is contained by both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection that is to be unioned before union with additional structure</param>
        ///// <param name="s2">Second structure to create union with extended structure collection</param>
        ///// <returns>A segment volume that is the union of both the extended structure collection and the parameter structure </returns>
        //public static SegmentVolume UnionStructures(this ICollection<Structure> s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.Or(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is contained by both submitted structure collections
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection that is to be unioned before union with additional structure</param>
        ///// <param name="s2">A collection of structures that will be added together to create union with extended structure</param>
        ///// <returns>A segment volume that is the union of both the extended structure collection and the parameter structure collection</returns>
        //public static SegmentVolume UnionStructures(this ICollection<Structure> s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.Or(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is the extended structure minus the parameter structure
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">Second structure to subtract with extended structure</param>
        ///// <returns>A segment volume that is the subtraction of the extended structure and the parameter structure</returns>
        //public static SegmentVolume SubStructures(this Structure s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is extended structure minus the parameter structure collection
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">A collection of structures that will be subtracted from the extended structure</param>
        ///// <returns>A segment volume that is the subtraction of both the extended structure and the parameter structure collection</returns>
        //public static SegmentVolume SubStructures(this Structure s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is the subtraction of the extended structure and the parameter structure
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection that is to be subtracted from by the additional structure</param>
        ///// <param name="s2">Second structure to subtract from the extended structure collection</param>
        ///// <returns>A segment volume that is the subtraction of both the extended structure collection by the parameter structure </returns>
        //public static SegmentVolume SubStructures(this ICollection<Structure> s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is subtraction of the extended structure collection by the submitted structure collection
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection that is to be subtracted from by the parametr structure collection</param>
        ///// <param name="s2">A collection of structures that will be added together and then subtracted from the extended structure</param>
        ///// <returns>A segment volume that is the subtraction of the extended structure collection by the parameter structure collection</returns>
        //public static SegmentVolume SubStructures(this ICollection<Structure> s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is the intersection of both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">Second structure to intersect with extended structure</param>
        ///// <returns>A segment volume that is the intersection of both the extended structure and the parameter structure</returns>
        //public static SegmentVolume IntersectionOfStructures(this Structure s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.And(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is intersection of both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">A collection of structures that will be added together and then intersected with the extended structure</param>
        ///// <returns>A segment volume that is the intersection of both the extended structure and the parameter structure collection</returns>
        //public static SegmentVolume IntersectionOfStructures(this Structure s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.And(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is intersection of both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection</param>
        ///// <param name="s2">Structure to intersect with extended structure collection</param>
        ///// <returns>A segment volume that is the intersection of both the extended structure collection and the parameter structure </returns>
        //public static SegmentVolume IntersectionOfStructures(this ICollection<Structure> s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.And(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is intersection of both submitted structure collections
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection</param>
        ///// <param name="s2">A collection of structures that will be added together and intersected with the extended structure collection</param>
        ///// <returns>A segment volume that is the intersection of both the extended structure collection and the parameter structure collection</returns>
        //public static SegmentVolume IntersectionOfStructures(this ICollection<Structure> s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.And(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is the non-intersecting structure of both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">Second structure to intersect with extended structure</param>
        ///// <returns>A segment volume that is the non-intersection of both the extended structure and the parameter structure</returns>
        //public static SegmentVolume NonOverlapStructure(this Structure s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.Xor(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is non-intersection of both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure</param>
        ///// <param name="s2">A collection of structures that will be added together and then intersected with the extended structure</param>
        ///// <returns>A segment volume that is the non-intersection of both the extended structure and the parameter structure collection</returns>
        //public static SegmentVolume NonOverlapStructure(this Structure s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.SegmentVolume;
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.Xor(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is the non-intersection of both submitted structures
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection</param>
        ///// <param name="s2">Structure to intersect with extended structure collection</param>
        ///// <returns>A segment volume that is the non-intersection of both the extended structure collection and the parameter structure </returns>
        //public static SegmentVolume NonOverlapStructure(this ICollection<Structure> s1, Structure s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.SegmentVolume;

        //    return sv1.Xor(sv2);
        //}

        ///// <summary>
        ///// Returns a new segment volume that is non-intersection of both submitted structure collections
        ///// </summary>
        ///// <param name="s1">Primary, extended structure collection</param>
        ///// <param name="s2">A collection of structures that will be added together and intersected with the extended structure collection</param>
        ///// <returns>A segment volume that is the non-intersection of both the extended structure collection and the parameter structure collection</returns>
        //public static SegmentVolume NonOverlapStructure(this ICollection<Structure> s1, ICollection<Structure> s2)
        //{
        //    SegmentVolume sv1 = s1.TotalSegmentVolume();
        //    SegmentVolume sv2 = s2.TotalSegmentVolume();

        //    return sv1.Xor(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structure to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingOutside(this Structure PrimaryStructure, Structure CropFromStructure, int CropDistance)
        //{
        //    var sv1 = PrimaryStructure.SegmentVolume;
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.Not().Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.Not().MarginGreaterThan50mm(CropDistance);
        //    }
        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structures to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingOutside(this Structure PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        //{
        //    var sv1 = PrimaryStructure.SegmentVolume;
        //    var crop_sv = CropFromStructure.TotalSegmentVolume();
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = crop_sv.Not().Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = crop_sv.Not().MarginGreaterThan50mm(CropDistance);
        //    }

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structure to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingOutside(this ICollection<Structure> PrimaryStructure, Structure CropFromStructure, int CropDistance)
        //{
        //    var sv1 = PrimaryStructure.TotalSegmentVolume();
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.Not().Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.Not().MarginGreaterThan50mm(CropDistance);
        //    }

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structures to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingOutside(this ICollection<Structure> PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        //{
        //    var sv1 = PrimaryStructure.TotalSegmentVolume();
        //    var crop_sv = CropFromStructure.TotalSegmentVolume();
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = crop_sv.Not().Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = crop_sv.Not().MarginGreaterThan50mm(CropDistance);
        //    }

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structure to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingInside(this Structure PrimaryStructure, Structure CropFromStructure, int CropDistance)
        //{
        //    if (PrimaryStructure.IsHighResolution && !CropFromStructure.IsHighResolution)
        //        CropFromStructure.ConvertToHighResolution();
        //    if (!PrimaryStructure.IsHighResolution && CropFromStructure.IsHighResolution)
        //        PrimaryStructure.ConvertToHighResolution();

        //    var sv1 = PrimaryStructure.SegmentVolume;
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.MarginGreaterThan50mm(CropDistance);
        //    }

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structure to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingInside(this Structure PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        //{
        //    var sv1 = PrimaryStructure.SegmentVolume;
        //    var crop_sv = CropFromStructure.TotalSegmentVolume();
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = crop_sv.Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = crop_sv.MarginGreaterThan50mm(CropDistance);
        //    }
        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structure to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingInside(this ICollection<Structure> PrimaryStructure, Structure CropFromStructure, int CropDistance)
        //{
        //    var sv1 = PrimaryStructure.TotalSegmentVolume();
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = CropFromStructure.SegmentVolume.MarginGreaterThan50mm(CropDistance);
        //    }

        //    return sv1.Sub(sv2);
        //}

        ///// <summary>
        ///// Takes the primary structure and returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance.
        ///// </summary>
        ///// <param name="PrimaryStructure">The base structure that is extended by this method.</param>
        ///// <param name="CropFromStructure">The structure to crop the base structure from.</param>
        ///// <param name="CropDistance">The distance to crop the Primary Structure from the other supplied structure. Limited to 0-50mm.</param>
        ///// <returns>Returns a <c>SegmentVolume</c> that is the Primary Volume cropped from the other supplied structure by the specified distance. </returns>
        //public static SegmentVolume CropExtendingInside(this ICollection<Structure> PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        //{
        //    var sv1 = PrimaryStructure.TotalSegmentVolume();
        //    var crop_sv = CropFromStructure.TotalSegmentVolume();
        //    SegmentVolume sv2;
        //    if (CropDistance <= 50)
        //    {
        //        sv2 = crop_sv.Margin(CropDistance);
        //    }
        //    else
        //    {
        //        sv2 = crop_sv.MarginGreaterThan50mm(CropDistance);
        //    }

        //    return sv1.Sub(sv2);
        //}

        /// <summary>
        /// The extension method returns a structure that is the base extended structure that is expanded by the supplied margin, regardless of size.
        /// </summary>
        /// <param name="structure">The base structure that is extended.</param>
        /// <param name="margin">The margin to expand the base structure by.</param>
        /// <returns>Returns a structure that is the expanded structure by the specified margin.</returns>
        public static Structure MarginGreaterThan50mm(this Structure structure, double margin)
        {
            int count_50 = (int)Math.Floor(margin / 50);

            if (count_50 == 0)
            {
                structure.SegmentVolume = structure.Margin(margin);
                return structure;
            }

            double remainder_50 = margin % 50;

            SegmentVolume str_with_margin = structure.SegmentVolume;

            for (int i = 0; i < count_50; i++)
            {
                str_with_margin = str_with_margin.Margin(50);
            }

            str_with_margin = str_with_margin.Margin(remainder_50);

            structure.SegmentVolume = str_with_margin;

            return structure;
        }

        /// <summary>
        /// The extension method returns a SegmentVolume that is the base extended SegmentVolume expanded by the supplied margin, regardless of size.
        /// </summary>
        /// <param name="sv">The base structure that is extended.</param>
        /// <param name="margin">The margin to expand the base structure by.</param>
        /// <returns>Returns a structure that is the expanded structure by the specified margin.</returns>
        public static SegmentVolume MarginGreaterThan50mm(this SegmentVolume sv, double margin)
        {
            int count_50 = (int)Math.Floor(margin / 50);

            if (count_50 == 0)
            {
                sv = sv.Margin(margin);
                return sv;
            }

            double remainder_50 = margin % 50;

            SegmentVolume str_with_margin = sv;

            for (int i = 0; i < count_50; i++)
            {
                str_with_margin = str_with_margin.Margin(50);
            }
            str_with_margin = str_with_margin.Margin(remainder_50);

            sv = str_with_margin;

            return sv;
        }

        /// <summary>
        /// This method provides access to Asymmetric Margins that are either under or over 50mm, but only outer margins. Will return a segment volume with the applied margin.
        /// </summary>
        /// <param name="sv">The segment volume that is extended to provide access to this method.</param>
        /// <param name="margins">The margins to be applied to the segment volume.</param>
        /// <returns>Returns a segment volume with the specified margin applied to the specified direction</returns>
        public static SegmentVolume OuterAsymMarginGreaterThan50mm(this SegmentVolume sv, int[] margins)
        {
            return AsymMarginGreaterThan50mm(sv, margins, StructureMarginGeometry.Outer);
        }

        /// <summary>
        /// This method provides access to Asymmetric Margins that are either under or over 50mm, but only outer margins. Will return a segment volume with the applied margin.
        /// </summary>
        /// <param name="sv">The segment volume that is extended to provide access to this method.</param>
        /// <param name="margins">The margins to be applied to the segment volume.</param>
        /// <returns>Returns a segment volume with the specified margin applied to the specified direction</returns>
        public static SegmentVolume InnerAsymMarginGreaterThan50mm(this SegmentVolume sv, int[] margins)
        {
            return AsymMarginGreaterThan50mm(sv, margins, StructureMarginGeometry.Inner);
        }

        /// <summary>
        /// This method provides access to Asymmetric Margins that are either under or over 50mm, but only outer margins. Will return a segment volume with the applied margin.
        /// </summary>
        /// <param name="sv">The segment volume that is extended to provide access to this method.</param>
        /// <param name="margins">The margins to be applied to the segment volume.</param>
        /// <param name="marginGeometry">Whether the margin is inner or outer, defaults to outer</param>
        /// <returns>Returns a segment volume with the specified margin applied to the specified direction</returns>
        public static SegmentVolume AsymMarginGreaterThan50mm(this SegmentVolume sv, int[] margins, StructureMarginGeometry marginGeometry = StructureMarginGeometry.Outer)
        {
            AxisAlignedMargins axisAligned;

            if (margins.All(a => a <= 50))
            {
                axisAligned = new AxisAlignedMargins(marginGeometry, margins[0], margins[1], margins[2], margins[3], margins[4], margins[5]);
                return sv.AsymmetricMargin(axisAligned);
            }

            List<Tuple<Direction, int, int>> applied_margins = new List<Tuple<Direction, int, int>>();

            for (int i = 0; i < margins.Length; i++)
            {
                int count_50 = (int)Math.Floor((double)margins[i] / 50);
                int remainder_50 = margins[i] % 50;
                Direction direction = (Direction)i;

                Tuple<Direction, int, int> am = new Tuple<Direction, int, int>(direction, count_50, remainder_50);
                applied_margins.Add(am);
            }

            SegmentVolume sv_with_margin = sv;

            int max_50 = applied_margins.Max(a => a.Item2);
            int pos_y_50 = applied_margins.First(a => a.Item1 == Direction.PositiveY).Item2;
            int neg_y_50 = applied_margins.First(a => a.Item1 == Direction.NegativeY).Item2;
            int pos_x_50 = applied_margins.First(a => a.Item1 == Direction.PositiveX).Item2;
            int neg_x_50 = applied_margins.First(a => a.Item1 == Direction.NegativeX).Item2;
            int pos_z_50 = applied_margins.First(a => a.Item1 == Direction.PositiveZ).Item2;
            int neg_z_50 = applied_margins.First(a => a.Item1 == Direction.NegativeZ).Item2;

            int pos_y_rm = applied_margins.First(a => a.Item1 == Direction.PositiveY).Item3;
            int neg_y_rm = applied_margins.First(a => a.Item1 == Direction.NegativeY).Item3;
            int pos_x_rm = applied_margins.First(a => a.Item1 == Direction.PositiveX).Item3;
            int neg_x_rm = applied_margins.First(a => a.Item1 == Direction.NegativeX).Item3;
            int pos_z_rm = applied_margins.First(a => a.Item1 == Direction.PositiveZ).Item3;
            int neg_z_rm = applied_margins.First(a => a.Item1 == Direction.NegativeZ).Item3;

            for (int i = 0; i < max_50; i++)
            {
                int mar_pos_y = (pos_y_50 > 0) ? 50 : 0;
                int mar_neg_y = (neg_y_50 > 0) ? 50 : 0;
                int mar_pos_x = (pos_x_50 > 0) ? 50 : 0;
                int mar_neg_x = (neg_x_50 > 0) ? 50 : 0;
                int mar_pos_z = (pos_z_50 > 0) ? 50 : 0;
                int mar_neg_z = (neg_z_50 > 0) ? 50 : 0;

                axisAligned = new AxisAlignedMargins(marginGeometry, mar_neg_x, mar_neg_y, mar_neg_z, mar_pos_x, mar_pos_y, mar_pos_z);
                sv_with_margin = sv_with_margin.AsymmetricMargin(axisAligned);

                pos_y_50--;
                neg_y_50--;
                pos_x_50--;
                neg_x_50--;
                pos_z_50--;
                neg_z_50--;
            }

            axisAligned = new AxisAlignedMargins(marginGeometry, neg_x_rm, neg_y_rm, neg_z_rm, pos_x_rm, pos_y_rm, pos_z_rm);
            sv_with_margin = sv_with_margin.AsymmetricMargin(axisAligned);

            sv = sv_with_margin;

            return sv;
        }

        /// <summary>
        /// Crop portion of Primary Structure extending outside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingOutside(this StructureSet set, ICollection<Structure> PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        {
            if (CropDistance < 0) throw new ArgumentException("Crop distance must be positive");

            if (PrimaryStructure.Any(a => a.IsHighResolution) || CropFromStructure.Any(a => a.IsHighResolution))
            {
                var temp1 = PrimaryStructure.ConvertAllToHighRes(set);
                var temp2 = CropFromStructure.ConvertAllToHighRes(set);
                var svp = temp1.TotalSegmentVolume(set);
                var svf = temp2.TotalSegmentVolume(set).Not().MarginGreaterThan50mm(CropDistance);

                SegmentVolume sv = svp.Sub(svf);

                set.RemoveStructuresFromStructureSet(temp1);
                set.RemoveStructuresFromStructureSet(temp2);
                return sv;
            }
            else
            {
                var svp = PrimaryStructure.TotalSegmentVolume(set);
                var svf = CropFromStructure.TotalSegmentVolume(set).Not().MarginGreaterThan50mm(CropDistance);
                return svp.Sub(svf);
            }
        }

        /// <summary>
        /// Crop portion of Primary Structure extending inside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingInside(this StructureSet set, ICollection<Structure> PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        {
            if (CropDistance < 0) throw new ArgumentException("Crop distance must be positive");

            if (PrimaryStructure.Any(a => a.IsHighResolution) || CropFromStructure.Any(a => a.IsHighResolution))
            {
                var temp1 = PrimaryStructure.ConvertAllToHighRes(set);
                var temp2 = CropFromStructure.ConvertAllToHighRes(set);
                var svp = temp1.TotalSegmentVolume(set);
                var svf = temp2.TotalSegmentVolume(set).MarginGreaterThan50mm(CropDistance);

                SegmentVolume sv = svp.Sub(svf);

                set.RemoveStructuresFromStructureSet(temp1);
                set.RemoveStructuresFromStructureSet(temp2);
                return sv;
            }
            else
            {
                var svp = PrimaryStructure.TotalSegmentVolume(set);
                var svf = CropFromStructure.TotalSegmentVolume(set).MarginGreaterThan50mm(CropDistance);
                return svp.Sub(svf);
            }
        }

        /// <summary>
        /// Converts all structures to High Resolution within the Collection
        /// </summary>
        /// <param name="Structures"></param>
        /// <param name="BaseStructureSet">Structure set is needed to add new structures</param>
        /// <returns>Returns a new collection of structures that have been converted to high resolution.</returns>
        public static ICollection<Structure> ConvertAllToHighRes(this ICollection<Structure> Structures, StructureSet BaseStructureSet)
        {
            List<Structure> hd_structures = new List<Structure>();
            for (int i = 0; i < Structures.Count; i++)
            {
                Structure temp = BaseStructureSet.AddStructure("CONTROL", Structures.ElementAt(i).Id.UniqueStructureId(BaseStructureSet));
                temp.SegmentVolume = Structures.ElementAt(i).SegmentVolume;
                if (temp.CanConvertToHighResolution())
                {
                    temp.ConvertToHighResolution();
                }
                hd_structures.Add(temp);
            }

            return hd_structures;
        }

        /// <summary>
        /// Removes all structures in the supplied collection from the structure set
        /// </summary>
        /// <param name="set">Extended structure set</param>
        /// <param name="Structures">The collection of structures to remove from the extended structure set</param>
        public static void RemoveStructuresFromStructureSet(this StructureSet set, ICollection<Structure> Structures)
        {
            for (int i = Structures.Count - 1; i >= 0; i--)
            {
                set.RemoveStructure(Structures.ElementAt(i));
            }
        }

        /// <summary>
        /// Generates a SegmentVolume that is the Non-Overlapping portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base structure set</param>
        /// <param name="StructureOne">The first collection of structures that non-overlapping portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that non-overlapping portions should be kept</param>
        /// <returns>The non-overlapped portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume NonOverlapStructure(this StructureSet set, ICollection<Structure> StructureOne, ICollection<Structure> StructureTwo)
        {
            if (StructureOne.Any(a => a.IsHighResolution) || StructureTwo.Any(a => a.IsHighResolution))
            {
                var temp1 = StructureOne.ConvertAllToHighRes(set);
                var temp2 = StructureTwo.ConvertAllToHighRes(set);

                var sv1 = temp1.TotalSegmentVolume(set);
                var sv2 = temp2.TotalSegmentVolume(set);

                SegmentVolume sv = sv1.Xor(sv2);

                set.RemoveStructuresFromStructureSet(temp1);
                set.RemoveStructuresFromStructureSet(temp2);
                return sv;
            }
            else
            {
                var sv1 = StructureOne.TotalSegmentVolume(set);
                var sv2 = StructureTwo.TotalSegmentVolume(set);

                SegmentVolume sv = sv1.Xor(sv2);
                return sv;
            }
        }

        /// <summary>
        /// Generates a SegmentVolume that is the intersected portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">The first collection of structures that intersected portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that v portions should be kept</param>
        /// <returns>The intersected portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume IntersectionOfStructures(this StructureSet set, ICollection<Structure> StructureOne, ICollection<Structure> StructureTwo)
        {
            if (StructureOne.Any(a => a.IsHighResolution) || StructureTwo.Any(a => a.IsHighResolution))
            {
                var temp1 = StructureOne.ConvertAllToHighRes(set);
                var temp2 = StructureTwo.ConvertAllToHighRes(set);

                var sv1 = temp1.TotalSegmentVolume(set);
                var sv2 = temp2.TotalSegmentVolume(set);

                SegmentVolume sv = sv1.And(sv2);

                set.RemoveStructuresFromStructureSet(temp1);
                set.RemoveStructuresFromStructureSet(temp2);
                return sv;
            }
            else
            {
                var sv1 = StructureOne.TotalSegmentVolume(set);
                var sv2 = StructureTwo.TotalSegmentVolume(set);

                SegmentVolume sv = sv1.And(sv2);
                return sv;
            }
        }

        /// <summary>
        /// Returns a new segment volume that is subtraction of the first structure collection by the second structure collection. Protected against high res or approved structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">Primary, extended structure collection that is to be subtracted from by the second structure collection</param>
        /// <param name="StructureTwo">A collection of structures that will be added together and then subtracted from the first collection</param>
        /// <returns>A segment volume that is the subtraction of the first structure collection by the second structure collection</returns>
        public static SegmentVolume SubStructures(this StructureSet set, ICollection<Structure> StructureOne, ICollection<Structure> StructureTwo)
        {
            if (StructureOne.Any(a => a.IsHighResolution) || StructureTwo.Any(a => a.IsHighResolution))
            {
                var temp1 = StructureOne.ConvertAllToHighRes(set);
                var temp2 = StructureTwo.ConvertAllToHighRes(set);

                var sv1 = temp1.TotalSegmentVolume(set);
                var sv2 = temp2.TotalSegmentVolume(set);

                SegmentVolume sv = sv1.Sub(sv2);

                set.RemoveStructuresFromStructureSet(temp1);
                set.RemoveStructuresFromStructureSet(temp2);
                return sv;
            }
            else
            {
                var sv1 = StructureOne.TotalSegmentVolume(set);
                var sv2 = StructureTwo.TotalSegmentVolume(set);

                SegmentVolume sv = sv1.Sub(sv2);
                return sv;
            }
        }

        /// <summary>
        /// Returns a new segment volume that is contained by both submitted structure collections
        /// </summary>
        /// <param name="set">The base extended structure set</param>
        /// <param name="StructureOne">Primary, first structure collection that is to be unioned before union with additional structure</param>
        /// <param name="StructureTwo">A collection of structures that will be added together to create union with extended structure</param>
        /// <returns>A segment volume that is the union of both the extended structure collection and the parameter structure collection</returns>
        public static SegmentVolume UnionStructures(this StructureSet set, ICollection<Structure> StructureOne, ICollection<Structure> StructureTwo)
        {
            SegmentVolume sv1 = StructureOne.TotalSegmentVolume(set);
            var tempstruct = StructureOne.First();
            tempstruct.SegmentVolume = sv1;

            SegmentVolume sv2 = StructureTwo.TotalSegmentVolume(set);
            var return_structure = StructureTwo.First();
            return_structure.SegmentVolume = sv2;

            return TotalSegmentVolume(new List<Structure>() { return_structure, tempstruct });

        }

        /// <summary>
        /// Generates a ring <c>SegmentVolume</c> from a collection of base structures that have been added together. Protected against approved and high resolution structures.
        /// </summary>
        /// <param name="structure">The base structure collection</param>
        /// <param name="ss">The structure set to use during calculation</param>
        /// <param name="StartDistance_mm">The distance the ring should start from the base structure in milimeters</param>
        /// <param name="EndDistance_mm">The distance the ring should end from the base structure in milimeters</param>
        /// <param name="HighResFlag">Sets whether the resulting segment volume should be a high resolution segment or not.</param>
        /// <returns>Returns a <c>SegmentVolume</c> that is the ring created at the specified distances from the base structure.</returns>
        public static SegmentVolume GenerateRing(this StructureSet ss, ICollection<Structure> structure, double StartDistance_mm, double EndDistance_mm, bool HighResFlag = false)
        {
            SegmentVolume sv = structure.TotalSegmentVolume(ss, HighResFlag);

            Structure exp1 = ss.AddStructure("CONTROL", "_Exp1");
            if (HighResFlag || structure.Any(a => a.IsHighResolution)) exp1.ConvertToHighResolution();
            Structure exp2 = ss.AddStructure("CONTROL", "_Exp2");
            if (HighResFlag || structure.Any(a => a.IsHighResolution)) exp2.ConvertToHighResolution();

            exp1.SegmentVolume = sv.MarginGreaterThan50mm(StartDistance_mm);
            exp2.SegmentVolume = sv.MarginGreaterThan50mm(EndDistance_mm);

            sv = exp2.Sub(exp1.SegmentVolume);

            ss.RemoveStructure(exp1);
            ss.RemoveStructure(exp2);

            return sv;
        }

        /// <summary>
        /// Crop portion of Primary Structure extending outside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingOutside(this StructureSet set, Structure PrimaryStructure, Structure CropFromStructure, int CropDistance)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(PrimaryStructure);

            List<Structure> s2 = new List<Structure>();
            s2.Add(CropFromStructure);

            return set.CropExtendingOutside(s1, s2, CropDistance);
        }

        /// <summary>
        /// Crop portion of Primary Structure extending outside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingOutside(this StructureSet set, Structure PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(PrimaryStructure);

            return set.CropExtendingOutside(s1, CropFromStructure, CropDistance);
        }

        /// <summary>
        /// Crop portion of Primary Structure extending outside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingOutside(this StructureSet set, ICollection<Structure> PrimaryStructure, Structure CropFromStructure, int CropDistance)
        {
            List<Structure> s2 = new List<Structure>();
            s2.Add(CropFromStructure);

            return set.CropExtendingOutside(PrimaryStructure, s2, CropDistance);
        }

        /// <summary>
        /// Crop portion of Primary Structure extending inside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingInside(this StructureSet set, Structure PrimaryStructure, Structure CropFromStructure, int CropDistance)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(PrimaryStructure);

            List<Structure> s2 = new List<Structure>();
            s2.Add(CropFromStructure);


            return set.CropExtendingInside(s1, s2, CropDistance);
        }

        /// <summary>
        /// Crop portion of Primary Structure extending inside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingInside(this StructureSet set, Structure PrimaryStructure, ICollection<Structure> CropFromStructure, int CropDistance)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(PrimaryStructure);


            return set.CropExtendingInside(s1, CropFromStructure, CropDistance);
        }

        /// <summary>
        /// Crop portion of Primary Structure extending inside of the CropFromStructure by the given CropDistance. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">The base structure set to call this method on.</param>
        /// <param name="PrimaryStructure">The primary structure that will be cropped from the second parameter</param>
        /// <param name="CropFromStructure">The structure the Primary structure will be cropped from</param>
        /// <param name="CropDistance">The distance in millimeters that the Primary will be cropped from the second structure</param>
        /// <returns>A SegmentVolume is returned that contains the newly formed cropped structure.</returns>
        public static SegmentVolume CropExtendingInside(this StructureSet set, ICollection<Structure> PrimaryStructure, Structure CropFromStructure, int CropDistance)
        {
            List<Structure> s2 = new List<Structure>();
            s2.Add(CropFromStructure);


            return set.CropExtendingInside(PrimaryStructure, s2, CropDistance);
        }

        /// <summary>
        /// Generates a ring <c>SegmentVolume</c> from a collection of base structures that have been added together. Protected against approved and high resolution structures.
        /// </summary>
        /// <param name="structure">The base structure collection</param>
        /// <param name="ss">The structure set to use during calculation</param>
        /// <param name="StartDistance_mm">The distance the ring should start from the base structure in milimeters</param>
        /// <param name="EndDistance_mm">The distance the ring should end from the base structure in milimeters</param>
        /// <param name="HighResFlag">Sets whether the resulting segment volume should be a high resolution segment or not.</param>
        /// <returns>Returns a <c>SegmentVolume</c> that is the ring created at the specified distances from the base structure.</returns>
        public static SegmentVolume GenerateRing(this StructureSet ss, Structure structure, double StartDistance_mm, double EndDistance_mm, bool HighResFlag = false)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(structure);

            return ss.GenerateRing(s1, StartDistance_mm, EndDistance_mm, HighResFlag);
        }

        /// <summary>
        /// Generates a SegmentVolume that is the Non-Overlapping portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base structure set</param>
        /// <param name="StructureOne">The first collection of structures that non-overlapping portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that non-overlapping portions should be kept</param>
        /// <returns>The non-overlapped portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume NonOverlapStructure(this StructureSet set, Structure StructureOne, Structure StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.NonOverlapStructure(s1, s2);
        }

        /// <summary>
        /// Generates a SegmentVolume that is the Non-Overlapping portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base structure set</param>
        /// <param name="StructureOne">The first collection of structures that non-overlapping portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that non-overlapping portions should be kept</param>
        /// <returns>The non-overlapped portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume NonOverlapStructure(this StructureSet set, Structure StructureOne, ICollection<Structure> StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            return set.NonOverlapStructure(s1, StructureTwo);
        }

        /// <summary>
        /// Generates a SegmentVolume that is the Non-Overlapping portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base structure set</param>
        /// <param name="StructureOne">The first collection of structures that non-overlapping portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that non-overlapping portions should be kept</param>
        /// <returns>The non-overlapped portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume NonOverlapStructure(this StructureSet set, ICollection<Structure> StructureOne, Structure StructureTwo)
        {
            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.NonOverlapStructure(StructureOne, s2);
        }

        /// <summary>
        /// Generates a SegmentVolume that is the intersected portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">The first collection of structures that intersected portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that v portions should be kept</param>
        /// <returns>The intersected portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume IntersectionOfStructures(this StructureSet set, Structure StructureOne, Structure StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.IntersectionOfStructures(s1, s2);
        }

        /// <summary>
        /// Generates a SegmentVolume that is the intersected portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">The first collection of structures that intersected portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that v portions should be kept</param>
        /// <returns>The intersected portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume IntersectionOfStructures(this StructureSet set, Structure StructureOne, ICollection<Structure> StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            return set.IntersectionOfStructures(s1, StructureTwo);
        }

        /// <summary>
        /// Generates a SegmentVolume that is the intersected portion of both lists of the structures provided. Protects against approved and high resolution structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">The first collection of structures that intersected portions should be kept</param>
        /// <param name="StructureTwo">The second collection of structure that v portions should be kept</param>
        /// <returns>The intersected portion of the two structure lists will be returned as a new segment volume.</returns>
        public static SegmentVolume IntersectionOfStructures(this StructureSet set, ICollection<Structure> StructureOne, Structure StructureTwo)
        {
            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.IntersectionOfStructures(StructureOne, s2);
        }

        /// <summary>
        /// Returns a new segment volume that is subtraction of the first structure collection by the second structure collection. Protected against high res or approved structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">Primary, extended structure collection that is to be subtracted from by the second structure collection</param>
        /// <param name="StructureTwo">A collection of structures that will be added together and then subtracted from the first collection</param>
        /// <returns>A segment volume that is the subtraction of the first structure collection by the second structure collection</returns>
        public static SegmentVolume SubStructures(this StructureSet set, Structure StructureOne, Structure StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.SubStructures(s1, s2);
        }

        /// <summary>
        /// Returns a new segment volume that is subtraction of the first structure collection by the second structure collection. Protected against high res or approved structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">Primary, extended structure collection that is to be subtracted from by the second structure collection</param>
        /// <param name="StructureTwo">A collection of structures that will be added together and then subtracted from the first collection</param>
        /// <returns>A segment volume that is the subtraction of the first structure collection by the second structure collection</returns>
        public static SegmentVolume SubStructures(this StructureSet set, Structure StructureOne, ICollection<Structure> StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            return set.SubStructures(s1, StructureTwo);
        }

        /// <summary>
        /// Returns a new segment volume that is subtraction of the first structure collection by the second structure collection. Protected against high res or approved structures.
        /// </summary>
        /// <param name="set">Base extended structure set</param>
        /// <param name="StructureOne">Primary, extended structure collection that is to be subtracted from by the second structure collection</param>
        /// <param name="StructureTwo">A collection of structures that will be added together and then subtracted from the first collection</param>
        /// <returns>A segment volume that is the subtraction of the first structure collection by the second structure collection</returns>
        public static SegmentVolume SubStructures(this StructureSet set, ICollection<Structure> StructureOne, Structure StructureTwo)
        {
            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.SubStructures(StructureOne, s2);
        }

        /// <summary>
        /// Returns a new segment volume that is contained by both submitted structure collections
        /// </summary>
        /// <param name="set">The base extended structure set</param>
        /// <param name="StructureOne">Primary, first structure collection that is to be unioned before union with additional structure</param>
        /// <param name="StructureTwo">A collection of structures that will be added together to create union with extended structure</param>
        /// <returns>A segment volume that is the union of both the extended structure collection and the parameter structure collection</returns>
        public static SegmentVolume UnionStructures(this StructureSet set, Structure StructureOne, Structure StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.UnionStructures(s1, s2);
        }

        /// <summary>
        /// Returns a new segment volume that is contained by both submitted structure collections
        /// </summary>
        /// <param name="set">The base extended structure set</param>
        /// <param name="StructureOne">Primary, first structure collection that is to be unioned before union with additional structure</param>
        /// <param name="StructureTwo">A collection of structures that will be added together to create union with extended structure</param>
        /// <returns>A segment volume that is the union of both the extended structure collection and the parameter structure collection</returns>
        public static SegmentVolume UnionStructures(this StructureSet set, Structure StructureOne, ICollection<Structure> StructureTwo)
        {
            List<Structure> s1 = new List<Structure>();
            s1.Add(StructureOne);

            return set.UnionStructures(s1, StructureTwo);
        }

        /// <summary>
        /// Returns a new segment volume that is contained by both submitted structure collections
        /// </summary>
        /// <param name="set">The base extended structure set</param>
        /// <param name="StructureOne">Primary, first structure collection that is to be unioned before union with additional structure</param>
        /// <param name="StructureTwo">A collection of structures that will be added together to create union with extended structure</param>
        /// <returns>A segment volume that is the union of both the extended structure collection and the parameter structure collection</returns>
        public static SegmentVolume UnionStructures(this StructureSet set, ICollection<Structure> StructureOne, Structure StructureTwo)
        {
            List<Structure> s2 = new List<Structure>();
            s2.Add(StructureTwo);

            return set.UnionStructures(StructureOne, s2);
        }

    }
}
