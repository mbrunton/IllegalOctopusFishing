﻿using System;
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
        protected BasicEffect basicEffect;
        protected Color diffuseColor;

        public BasicGameObject(ExtremeSailingGame game)
            : base(game)
        {
            
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                Projection = this.Projection,
                World = this.World,
                View = this.View,
                VertexColorEnabled = true
            };
        }

        public override void SetupLighting(Sky sky, HeavenlyBody sun, HeavenlyBody moon)
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
            ambientLightVector = sky.getAmbientLight() * ambientLightVector;
            basicEffect.AmbientLightColor = ambientLightVector;
            basicEffect.DirectionalLight0.DiffuseColor = diffuseColor.ToVector3();
            basicEffect.DirectionalLight1.DiffuseColor = diffuseColor.ToVector3();
            basicEffect.DirectionalLight0.SpecularColor = sun.getSpecularColor().ToVector3();
            basicEffect.DirectionalLight1.SpecularColor = moon.getSpecularColor().ToVector3();
        }

        public override void UpdateLightingDirections(HeavenlyBody sun, HeavenlyBody moon)
        {
            basicEffect.DirectionalLight0.Direction = sun.getDir();
            basicEffect.DirectionalLight1.Direction = moon.getDir();
        }
        
        public override void Draw(GameTime gameTime)
        {
            basicEffect.View = this.View;
            basicEffect.Projection = this.Projection;

            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
