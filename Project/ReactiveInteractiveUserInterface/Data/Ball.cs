//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________


using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall {
        #region ctor

        public double Diameter { get; init; }
     
        internal Ball(Vector initialPosition, Vector initialVelocity, double boardWidth, double boardHeight, double mass, double diameter) {
            Position = initialPosition;
            Velocity = initialVelocity;

            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
            Mass = mass;
            Diameter = diameter;

            _isMoving = true;

            Task.Run(MoveLoop);
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public double Mass { get; init; }
        public object BallLock { get; } = new object();

        public IVector Position { get; private set; } = new Vector(0, 0);
        public IVector Velocity { get; set; } = new Vector(0, 0);

        #endregion IBall

        #region private

        private readonly double _boardWidth;
        private readonly double _boardHeight;
        private bool _isMoving;
        private readonly int _delayMs = 15;

        private void RaiseNewPositionChangeNotification() {
            NewPositionNotification?.Invoke(this, Position);
        }

        private async Task MoveLoop()
        {
            Stopwatch stopwatch = new Stopwatch();

            while (_isMoving) {
                stopwatch.Restart();

                lock (BallLock) {
                    Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);
                }
                RaiseNewPositionChangeNotification();

                stopwatch.Stop();
                int timeToWait = _delayMs - (int)stopwatch.ElapsedMilliseconds;

                if (timeToWait > 0) {
                    await Task.Delay(timeToWait);
                }
                else {
                    await Task.Yield();
                }
            }
        }

        #endregion private

        #region IDisposable

        public void Dispose() {
            _isMoving = false;
        }

        #endregion IDisposable
    }
}
