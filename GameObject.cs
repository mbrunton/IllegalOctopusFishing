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
    abstract class GameObject
    {
        protected IllegalOctopusFishingGame game;
        protected BasicEffect basicEffect;
        protected Color diffuseColor;

        public GameObject(IllegalOctopusFishingGame game)
        {
            this.game = game;

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                Projection = Matrix.Identity,
                World = Matrix.Identity,
                View = Matrix.Identity
            };
        }

        public void SetupLighting(float ambientLight, HeavenlyBody sun, HeavenlyBody moon)
        {
            if (diffuseColor == null)
            {
                throw new ArgumentException("diffuseColor is null, but SetupLighting is being called");
            }
            
            basicEffect.LightingEnabled = true;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight1.Enabled = true;

            Vector3 ambientLightVector = new Vector3(diffuseColor.ToVector3().ToArray());
            if (!MathUtil.IsZero(ambientLightVector.LengthSquared()))
            {
                ambientLightVector.Normalize();
            }
            ambientLightVector = ambientLight * ambientLightVector;
            basicEffect.AmbientLightColor = ambientLightVector;
            
            basicEffect.DirectionalLight0.DiffuseColor = diffuseColor.ToVector3();
            basicEffect.DirectionalLight1.DiffuseColor = diffuseColor.ToVector3();

            basicEffect.DirectionalLight0.SpecularColor = sun.getSpecularColor().ToVector3();
            basicEffect.DirectionalLight1.SpecularColor = moon.getSpecularColor().ToVector3();
        }

        internal void AlignWithCamera(Camera camera)
        {
            basicEffect.View = camera.getView();
            basicEffect.Projection = camera.getProjection();
        }

        internal void SetLightingDirections(HeavenlyBody sun, HeavenlyBody moon)
        {
            basicEffect.DirectionalLight0.Direction = sun.getDir();
            basicEffect.DirectionalLight1.Direction = moon.getDir();
        }

        abstract public void Draw(GameTime gameTime);
    }
}
