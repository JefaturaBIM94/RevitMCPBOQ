using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.ToolHandlers;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public class RevitRequestDispatcher
    {
        private readonly Dictionary<string, IToolHandler> _handlers;

        public RevitRequestDispatcher()
        {
            _handlers = new Dictionary<string, IToolHandler>(StringComparer.OrdinalIgnoreCase)
{
                { "ping", new PingToolHandler() },
                { "list_available_tools", new ListAvailableToolsToolHandler() },
                { "active_document_info", new ActiveDocumentInfoToolHandler() },
                { "diagnose_selection", new DiagnoseSelectionToolHandler() },
                { "run_preconstruccion_4", new RunPreconstruccion4ToolHandler() },
                { "expand_electrical_detail", new ExpandElectricalDetailToolHandler() },
                { "run_preconstruccion_5", new RunPreconstruccion5ToolHandler() },
                {"run_preconstruccion_1", new RunPreconstruccion1ToolHandler() },
                {"run_preconstruccion_2", new RunPreconstruccion2ToolHandler() },
                {"run_preconstruccion_6", new RunPreconstruccion6ToolHandler() },
                {"run_preconstruccion_3", new RunPreconstruccion3ToolHandler() },
            };
        }

        public ResponseEnvelope Dispatch(UIApplication uiApp, RequestEnvelope request)
        {
            if (uiApp == null)
            {
                return new ResponseEnvelope
                {
                    Ok = false,
                    Error = "UIApplication es null."
                };
            }

            if (request == null)
            {
                return new ResponseEnvelope
                {
                    Ok = false,
                    Error = "RequestEnvelope es null."
                };
            }

            string command = (request.CommandName ?? "").Trim();

            IToolHandler handler;
            if (!_handlers.TryGetValue(command, out handler))
            {
                return new ResponseEnvelope
                {
                    Ok = false,
                    Error = "Comando no soportado: '" + request.CommandName + "'."
                };
            }

            return handler.Handle(uiApp, request);
        }
    }
}