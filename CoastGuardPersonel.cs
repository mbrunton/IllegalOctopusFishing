using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class CoastGuardPersonel : MovingGameObject
    {
        public CoastGuardPersonel(Vector3 startingPos) : base(startingPos)
        {
            float mass = 1000f;
            float acc = 6f;
            base.SetMass(mass);
            base.SetAcc(acc);
        }
    }
}
