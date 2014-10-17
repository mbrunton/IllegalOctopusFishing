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

        public ModelGameObject(ExtremeSailingGame game, Vector3 startPos, String modelName) : base(game, "ModelPhong")
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

        public override void Draw(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon)
        {
            SetEffectValues(camera, ambientColor, sun, moon);
            model.Draw(game.GraphicsDevice, World, View, Projection, effectOverride : effect);
        }
    }
}
