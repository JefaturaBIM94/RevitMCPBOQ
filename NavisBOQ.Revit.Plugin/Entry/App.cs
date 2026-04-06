using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Automation;
using NavisBOQ.Revit.Plugin.Infrastructure;

namespace NavisBOQ.Revit.Plugin.Entry
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            RevitAppContext.Initialize(application);

            if (RevitBridgeState.BridgeHandler == null)
                RevitBridgeState.BridgeHandler = new BridgeRequestHandler();

            if (RevitBridgeState.BridgeExternalEvent == null)
                RevitBridgeState.BridgeExternalEvent = ExternalEvent.Create(RevitBridgeState.BridgeHandler);

            application.Idling += BridgePoller.OnIdling;
            RevitAppContext.BridgeHookRegistered = true;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            if (RevitAppContext.BridgeHookRegistered)
            {
                application.Idling -= BridgePoller.OnIdling;
                RevitAppContext.BridgeHookRegistered = false;
            }

            RevitBridgeState.BridgeExternalEvent = null;
            RevitBridgeState.BridgeHandler = null;
            RevitBridgeState.IsProcessing = false;
            RevitBridgeState.LastRequestFingerprint = "";

            return Result.Succeeded;
        }
    }
}