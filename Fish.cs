using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Fish : MovingGameObject
    {
        public Fish(Vector3 startingPos) : base(startingPos)
        {
            float fishMass = 10f;
            float fishAcc = 4f;
            base.SetMass(fishMass);
            base.SetAcc(fishAcc);
        }
    }
}
