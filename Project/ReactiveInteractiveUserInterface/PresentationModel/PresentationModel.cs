//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelImplementation : ModelAbstractApi {
        private bool Disposed = false;
        private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable;
        private readonly UnderneathLayerAPI layerBellow;

        public event EventHandler<BallChaneEventArgs>? BallChanged;

        internal ModelImplementation() : this(null) { }

        internal ModelImplementation(UnderneathLayerAPI underneathLayer) {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;

            eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(h => BallChanged += h, h => BallChanged -= h);

        }

        public override void Dispose() {
            if (Disposed) throw new ObjectDisposedException(nameof(ModelImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer) {
            EventHandler<BallChaneEventArgs> handler = (sender, args) => {
                observer.OnNext(args.Ball);
            };

            BallChanged += handler;

            return new Unsubscriber(() => BallChanged -= handler);
        }

        public override void Start(int numberOfBalls) {
            layerBellow.Start(numberOfBalls, StartHandler);
        }

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball) {
            ModelBall newBall = new ModelBall(position.x, position.y, ball);

            BallChanged?.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
        }

        private class Unsubscriber : IDisposable {
            private readonly Action _unsubscribe;
            public Unsubscriber(Action unsubscribe) => _unsubscribe = unsubscribe;
            public void Dispose() => _unsubscribe?.Invoke();
        }

        #region TestingInstrumentation

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed) {
            returnInstanceDisposed(Disposed);
        }

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnLayer) {
            returnLayer(layerBellow);
        }

        [Conditional("DEBUG")]
        internal void CheckBallChangedEvent(Action<bool> returnEventNull) {
            returnEventNull(BallChanged == null);
        }

        #endregion TestingInstrumentation
    }


    public class BallChaneEventArgs : EventArgs {
        public IBall Ball { get; set; }
    }

    
}