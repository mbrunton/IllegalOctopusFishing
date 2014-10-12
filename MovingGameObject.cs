using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    abstract class MovingGameObject : GameObject
    {
        private Vector3 pos, dir, vel, up;
        private float mass;
        private float acc; // acceleration

        public MovingGameObject(Vector3 pos)
        {
            this.pos = pos;
            this.up = Vector3.UnitY;
            this.dir = Vector3.UnitX;
        }

        public void SetMass(float mass)
        {
            this.mass = mass;
        }

        public void SetAcc(float acc)
        {
            this.acc = acc;
        }
    }
}
