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
        private Buffer<VertexPositionNormalColor> vertices;
        private BasicEffect basicEffect;

        public GameObject(IllegalOctopusFishingGame game)
        {
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                Projection = Matrix.Identity,
                World = Matrix.Identity,
                View = Matrix.Identity
            };
        }

        public void SetupLighting(Vector3 ambientLight, HeavenlyBody sun, HeavenlyBody moon)
        {
            basicEffect.LightingEnabled = true;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight1.Enabled = true;

            basicEffect.AmbientLightColor = ambientLight;

            throw new NotImplementedException();
        }
    }
}
