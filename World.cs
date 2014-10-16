﻿using System;
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

        public World(IllegalOctopusFishingGame game, Player.BoatSize selectedBoat)
        {
            this.game = game;
            this.objectsForDrawing = new List<GameObject>();

            this.worldSize = 8000f;
            this.seaLevel = 0;
            
            this.wind = new Wind();
            this.gravity = new Gravity(-1 * Vector3.UnitY);

            // sun
            this.secsPerGameDay = 180;
            Vector3 sunInitialDir = -1 * Vector3.UnitY;
            Vector3 sunRevolutionNormal = Vector3.UnitX; // normal to sun's plane of motion
            Color sunSpectralColor = Color.LightYellow;
            this.sun = new HeavenlyBody(sunInitialDir, sunRevolutionNormal, secsPerGameDay, sunSpectralColor);

            // moon
            Vector3 moonInitialDir = Vector3.UnitY;
            Vector3 moonRevolutionNormal = Vector3.UnitZ; // normal to moon's plane of motion
            Color moonSpectralColor = Color.GhostWhite;
            this.moon = new HeavenlyBody(moonInitialDir, moonRevolutionNormal, secsPerGameDay, moonSpectralColor);

            // sky
            Color noonColor = Color.CornflowerBlue;
            Color midnightColor = Color.DarkBlue;
            this.sky = new Sky(noonColor, midnightColor);

            // terrain
            this.terrain = new Terrain(game, worldSize, seaLevel);
            objectsForDrawing.Add(terrain);
            
            // ocean
            this.ocean = new Ocean(game, worldSize, seaLevel);
            objectsForDrawing.Add(ocean);

            // player
            Vector3 playerStartPos = terrain.getPlayerStartPos();
            String playerBoatModelName, playerSailModelName;
            if (selectedBoat == Player.BoatSize.SMALL)
            {
                playerBoatModelName = "smallboat";
                playerSailModelName = "smallsail";
            }
            else
            {
                playerBoatModelName = "largeboat";
                playerSailModelName = "largeSail";
            }
            this.player = new Player(game, playerStartPos, playerBoatModelName, playerSailModelName, selectedBoat);
            objectsForDrawing.Add(player);

            // camera
            this.camera = new Camera(game, player.pos, player.dir, player.vel);

            // fish
            //numFish = 100;
            numFish = 0;
            this.fish = new List<Fish>(numFish);
            for (int i = 0; i < numFish; i++)
            {
                Vector3 fishStartPos = terrain.getRandomUnderWaterLocation();
                Fish f = new Fish(game, fishStartPos, "fish");
                fish.Add(f);
                objectsForDrawing.Add(f);
            }

            //coastguard
            //numCoastGuard = 20;
            numCoastGuard = 0;
            this.coastGuard = new List<CoastGuardPersonel>(numCoastGuard);
            for (int i = 0; i < numCoastGuard; i++)
            {
                Vector3 coastGuardStartPos = terrain.getRandomOnWaterLocation();
                CoastGuardPersonel c = new CoastGuardPersonel(game, coastGuardStartPos, "coastguard");
                coastGuard.Add(c);
            }

            // harpoons
            harpoons = new List<Harpoon>();

            // setup lighting
            foreach (GameObject obj in objectsForDrawing)
            {
                obj.SetupLighting(sky, sun, moon);
            }
        }

        internal void Update(GameTime gameTime, KeyboardState keyboardState, AccelerometerReading accelerometerReading)
        {
            // player
            Dictionary<Player.HullPositions, Vector3> playerHullPositions = player.getHullPositions();
            Dictionary<Player.HullPositions, float> playerHullTerrainHeights = terrain.getTerrainHeightsForPlayerHull(playerHullPositions);
            Dictionary<Player.HullPositions, float> playerHullOceanHeights = ocean.getOceanHeightsForPlayerHull(playerHullPositions);
            if (keyboardState.IsKeyDown(Keys.Left) && !keyboardState.IsKeyDown(Keys.Right))
            {
                player.turnLeft(gameTime);
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && !keyboardState.IsKeyDown(Keys.Left))
            {
                player.turnRight(gameTime);
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                player.releaseSlack(gameTime);
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                player.reduceSlack(gameTime);
            }
            player.Update(gameTime, playerHullPositions, playerHullTerrainHeights, playerHullOceanHeights, wind, gravity);

            camera.Update(player.pos, player.dir, player.vel);

            foreach (Fish f in fish)
            {
                float fishTerrainHeight = terrain.getTerrainHeightAtPosition(f.pos.X, f.pos.Z);
                float fishOceanHeight = ocean.getOceanHeightAtPosition(f.pos.X, f.pos.Z);
                f.Update(gameTime, fishTerrainHeight, fishOceanHeight, gravity);
            }

            foreach (CoastGuardPersonel c in coastGuard)
            {
                float coastGuardTerrainHeight = terrain.getTerrainHeightAtPosition(c.pos.X, c.pos.Z);
                float coastGuardOceanHeight = ocean.getOceanHeightAtPosition(c.pos.X, c.pos.Z);
                c.Update(gameTime, coastGuardTerrainHeight, coastGuardOceanHeight, gravity);
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
            wind.Update(gameTime);
        }

        internal void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(sky.getColor());

            foreach (GameObject obj in objectsForDrawing)
            {
                obj.AlignWithCamera(camera);
                obj.UpdateLightingDirections(sun, moon);
                obj.Draw(gameTime);
            }
        }
    }
}
