using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    public class Sky
    {
        private Color color;
        private Color noonColor;
        private Color midnightColor;
        private float ambientLight;

        public Sky(Color noonColor, Color midnightColor)
        { 
            this.noonColor = noonColor;
            this.midnightColor = midnightColor;

            this.ambientLight = noonColor.ToVector3().Length();
            this.ambientLight += midnightColor.ToVector3().Length();
            this.ambientLight = ambientLight / 2f;

            this.color = new Color(noonColor.ToVector3());
        }

        public void Update(HeavenlyBody sun, HeavenlyBody moon) 
        {
            //throw new NotImplementedException();
        }

        public Color getColor()
        {
            return this.color;
        }

        internal float getAmbientLight()
        {
            return this.ambientLight;
        }
    }
}
