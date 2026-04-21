//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelImplementation : ModelAbstractApi
    {
        private bool Disposed = false;
        private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable;
        private readonly UnderneathLayerAPI layerBellow;

        public event EventHandler<BallChaneEventArgs> BallChanged;

        internal ModelImplementation() : this(null) { }

        internal ModelImplementation(UnderneathLayerAPI underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
            eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(
                h => BallChanged += h,
                h => BallChanged -= h);
        }

        public override void Dispose()
        {
            if (Disposed) return;
            layerBellow.Dispose();
            Disposed = true;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer)
        {
            return eventObservable.Subscribe(
                x => observer.OnNext(x.EventArgs.Ball),
                ex => observer.OnError(ex),
                () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls)
        {
            layerBellow.Start(numberOfBalls, StartHandler);
        }

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
        {
            ModelBall newBall = new ModelBall(position.x, position.y, ball);

            BallChanged?.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
        }
    }

    public class BallChaneEventArgs : EventArgs
    {
        public IBall Ball { get; set; }
    }
}