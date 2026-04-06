using System;
using System.IO;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.Entry
{
    [Transaction(TransactionMode.Manual)]
    public class AppCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            string logPath = Path.Combine(BridgePaths.Root, "plugin-log.txt");

            try
            {
                Directory.CreateDirectory(BridgePaths.Root);
                Log(logPath, "---- Execute iniciado ----");

                if (!RevitAppContext.IsInitialized)
                {
                    Log(logPath, "RevitAppContext no inicializado.");
                    TaskDialog.Show("NavisBOQ", "RevitAppContext no está inicializado.");
                    return Result.Failed;
                }

                Log(logPath, "RevitAppContext OK");

                if (!File.Exists(BridgePaths.RequestFile))
                {
                    Log(logPath, "No existe request.json");
                    TaskDialog.Show("NavisBOQ", "No existe request.json pendiente.");
                    return Result.Succeeded;
                }

                string requestJson = File.ReadAllText(BridgePaths.RequestFile, Encoding.UTF8);
                Log(logPath, "request.json leído");

                if (string.IsNullOrWhiteSpace(requestJson))
                {
                    Log(logPath, "request.json vacío");
                    TaskDialog.Show("NavisBOQ", "request.json está vacío.");
                    return Result.Failed;
                }

                JObject node = JObject.Parse(requestJson);
                Log(logPath, "request.json parseado");

                string tool = node["tool"] != null ? node["tool"].Value<string>() : "";
                JToken paramsToken = node["params"];
                string payloadJson = paramsToken != null ? paramsToken.ToString() : "{}";

                Log(logPath, "tool = " + (tool ?? "(null)"));
                Log(logPath, "payloadJson = " + payloadJson);

                var request = new RequestEnvelope
                {
                    CommandName = tool ?? "",
                    PayloadJson = payloadJson
                };

                var dispatcher = new RevitRequestDispatcher();
                Log(logPath, "dispatcher creado");

                var response = dispatcher.Dispatch(commandData.Application, request);
                Log(logPath, "dispatcher ejecutado");

                string output = response != null
                    ? JsonConvert.SerializeObject(new
                    {
                        ok = response.Ok,
                        message = response.Message,
                        data = response.DataJson,
                        error = response.Error
                    })
                    : "{\"ok\":false,\"error\":\"Null response from dispatcher.\"}";

                Log(logPath, "response serializada");
                Log(logPath, "response = " + output);

                File.WriteAllText(BridgePaths.ResponseFile, output, Encoding.UTF8);
                Log(logPath, "response.json escrita");

                TaskDialog.Show(
                    "NavisBOQ Bridge",
                    "Request procesada:\n" + (tool ?? "") + "\n\nResponse escrita en response.json");

                Log(logPath, "TaskDialog mostrado");
                Log(logPath, "---- Execute terminado OK ----");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                try
                {
                    Log(logPath, "EXCEPTION: " + ex);
                }
                catch { }

                TaskDialog.Show("NavisBOQ ERROR", ex.ToString());
                return Result.Failed;
            }
        }

        private static void Log(string path, string message)
        {
            File.AppendAllText(
                path,
                "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message + Environment.NewLine,
                Encoding.UTF8);
        }
    }
}