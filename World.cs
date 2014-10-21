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
        internal Player player;
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
            float playerLength, playerWidth, playerHeight;
            if (selectedBoat == Player.BoatSize.SMALL)
            {
                playerLength = 10f;
                playerWidth = 4f;
                playerHeight = 5f;
            }
            else
            {
                throw new Exception("large boat not yet implemented sozzle");
            }
            Vector3 playerStartPos = terrain.getPlayerStartPos();
            Model playerBoatModel = modelNameToModel[ExtremeSailingGame.ModelNames.SMALLBOAT];
            Model playerSailModel = modelNameToModel[ExtremeSailingGame.ModelNames.SMALLSAIL];
            this.player = new Player(game, playerStartPos, playerBoatModel, playerSailModel, selectedBoat, playerLength, playerWidth, playerHeight);
            objectsForDrawing.Add(player);

            // camera
            this.camera = new Camera(game, player.pos, player.dir, player.vel);

            // fish
            //numFish = 100;
            numFish = 0;
            float fishLength = 4;
            float fishWidth = 1;
            float fishHeight = 2;
            this.fish = new List<Fish>(numFish);
            for (int i = 0; i < numFish; i++)
            {
                Vector3 fishStartPos = terrain.getRandomUnderWaterLocation();
                Fish f = new Fish(game, fishStartPos, modelNameToModel[ExtremeSailingGame.ModelNames.FISH], fishLength, fishWidth, fishHeight);
                fish.Add(f);
                objectsForDrawing.Add(f);
            }

            //coastguard
            //numCoastGuard = 20;
            numCoastGuard = 100;
            float coastGuardLength = 11f;
            float coastGuardWidth = 5f;
            float coastGuardHeight = 6f;
            this.coastGuard = new List<CoastGuardPersonel>(numCoastGuard);
            for (int i = 0; i < numCoastGuard; i++)
            {
                Vector3 coastGuardStartPos = terrain.getRandomOnWaterLocation();
                // FOR TESTING PURPOSES
                //Vector3 coastGuardStartPos = new Vector3(135, 0, 0);

                CoastGuardPersonel c = new CoastGuardPersonel(game, coastGuardStartPos, modelNameToModel[ExtremeSailingGame.ModelNames.COASTGUARD], 
                                                                game.difficulty, coastGuardLength, coastGuardWidth, coastGuardHeight);
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

            // player turning
            if (keyboardState.IsKeyDown(Keys.Left) && !keyboardState.IsKeyDown(Keys.Right))
            {
                player.turnLeft(gameTime);
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && !keyboardState.IsKeyDown(Keys.Left))
            {
                player.turnRight(gameTime);
            }
            else if (accelerometerReading != null)
            {
                float accelX = (float)accelerometerReading.AccelerationX;
                if (Math.Abs(accelX) > 0.02f)
                {
                    player.turn(-1 * accelX, gameTime);
                }
            }

            // player adjusting sail
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                player.releaseSlack(gameTime);
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                player.reduceSlack(gameTime);
            }

            // player firing
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                player.Fire(this);
            }

            // player update
            player.Update(gameTime, playerHullPositions, playerHullTerrainHeights, playerHullOceanHeights, wind, gravity);

            // camera update
            camera.Update(player.pos, player.dir, player.vel);

            // fish update
            foreach (Fish f in fish)
            {
                float fishTerrainHeight = terrain.getTerrainHeightAtPosition(f.pos.X, f.pos.Z);
                float fishOceanHeight = ocean.getOceanHeightAtPosition(f.pos.X, f.pos.Z);
                f.Update(gameTime, fishTerrainHeight, fishOceanHeight, gravity);
            }

            // coastguard update
            foreach (CoastGuardPersonel c in coastGuard)
            {
                float coastGuardTerrainHeight = terrain.getTerrainHeightAtPosition(c.pos.X, c.pos.Z);
                float coastGuardOceanHeight = ocean.getOceanHeightAtPosition(c.pos.X, c.pos.Z);
                c.Update(this, gameTime, coastGuardTerrainHeight, coastGuardOceanHeight, player.pos, player.dir);
            }

            // TODO:
            /* player eating fish
             * player/enemy crash
             */

            // harpoon updates/collision detection
            List<Harpoon> harpoonsToDelete = new List<Harpoon>();
            List<CoastGuardPersonel> coastGuardToDelete = new List<CoastGuardPersonel>();
            foreach (Harpoon harpoon in harpoons)
            {
                harpoon.Update(gameTime, gravity);
                if (harpoon.isDead)
                {
                    if (harpoon.deathCooldown == 0) {
                        harpoonsToDelete.Add(harpoon);
                    }

                    continue;
                }
                float harpoonTerrainHeight = terrain.getTerrainHeightAtPosition(harpoon.pos.X, harpoon.pos.Z);
                if (harpoonTerrainHeight > harpoon.pos.Y)
                {
                    harpoon.Kill();
                    continue;
                }
                if (harpoon.cooloff == 0)
                {
                    bool isPlayerHit = harpoon.checkIfHit(player);
                    if (isPlayerHit)
                    {
                        player.health -= harpoon.damage;
                        harpoon.Kill();
                        continue;

                    }
                    foreach (CoastGuardPersonel c in coastGuard)
                    {
                        bool isHit = harpoon.checkIfHit(c);
                        if (isHit)
                        {
                            coastGuardToDelete.Add(c);
                            harpoon.Kill();
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

            // update HUD
            game.updateHUD(player.health, player.dir, wind.dir, wind.speed);
        }

        internal void AddHarpoon(Vector3 pos, Vector3 dir, float shooterSpeed)
        {
            float harpoonLength = 4f;
            float harpoonWidth = 0.5f;
            float harpoonHeight = 0.5f;
            Harpoon harpoon = new Harpoon(game, pos, Vector3.UnitX, dir, shooterSpeed, modelNameToModel[ExtremeSailingGame.ModelNames.HARPOON], harpoonLength, harpoonWidth, harpoonHeight);
            this.harpoons.Add(harpoon);
            objectsForDrawing.Add(harpoon);
        }

        internal Color getSkyColor()
        {
            Vector3 midday = new Vector3(0, -1, 0);
            Vector3 sunDir = sun.pos;
            sunDir.Normalize();
            float cos = Vector3.Dot(midday, sunDir) / (midday.Length() * sunDir.Length());

            // cos == 1 -> midday, cos == -1 -> midnight, cos == 0 -> dawn/dusk
            cos = (cos + 1) / 2; // now in [0, 1]
            return new Color((float)Math.Min(1 / Math.Abs(cos), 0.2), (float)Math.Pow(cos, 2), (float)Math.Max(cos, 0.4));
        }

        internal void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(getSkyColor());

            foreach (GameObject obj in objectsForDrawing)
            {
                obj.Draw(camera, ambientColor, sun, moon);
            }
        }
    }
}
