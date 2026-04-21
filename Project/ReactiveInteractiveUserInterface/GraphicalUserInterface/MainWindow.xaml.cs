//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private IDisposable _observer = null;
        private ModelAbstractApi _modelLayer;
        private bool _disposed = false;
        private int _ballsCount = 5;
        private readonly SynchronizationContext _syncContext;

        public ObservableCollection<IBall> Balls { get; } = new ObservableCollection<IBall>();

        public MainWindowViewModel() : this(null) { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _modelLayer = modelLayerAPI ?? ModelAbstractApi.CreateModel();

            _observer = _modelLayer.Subscribe<IBall>(ball =>
            {
                _syncContext.Post(_ => Balls.Add(ball), null);
            });

            StartCommand = new RelayCommand(ExecuteStart);
        }

        public ICommand StartCommand { get; }

        public int BallsCount
        {
            get => _ballsCount;
            set { _ballsCount = value; RaisePropertyChanged(); }
        }

        private void ExecuteStart()
        {
            Balls.Clear();
            _modelLayer.Start(BallsCount);
        }

        public void Start(int numberOfBalls) => _modelLayer.Start(numberOfBalls);

        public void Dispose()
        {
            if (_disposed) return;
            _observer?.Dispose();
            _modelLayer.Dispose();
            _disposed = true;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged;
    }
}