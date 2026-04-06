using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;

namespace NavisBOQ.Revit.McpServer.Transport
{
    public class PluginBridgeClient
    {
        public JsonObject Call(string toolName, JsonObject arguments, int timeoutMs)
        {
            Directory.CreateDirectory(BridgePaths.Root);

            if (File.Exists(BridgePaths.ResponseFile))
                File.Delete(BridgePaths.ResponseFile);

            JsonNode safeArguments = arguments != null
                ? JsonNode.Parse(arguments.ToJsonString())
                : new JsonObject();

            var request = new JsonObject
            {
                ["tool"] = toolName,
                ["params"] = safeArguments
            };

            File.WriteAllText(
                BridgePaths.RequestFile,
                request.ToJsonString(),
                Encoding.UTF8);

            int waited = 0;
            while (waited < timeoutMs)
            {
                if (File.Exists(BridgePaths.ResponseFile))
                {
                    string json = File.ReadAllText(BridgePaths.ResponseFile, Encoding.UTF8);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var node = JsonNode.Parse(json) as JsonObject;
                        if (node != null)
                            return node;
                    }
                }

                Thread.Sleep(250);
                waited += 250;
            }

            return new JsonObject
            {
                ["ok"] = false,
                ["error"] = "Timeout esperando response del plugin Revit. Ejecuta el comando del plugin para procesar la request."
            };
        }
    }
}