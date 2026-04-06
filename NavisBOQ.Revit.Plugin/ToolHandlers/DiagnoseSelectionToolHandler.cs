using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Models;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class DiagnoseSelectionToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "diagnose_selection"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            var options = new RunOptions
            {
                ScopeMode = "selection",
                OutputMode = "detail",
                StrictLimits = true
            };

            var parameterReader = new RevitParameterReaderService();
            var snapshotService = new RevitSnapshotService(parameterReader);
            var scopeService = new RevitSelectionScopeService();
            var extractionService = new RevitSnapshotExtractionService(scopeService, snapshotService);

            var snapshots = extractionService.ExtractSnapshots(uiApp, options);

            var rows = snapshots
                .Select(x => new SelectionDiagnosticRow
                {
                    ElementId = x.ElementId ?? "",
                    UniqueId = x.UniqueId ?? "",
                    Category = x.Category ?? "",
                    CategoryId = x.CategoryId ?? "",
                    Family = x.Family ?? "",
                    Type = x.Type ?? "",
                    Level = x.Level ?? "",
                    SystemName = x.SystemName ?? "",
                    SizeText = x.SizeText ?? "",
                    LengthM = x.LengthM
                })
                .ToList();

            var categorias = rows
                .GroupBy(x => x.Category ?? "")
                .Select(g => new
                {
                    categoria = g.Key,
                    cantidad = g.Count()
                })
                .OrderByDescending(x => x.cantidad)
                .ToList();

            var payload = new
            {
                total = rows.Count,
                categorias = categorias,
                elementos = rows
            };

            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Diagnóstico de selección ejecutado.",
                DataJson = JsonConvert.SerializeObject(payload)
            };
        }
    }
}