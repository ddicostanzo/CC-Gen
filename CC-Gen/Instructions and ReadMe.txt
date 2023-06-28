OSU Structure Generator From Text Files v1.0 - Help File and Syntax

The basic premise of the OSU Structure Generator From Text is to quickly generate multiple structures from a template that is provided by the user. The Structure Set that is in Context, i.e. visible in the views within External Beam Planning, can have structures added to it or structures that exist can be overwritten with the script.

Note: All distances are in millimeters unless otherwise noted. In addition, operators and structure IDs are not case sensitive. All are converted to UPPER CASE prior to trying to match. However, the supplied Structure ID for the new structure will generate with the case as supplied.

The general format for a structure is as follows:
StructureID,DICOM Type = BaseStructure.Operation1(argument1,argument2,...).Operation2(argument1...)

The first supplied value from the user is the Structure ID for a newly created structure. It must conform to the Eclipse naming conventions: 16 characters max length, no trailing or leading spaces, etc. If the supplied ID is longer than 16 characters, it will be truncated to 16.

The second provided value from the user is the DICOM Type of the structure. This value is required for new structures, but is not if the user is replacing a structure geometry. Valid DICOM Types are: GTV, CTV, PTV, AVOIDANCE, CONTROL, MARKER, or ORGAN.

An equal sign must exist between the structure identification and the operations to take place.

The first value on the right side of the equal sign must be a structure that exists in the StructureSet and has a geometry. It will be used as the basis for all operations. 

A period (.) must separate the base structure from the first operation, and each operation from its predecessor. 

Following a period any operator is a valid selection and following a operator must exist an opening and closing parenthesis.

Valid operators, the arguments allowed, and their actions are:

Ring(Start Distance (mm), End Distance (mm)[, true]) => Will generate a ring from the base structure beginning at the Start Distance from the base structure, and ending at the End Distance. Optionally, you can provide a third parameter that states "true" which will convert the Ring to a high resolution structure.

Or(Structure1,Structure2, etc) =>  This operator will return all portions of the supplied structures and the Base Structure. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

And(Structure1,Structure2, etc) => This operator will return the intersecting portions of the supplied structures with the Base Structure. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

Sub(Structure1,Structure2, etc) => This operator will return the subtraction of the BaseStructure by the union of all supplied structures. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

Not(Structure1,Structure2, etc) => This operator will return the non-overlapping portion of the Base Structure with the union of all supplied structures. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

CropOut(StructureToCropFrom, CropDistance (mm)) => This operator will return the BaseStructure that has the portion extending outside of the StructureToCropFrom and do so by an additional margin supplied in the CropDistance. The CropDistance must by greater than or equal to 0.

CropIn(StructureToCropFrom, CropDistance (mm)) => This operator will return the BaseStructure that has the portion extending inside of the StructureToCropFrom and do so by an additional margin supplied in the CropDistance. The CropDistance must by greater than or equal to 0.

Margin(MarginSize (mm)) => This operator will return the BaseStructure with a margin added, equal to the MarginSize. The MarginSize must by greater than or equal to 0.

PTVALL() => This operator will return the union of all structures whose IDs' start with PTV. A Base Structure, which exists in the structure set is required but will not be used.

HighRes() => This operator will return the Base Structure converted to a high resolution segment (if needed). It can be applied as the final operator in a list or on its own.

Finally, in order to overwrite an already existing structure in the structure set, the user must supply an ! or ~ in front of the Structure Id and not supply a DICOM Type. An example as shown below:
~PTVHD = PTV.Margin(3)