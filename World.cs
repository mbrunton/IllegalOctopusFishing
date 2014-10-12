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
        private Gravity gravity;
        private float secsPerGameDay; // number of seconds per in-game day
        private HeavenlyBody sun, moon;
        private Camera camera;

        private int numFish, numCoastGuard;
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
            this.wind = new Wind();
            this.gravity = new Gravity(-1 * Vector3.UnitY, 9.81f);

            this.secsPerGameDay = 180;
            Vector3 sunInitialDir = -1 * Vector3.UnitY;
            Vector3 sunRevolutionNormal = Vector3.UnitX; // normal to sun's plane of motion
            Color sunSpectralColor = Color.LightYellow;
            this.sun = new HeavenlyBody(sunInitialDir, sunRevolutionNormal, secsPerGameDay, sunSpectralColor);

            Vector3 moonInitialDir = Vector3.UnitY;
            Vector3 moonRevolutionNormal = Vector3.UnitZ; // normal to moon's plane of motion
            Color moonSpectralColor = Color.GhostWhite;
            this.moon = new HeavenlyBody(moonInitialDir, moonRevolutionNormal, secsPerGameDay, moonSpectralColor);

            this.camera = new Camera();

            numFish = 100;
            this.fish = new List<Fish>(numFish);
            for (int i = 0; i < numFish; i++)
            {
                Vector3 fishStartingLoc = terrain.getUnderWaterLocation();
                fish.Add(new Fish(fishStartingLoc));
            }
                numCoastGuard = 20;
            this.coastGuard = new List<CoastGuardPersonel>(numCoastGuard);
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
