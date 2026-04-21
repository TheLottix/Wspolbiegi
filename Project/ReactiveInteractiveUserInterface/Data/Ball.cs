//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    #endregion IBall

    #region private

    private Vector Position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(double boardWidth, double boardHeight)
    {
        double diameter = 20.0;

        double nextX = Position.x + Velocity.x;
        double nextY = Position.y + Velocity.y;

        if (nextX <= 0)
        {
            nextX = 0;
            Velocity = new Vector(-Velocity.x, Velocity.y);
        }
        else if (nextX + diameter >= boardWidth)
        {
            nextX = boardWidth - diameter;
            Velocity = new Vector(-Velocity.x, Velocity.y);
        }

        if (nextY <= 0)
        {
            nextY = 0;
            Velocity = new Vector(Velocity.x, -Velocity.y);
        }
        else if (nextY + diameter >= boardHeight)
        {
            nextY = boardHeight - diameter;
            Velocity = new Vector(Velocity.x, -Velocity.y);
        }

        Position = new Vector(nextX, nextY);
        RaiseNewPositionChangeNotification();
    }

        #endregion private
    }
}