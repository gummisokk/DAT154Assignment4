using Simulation.Service;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Simulation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    

    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e) // SANITY CHECK: This is not recommended in production code
        {
            base.OnStartup(e);

            var api = new FakeApiService();
            var sim = new SimulationService(api);

            await sim.Initialize();

            System.Diagnostics.Debug.WriteLine(sim.CurrentCase?.Patient.FullName);
        }
    }


}
