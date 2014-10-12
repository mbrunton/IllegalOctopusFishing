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
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
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

            Vector3 playerStartPos = terrain.getPlayerStartPos();
            this.player = new Player(playerStartPos);
            this.camera = new Camera();

            numFish = 100;
            this.fish = new List<Fish>(numFish);
            for (int i = 0; i < numFish; i++)
            {
                Vector3 fishStartPos = terrain.getUnderWaterLocation();
                fish.Add(new Fish(fishStartPos));
            }
            numCoastGuard = 20;
            this.coastGuard = new List<CoastGuardPersonel>(numCoastGuard);
            for (int i = 0; i < numCoastGuard; i++)
            {
                Vector3 coastGuardStartPos = terrain.getOnWaterLocation();
                coastGuard.Add(new CoastGuardPersonel(coastGuardStartPos));
            }
        }

        internal void setSelectedBoat(string selectedBoat)
        {
            switch (selectedBoat)
            {
                case "smallBoat":
                    player.SetMass(500f);
                    player.SetAcc(5f);
                    break;
                case "largeBoat":
                    player.SetMass(1000f);
                    player.SetAcc(7f);
                    break;
                default:
                    throw new ArgumentException("selectedBoat string is not recognized");
            }
        }

        internal void Update(GameTime gameTime, KeyboardState keyboardState, AccelerometerReading accelerometerReading)
        {
            throw new NotImplementedException();
        }

        internal void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
