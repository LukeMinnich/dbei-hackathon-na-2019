using System;

namespace Kataclysm.Common
{
    public static class ProjectSettings
    {
        public const double GRAVITY = 386.088; //in/s^2
        public static double ORTHOGONAL_TOLERANCE = 1.0 / 128.0; 
        public static double DISTANCE_TOLERANCE = (1.0 / 128.0) / Math.Sin(Math.PI / 4.0);
        public static double WALL_LINE_TOLERANCE = 3.5;
        public static double WALL_ANGLE_TOLERANCE = 2.0 * Math.PI / 360.0; //Tolerance of 1 degree

        public static int DEFAULT_SIGNIFICANT_DIGITS = 3;

        public static string DEFAULT_SAWN_LUMBER_SPECIES = "Douglas Fir-Larch";
        public static string DEFAULT_SAWN_LUMBER_GRADE = "Select Structural";
        public static string DEFAULT_GLULAM_COMBINATION_SYMBOL = "24F-V4";
        public static string DEFAULT_GLULAM_SPECIES_COMBINATION = "DF/DF";
    }
}
