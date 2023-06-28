# CC-Gen Manuscript Repository

The basic premise of CC-Gen is to quickly generate multiple structures from a template that is provided by the user. The Structure Set that is in Context, i.e. visible in the views within External Beam Planning, can have structures added to it or structures that exist can be overwritten with the script.

<b>Note:</b> All distances are in millimeters unless otherwise noted. In addition, operators and structure IDs are not case sensitive. All are converted to UPPER CASE prior to trying to match. However, the supplied Structure ID for the new structure will generate with the case as supplied.

# General Format of a Text File Template
The general format for a structure is as follows:

StructureID,DICOM Type = BaseStructure.Operation1(argument1,argument2,...).Operation2(argument1...)

The first supplied value from the user is the Structure ID for a newly created structure. It must conform to the Eclipse naming conventions: 16 characters max length, no trailing or leading spaces, etc. If the supplied ID is longer than 16 characters, it will be truncated to 16.

The second provided value from the user is the DICOM Type of the structure. This value is required for new structures, but is not if the user is replacing a structure geometry. Valid DICOM Types are: GTV, CTV, PTV, AVOIDANCE, CONTROL, MARKER, or ORGAN.

An equal sign must exist between the structure identification and the operations to take place.

The first value on the right side of the equal sign must be a structure that exists in the StructureSet and has a geometry. It will be used as the basis for all operations. 

A period (.) must separate the base structure from the first operation, and each operation from its predecessor. 

Following a period any operator is a valid selection and following an operator must exist an opening and closing parenthesis.

# Operations, Arguments, and Actions

Valid operators, the arguments allowed, and their actions are:

<b>Overwriting an Existing Structure</b> => In order to overwrite an already existing structure in the structure set, the user must supply an ! or ~ in front of the Structure Id and not supply a DICOM Type. An example as shown below would add 3mm of symmetric margin to the PTV and overwrite the PTVHD structure with that geometry:
<ul>
  <li>!PTV = CTV.Margin(3)</li>
  <li>~PTV = CTV.Margin(3)</li>
</ul>

<b>Ring</b>(Start Distance (mm), End Distance (mm)[, true]) => Will generate a ring from the base structure beginning at the Start Distance from the base structure, and ending at the End Distance. Optionally, you can provide a third parameter that states "true" which will convert the Ring to a high resolution structure.

<b>Or</b>(Structure1,Structure2, etc) =>  This operator will return all portions of the supplied structures and the Base Structure. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

<b>And</b>(Structure1,Structure2, etc) => This operator will return the intersecting portions of the supplied structures with the Base Structure. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

<b>Sub</b>(Structure1,Structure2, etc) => This operator will return the subtraction of the BaseStructure by the union of all supplied structures. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

<b>Not</b>(Structure1,Structure2, etc) => This operator will return the non-overlapping portion of the Base Structure with the union of all supplied structures. Any number of structures, greater than or equal to 1, can be provided in the parameters, but must be separated by a comma, exist in the structure set already, and have segments drawn.

<b>CropOut</b>(StructureToCropFrom, CropDistance (mm)) => This operator will return the BaseStructure that has the portion extending outside of the StructureToCropFrom and do so by an additional margin supplied in the CropDistance. The CropDistance must by greater than or equal to 0.

<b>CropIn</b>(StructureToCropFrom, CropDistance (mm)) => This operator will return the BaseStructure that has the portion extending inside of the StructureToCropFrom and do so by an additional margin supplied in the CropDistance. The CropDistance must by greater than or equal to 0.

<b>Margin</b>(MarginSize (mm)) => This operator will return the BaseStructure with a margin added, equal to the MarginSize. The MarginSize must by greater than or equal to 0.

<b>AsymMargin</b>(Ant (mm), Post (mm), Left (mm), Right (mm), Sup (mm), Inf (mm), Inner or Outer) => This operator will return the BaseStructure with a margin applied asymmetrically with size equal to the parameter associated. The margin sizes in each direction must by greater than or equal to 0 and whole numbers. The final argument is to specify Inner or Outer margin application. The default (leaving blank) will apply an Outer margin. Typing the word Inner as the last (7th) parameter will apply the specified sizes as Inner margins.
<ul>
  <li>Example Inner Margin: InnerMarg, Control = Body.AsymMargin(10, 10, 10, 10, 10, 10, Inner)</li>
  <li>Example Outer Margin #1: OuterMarg1, Control = Body.AsymMargin(10, 10, 10, 10, 10, 10)</li>
  <li>Example Outer Margin #2: OuterMarg2, Control = Body.AsymMargin(10, 10, 10, 10, 10, 10, Outer)</li>
</ul>

<b>PTVALL</b>() => This operator will return the union of all structures whose IDs' start with PTV. A Base Structure, which exists in the structure set is required but will not be used.

<b>HighRes</b>() => This operator will return the Base Structure converted to a high resolution segment (if needed). It can be applied as an operator in a list or on its own.

<b>All</b>(SearchText,(Starts,In,Ends)) => This operator will search the entire structure set and union all of the structures that start with, contain, or end with the specified text. Other operations can be applied to the result of this structure.

<b>SIMTPTV</b>() => This operator will search the entire structure set for structures that begin with PTV. It will then duplicate those structures to z_PTVxx_10mm and apply a 10mm expansion to those structures. Other operations cannot be applied to the result of this structure.

<b>List</b>(SearchText,(Starts,In,Ends)) => This operator will search the entire structure set and identify all of the structures that start with, contain, or end with the specified text. After identifying the list of structures that match the search criteria, all additional operations will be performed iteratively on each of the individual structures in the list. The base structure is ignored in this use case. For instance, to generate high resolution, 3mm PTVs for each GTV contained in the structure set, the code would be:
<ul>
  <li>PTV, PTV = Body.List(GTV,Starts).HighRes().Margin(3)</li>
</ul>
<b>Note:</b> The structure ID “PTV” will be iterated over and make structures “PTV1”, “PTV2”, “PTV3”, etc. Each of those structures will have the DICOM type PTV applied. It is currently NOT possible to create structures with different DICOM types.
