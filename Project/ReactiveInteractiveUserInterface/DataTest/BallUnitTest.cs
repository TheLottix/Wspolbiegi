//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest {
        [TestMethod]
        public void ConstructorTestMethod() {
            Vector initialPosition = new Vector(10.0, 10.0);
            Vector initialVelocity = new Vector(0.0, 0.0);

            Ball newInstance = new(initialPosition, initialVelocity, 100.0, 100.0, 10.0, 5.0);

            Assert.AreEqual(10.0, newInstance.Position.x);
            Assert.AreEqual(10.0, newInstance.Position.y);
            Assert.AreEqual(10.0, newInstance.Mass);
            Assert.AreEqual(5.0, newInstance.Diameter);

            newInstance.Dispose();
            }

    }
}