//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI {
        private bool Disposed = false;
        private readonly UnderneathLayerAPI layerBellow;
        private readonly List<Data.IBall> _logicBalls = new List<Data.IBall>();
        private readonly object _collisionLock = new object();

        public BusinessLogicImplementation() : this(null) { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer) {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed) {
            returnInstanceDisposed(Disposed);
        }

        public override void Dispose() {
            if (Disposed) throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler) {
            if (Disposed) throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

            layerBellow.Start(numberOfBalls, (startingPosition, databall) => {
                lock (_collisionLock) {
                    _logicBalls.Add(databall);
                }
                databall.NewPositionNotification += CheckCollisions;
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall));
            });
        }

        private void CheckCollisions(object? sender, IVector e)
        {
            Data.IBall currentBall = (Data.IBall)sender!;

            lock (_collisionLock)
            {
                foreach (var otherBall in _logicBalls)
                {
                    if (currentBall == otherBall) continue;
                    lock (currentBall.BallLock)
                    {
                        lock (otherBall.BallLock)
                        {
                            double dx = currentBall.Position.x - otherBall.Position.x;
                            double dy = currentBall.Position.y - otherBall.Position.y;
                            double distance = Math.Sqrt(dx * dx + dy * dy);
                            double minDistance = (currentBall.Diameter / 2.0) + (otherBall.Diameter / 2.0);

                            if (distance <= minDistance)
                            {
                                if ((currentBall.Velocity.x - otherBall.Velocity.x) * dx +
                                    (currentBall.Velocity.y - otherBall.Velocity.y) * dy < 0)
                                {
                                    double m1 = currentBall.Mass;
                                    double m2 = otherBall.Mass;

                                    double v1x = currentBall.Velocity.x;
                                    double v1y = currentBall.Velocity.y;

                                    double v2x = otherBall.Velocity.x;
                                    double v2y = otherBall.Velocity.y;

                                    double newV1x = (v1x * (m1 - m2) + 2 * m2 * v2x) / (m1 + m2);
                                    double newV2x = (v2x * (m2 - m1) + 2 * m1 * v1x) / (m1 + m2);

                                    double newV1y = (v1y * (m1 - m2) + 2 * m2 * v2y) / (m1 + m2);
                                    double newV2y = (v2y * (m2 - m1) + 2 * m1 * v1y) / (m1 + m2);

                                    currentBall.Velocity = new LogicVector(newV1x, newV1y);
                                    otherBall.Velocity = new LogicVector(newV2x, newV2y);
                                }
                            }
                        }
                    }
                }
            }
        }

        private record LogicVector(double x, double y) : Data.IVector;
    }
}