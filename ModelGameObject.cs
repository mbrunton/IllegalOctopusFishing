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
    using System.Diagnostics;
    abstract public class ModelGameObject : GameObject
    {
        internal Model model;
        internal Vector3 pos, dir, vel, up;
        internal Vector3 modelDir; // direction model is facing in its own coord sys
        internal float acc;
        internal float maxVel;

        public ModelGameObject(ExtremeSailingGame game, Vector3 startPos, Model model) : base(game)
        {
            this.pos = startPos;
            this.up = Vector3.UnitY;
            this.modelDir = Vector3.UnitX;
            this.dir = modelDir;
            this.model = model;

            String effectName = "ModelPhong";
            this.effect = game.Content.Load<Effect>(effectName).Clone();
        }
        
        internal Matrix getWorld()
        {
            // use pos, dir, up to generate a world matrix
            Matrix translation = getTranslationMatrix();
            Matrix rotation = getRotationMatrix();
            return rotation * translation;
        }

        internal Matrix getRotationMatrix()
        {
            Vector3 xzDir = new Vector3(dir.X, 0, dir.Z);
            xzDir.Normalize();
            float angle = (float)Math.Acos(Vector3.Dot(modelDir, xzDir));
            Vector3 cross = Vector3.Cross(modelDir, xzDir);
            if (!MathUtil.IsZero(cross.Y) && cross.Y < 0)
            {
                angle = 2 * (float)Math.PI - angle;
            }

            Matrix rotation;
            if (angle > 0.001f)
            {
                rotation = Matrix.RotationY(angle);
            } else {
                rotation = Matrix.Identity;
            }

            return rotation;
        }

        internal Matrix getTranslationMatrix()
        {
            Matrix translation = Matrix.Translation(pos);
            return translation;
        }

        internal override void SetEffectValues(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon)
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

        public override void Draw(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon)
        {
            SetEffectValues(camera, ambientColor, sun, moon);
            model.Draw(game.GraphicsDevice, World, View, Projection, effectOverride : effect);
        }
    }
}
