using Shared_Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Controls.Primitives;

namespace Simulation.Service
{
    public class SimulationService
    {
        private readonly IApiService _api;
        private readonly object _vitalsLock = new();

        public Case? CurrentCase { get; private set; }
        public ObservableCollection<ActionLog> EventLog { get; } = new();
        public ObservableCollection<VitalSnapshot> VitalsHistory { get; } = new();

        private CancellationTokenSource _backgroundCts;

        public SimulationService(IApiService api)
        {
            _api = api;
        }

        public async Task Initialize()
        {
            CurrentCase = await _api.GetActiveCase();

            if (CurrentCase?.CurrentVitals != null)
            {
                RecordVitalsSnapshot(CurrentCase.CurrentVitals, DateTime.Now);
            }
        }

        public async Task RegisterMedicationAdministration(Medication med, string route, string dose)
        {
            var intervention = new Intervention
            {
                Type = $"Med:{med.Name}",
                Timestamp = DateTime.Now
            };

            var log = new ActionLog
            {
                Timestamp = intervention.Timestamp,
                Description = $"Administered {med.Name} {dose} {route}"
            };
            EventLog.Add(log);
            await _api.SendAction(log);

            await ApplyIntervention(intervention);
        }
        public async Task ApplyIntervention(Intervention intervention)
        {
            _backgroundCts?.Cancel();

            if (CurrentCase == null)
                return;

            if(intervention.Type.StartsWith("Med:", StringComparison.OrdinalIgnoreCase))
            {
                var med = intervention.Type.Substring(4);
                if (string.Equals(med, "Labetalol", StringComparison.OrdinalIgnoreCase))
                {
                    await ApplyTimedEffect(
                        deltaPerStep: new VitalDelta { SystolicBP = -5, HeartRate = -2 },
                        steps: 5,
                        stepDelayMs: 400
                        );
                }
                else
                {
                    ApplyImediateDelta(new VitalDelta { SystolicBP = -10 });
                }
            }
            else if (intervention.Type.StartsWith("Fluid:", StringComparison.OrdinalIgnoreCase))
            {
                var parts = intervention.Type.Split(":");
                if (parts.Length == 2 && double.TryParse(parts[1], out var ml))
                {
                    var multiplier = Math.Min(1.0, ml / 1000.0);
                    ApplyImediateDelta(new VitalDelta { SystolicBP = (int)(10 * multiplier), DiastolicBP = (int)(5 * multiplier) });

                }
            }
            else if (intervention.Type.StartsWith("Oxygen:", StringComparison.OrdinalIgnoreCase))
            {
                await ApplyTimedEffect(new VitalDelta { SpO2 = 1 }, steps: 5, stepDelayMs: 300);
            }
            else
            {
                ApplyImediateDelta(new VitalDelta { });
            }
            if (CurrentCase.CurrentVitals != null)
            {
                RecordVitalsSnapshot(CurrentCase.CurrentVitals, DateTime.Now);
            }
            await Task.CompletedTask;
        }

        private void ApplyImediateDelta(VitalDelta delta)
        {
            lock (_vitalsLock)
            {
                var v = CurrentCase!.CurrentVitals!;
                v.SystolicBP += delta.SystolicBP;
                v.DiastolicBP += delta.DiastolicBP;
                v.HeartRate += delta.HeartRate;
                v.RespiratoryRate += delta.RespiratoryRate;
                v.SpO2 += delta.SpO2;
                v.Temperature += delta.Temperature;

                ClampVitals(v);
            }
        }

        private async Task ApplyTimedEffect(VitalDelta deltaPerStep, int steps, int stepDelayMs)
        {
            _backgroundCts = new CancellationTokenSource();
            var ct = _backgroundCts.Token;

            try
            {
                for (int i = 0; i < steps; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    lock (_vitalsLock)
                    {
                        var v = CurrentCase!.CurrentVitals!;
                        v.SystolicBP += deltaPerStep.SystolicBP;
                        v.DiastolicBP += deltaPerStep.DiastolicBP;
                        v.HeartRate += deltaPerStep.HeartRate;
                        v.RespiratoryRate += deltaPerStep.RespiratoryRate;
                        v.SpO2 += deltaPerStep.SpO2;
                        v.Temperature += deltaPerStep.Temperature;

                        ClampVitals(v);
                    }

                    // record snapshot at each step for history/curve
                    RecordVitalsSnapshot(CloneVitals(CurrentCase!.CurrentVitals!), DateTime.Now);

                    await Task.Delay(stepDelayMs, ct);
                }
            }
            catch (OperationCanceledException)
            { }
        }
        private void ClampVitals(VitalSigns v)
        {
            v.SystolicBP = Math.Max(40, v.SystolicBP);
            v.DiastolicBP = Math.Max(20, v.DiastolicBP);
            v.SpO2 = Math.Min(100, Math.Max(50, v.SpO2));
            v.HeartRate = Math.Max(20, v.HeartRate);
        }

        private void RecordVitalsSnapshot(VitalSigns v, DateTime timestamp)
        {
            var copy = CloneVitals(v);
            VitalsHistory.Add(new VitalSnapshot { Timestamp = timestamp, Vitals = copy });
        }

        private VitalSigns CloneVitals(VitalSigns v)
        {
            return new VitalSigns
            {
                SystolicBP = v.SystolicBP,
                DiastolicBP = v.DiastolicBP,
                HeartRate = v.HeartRate,
                RespiratoryRate = v.RespiratoryRate,
                SpO2 = v.SpO2,
                Temperature = v.Temperature
            };
        }

        // small helper types
        private class VitalDelta
        {
            public int SystolicBP { get; set; }
            public int DiastolicBP { get; set; }
            public int HeartRate { get; set; }
            public int RespiratoryRate { get; set; }
            public int SpO2 { get; set; }
            public double Temperature { get; set; }
        }
    }

    // Snapshot type used for UI binding / history
    public class VitalSnapshot
    {
        public DateTime Timestamp { get; set; }
        public VitalSigns Vitals { get; set; } = new VitalSigns();
    }
}
