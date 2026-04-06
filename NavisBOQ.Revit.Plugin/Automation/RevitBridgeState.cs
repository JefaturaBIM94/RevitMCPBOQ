using Autodesk.Revit.UI;

namespace NavisBOQ.Revit.Plugin.Automation
{
    public static class RevitBridgeState
    {
        public static ExternalEvent BridgeExternalEvent { get; set; }
        public static BridgeRequestHandler BridgeHandler { get; set; }

        public static bool IsProcessing { get; set; }
        public static string LastRequestFingerprint { get; set; } = "";
    }
}