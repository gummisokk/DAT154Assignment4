using Shared_Library.Models;
using Simulation.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Simulation.ViewModels
{
    public class SimulationViewModel : INotifyPropertyChanged
    {
        private readonly SimulationService _simulation;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<ActionLog> EventLog => _simulation.EventLog;
        public ObservableCollection<VitalSnapshot> VitalsHistory => _simulation.VitalsHistory;

        public VitalSigns? Vitals => _simulation.CurrentCase?.CurrentVitals;

        public ICommand GiveLabetalolCommand { get; }
        public ICommand AdministerFluidCommand { get; }

        public SimulationViewModel()
        {
            var api = new FakeApiService();
            _simulation = new SimulationService(api);

            GiveLabetalolCommand = new RelayCommand(async _ => await GiveLabetalol());
            AdministerFluidCommand = new RelayCommand(async _ => await AdministerFluid(500));
        }

        public async Task Initialize()
        {
            await _simulation.Initialize();
            OnPropertyChanged(nameof(Vitals));
        }

        public async Task GiveLabetalol()
        {
            // create a Medication instance or call method that encodes med in intervention
            var med = new Medication { Name = "Labetalol", Dose = "20 mg", Route = "IV" };
            await _simulation.RegisterMedicationAdministration(med, med.Route, med.Dose);
            OnPropertyChanged(nameof(Vitals));
        }

        public async Task AdministerFluid(double ml)
        {
            await _simulation.RegisterFluidAdministration(ml);
            OnPropertyChanged(nameof(Vitals));
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // minimal RelayCommand for WPF bindings
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public async void Execute(object? parameter) => await _execute(parameter);

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}