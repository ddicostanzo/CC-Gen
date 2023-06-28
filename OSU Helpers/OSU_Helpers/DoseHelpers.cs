using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using static OSU_Helpers.Enumerables;

namespace OSU_Helpers
{
    /// <summary>
    /// <c>DoseHelpers</c> provides methods to export dose planes and profiles. 
    /// </summary>
    public static class DoseHelpers
    {
        /// <summary>
        /// Locates the planes in X, Y, and Z that a point is located within an image
        /// </summary>
        /// <param name="vvector">Point for planar identification</param>
        /// <param name="dose">Image that is used to identify the planes</param>
        /// <returns>VVector with plane number of point in X, Y, and Z</returns>
        public static VVector VVectorPlaneLocation(VVector vvector, Dose dose)
        {
            VVector plane = new VVector();

            plane.x = VVector_Plane(dose.Origin.x, dose.XSize, dose.XRes, vvector.x);
            plane.y = VVector_Plane(dose.Origin.y, dose.YSize, dose.YRes, vvector.y);
            plane.z = VVector_Plane(dose.Origin.z, dose.ZSize, dose.ZRes, vvector.z);

            return plane;
        }

        /// <summary>
        /// Method for determining the plane that a specific point is located on
        /// </summary>
        /// <param name="origin">Origin of the of the dose object</param>
        /// <param name="size">Size of the image in X, Y, or Z</param>
        /// <param name="res">X, Y, or Z resolution of the image</param>
        /// <param name="point">Point location in X, Y, or Z</param>
        /// <returns>Returns the plane number</returns>
        private static int VVector_Plane(double origin, double size, double res, double point)
        {
            for (int i = 0; i < size; i++)
            {
                double plane_location = (origin + (i * res));
                if ((point >= (plane_location - (res / 2))) && (point <= ((res / 2) + plane_location)))
                {
                    return i;
                }
            }
            throw new ApplicationException("No plane contains the point.");
        }

        /// <summary>
        /// Gets the voxel values of the specified plane. If dose is needed, it will convert to dose values and if an image is provided, it will convert to HU or intensity.
        /// </summary>
        /// <param name="dose">Image or Dose requesting the voxel values from.</param>
        /// <param name="plane_number">The plane number requesting the voxels from.</param>
        /// <param name="plane">The plane that the voxels should come from</param>
        /// <returns></returns>
        public static double[,] GetVoxelsOnPlane(Dose dose, int plane_number, PlaneDirection plane)
        {
            double[,] plane_array = GetSizeOfPlaneDoubleArray(plane, dose);
            plane_array = GetDoseVoxelValues(dose, plane_number, plane);

            return plane_array;
        }

        /// <summary>
        /// Determines the array size for a Dose based upon the plane needed.
        /// </summary>
        /// <param name="plane">PlaneDirection is required to identify which plane is needed.</param>
        /// <param name="dose">Dose requesting plane information for</param>
        /// <returns>Returns an array of doubles with the specified size based upon the plan</returns>
        private static double[,] GetSizeOfPlaneDoubleArray(PlaneDirection plane, Dose dose)
        {
            switch (plane)
            {
                case PlaneDirection.Axial:
                    return new double[dose.XSize, dose.YSize];
                case PlaneDirection.Coronal:
                    return new double[dose.XSize, dose.ZSize];
                case PlaneDirection.Sagittal:
                    return new double[dose.YSize, dose.ZSize];
                default:
                    throw new Exception("Incorrect or no plane given to method GetPlaneDoubleArray.");
            }
        }

        private static double[,] GetDoseVoxelValues(Dose dose, int plane_number, PlaneDirection plane)
        {
            double[,] array = GetSizeOfPlaneDoubleArray(plane, dose);
            if (dose.GetType().Equals(typeof(Dose)) || dose.GetType().Equals(typeof(PlanningItemDose)) || dose.GetType().Equals(typeof(BeamDose)))
            {
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        array[i, j] = GetDoseValueOnPlane(plane_number, dose, i, j, plane).Dose;
                    }
                }
            }
           
