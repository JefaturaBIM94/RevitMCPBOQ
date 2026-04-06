using Autodesk.Revit.UI;

namespace NavisBOQ.Revit.Plugin.Infrastructure
{
    public static class RevitAppContext
    {
        public static bool IsInitialized { get; private set; }
        public static UIControlledApplication ControlledApplication { get; private set; }
        public static UIApplication UiApplication { get; set; }
        public static bool BridgeHookRegistered { get; set; }

        public static void Initialize(UIControlledApplication app)
        {
            ControlledApplication = app;
            IsInitialized = true;
        }
    }
}