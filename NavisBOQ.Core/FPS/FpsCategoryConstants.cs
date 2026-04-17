using System;

namespace NavisBOQ.Core.FPS
{
    public static class FpsCategoryConstants
    {
        public static bool IsPipeLike(string category)
        {
            return Eq(category, "Pipes")
                || Eq(category, "Tuberías");
        }

        public static bool IsFlexPipeLike(string category)
        {
            return Eq(category, "Flex Pipes")
                || Eq(category, "Tuberías flexibles");
        }

        public static bool IsPipeFittingLike(string category)
        {
            return Eq(category, "Pipe Fittings")
                || Eq(category, "Uniones de tubería");
        }

        public static bool IsPipeAccessoryLike(string category)
        {
            return Eq(category, "Pipe Accessories")
                || Eq(category, "Accesorios de tuberías");
        }

        public static bool IsSprinklerLike(string category)
        {
            return Eq(category, "Sprinklers")
                || Eq(category, "Rociadores");
        }

        public static bool IsGenericLike(string category)
        {
            return Eq(category, "Generic Models")
                || Eq(category, "Modelos genéricos");
        }

        public static bool IsPlumbingFixtureLike(string category)
        {
            return Eq(category, "Plumbing Fixtures")
                || Eq(category, "Aparatos sanitarios");
        }

        public static bool IsPlumbingEquipmentLike(string category)
        {
            return Eq(category, "Plumbing Equipment")
                || Eq(category, "Equipos sanitarios");
        }

        public static bool IsPieceLike(string category)
        {
            return IsPipeFittingLike(category)
                || IsPipeAccessoryLike(category)
                || IsSprinklerLike(category)
                || IsGenericLike(category)
                || IsPlumbingFixtureLike(category)
                || IsPlumbingEquipmentLike(category);
        }

        private static bool Eq(string a, string b)
        {
            return string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}