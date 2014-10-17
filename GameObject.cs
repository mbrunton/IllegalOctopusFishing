using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    abstract public class GameObject
    {
        protected ExtremeSailingGame game;
        internal Effect effect;

        internal Matrix View;
        internal Matrix Projection;
        internal Matrix World;

        public GameObject(ExtremeSailingGame game, String effectName)
        {
            this.game = game;
            this.View = Matrix.Identity;
            this.Projection = Matrix.Identity;
            this.World = Matrix.Identity;

            this.effect = game.Content.Load<Effect>(effectName).Clone();
        }

        abstract public void Draw(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon);

        internal void SetEffectValues(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon)
        {
            /* shader parameters
            float4x4 World;
            float4x4 View;
            float4x4 Projection;
            float4 cameraPos;
            float4 sunLightAmbCol;
            float4 sunLightPntPos;
            float4 sunLightPntCol;
            float4 moonLightAmbCol;
            float4 moonLightPntPos;
            float4 moonLightPntCol;
            float4x4 worldInvTrp;
             */

            View = camera.view;
            Projection = camera.projection;

            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["cameraPos"].SetValue(camera.pos.ToArray());
            
            effect.Parameters["sunLightAmbCol"].SetValue(ambientColor.ToVector3());
            effect.Parameters["sunLightPntPos"].SetValue(sun.pos);
            effect.Parameters["sunLightPntCol"].SetValue(sun.specularColor.ToVector3());
            effect.Parameters["moonLightAmbCol"].SetValue(ambientColor.ToVector3());
            effect.Parameters["moonLightPntPos"].SetValue(moon.pos);
            effect.Parameters["moonLightPntCol"].SetValue(moon.specularColor);
            
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
        }
    }
}
