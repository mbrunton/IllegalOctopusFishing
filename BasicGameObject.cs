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
            : base(game, "VertexPhong")
        {
            
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
