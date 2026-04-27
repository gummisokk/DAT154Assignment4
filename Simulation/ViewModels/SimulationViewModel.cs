using Simulation.Models;
using Simulation.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.ViewModels
{
    public class SimulationViewModel
    {
        private readonly SimulationService _simulation;

        public VitalSigns? Vitals => _simulation.CurrentCase?.CurrentVitals;

        public SimulationViewModel()
        {
            var api = new FakeApiService();
            _simulation = new SimulationService(api);
        }

        public async Task Initialize()
        {
            await _simulation.Initialize();
        }

        public async Task GiveLabetalol()
        {
            await _simulation.ApplyIntervention(new Intervention
            {
                Type = "Labetalol20",
                Timestamp = DateTime.Now
            });
        }
    }
}
