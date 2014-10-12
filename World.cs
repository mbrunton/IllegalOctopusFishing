using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Input;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class World
    {
        private IllegalOctopusFishingGame game;
        private Player player;
        private float worldSize;
        private float seaLevel;
        private Terrain terrain;
        private Ocean ocean;
        private Wind wind;
        private HeavenlyBody sun, moon;
        private Camera camera;

        private List<Fish> fish;
        private List<CoastGuardPersonel> coastGuard;
        private List<Harpoon> harpoons;

        public World(IllegalOctopusFishingGame game)
        {
            this.game = game;
            this.player = new Player();

            this.worldSize = 1000;
            this.seaLevel = 0;
            this.terrain = new Terrain(worldSize);
            this.ocean = new Ocean(worldSize, seaLevel);
        }

        internal void Update(GameTime gameTime, SharpDX.Toolkit.Input.KeyboardState keyboardState, AccelerometerReading accelerometerReading)
        {
            throw new NotImplementedException();
        }

        internal void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
