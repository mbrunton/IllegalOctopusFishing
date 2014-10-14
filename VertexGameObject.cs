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
    class VertexGameObject : GameObject
    {
        protected Buffer<VertexPositionNormalColor> vertices;
        protected VertexInputLayout inputLayout;

        public VertexGameObject(IllegalOctopusFishingGame game)
            : base(game)
        {
            basicEffect.VertexColorEnabled = true;
        }
        
        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
