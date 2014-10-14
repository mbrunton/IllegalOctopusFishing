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
        private Sky sky;
        private Camera camera;

        private int numFish, numCoastGuard;
        private List<Fish> fish;
        private List<CoastGuardPersonel> coastGuard;
        private List<Harpoon> harpoons;

        private List<GameObject> objectsForDrawing;

        public World(IllegalOctopusFishingGame game)
        {
            this.game = game;
            this.objectsForDrawing = new List<GameObject>();

            this.worldSize = 500f;
            this.seaLevel = 0;
            this.terrain = new Terrain(game, worldSize, seaLevel);
            objectsForDrawing.Add(terrain);
            
            this.ocean = new Ocean(game, worldSize, seaLevel);
            objectsForDrawing.Add(ocean);
            
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

            Color noonColor = Color.CornflowerBlue;
            Color midnightColor = Color.DarkBlue;
            this.sky = new Sky(noonColor, midnightColor);

            Vector3 playerStartPos = terrain.getPlayerStartPos();
            this.player = new Player(game, playerStartPos);
            objectsForDrawing.Add(player);

            this.camera = new Camera(game, player.pos, player.dir, player.vel);

            numFish = 100;
            this.fish = new List<Fish>(numFish);
            for (int i = 0; i < numFish; i++)
            {
                Vector3 fishStartPos = terrain.getRandomUnderWaterLocation();
                Fish f = new Fish(game, fishStartPos);
                fish.Add(f);
                objectsForDrawing.Add(f);
            }
            numCoastGuard = 20;
            this.coastGuard = new List<CoastGuardPersonel>(numCoastGuard);
            for (int i = 0; i < numCoastGuard; i++)
            {
                Vector3 coastGuardStartPos = terrain.getRandomOnWaterLocation();
                CoastGuardPersonel c = new CoastGuardPersonel(game, coastGuardStartPos);
                coastGuard.Add(c);
            }
            harpoons = new List<Harpoon>();

            foreach (GameObject obj in objectsForDrawing)
            {
                obj.SetupLighting(sky.getAmbientLight(), sun, moon);
            }
        }

        internal void setPlayerModel(Player.BoatSize selectedBoat, String modelName)
        {
            player.setModel(selectedBoat, modelName);
            player.setModelLighting(sky.getAmbientLight(), sun, moon);
        }

        internal void Update(GameTime gameTime, KeyboardState keyboardState, AccelerometerReading accelerometerReading)
        {
            Dictionary<Player.HullPositions, float> playerHullHeights = terrain.getTerrainHeightsForPlayerHull(player.getHullPositions());
            player.Update(gameTime, playerHullHeights, seaLevel, wind, gravity);

            camera.Update(player.pos, player.dir, player.vel);

            foreach (Fish f in fish)
            {
                float fishTerrainHeight = terrain.getTerrainHeightAtPosition(f.pos.X, f.pos.Z);
                f.Update(gameTime, fishTerrainHeight, seaLevel, gravity);
            }

            foreach (CoastGuardPersonel c in coastGuard)
            {
                float coastGuardTerrainHeight = terrain.getTerrainHeightAtPosition(c.pos.X, c.pos.Z);
                c.Update(gameTime, coastGuardTerrainHeight, seaLevel, gravity);
            }

            // TODO:
            /* player eating fish
             * enemy spotting player
             * enemy shooting at player
             * player hit by harpoon
             */

            sun.Update(gameTime);
            moon.Update(gameTime);
            sky.Update(sun, moon);
            ocean.Update(gameTime);

            foreach (GameObject obj in objectsForDrawing)
            {
                obj.AlignWithCamera(camera);
                obj.SetLightingDirections(sun, moon);
            }
        }

        internal void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(sky.getColor());

            foreach (GameObject obj in objectsForDrawing)
            {
                obj.Draw(gameTime);
            }
        }
    }
}
