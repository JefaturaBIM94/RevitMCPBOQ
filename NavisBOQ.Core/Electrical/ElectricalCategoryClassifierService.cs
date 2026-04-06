using System.Linq;
using NavisBOQ.Core.Constants;

namespace NavisBOQ.Core.Electrical
{
    public class ElectricalCategoryClassifierService
    {
        public bool TryClassify(string revitCategory, out string boqCategory, out string unit)
        {
            boqCategory = "";
            unit = "";

            if (string.IsNullOrWhiteSpace(revitCategory))
                return false;

            string category = revitCategory.Trim().ToLowerInvariant();

            if (ElectricalCategoryConstants.StrictlyForbiddenPlumbingCategories
                .Any(x => category.Contains(x.ToLowerInvariant())))
                return false;

            foreach (var kv in ElectricalCategoryConstants.BoqMap)
            {
                foreach (var cat in kv.Value.RevitCategories)
                {
                    if (category.Contains(cat.ToLowerInvariant()))
                    {
                        boqCategory = kv.Value.CanonicalName;
                        unit = kv.Value.Unit;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}