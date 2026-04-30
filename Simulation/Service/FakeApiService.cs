using Shared_Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Service
{
    public class FakeApiService : IApiService
    {
        private readonly List<ActionLog> _logs = new();

        public Task<Case> GetActiveCase()
        {
            return Task.FromResult(GetCase01());
        }

        public Task SendAction(ActionLog action)
        {
            _logs.Add(action);

            //debug output
            Console.WriteLine($"{action.Timestamp}: {action.Description}");

            return Task.CompletedTask;
        }

        private Case GetCase01()
        {
            return new Case
            {
                Id = "CASE-01",
                Patient = new Patient
                {
                    FullName = "Kari Olsen",
                    Age = 58,
                    Sex = "Female"
                },
                CurrentVitals = new VitalSigns
                {
                    SystolicBP = 210,
                    DiastolicBP = 120,
                    HeartRate = 102,
                    RespiratoryRate = 18,
                    SpO2 = 97,
                    Temperature = 37.2
                },
                Medications = new List<Medication>
            {
                new Medication { Name = "Amlodipine", Dose = "5 mg", Route = "Oral" },
                new Medication { Name = "Metformin", Dose = "1000 mg", Route = "Oral" },
                new Medication { Name = "Atorvastatin", Dose = "40 mg", Route = "Oral" }
            },
                Allergies = new List<string>
            {
                "ACE Inhibitors",
                "Penicillin"
            }
            };
        }
    }
}
