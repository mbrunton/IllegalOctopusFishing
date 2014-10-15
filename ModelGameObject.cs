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
        internal Vector3 initialDir; // direction model is facing in its own coord sys
        internal float mass;
        internal float acc;
        internal float maxVel;

        public ModelGameObject(IllegalOctopusFishingGame game, Vector3 startPos, String modelName) : base(game)
        {
            this.pos = startPos;
            this.up = Vector3.UnitY;
            this.initialDir = Vector3.UnitX;
            this.dir = initialDir;

            bool success = game.nameToModel.TryGetValue(modelName, out model);
            if (!success)
            {
                throw new ArgumentException("failed to load model");
            }
        }

        public override void SetupLighting(Sky sky, HeavenlyBody sun, HeavenlyBody moon)
        {
            //this.effect = game.Content.Load<Effect>("Phong");
            // TODO: work out what to do here
            //BasicEffect.EnableDefaultLighting(model, true);
            /*
            foreach (ModelMesh mesh in model.Meshes)
            {
                for (int i = 0; i < mesh.Effects.Count; i++)
                {
                    game.Content.Load<Effect>("Phong");
                }
                //mesh.Effects[0].Parameters["DirLight0SpecularColor"].SetValue(sun.getSpecularColor());
                //mesh.Effects[0].Parameters["DiffuseColor"].SetValue(new Vector4());
            }*/
        }

        public override void UpdateLightingDirections(HeavenlyBody sun, HeavenlyBody moon)
        {
            // TODO
            /*
            foreach (ModelMesh mesh in model.Meshes)
            {
                //mesh.Effects[0].Parameters["DirLight0Direction"].SetValue(sun.getDir());
                //mesh.Effects[0].Parameters["DirLight1Direction"].SetValue(moon.getDir());
            }*/
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
            float angle = (float)Math.Acos(Vector3.Dot(initialDir, xzDir));
            Vector3 cross = Vector3.Cross(initialDir, dir);
            if (cross.Y < 0)
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

        public override void Draw(GameTime gameTime)
        {
            model.Draw(game.GraphicsDevice, World, View, Projection);
        }
    }
}
