using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Electrical;
using NavisBOQ.Core.Models;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class ExpandElectricalDetailToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "expand_electrical_detail"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            var options = ParseOptions(request != null ? request.PayloadJson : "");

            var parameterReader = new RevitParameterReaderService();
            var snapshotService = new RevitSnapshotService(parameterReader);
            var scopeService = new RevitSelectionScopeService();
            var extractionService = new RevitSnapshotExtractionService(scopeService, snapshotService);

            var classifier = new ElectricalCategoryClassifierService();
            var mapper = new ElectricalQuantityMapperService();

            var snapshots = extractionService.ExtractSnapshots(uiApp, options);

            var rows = new List<ElectricalRunRow>();

            foreach (var snap in snapshots)
            {
                string boqCategory;
                string unit;

                if (!classifier.TryClassify(snap.Category, out boqCategory, out unit))
                    continue;

                var row = mapper.Map(snap, boqCategory, unit);
                if (row != null)
                    rows.Add(row);
            }

            if (!string.IsNullOrWhiteSpace(options.FilterCategory))
            {
                rows = rows
                    .Where(x => string.Equals(x.CategoriaBoq, options.FilterCategory, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(options.FilterType))
            {
                rows = rows
                    .Where(x => string.Equals(x.Tipo, options.FilterType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var result = new
            {
                total = rows.Count,
                detalle = rows.Take(1000).ToList()
            };

            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Detalle expandido correctamente.",
                DataJson = JsonConvert.SerializeObject(result)
            };
        }

        private RunOptions ParseOptions(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new RunOptions();

            try
            {
                return JsonConvert.DeserializeObject<RunOptions>(json) ?? new RunOptions();
            }
            catch
            {
                return new RunOptions();
            }
        }
    }
}