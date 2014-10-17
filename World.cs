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
        private ExtremeSailingGame game;
        private Dictionary<ExtremeSailingGame.ModelNames, Model> modelNameToModel;
        private Player player;
        private float worldSize;
        private float seaLevel;
        private Terrain terrain;
        private Ocean ocean;
        private Wind wind;
        private Gravity gravity;
        private float secsPerGameDay; // number of seconds per in-game day
        private HeavenlyBody sun, moon;
        private Color ambientColor;
        private Camera camera;

        private int numFish, numCoastGuard;
        private List<Fish> fish;
        private List<CoastGuardPersonel> coastGuard;
        private List<Harpoon> harpoons;

        private List<GameObject> objectsForDrawing;

        public World(ExtremeSailingGame game, Dictionary<ExtremeSailingGame.ModelNames, Model> modelNameToModel, Player.BoatSize selectedBoat)
        {
            this.game = game;
            this.modelNameToModel = modelNameToModel;
            this.objectsForDrawing = new List<GameObject>();

            this.worldSize = 16000f;
            this.seaLevel = 0;
            
            this.wind = new Wind();
            this.gravity = new Gravity(-1 * Vector3.UnitY);

            // sun
            this.secsPerGameDay = 18;
            float sunDistFromOrigin = worldSize;
            Vector3 sunInitialPos = sunDistFromOrigin * Vector3.UnitY;
            Vector3 sunRevolutionNormal = Vector3.UnitX; // normal to sun's plane of motion
            Color sunSpectralColor = new Color(0.8f, 0.4f, 0.1f);
            this.sun = new HeavenlyBody(sunInitialPos, sunDistFromOrigin, sunRevolutionNormal, secsPerGameDay, sunSpectralColor);

            // moon
            float moonDistFromOrigin = sunDistFromOrigin;
            Vector3 moonInitialPos = -1 * moonDistFromOrigin * Vector3.UnitY;
            Vector3 moonRevolutionNormal = Vector3.UnitZ; // normal to moon's plane of motion
            Color moonSpectralColor = new Color(.4f, .4f, .4f);
            this.moon = new HeavenlyBody(moonInitialPos, moonDistFromOrigin, moonRevolutionNormal, secsPerGameDay, moonSpectralColor);

            // sky
            ambientColor = new Color(.3f, .3f, .3f);

            // terrain
            this.terrain = new Terrain(game, worldSize, seaLevel);
            objectsForDrawing.Add(terrain);
            
            // ocean
            this.ocean = new Ocean(game, worldSize, seaLevel);
            objectsForDrawing.Add(ocean);

            // player
            Vector3 playerStartPos = terrain.getPlayerStartPos();
            Model playerBoatModel = modelNameToModel[ExtremeSailingGame.ModelNames.SMALLBOAT];
            Model playerSailModel = modelNameToModel[ExtremeSailingGame.ModelNames.SMALLSAIL];
            this.player = new Player(game, playerStartPos, playerBoatModel, playerSailModel, selectedBoat);
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
                Fish f = new Fish(game, fishStartPos, modelNameToModel[ExtremeSailingGame.ModelNames.FISH]);
                fish.Add(f);
                objectsForDrawing.Add(f);
            }

            //coastguard
            //numCoastGuard = 20;
            numCoastGuard = 20;
            this.coastGuard = new List<CoastGuardPersonel>(numCoastGuard);
            for (int i = 0; i < numCoastGuard; i++)
            {
                Vector3 coastGuardStartPos = terrain.getRandomOnWaterLocation();
                coastGuardStartPos = (1 / 200) * coastGuardStartPos;
                //Vector3 coastGuardStartPos = new Vector3();
                CoastGuardPersonel c = new CoastGuardPersonel(game, coastGuardStartPos, modelNameToModel[ExtremeSailingGame.ModelNames.COASTGUARD]);
                coastGuard.Add(c);
                objectsForDrawing.Add(c);
            }

            // harpoons
            harpoons = new List<Harpoon>();
        }

        internal void Update(GameTime gameTime, KeyboardState keyboardState, AccelerometerReading accelerometerReading)
        {
            // player
            if (player.health <= 0)
            {
                // game over!
                game.GameOver();
            }
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
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                player.Fire(this);
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
                c.Update(this, gameTime, coastGuardTerrainHeight, coastGuardOceanHeight);
            }

            // TODO:
            /* player eating fish
             * enemy spotting player
             * enemy shooting at player
             * player hit by harpoon
             * enemy hit by harpoon
             * harpoon hit ground
             */

            List<Harpoon> harpoonsToDelete = new List<Harpoon>();
            List<CoastGuardPersonel> coastGuardToDelete = new List<CoastGuardPersonel>();
            foreach (Harpoon harpoon in harpoons)
            {
                harpoon.Update(gameTime, gravity);
                float harpoonTerrainHeight = terrain.getTerrainHeightAtPosition(harpoon.pos.X, harpoon.pos.Z);
                if (harpoonTerrainHeight > harpoon.pos.Y)
                {
                    harpoonsToDelete.Add(harpoon);
                    continue;
                }
                if (harpoon.cooloff == 0)
                {
                    float playerDist = (player.pos - harpoon.pos).Length();
                    if (playerDist < harpoon.attackRange)
                    {
                        player.health -= harpoon.damage;
                        harpoonsToDelete.Add(harpoon);
                        continue;

                    }
                    foreach (CoastGuardPersonel c in coastGuard)
                    {
                        float coastGuardDist = (c.pos - harpoon.pos).Length();
                        if (coastGuardDist < harpoon.attackRange)
                        {
                            coastGuardToDelete.Add(c);
                            harpoonsToDelete.Add(harpoon);
                            break;
                        }
                    }
                }
            }

            foreach (Harpoon harpoon in harpoonsToDelete)
            {
                harpoons.Remove(harpoon);
                objectsForDrawing.Remove(harpoon);
            }
            foreach (CoastGuardPersonel c in coastGuardToDelete)
            {
                coastGuard.Remove(c);
                objectsForDrawing.Remove(c);
            }


            sun.Update(gameTime);
            moon.Update(gameTime);
            ocean.Update(gameTime);
            wind.Update(gameTime);
        }

        internal void AddHarpoon(Vector3 pos, Vector3 dir, Vector3 initialVel)
        {
            Harpoon harpoon = new Harpoon(game, pos, Vector3.UnitX, dir, initialVel, modelNameToModel[ExtremeSailingGame.ModelNames.HARPOON]);
            this.harpoons.Add(harpoon);
            objectsForDrawing.Add(harpoon);
        }

        internal void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (GameObject obj in objectsForDrawing)
            {
                obj.Draw(camera, ambientColor, sun, moon);
            }
        }
    }
}
