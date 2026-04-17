using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.FPS
{
    public class FpsSystemResolverService
    {
        public FpsSystemInfo Resolve(ElementSnapshot snap)
        {
            return new FpsSystemInfo
            {
                Classification = !string.IsNullOrWhiteSpace(snap.SystemClassification)
                    ? snap.SystemClassification.Trim()
                    : "Sin sistema FPS",

                Name = !string.IsNullOrWhiteSpace(snap.SystemName)
                    ? snap.SystemName.Trim()
                    : "Sin sistema FPS",

                Type = !string.IsNullOrWhiteSpace(snap.SystemType)
                    ? snap.SystemType.Trim()
                    : ""
            };
        }
    }
}