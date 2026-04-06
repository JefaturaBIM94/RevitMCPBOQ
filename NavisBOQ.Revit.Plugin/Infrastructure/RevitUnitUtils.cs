using Autodesk.Revit.DB;

namespace NavisBOQ.Revit.Plugin.Infrastructure
{
    public static class RevitUnitUtils
    {
        public static double ToMeters(double internalValue)
        {
            return UnitUtils.ConvertFromInternalUnits(internalValue, UnitTypeId.Meters);
        }

        public static double ToSquareMeters(double internalValue)
        {
            return UnitUtils.ConvertFromInternalUnits(internalValue, UnitTypeId.SquareMeters);
        }

        public static double ToCubicMeters(double internalValue)
        {
            return UnitUtils.ConvertFromInternalUnits(internalValue, UnitTypeId.CubicMeters);
        }

        public static double ToDegrees(double radians)
        {
            return radians * (180.0 / System.Math.PI);
        }
    }
}
