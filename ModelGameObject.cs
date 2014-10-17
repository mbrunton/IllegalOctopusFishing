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
        internal Effect effect;
        internal Vector3 pos, dir, vel, up;
        internal Vector3 modelDir; // direction model is facing in its own coord sys
        internal float acc;
        internal float maxVel;

        public ModelGameObject(ExtremeSailingGame game, Vector3 startPos, String modelName) : base(game)
        {
            this.pos = startPos;
            this.up = Vector3.UnitY;
            this.modelDir = Vector3.UnitX;
            this.dir = modelDir;

            bool success = game.nameToModel.TryGetValue(modelName, out model);
            if (!success)
            {
                throw new ArgumentException("failed to load model");
            }

            this.effect = game.Content.Load<Effect>("ModifiedPhong").Clone();
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

        public override void Draw(GameTime gameTime, Camera camera, Sky sky, HeavenlyBody sun, HeavenlyBody moon)
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
            
            effect.Parameters["sunLightAmbCol"].SetValue(sky.color.ToVector4().ToArray());
            effect.Parameters["sunLightPntPos"].SetValue(sun.pos.ToArray());
            effect.Parameters["sunLightPntCol"].SetValue(sun.specularColor.ToVector4().ToArray());
            effect.Parameters["moonLightAmbCol"].SetValue(sky.color.ToVector3().ToArray());
            effect.Parameters["moonLightPntPos"].SetValue(moon.pos.ToArray());
            effect.Parameters["moonLightPntCol"].SetValue(moon.specularColor.ToVector4().ToArray());
            
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
            

            /* regular Phong.fx
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["cameraPos"].SetValue(camera.pos.ToArray());
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
            */

            model.Draw(game.GraphicsDevice, World, View, Projection, effectOverride : effect);
            //model.Draw(game.GraphicsDevice, World, View, Projection);
        }
    }
}
