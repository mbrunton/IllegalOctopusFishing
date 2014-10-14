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
    abstract public class ModelGameObject : GameObject
    {
        internal Model model;
        internal Vector3 pos, dir, vel, up;
        internal float mass;
        internal float acc;
        internal float maxVel;

        public ModelGameObject(IllegalOctopusFishingGame game, Vector3 startPos, String modelName) : base(game)
        {
            this.pos = startPos;
            this.up = Vector3.UnitY;
            this.dir = Vector3.UnitX;

            bool success = game.nameToModel.TryGetValue(modelName, out model);
            if (!success)
            {
                throw new ArgumentException("failed to load model");
            }
        }

        public override void SetupLighting(Sky sky, HeavenlyBody sun, HeavenlyBody moon)
        {
            // maybe i don't have to do anything here
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                mesh.Effects[0].Parameters["DiffuseColor"].SetValue(new Vector4());
            }
        }

        public override void UpdateLightingDirections(HeavenlyBody sun, HeavenlyBody moon)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                mesh.Effects[0].Parameters["DirLight0Direction"].SetValue(sun.getDir());
                mesh.Effects[0].Parameters["DirLight1Direction"].SetValue(moon.getDir());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            model.Draw(game.GraphicsDevice, World, View, Projection);
        }
    }
}