            return array;
        }

        /// <summary>
        /// Returns a dose value from specific point on a plane for a given i,j coordinate
        /// </summary>
        /// <param name="plane_number">The plane to return the dose value from</param>
        /// <param name="dose">The dose requesting the point from</param>
        /// <param name="i">The column of the voxel of the desired value</param>
        /// <param name="j">The row of the voxel of the desired value</param>
        /// <param name="plane">The requested plane direction for the desired voxel</param>
        /// <returns>A DoseValue is returned for the requested point in 3D space</returns>
        public static DoseValue GetDoseValueOnPlane(int plane_number, Dose dose, int i, int j, PlaneDirection plane)
        {
            VVector point = new VVector();
            switch (plane)
            {
                case PlaneDirection.Axial:
                    point.x = i * dose.XRes + dose.Origin.x;
                    point.y = j * dose.YRes + dose.Origin.y;
                    point.z = plane_number * dose.ZRes + dose.Origin.z;
                    break;
                case PlaneDirection.Coronal:
                    point.x = i * dose.XRes + dose.Origin.x;
                    point.y = plane_number * dose.YRes + dose.Origin.y;
                    point.z = j * dose.ZRes + dose.Origin.z;
                    break;
                case PlaneDirection.Sagittal:
                    point.x = plane_number * dose.XRes + dose.Origin.x;
                    point.y = i * dose.YRes + dose.Origin.y;
                    point.z = j * dose.ZRes + dose.Origin.z;
                    break;
            }

            DoseValue dv = dose.GetDoseToPoint(point);
            return dv;
        }

        /// <summary>
        /// Generates a dose profile using the supplied point, plane, and dose.
        /// </summary>
        /// <param name="point">VVector point that is on the plane.</param>
        /// <param name="plane">PlaneDirection of the requested dose plane </param>
        /// <param name="dose">Dose to pull the dose plane from</param>
        /// <param name="int_profile">Used for iterating over the entire dose volume in a plane, otherwise a single profile will be returned.</param>
        /// <returns>Returns a dose profile</returns>
        public static DoseProfile GetDoseProfileInOrthogonalDirection(VVector point, PlaneDirection plane, Dose dose, int int_profile = 0)
        {
            double dosex = dose.XSize * dose.XRes;
            double dosey = dose.YSize * dose.YRes;
            double dosez = dose.ZSize * dose.ZRes;
            VVector start;
            VVector stop;
            double[] buffer;

            switch (plane)
            {
                case PlaneDirection.Axial:
                    start = new VVector(dose.Origin.x, dose.Origin.y + int_profile, point.z);
                    stop = new VVector(dose.Origin.x + dosex, dose.Origin.y + int_profile, point.z);
                    break;
                case PlaneDirection.Coronal:
                    start = new VVector(dose.Origin.x, point.y, dose.Origin.z + dosez - int_profile);
                    //start.x = (dose.GetDoseToPoint(start).Dose == double.NaN) ? start.x + 1 : start.x;
                    stop = new VVector((dose.Origin.x + dosex), point.y, dose.Origin.z + dosez - int_profile);
                    //stop.x = (dose.GetDoseToPoint(stop).Dose == double.NaN) ? stop.x + 1 : stop.x;
                    break;
                case PlaneDirection.Sagittal:
                    start = new VVector(point.x, dose.Origin.y + dosey, dose.Origin.z + dosez - int_profile);
                    stop = new VVector(point.x, dose.Origin.y, dose.Origin.z + dosez - int_profile);
                    break;
                default:
                    throw new Exception("An invalid plane direction was sent to GetDoseProfile.");
            }


            buffer = new double[(int)Math.Round((start - stop).Length, 0)];
            return dose.GetDoseProfile(start, stop, buffer);

        }

        /// <summary>
        /// Generates a full dose plane as List of DoseProfiles for a given center point, plane direction, and dose.
        /// </summary>
        /// <param name="dose">The input dose to be used for the plane generation.</param>
        /// <param name="plane">PlaneDirection of the plane being acquired.</param>
        /// <param name="point_on_plane">VVector that is one the plane needed.</param>
        /// <returns>List of dose profiles for the 3D dose provided in the plane requested</returns>
        public static List<DoseProfile> DosePlaneAsList(Dose dose, PlaneDirection plane, VVector point_on_plane)
        {
            int loop_count;

            switch (plane)
            {
                case PlaneDirection.Axial:
                    loop_count = (int)Math.Round(dose.YSize * dose.YRes, 0);
                    break;
                case PlaneDirection.Coronal:
                    loop_count = (int)Math.Round(dose.ZSize * dose.ZRes, 0);
                    break;
                case PlaneDirection.Sagittal:
                    loop_count = (int)Math.Round(dose.XSize * dose.XRes, 0);
                    break;
                default:
                    throw new Exception("An invalid plane direction was sent to GetDoseProfile.");
            }
            //loop_count += 20;
            List<DoseProfile> profiles = new List<DoseProfile>();

            for (int i = 0; i < loop_count; i++)
            {
                DoseProfile dp = GetDoseProfileInOrthogonalDirection(point_on_plane, plane, dose, i);
                if (dp == null)
                {
                    continue;
                }
                profiles.Add(dp);
            }

            return profiles;

        }

        /// <summary>
        /// Generates a full dose plane as array of doubles for a given center point, plane direction, and dose.
        /// </summary>
        /// <param name="dose">The input dose to be used for the plane generation.</param>
        /// <param name="plane">PlaneDirection of the plane being acquired.</param>
        /// <param name="point_on_plane">VVector that is one the plane needed.</param>
        /// <returns>2D array of dose profiles for the 3D dose provided in the plane requested</returns>
        public static double[,] DoseProfileAsArray(Dose dose, PlaneDirection plane, VVector point_on_plane)
        {
            return ConvertListDoseProfilesToArray(DosePlaneAsList(dose, plane, point_on_plane));
        }

        /// <summary>
        /// Generates a double[,] array from a list of dose profiles
        /// </summary>
        /// <param name="profiles">List of DoseProfiles</param>
        /// <returns>double[,] array of all dose profiles</returns>
        private static double[,] ConvertListDoseProfilesToArray(List<DoseProfile> profiles)
        {
            profiles = profiles.Where(a => !a.All(b => double.IsNaN(b.Value))).Select(a => a).ToList();

            int rows = profiles.Count;
            int cols = profiles.Select(a => a.Count).Max();
            //rows = (profiles.Count - (profiles.Count % 10));
            double[,] dose_plane = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < (cols - (cols % 10)); j++)
                {
                    if (j >= profiles[i].Count)
                    {
                        dose_plane[i, j] = 0;
                    }
                    else
                    {
                        dose_plane[i, j] = profiles[i][j].Value;
                    }
                }
            }

            return dose_plane;
        }

        /// <summary>
        /// Returns a byte array for writing to a DICOM file for the specified plane
        /// </summary>
        /// <param name="plane">The specified double array containing the planar information for conversion</param>
        /// <returns>Returns a byte array for inclusion in the DICOM file.</returns>
        public static byte[] ConvertPlaneToBytes(double[,] plane)
        {
            byte[] plane_bytes = new byte[plane.GetLength(0) * plane.GetLength(1) * 4];

            int dimensions = plane.Rank;
            int[] dim_length = new int[dimensions];
            for (int d = 0; d < dimensions; d++)
            {
                dim_length[d] = plane.GetLength(d);
            }
            int index = 0;
            for (int i = 0; i < dim_length[0]; i++)
            {
                for (int j = 0; j < dim_length[1]; j++)
                {
                    uint val = (uint)(plane[i, j] * 1.0e4);
                    byte[] voxel = BitConverter.GetBytes(val);
                    plane_bytes[index] = voxel[0];
                    plane_bytes[index + 1] = voxel[1];
                    plane_bytes[index + 2] = voxel[2];
                    plane_bytes[index + 3] = voxel[3];
                    index += 4;
                }
            }

            return plane_bytes;
        }
    }
}
