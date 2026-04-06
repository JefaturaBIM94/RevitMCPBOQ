using NavisBOQ.Core.Constants;

namespace NavisBOQ.Core.HVAC
{
    public class HvacCategoryClassifierService
    {
        public bool TryClassify(string category, out string boqCategory, out string unit)
        {
            boqCategory = "";
            unit = "pza";

            if (string.IsNullOrWhiteSpace(category))
                return false;

            if (HvacCategoryConstants.Map.TryGetValue(category.Trim(), out var hit))
            {
                boqCategory = hit.BoqCategory;
                unit = hit.Unit;
                return true;
            }

            return false;
        }
    }
}
