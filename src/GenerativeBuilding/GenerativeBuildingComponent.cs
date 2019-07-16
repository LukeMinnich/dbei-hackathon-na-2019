using System;
using System.Collections.Generic;
using System.IO;
using BuildingLayout;
using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace GenerativeBuilding
{
    public class GenerativeBuildingComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GenerativeBuildingComponent()
          : base("GenerativeBuilding", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPathParameter("UnitDefCSV", "UnitDefCSV", "Unit Definition CSV", GH_ParamAccess.item);
            pManager.AddPathParameter("UnitMixCSV", "UnitMixCSV", "Unit Mix CSV", GH_ParamAccess.item);

            pManager.AddLineParameter("MainCorridor", "Main Corridor", "Main Corridor", GH_ParamAccess.item);
            pManager.AddLineParameter("LeftLeg", "Left Corridor", "Left Corridor", GH_ParamAccess.item);
            pManager.AddLineParameter("MiddleLeg", "Middle Corridor", "Middle Corridor", GH_ParamAccess.item);
            pManager.AddLineParameter("RightLeg", "Right Corridor", "Right Corridor", GH_ParamAccess.item);

            pManager.AddNumberParameter("Unit Depth", "Unit Depth", "Unit Depth", GH_ParamAccess.item);
            pManager.AddNumberParameter("Hall Width", "Hall Width", "Hall Width", GH_ParamAccess.item);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Units", "Units", "Units", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var defPath = "";
            var mixPath = "";
            var mainCorridor = new Line();
            var leftLeg = new Line();
            var middleLeg = new Line();
            var rightLeg = new Line();
            double unitDepth = 0;
            double hallWidth = 0;


            DA.SetData(0, defPath);
            DA.SetData(1, mixPath);
            DA.SetData(2, mainCorridor);
            DA.SetData(3, leftLeg);
            DA.SetData(4, middleLeg);
            DA.SetData(5, rightLeg);
            DA.SetData(6, unitDepth);
            DA.SetData(7, hallWidth);

            var unitList = new List<BuildingUnit>();

            var csv = File.ReadAllLines(defPath);

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(',');

                unitList.Add(new BuildingUnit(lineSplit[0], lineSplit[1], Convert.ToDouble(lineSplit[2]), Convert.ToDouble(lineSplit[3]), Convert.ToDouble(lineSplit[4]), Convert.ToDouble(lineSplit[5])));
            }


            BuildingUnitMix desiredUnitMix = new BuildingUnitMix();

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(',');

                desiredUnitMix.Add(lineSplit[0], Convert.ToDouble(lineSplit[1]));
            }

            var mainCorridorObject = new MainCorridor();

            var leftStart = new Point2D
            var leftLegObject = new Corridor(new Line2D())

            

            var units = FittingAlgorithm.CreateUnitLine(unitList, desiredUnitMix,);


        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a15d5ad7-d3ca-4d74-a0d1-7f8549cb2ba6"); }
        }
    }
}
