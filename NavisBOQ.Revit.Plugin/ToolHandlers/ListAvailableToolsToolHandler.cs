using System;
using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;
using Newtonsoft.Json;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class ListAvailableToolsToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "list_available_tools"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            var payload = new
            {
                servidor = "NavisBOQ Revit MCP",
                ejecutado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                tools = new object[]
                {
                    new
                    {
                        name = "ping",
                        description = "Valida que el plugin Revit y el bridge estén activos."
                    },
                    new
                    {
                        name = "list_available_tools",
                        description = "Lista las tools disponibles en el plugin Revit."
                    },
                    new
                    {
                        name = "active_document_info",
                        description = "Devuelve información básica del documento activo en Revit."
                    },
                    new
                    {
                        name = "diagnose_selection",
                        description = "Devuelve diagnóstico de la selección actual en Revit."
                    },
                    new
                    {
                        name = "run_preconstruccion_1",
                        description = "Corrida 1 Arquitectura / General."
                    },
                    new
                    {
                        name = "run_preconstruccion_2",
                        description = "Corrida 2 Estructura / Concreto."
                    },
                    new
                    {
                        name = "run_preconstruccion_3",
                        description = "Corrida 3 Estructura metálica en kg."
                    },
                    new
                    {
                        name = "run_preconstruccion_4",
                        description = "Corrida 4 Eléctrica."
                    },
                    new
                    {
                        name = "expand_electrical_detail",
                        description = "Expande el detalle eléctrico con perfiles ligeros o completos."
                    },
                    new
                    {
                        name = "run_preconstruccion_5",
                        description = "Corrida 5 HVAC."
                    },
                    new
                    {
                        name = "run_preconstruccion_6",
                        description = "Corrida 6 Acero de refuerzo / Rebar."
                    },
                    new
                    {
                        name = "run_preconstruccion_7",
                        description = "Corrida 7 Fire Protection System (FPS)."
                    }
                }
            };

            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Tools disponibles.",
                DataJson = JsonConvert.SerializeObject(payload)
            };
        }
    }
}