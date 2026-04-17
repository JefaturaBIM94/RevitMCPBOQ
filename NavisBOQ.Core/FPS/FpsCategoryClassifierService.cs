namespace NavisBOQ.Core.FPS
{
    public class FpsCategoryClassifierService
    {
        public bool TryClassify(string revitCategory, out string boqCategory, out string unit)
        {
            boqCategory = "";
            unit = "pza";

            if (FpsCategoryConstants.IsPipeLike(revitCategory))
            {
                boqCategory = "Tuberías FPS";
                unit = "ml";
                return true;
            }

            if (FpsCategoryConstants.IsFlexPipeLike(revitCategory))
            {
                boqCategory = "Tuberías flexibles FPS";
                unit = "ml";
                return true;
            }

            if (FpsCategoryConstants.IsPipeFittingLike(revitCategory))
            {
                boqCategory = "Uniones de tubería FPS";
                unit = "pza";
                return true;
            }

            if (FpsCategoryConstants.IsPipeAccessoryLike(revitCategory))
            {
                boqCategory = "Accesorios de tubería FPS";
                unit = "pza";
                return true;
            }

            if (FpsCategoryConstants.IsSprinklerLike(revitCategory))
            {
                boqCategory = "Rociadores";
                unit = "pza";
                return true;
            }

            if (FpsCategoryConstants.IsGenericLike(revitCategory))
            {
                boqCategory = "Modelos genéricos FPS";
                unit = "pza";
                return true;
            }

            if (FpsCategoryConstants.IsPlumbingFixtureLike(revitCategory))
            {
                boqCategory = "Aparatos sanitarios FPS";
                unit = "pza";
                return true;
            }

            if (FpsCategoryConstants.IsPlumbingEquipmentLike(revitCategory))
            {
                boqCategory = "Equipos sanitarios FPS";
                unit = "pza";
                return true;
            }

            return false;
        }
    }
}