namespace Kataclysm.StructuralAnalysis.Rigid
{
    public abstract class GeneralLightFramedWall : GeneralWall
    {
        #region Public Methods
        /// <summary>
        /// Returns the actual framing depth of the wall
        /// </summary>
        /// <returns></returns>
        public double WallFramingDepth()
        {
            //This method takes in WallThickness and returns the assumed wall framing depth
            if (WallThickness < 5.5)
            {
                return 3.5; //If the wall thickness is less than 5.5 inches, the framing depth must be 3.5 inches (4x nominal) 
            }
            else if (WallThickness < 7.25)
            {
                return 5.5; //If the wall thickness is less than 7.25 inches, the framing depth must be 5.5 inches (6x nominal) 
            }
            else if (WallThickness < 9.25)
            {
                return 7.25; //If the wall thickness is less than 9.25 inches, the framing depth must be 7.25 inches (8x nominal) 
            }
            else
            {
                return 0; //This will be a catch...if the wall thickness is not one defined above, it probably is not right
            }
        }
        #endregion
    }
}
