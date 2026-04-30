using Shared_Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Service
{
    public class SimulationService
    {
        private readonly IApiService _api;

        public Case? CurrentCase { get; private set; }
        public List<ActionLog> EventLog { get; private set; } = new();

        public SimulationService(IApiService api)
        {
            _api = api;
        }

        public async Task Initialize()
        {
            CurrentCase = await _api.GetActiveCase();
        }

        public async Task ApplyIntervention(Intervention intervention)
        {
            // Apply fysisk effect
            ApplyEffect(intervention);

            // Log event
            var log = new ActionLog
            {
                Timestamp = DateTime.Now,
                Description = intervention.Type
            };

            EventLog.Add(log);

            // Send to "fake" backend 
            await _api.SendAction(log);
        }

        private void ApplyEffect(Intervention intervention)
        {
            var vitals = CurrentCase.CurrentVitals;

            switch (intervention.Type)
            {
                case "Labetalol20":
                    vitals.SystolicBP -= 25;
                    vitals.HeartRate -= 10;
                    break;

                case "GTN":
                    vitals.SystolicBP -= 15;
                    break;

                case "NoAction":
                    vitals.SystolicBP += 5;
                    vitals.HeartRate += 2;
                    break;
            }

            ClampVitals(vitals);
        }

        private void ClampVitals(VitalSigns v)
        {
            v.SystolicBP = Math.Max(60, v.SystolicBP);
            v.DiastolicBP = Math.Max(40, v.DiastolicBP);
            v.SpO2 = Math.Min(100, v.SpO2);
        }
    }
}
