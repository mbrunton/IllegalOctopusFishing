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

        public BasicGameObject(ExtremeSailingGame game)
            : base(game)
        {
            String effectName = "VertexPhong";
            this.effect = game.Content.Load<Effect>(effectName).Clone();
        }

        internal override void SetEffectValues(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon)
        {
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
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw
            effect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
