using Autodesk.Revit.UI;

namespace NavisBOQ.Revit.Plugin.Infrastructure
{
    public static class RevitAppContext
    {
        public static bool IsInitialized { get; private set; }
        public static UIControlledApplication ControlledApplication { get; private set; }
        public static UIApplication UiApplication { get; private set; }
        public static bool BridgeHookRegistered { get; set; }

        public static void Initialize(UIControlledApplication app)
        {
            ControlledApplication = app;
            IsInitialized = true;
        }

        public static void SetUiApplication(UIApplication uiApp)
        {
            UiApplication = uiApp;
        }

        public static bool HasUiApplication()
        {
            return UiApplication != null;
        }
    }
}