using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Policies
{
    public class ExecutionModePolicyService
    {
        public ExecutionModeDecision EvaluateForRun(string runName, RunOptions options, ScopePreflight preflight)
        {
            var result = new ExecutionModeDecision();

            string tool = (runName ?? "").Trim().ToLowerInvariant();
            string scope = (options != null ? options.ScopeMode : "all");
            scope = (scope ?? "all").Trim().ToLowerInvariant();
            bool strict = options != null ? options.StrictLimits : true;

            if (scope == "selection")
            {
                result.Mode = "auto_safe";
                result.AllowAutoRun = true;
                result.ForceSummary = false;
                result.Reason = "Selección manual: alcance controlado.";
                return result;
            }

            string riskBand = preflight != null ? preflight.RiskBand : "green";
            riskBand = (riskBand ?? "green").Trim().ToLowerInvariant();

            int candidates = preflight != null ? preflight.CandidateItems : 0;

            if (tool == "run_preconstruccion_4")
            {
                if (riskBand == "red")
                {
                    result.Mode = strict ? "manual_required" : "auto_summary_only";
                    result.AllowAutoRun = !strict;
                    result.ForceSummary = true;
                    result.Reason = strict
                        ? "Corrida 4: el alcance eléctrico excede el umbral seguro."
                        : "Corrida 4: alcance rojo; se permite solo resumen por StrictLimits=false.";
                    result.Warnings.Add("El alcance eléctrico puede incluir demasiados elementos o mezcla disciplinaria.");
                    result.SuggestedActions.Add("Segmenta por nivel.");
                    result.SuggestedActions.Add("Usa selección actual más acotada.");
                    return result;
                }

                if (riskBand == "yellow" || scope == "selection_set" || scope == "all" || scope == "level")
                {
                    result.Mode = "auto_summary_only";
                    result.AllowAutoRun = true;
                    result.ForceSummary = true;
                    result.Reason = "Corrida 4 permitida en automático, pero limitada a resumen por seguridad.";
                    result.Warnings.Add("Si necesitas detalle completo, reduce el alcance.");
                    return result;
                }

                result.Mode = "auto_safe";
                result.AllowAutoRun = true;
                result.ForceSummary = false;
                result.Reason = "Corrida 4 segura para ejecución completa.";
                return result;
            }

            if (tool == "run_preconstruccion_3")
            {
                if (riskBand == "red" || candidates > 1500)
                {
                    result.Mode = strict ? "manual_required" : "auto_summary_only";
                    result.AllowAutoRun = !strict;
                    result.ForceSummary = true;
                    result.Reason = strict
                        ? "Corrida 3: alcance demasiado grande para lectura estable."
                        : "Corrida 3: alcance muy grande; se permite solo resumen por StrictLimits=false.";
                    result.Warnings.Add("Si necesitas detalle completo, reduce el alcance.");
                    return result;
                }

                if (riskBand == "yellow" || scope == "selection_set" || scope == "all" || scope == "level")
                {
                    result.Mode = "auto_summary_only";
                    result.AllowAutoRun = true;
                    result.ForceSummary = true;
                    result.Reason = "Corrida 3 fuera de selección manual: permitir solo resumen por seguridad.";
                    result.Warnings.Add("Si el resultado depende mucho de fallbacks, usa selección más acotada.");
                    return result;
                }

                result.Mode = "auto_safe";
                result.AllowAutoRun = true;
                result.ForceSummary = false;
                result.Reason = "Corrida 3 segura para ejecución completa.";
                return result;
            }

            if (tool == "run_preconstruccion_2")
            {
                if (riskBand == "red")
                {
                    result.Mode = strict ? "manual_required" : "auto_summary_only";
                    result.AllowAutoRun = !strict;
                    result.ForceSummary = true;
                    result.Reason = strict
                        ? "Corrida 2: el alcance excede el umbral seguro."
                        : "Corrida 2: alcance rojo; se permite solo resumen por StrictLimits=false.";
                    result.Warnings.Add("La corrida estructural puede incluir demasiados elementos.");
                    return result;
                }

                if (riskBand == "yellow" || scope == "selection_set" || scope == "all" || scope == "level")
                {
                    result.Mode = "auto_summary_only";
                    result.AllowAutoRun = true;
                    result.ForceSummary = true;
                    result.Reason = "Corrida 2 permitida en automático, pero limitada a resumen por seguridad.";
                    result.Warnings.Add("Si el resumen sale parcial, reduce el alcance.");
                    return result;
                }

                result.Mode = "auto_safe";
                result.AllowAutoRun = true;
                result.ForceSummary = false;
                result.Reason = "Corrida 2 segura para ejecución completa.";
                return result;
            }

            if (tool == "run_preconstruccion_1")
            {
                if (riskBand == "red")
                {
                    result.Mode = strict ? "manual_required" : "auto_summary_only";
                    result.AllowAutoRun = !strict;
                    result.ForceSummary = true;
                    result.Reason = strict
                        ? "Corrida 1: el alcance excede el umbral seguro."
                        : "Corrida 1: alcance rojo; se permite solo resumen por StrictLimits=false.";
                    result.Warnings.Add("Si necesitas detalle completo, segmenta más el alcance.");
                    return result;
                }

                if (riskBand == "yellow" || scope == "selection_set" || scope == "all")
                {
                    result.Mode = "auto_summary_only";
                    result.AllowAutoRun = true;
                    result.ForceSummary = true;
                    result.Reason = "Corrida 1 puede ejecutarse en automático, con salida resumida por seguridad.";
                    result.Warnings.Add("Si necesitas detalle completo, reduce el alcance.");
                    return result;
                }

                result.Mode = "auto_safe";
                result.AllowAutoRun = true;
                result.ForceSummary = false;
                result.Reason = "Sin señales de riesgo especial.";
                return result;
            }

            result.Mode = "auto_safe";
            result.AllowAutoRun = true;
            result.ForceSummary = false;
            result.Reason = "Sin señales de riesgo especial.";
            return result;
        }
    }
}