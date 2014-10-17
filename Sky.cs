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
        internal Color color;
        private Color noonColor;
        private Color midnightColor;

        public Sky(Color noonColor, Color midnightColor)
        { 
            this.noonColor = noonColor;
            this.midnightColor = midnightColor;

            this.color = new Color(noonColor.ToVector3());
        }

        public void Update(HeavenlyBody sun, HeavenlyBody moon) 
        {
            if (sun.pos.Y > 0)
            {
                color = noonColor;
            }
            else
            {
                color = midnightColor;
            }

        }
    }
}
