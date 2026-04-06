using System.Text.Json.Nodes;
using NavisBOQ.Revit.McpServer.Transport;

namespace NavisBOQ.Revit.McpServer.Mcp
{
    public class McpServer
    {
        public string Handle(string inputJson)
        {
            JsonNode requestNode;

            try
            {
                requestNode = JsonNode.Parse(inputJson);
            }
            catch
            {
                return BuildError(null, -32700, "Invalid JSON");
            }

            if (requestNode == null || requestNode["method"] == null)
                return BuildError(null, -32600, "Invalid request");

            string method = requestNode["method"] != null
                ? requestNode["method"]!.GetValue<string>()
                : "";

            JsonNode id = requestNode["id"];
            JsonObject @params = requestNode["params"] as JsonObject;

            // IMPORTANTE:
            // Las notifications JSON-RPC / MCP no se responden.
            if (id == null && method.StartsWith("notifications/"))
                return "";

            switch (method)
            {
                case "initialize":
                    return HandleInitialize(id);

                case "tools/list":
                    return HandleToolsList(id);

                case "tools/call":
                    return HandleToolsCall(id, @params);

                default:
                    return BuildError(id, -32601, "Method not found: " + method);
            }
        }

        private string HandleInitialize(JsonNode id)
        {
            var result = new JsonObject
            {
                ["protocolVersion"] = "2024-11-05",
                ["capabilities"] = new JsonObject
                {
                    ["tools"] = new JsonObject()
                },
                ["serverInfo"] = new JsonObject
                {
                    ["name"] = "NavisBOQ.Revit.McpServer",
                    ["version"] = "0.1.0"
                }
            };

            return BuildResult(id, result);
        }

        private string HandleToolsList(JsonNode id)
        {
            var tools = new JsonArray
            {
                new JsonObject
                {
                    ["name"] = "ping",
                    ["description"] = "Valida que el plugin/servidor Revit esté operativo.",
                    ["inputSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject()
                    }
                },
                new JsonObject
                {
                    ["name"] = "list_available_tools",
                    ["description"] = "Lista las tools disponibles del plugin Revit.",
                    ["inputSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject()
                    }
                },
                new JsonObject
                {
                    ["name"] = "active_document_info",
                    ["description"] = "Devuelve información básica del documento activo de Revit.",
                    ["inputSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject()
                    }
                },
                new JsonObject
                {
                    ["name"] = "diagnose_selection",
                    ["description"] = "Devuelve diagnóstico de la selección actual en Revit.",
                    ["inputSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject()
                    }
                },
                new JsonObject
                {
                    ["name"] = "run_preconstruccion_4",
                    ["description"] = "Ejecuta la Corrida 4 eléctrica en Revit.",
                    ["inputSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["scope_mode"] = new JsonObject { ["type"] = "string" },
                            ["level"] = new JsonObject { ["type"] = "string" },
                            ["output_mode"] = new JsonObject { ["type"] = "string" },
                            ["max_items"] = new JsonObject { ["type"] = "integer" },
                            ["max_nodes"] = new JsonObject { ["type"] = "integer" },
                            ["strict_limits"] = new JsonObject { ["type"] = "boolean" }
                        }
                    }
                },
                new JsonObject
                {
                    ["name"] = "expand_electrical_detail",
                    ["description"] = "Expande el detalle eléctrico desde Revit con filtros opcionales.",
                    ["inputSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["scope_mode"] = new JsonObject { ["type"] = "string" },
                            ["level"] = new JsonObject { ["type"] = "string" },
                            ["filterCategory"] = new JsonObject { ["type"] = "string" },
                            ["filterType"] = new JsonObject { ["type"] = "string" },
                            ["output_mode"] = new JsonObject { ["type"] = "string" }
                        }
                    }
                },
                new JsonObject
                {
                    ["name"] = "run_preconstruccion_5",
                    ["description"] = "Ejecuta la Corrida 5 HVAC en Revit.",
                    ["inputSchema"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["scope_mode"] = new JsonObject { ["type"] = "string" },
                            ["selection_set"] = new JsonObject { ["type"] = "string" },
                            ["level"] = new JsonObject { ["type"] = "string" },
                            ["filterCategory"] = new JsonObject { ["type"] = "string" },
                            ["filterType"] = new JsonObject { ["type"] = "string" },
                            ["output_mode"] = new JsonObject { ["type"] = "string" },
                            ["max_items"] = new JsonObject { ["type"] = "integer" },
                            ["max_nodes"] = new JsonObject { ["type"] = "integer" },
                            ["strict_limits"] = new JsonObject { ["type"] = "boolean" }
                        }
                    }
                }
            };

            var result = new JsonObject
            {
                ["tools"] = tools
            };

            return BuildResult(id, result);
        }

        private string HandleToolsCall(JsonNode id, JsonObject @params)
        {
            if (@params == null)
                return BuildError(id, -32602, "Missing params");

            string toolName = @params["name"] != null
                ? @params["name"]!.GetValue<string>()
                : "";

            JsonObject arguments = @params["arguments"] as JsonObject ?? new JsonObject();

            var bridge = new PluginBridgeClient();

            // Timeout ampliado para corridas HVAC y pruebas con modelos más pesados
            JsonObject pluginResponse = bridge.Call(toolName, arguments, 60000);

            var result = new JsonObject
            {
                ["content"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["type"] = "text",
                        ["text"] = pluginResponse.ToJsonString()
                    }
                }
            };

            return BuildResult(id, result);
        }

        private string BuildResult(JsonNode id, JsonNode result)
        {
            var payload = new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id != null ? id.DeepClone() : null,
                ["result"] = result
            };

            return payload.ToJsonString();
        }

        private string BuildError(JsonNode id, int code, string message)
        {
            var payload = new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id != null ? id.DeepClone() : null,
                ["error"] = new JsonObject
                {
                    ["code"] = code,
                    ["message"] = message
                }
            };

            return payload.ToJsonString();
        }
    }
}