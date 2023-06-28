using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OSU_Helpers
{
    /// <summary>
    /// The ESAPI <c>Structure</c> extension class to help with manipulation and calculations
    /// </summary>
    public class StructureExt
    {
        /// <summary>
        /// The parent ESAPI <c>Structure</c> useful for navigating
        /// </summary>
        public Structure _parent_structure { get; set; }
        /// <summary>
        /// The identifier of the Structure. 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The DICOM type of the structure, for example, PTV, MARKER, or ORGAN. 
        /// </summary>
        public string DicomType { get; set; }
        /// <summary>
        /// The color of the structure. 
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// e center point of the structure.
        /// </summary>
        public VVector CenterPoint { get; set; }
        /// <summary>
        /// true if this structure is a high-resolution structure. Otherwise false. 
        /// </summary>
        public bool IsHighResolution { get; set; }
        /// <summary>
        /// The mesh geometry. 
        /// </summary>
        public System.Windows.Media.Media3D.MeshGeometry3D MeshGeometry { get; set; }
        /// <summary>
        /// The DICOM ROI Number of the structure. 
        /// </summary>
        public int ROINumber { get; set; }
        /// <summary>
        /// Provides access to the segment volume of the structure. 
        /// </summary>
        public SegmentVolume SegmentVolume { get; set; }
        /// <summary>
        /// A collection of structure codes attached to this structure. 
        /// </summary>
        public IEnumerable<StructureCodeInfo> StructureCodeInfos { get; set; }
        /// <summary>
        /// The calculated volume. 
        /// </summary>
        public double Volume { get; set; }
        /// <summary>
        /// Checks if the structure has a segment. 
        /// </summary>
        public bool HasSegment { get; set; }
        /// <summary>
        /// Checks if the structure is empty. 
        /// </summary>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Constructor for the <c>StructureExt</c> class.
        /// </summary>
        /// <param name="s">Requires an ESAPI <c>Structure</c> as input</param>
        public StructureExt(Structure s)
        {
            _parent_structure = s;
            Id = s.Id;
            DicomType = s.DicomType;
            Color = s.Color;
            CenterPoint = s.CenterPoint;
            IsHighResolution = s.IsHighResolution;
            MeshGeometry = s.MeshGeometry;
            ROINumber = s.ROINumber;
            SegmentVolume = s.SegmentVolume;
            StructureCodeInfos = s.StructureCodeInfos;
            Volume = s.Volume;
            HasSegment = s.HasSegment;
            IsEmpty = s.IsEmpty;
        }

    }
}
