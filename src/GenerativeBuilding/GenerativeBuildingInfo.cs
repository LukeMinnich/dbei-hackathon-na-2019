using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace GenerativeBuilding
{
    public class GenerativeBuildingInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "GenerativeBuilding";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("02986737-db2b-469c-9592-a22fb2c3f1d1");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
