using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.Automation
{
    public class BridgeRequestHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            string logPath = Path.Combine(BridgePaths.Root, "bridge-auto-log.txt");

            try
            {
                Directory.CreateDirectory(BridgePaths.Root);
                Log(logPath, "---- ExternalEvent Execute iniciado ----");

                if (app == null)
                {
                    WriteErrorResponse("UIApplication es null.");
                    Log(logPath, "UIApplication es null.");
                    return;
                }

                if (app.ActiveUIDocument == null || app.ActiveUIDocument.Document == null)
                {
                    WriteErrorResponse("No hay documento activo en Revit.");
                    Log(logPath, "No hay documento activo.");
                    return;
                }

                if (!File.Exists(BridgePaths.RequestFile))
                {
                    Log(logPath, "No existe request.json");
                    return;
                }

                string requestJson = File.ReadAllText(BridgePaths.RequestFile, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(requestJson))
                {
                    WriteErrorResponse("request.json está vacío.");
                    Log(logPath, "request.json vacío.");
                    return;
                }

                string fingerprint = ComputeSha1(requestJson);
                RevitBridgeState.LastRequestFingerprint = fingerprint;
                RevitBridgeState.IsProcessing = true;

                Log(logPath, "request.json leído.");
                Log(logPath, "fingerprint = " + fingerprint);

                JObject node = JObject.Parse(requestJson);

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
                var response = dispatcher.Dispatch(app, request);

                string output = response != null
                    ? JsonConvert.SerializeObject(new
                    {
                        ok = response.Ok,
                        message = response.Message,
                        data = response.DataJson,
                        error = response.Error
                    })
                    : "{\"ok\":false,\"error\":\"Null response from dispatcher.\"}";

                File.WriteAllText(BridgePaths.ResponseFile, output, Encoding.UTF8);

                if (File.Exists(BridgePaths.RequestFile))
                    File.Delete(BridgePaths.RequestFile);

                Log(logPath, "response.json escrita.");
                Log(logPath, "request.json eliminada.");
                Log(logPath, "response = " + output);
                Log(logPath, "---- ExternalEvent Execute terminado OK ----");
            }
            catch (Exception ex)
            {
                try
                {
                    WriteErrorResponse(ex.Message);
                    Log(logPath, "EXCEPTION: " + ex);
                }
                catch { }
            }
            finally
            {
                RevitBridgeState.IsProcessing = false;
            }
        }

        public string GetName()
        {
            return "NavisBOQ Revit Bridge Request Handler";
        }

        private static void WriteErrorResponse(string errorMessage)
        {
            Directory.CreateDirectory(BridgePaths.Root);

            string output = JsonConvert.SerializeObject(new
            {
                ok = false,
                error = errorMessage
            });

            File.WriteAllText(BridgePaths.ResponseFile, output, Encoding.UTF8);
        }

        private static string ComputeSha1(string text)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text ?? "");
                byte[] hash = sha1.ComputeHash(bytes);
                var sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
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