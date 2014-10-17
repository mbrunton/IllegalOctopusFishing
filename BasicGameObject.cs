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
    public class BasicGameObject : GameObject
    {
        protected Buffer<VertexPositionNormalColor> vertices;
        protected VertexInputLayout inputLayout;
        //protected BasicEffect basicEffect;
        Effect effect;
        protected Color diffuseColor;

        public BasicGameObject(ExtremeSailingGame game)
            : base(game)
        {
            /*
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            basicEffect.LightingEnabled = true;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight1.Enabled = true;
            */
            this.effect = game.Content.Load<Effect>("Phong").Clone();
        }
        
        public override void Draw(GameTime gameTime, Camera camera, Sky sky, HeavenlyBody sun, HeavenlyBody moon)
        {
            if (diffuseColor == null)
            {
                throw new ArgumentException("diffuseColor is null, but Draw relies on it yo");
            }

            View = camera.view;
            Projection = camera.projection;

            /*
            basicEffect.World = World;
            basicEffect.View = View;
            basicEffect.Projection = Projection;

            basicEffect.DirectionalLight0.Direction = -sun.pos;
            basicEffect.DirectionalLight1.Direction = -moon.pos;

            basicEffect.AmbientLightColor = sky.color.ToVector3();
            basicEffect.DirectionalLight0.DiffuseColor = diffuseColor.ToVector3();
            basicEffect.DirectionalLight1.DiffuseColor = diffuseColor.ToVector3();
            basicEffect.DirectionalLight0.SpecularColor = sun.specularColor.ToVector3();
            basicEffect.DirectionalLight1.SpecularColor = moon.specularColor.ToVector3();
            */

            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["cameraPos"].SetValue(camera.pos);
            effect.Parameters["worldInvTrp"].SetValue(Matrix.Transpose(Matrix.Invert(World)));

            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw
            effect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
