using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Sky
    {
        private Color color;
        private Color noonColor;
        private Color midnightColor;
        private Vector3 ambientLight;

        public Sky(Color noonColor, Color midnightColor, Vector3 ambientLight)
        {
            this.noonColor = noonColor;
            this.midnightColor = midnightColor;
            this.ambientLight = ambientLight;

            this.color = new Color(noonColor.ToVector3());
        }

        public void Update(HeavenlyBody sun, HeavenlyBody moon) 
        {
            throw new NotImplementedException();
        }

        public Color getColor()
        {
            return this.color;
        }

        internal Vector3 getAmbientLight()
        {
            return this.ambientLight;
        }
    }
}
