using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.HVAC
{
    public class HvacSystemResolverService
    {
        public HvacSystemInfo Resolve(ElementSnapshot snap)
        {
            var classification = string.IsNullOrWhiteSpace(snap.SystemClassification)
                ? "Sin sistema HVAC"
                : snap.SystemClassification.Trim();

            var name = string.IsNullOrWhiteSpace(snap.SystemName)
                ? "Sin sistema HVAC"
                : snap.SystemName.Trim();

            var type = string.IsNullOrWhiteSpace(snap.SystemType)
                ? ""
                : snap.SystemType.Trim();

            return new HvacSystemInfo
            {
                Classification = classification,
                Name = name,
                Type = type
            };
        }
    }
}
