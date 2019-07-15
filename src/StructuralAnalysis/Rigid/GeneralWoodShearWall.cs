using System;
using System.Collections.Generic;
using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public abstract class GeneralWoodShearWall : GeneralLightFramedShearWall
    {
        #region Public Parameters
        
        public double FloorDepth { get; set; } //In inches
        public bool UseStudPacks { get; set; } = true;
        public int MaxNumberInStudPack { get; set; } = 3;
        public double StudThickness { get; set; } = 1.5;
        public double MaxMemberSize { get; set; } = 48d;
        public bool Blocked { get; set; } = true;
        public double FieldNailSpacing { get; set; } = 12.0;
        #endregion

        #region Results Parameters
        public GeneralShearWallDesignResults DesignResults { get; set; }

        public WallTopPlate TopPlate { get; set; }
        #endregion

        #region Class Protected Variables
        //CLASS PROTECTED VARIABLES
        //===================================================================
        protected ProjectWoodDesignSettings ProjectDesignSettings;
        protected ChordDesignMethod ChordDesignMethod;
        protected double MaxCompressionLenRatio;
        protected string StudSpecies;
        protected string StudGrade;
        protected GradingMethodType StudGradingMethod;
        protected GradingAgencyType StudGradingAgency;
        protected double MaxStudSpacing = 24; //In inches
        protected double MinimumStudSpacing = 12;
        protected string PostSpecies;
        protected string PostGrade;
        protected GradingMethodType PostGradingMethod;
        protected GradingAgencyType PostGradingAgency;
        protected string PlateSpecies;
        protected string PlateGrade;
        protected GradingMethodType PlateGradingMethod;
        protected GradingAgencyType PlateGradingAgency;
        protected string PanelThickness; //Can also be a result, depending on if the panel thickness was passed (optional parameter)
        protected string PanelGrade;
        protected string PanelType;
        protected bool AllowDoubleSided = true;

        protected double Temperature;
        protected bool AlignWallStudsWithJoists = true; //If true, the program creates a stud pack with the required number of studs instead of decreasing the stud spacing
        protected double Studs_SpacingIncrement = 4; //If the program is allowed to reduce the stud spacing, this is the increment that it will reduce the spacing by (in inches)
        protected List<double> Sizes = new List<double> { 1.5, 3, 3.5, 4.5, 5.5, 6, 7.25, 7.5, 9, 9.25, 10.5, 11.25 }; //These are the sizes available for use when sizing end posts

        protected List<string> AllowableGlulamCombinationSymbols = new List<string>() { "24F-V4", "24F-V8" };
        public DesignMethodType WallDesignMethod
        {
            get
            {
                return DesignMethod;
            }
        }
        //===================================================================
        #endregion

        public ProjectWoodDesignSettings WoodDesignSettings
        {
            get
            {
                return ProjectDesignSettings;
            }
            set
            {
                ProjectDesignSettings = value;

                //Now set all of the class protected variables
                ChordDesignMethod = ProjectDesignSettings.ChordDesign;
                MaxCompressionLenRatio = ProjectDesignSettings.DistributedChordLengthRatio;
                StudSpecies = ProjectDesignSettings.StudSpecies;
                StudGrade = ProjectDesignSettings.StudGrade;
                StudGradingMethod = ProjectDesignSettings.StudGradingMethod;
                StudGradingAgency = ProjectDesignSettings.StudGradingAgency;
                MinimumStudSpacing = ProjectDesignSettings.MinimumStudSpacing;
                MaxStudSpacing = ProjectDesignSettings.MaximumStudSpacing;
                PostSpecies = ProjectDesignSettings.PostSpecies;
                PostGrade = ProjectDesignSettings.PostGrade;
                PostGradingMethod = ProjectDesignSettings.PostGradingMethod;
                PostGradingAgency = ProjectDesignSettings.PostGradingAgency;
                PlateSpecies = ProjectDesignSettings.PlateSpecies;
                PlateGrade = ProjectDesignSettings.PlateGrade;
                PlateGradingMethod = ProjectDesignSettings.PlateGradingMethod;
                PlateGradingAgency = ProjectDesignSettings.PlateGradingAgency;
                PanelThickness = ProjectDesignSettings.PanelThicknessString;

                switch(ExposureLocation)
                    {
                    case ExposureLocation.Interior:
                        PanelGrade = ProjectDesignSettings.PanelGradeInterior;
                        break;
                    case ExposureLocation.Exterior:
                        PanelGrade = ProjectDesignSettings.PanelGradeExterior;
                        break;
                    };

                PanelType = ProjectDesignSettings.PanelType;
                Temperature = ProjectDesignSettings.Temperature;
                AlignWallStudsWithJoists = ProjectDesignSettings.AlignWallStudsWithJoists;
                Studs_SpacingIncrement = ProjectDesignSettings.StudSpacingIncrement;
                DesignMethod = ProjectDesignSettings.DesignMethod;
                MaxAllowableDCR = ProjectDesignSettings.MaximumDCR;
                AllowDoubleSided = ProjectDesignSettings.AllowDoubleSided;


                if (MinimumStudSpacing >= 0)
                {

                }
            }
        }

        public GeneralWoodShearWall(string wallID, string pierID, BuildingLevel level, double wallThickness, double wallLength, double wallHeight, double floorDepth, List<LinearElementLoad> gravityLoads, List<LateralLoad> lateralLoads, ProjectWoodDesignSettings woodDesignSettings, ExposureLocation exposureLocation)
        {
            WallThickness = wallThickness;
            WallLength = wallLength;
            WallHeight = wallHeight; //Level to level
            FloorDepth = floorDepth;
            GravityLoads = gravityLoads;
            CumulativeLateralLoads = lateralLoads;
            WallID = wallID;
            ExposureLocation = exposureLocation;
            switch (ExposureLocation)
            {
                case ExposureLocation.Interior:
                    PanelGrade = woodDesignSettings.PanelGradeInterior;
                    break;
                case ExposureLocation.Exterior:
                    PanelGrade = woodDesignSettings.PanelGradeExterior;
                    break;
            };
            PierID = pierID;
            WallLength = wallLength;
            TopLevel = level;
            WoodDesignSettings = woodDesignSettings;
            CalcLog = new CalculationLog(wallID);

            LoadCombos = GenerateLoadCombos();
            Sizes = GetAllowablePostSizeList();
        }

        public GeneralWoodShearWall(string wallID, string pierID, BuildingLevel level, double wallThickness, double wallLength, double wallHeight, double floorDepth, List<LinearElementLoad> gravityLoads, List<LateralLoad> lateralLoads, ProjectWoodDesignSettings woodDesignSettings) :
            this(wallID, pierID, level, wallThickness, wallLength, wallHeight, floorDepth, gravityLoads, lateralLoads, woodDesignSettings, 0)
        {
        }

        public GeneralWoodShearWall(string wallID, string pierID, BuildingLevel level, double wallThickness, double wallLength, double wallHeight, double floorDepth, List<LinearElementLoad> gravityLoads, ProjectWoodDesignSettings woodDesignSettings, ExposureLocation exposureLocation) :
            this(wallID, pierID, level, wallThickness, wallLength, wallHeight, floorDepth, gravityLoads, new List<LateralLoad>(), woodDesignSettings, exposureLocation)
        {
        }

        public GeneralWoodShearWall(string wallID, string pierID, BuildingLevel level, double wallThickness, double wallLength, double wallHeight, double floorDepth, List<LinearElementLoad> gravityLoads, ProjectWoodDesignSettings woodDesignSettings) :
            this(wallID, pierID, level, wallThickness, wallLength, wallHeight, floorDepth, gravityLoads, new List<LateralLoad>(), woodDesignSettings, 0)
        {
        }

        public GeneralWoodShearWall(string wallID, string pierID, BuildingLevel level, double wallThickness, double wallLength, double wallHeight, double floorDepth, ProjectWoodDesignSettings woodDesignSettings, ExposureLocation exposureLocation) :
    this(wallID, pierID, level, wallThickness, wallLength, wallHeight, floorDepth, new List<LinearElementLoad>(), new List<LateralLoad>(), woodDesignSettings, exposureLocation)
        {
        }

        public GeneralWoodShearWall(string wallID, string pierID, BuildingLevel level, double wallThickness, double wallLength, double wallHeight, double floorDepth, ProjectWoodDesignSettings woodDesignSettings) :
            this(wallID, pierID, level, wallThickness, wallLength, wallHeight, floorDepth, new List<LinearElementLoad>(), new List<LateralLoad>(), woodDesignSettings, 0)
        {
        }

        protected abstract void DesignShearWallEnds();

        protected abstract void DesignSheathing();

        protected abstract double CheckWallEndPostDesign(Dictionary<LoadCombination, WallChordForces> chordForcesList, Dictionary<LoadCombination, WallChordForces> incrementalChordForcesList);
        
        protected List<double> GetAllowablePostSizeList()
        {
            double depth = WallFramingDepth();

            List<double> SectionSizes = new List<double>();

            foreach (SawnLumberSection SLS in WoodResources.SawnLumberSectionList)
            {
                if (SLS.Width == depth)
                {
                    if (SLS.Depth <= MaxMemberSize)
                    {
                        SectionSizes.Add(SLS.Depth);
                    }
                }

                if (SLS.Depth == depth)
                {
                    if (SLS.Width <= MaxMemberSize)
                    {
                        if (SectionSizes.Contains(SLS.Width) == false)
                        {
                            SectionSizes.Add(SLS.Width);
                        }
                    }
                }
            }

            SectionSizes.Sort((a, b) => a.CompareTo(b));

            List<double> studPackSizes = new List<double>();

            for (int i = 1; i <= MaxNumberInStudPack; i++)
            {
                double thickness = i * StudThickness;

                studPackSizes.Add(thickness);
            }

            for (int i = studPackSizes.Count - 1; i >= 0; i--)
            {
                double t = studPackSizes[i];
                int counter = 0;

                while (counter < SectionSizes.Count)
                {
                    if (SectionSizes[counter] == t)
                    {
                        break;
                    }

                    if (t > SectionSizes[counter])
                    {
                        SectionSizes.Insert(counter + 1, t);
                        break;
                    }

                    counter++;
                }
            }


            return SectionSizes;
        }
        
        protected abstract WallChordForces ComputeChordForcesGivenWallEndDesigns(ShearWallEndPost leftEndPost, ShearWallTensionRestraint leftRestraint, ShearWallEndPost rightEndPost, ShearWallTensionRestraint rightRestraint, LoadCombination loadCombo, double factoredOverturning);

        protected SawnLumberMember GetSawnLumberMember(double d1, double d2, double WallDepth, double height)
        {
            double depth = WallDepth;
            double thickness = 0;
            int NumStuds = 1;

            if (d1 == depth)
            {
                thickness = d2;
            }
            else
            {
                thickness = d1;
            }

            double le1, le2;
            if (depth >= thickness)
            {
                le1 = height; //If the depth (or thickness of the wall) is greater than the perpendicular dimension, then the strong axis of the post is along the same dimension as the width of the wall
                le2 = 6;
            }
            else
            {
                le1 = 6;
                le2 = height;
            }

            SawnLumberMember SLM = null;
            if (thickness % StudThickness == 0)
            {
                if (thickness / StudThickness <= MaxNumberInStudPack)
                {
                    NumStuds = Convert.ToInt32(thickness / StudThickness);
                    SLM = new SawnLumberMember(SawnLumberSection.FromDimensions(depth, StudThickness), GetWoodMaterialFromList(StudSpecies, StudGrade, SawnLumberType.DimensionedLumber, StudGradingMethod, StudGradingAgency)) { NumberOfMembers = NumStuds, le1 = le1, le2 = le2 };
                }
                else
                {
                    //thickness = GetNextLargerNonBuiltUpThickness(thickness);

                    if (depth >= thickness)
                    {
                        le1 = height; //If the depth (or thickness of the wall) is greater than the perpendicular dimension, then the strong axis of the post is along the same dimension as the width of the wall
                        le2 = 6;
                    }
                    else
                    {
                        le1 = 6;
                        le2 = height;
                    }

                    NumStuds = 1;
                    SawnLumberType WoodType = SawnLumberType.DimensionedLumber;
                    string WoodGrade = StudGrade;
                    string WoodSpecies = StudSpecies;
                    GradingMethodType WoodGradingMethod = StudGradingMethod;
                    GradingAgencyType WoodGradingAgency = StudGradingAgency;

                    if (thickness > StudThickness)
                    {
                        WoodGrade = PostGrade;
                        WoodSpecies = PostSpecies;
                        WoodGradingMethod = PostGradingMethod;
                        WoodGradingAgency = PostGradingAgency;
                    }

                    if (Math.Min(depth, thickness) > 3.5)
                    {
                        WoodType = SawnLumberType.Timbers; //If the minimum dimension of the post is greater than 3.5 inches, it is classified as "Posts and Timbers"
                        WoodGrade = GetPostAndTimberGrade(PostGrade);
                        WoodSpecies = PostSpecies;
                    }

                    SLM = new SawnLumberMember(SawnLumberSection.FromDimensions(depth, thickness), GetWoodMaterialFromList(WoodSpecies, WoodGrade, WoodType, WoodGradingMethod, WoodGradingAgency)) { NumberOfMembers = NumStuds, le1 = le1, le2 = le2 };
                }
            }
            else
            {
                NumStuds = 1;
                SawnLumberType WoodType = SawnLumberType.DimensionedLumber;
                string WoodGrade = StudGrade;
                string WoodSpecies = StudSpecies;
                GradingMethodType WoodGradingMethod = StudGradingMethod;
                GradingAgencyType WoodGradingAgency = StudGradingAgency;

                if (thickness > StudThickness)
                {
                    WoodGrade = PostGrade;
                    WoodSpecies = PostSpecies;
                    WoodGradingMethod = PostGradingMethod;
                    WoodGradingAgency = PostGradingAgency;
                }

                if (Math.Min(depth, thickness) > 3.5)
                {
                    WoodType = SawnLumberType.Timbers; //If the minimum dimension of the post is greater than 3.5 inches, it is classified as "Posts and Timbers"
                    WoodGrade = GetPostAndTimberGrade(PostGrade);
                    WoodSpecies = PostSpecies;
                }
                SLM = new SawnLumberMember(SawnLumberSection.FromDimensions(depth, thickness), GetWoodMaterialFromList(WoodSpecies, WoodGrade, WoodType, WoodGradingMethod, WoodGradingAgency)) { NumberOfMembers = NumStuds, le1 = le1, le2 = le2 };
            }

            return SLM;
        }

        protected int GetNextLargerGlulamIndex(double wallDepth, int currentIndex)
        {
            int startIndex = currentIndex + 1;

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (currentIndex < WoodResources.GluedLaminatedSectionList.Count - 1)
            {
                for (int i = startIndex; i < WoodResources.GluedLaminatedSectionList.Count; i++)
                {
                    if (WoodResources.GluedLaminatedSectionList[i].Width == wallDepth)
                    {
                        if (currentIndex < 0)
                        {
                            return i;
                        }
                        else
                        {
                            if (WoodResources.GluedLaminatedSectionList[i].Depth > WoodResources.GluedLaminatedSectionList[currentIndex].Depth)
                            {
                                return i;
                            }
                        }
                    }

                    if (i == WoodResources.GluedLaminatedSectionList.Count - 1)
                    {
                        return currentIndex;
                    }
                }
            }
            else
            {
                return currentIndex;
            }

            return currentIndex;
        }

        protected WoodDesignFrameMember GetRequiredEndPostFrameMember(WoodDesignFrameMember endPostFrameMember, int startIndex, double wallDepth, double wallHeight)
        {
            if (endPostFrameMember.GetType() == typeof(SawnLumberMember))
            {
                if (startIndex <= Sizes.Count - 1)
                {
                    return GetSawnLumberMember(wallDepth, Sizes[startIndex], wallDepth, WallHeight - FloorDepth);
                }
                else
                {
                    return GetSawnLumberMember(wallDepth, Sizes[Sizes.Count - 1], wallDepth, WallHeight - FloorDepth);
                }
            }
            else if (endPostFrameMember.GetType() == typeof(GluedLaminatedMember))
            {
                if (startIndex <= WoodResources.GluedLaminatedSectionList.Count - 1)
                {
                    if (WoodResources.GluedLaminatedSectionList[startIndex].Width == wallDepth)
                    {
                        double sectionWidth = WoodResources.GluedLaminatedSectionList[startIndex].Width;

                        double sectionDepth = WoodResources.GluedLaminatedSectionList[startIndex].Depth;

                        double le1, le2;
                        if (wallDepth >= sectionDepth)
                        {
                            le1 = wallHeight; //If the depth (or thickness of the wall) is greater than the perpendicular dimension, then the strong axis of the post is along the same dimension as the width of the wall
                            le2 = 6;
                        }
                        else
                        {
                            le1 = 6;
                            le2 = wallHeight;
                        }

                        GluedLaminatedMember newMember = new GluedLaminatedMember(WoodResources.GluedLaminatedSectionList[startIndex], (GluedLaminatedMaterial)endPostFrameMember.Material, WallHeight - FloorDepth);
                        newMember.le1 = le1;
                        newMember.le2 = le2;

                        return newMember;
                    }
                    else
                    {
                        return endPostFrameMember;
                    }
                }
                else
                {
                    return endPostFrameMember;
                }
            }
            else
            {
                throw new NotImplementedException("Only sawn lumber and glulam end posts are supported at this time");
            }
        }
        
        protected bool WoodSpeciesComparer(string species1, string species2)
        {
            if (string.Compare(species1, species2, ignoreCase: true) == 0)
            {
                return true;
            }
            else if (species1.Contains("douglas fir", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (species2.Contains("douglas fir", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }




    }
}
