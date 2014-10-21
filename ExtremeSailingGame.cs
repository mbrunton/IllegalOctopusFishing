using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Sensors;
using Windows.UI.Input;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class ExtremeSailingGame : Game
    {
        public enum ModelNames { SMALLBOAT, SMALLSAIL, HARPOON, COASTGUARD, FISH };

        private MainPage mainPage;
        private bool isPaused;

        public Player.BoatSize selectedBoat;
        public int difficulty;

        private GraphicsDeviceManager graphicsDeviceManager;
        private KeyboardState keyboardState;
        private AccelerometerReading accelerometerReading;
        private GameInput input;

        private World world;

        public ExtremeSailingGame(MainPage mainPage, Player.BoatSize selectedBoat, int difficulty)
        {
            this.mainPage = mainPage;
            this.isPaused = true;
            this.selectedBoat = selectedBoat;
            this.difficulty = difficulty;

            graphicsDeviceManager = new GraphicsDeviceManager(this);
            input = new GameInput(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Initialise event handling.
            input.gestureRecognizer.Tapped += Tapped;
            input.gestureRecognizer.ManipulationStarted += OnManipulationStarted;
            input.gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
            input.gestureRecognizer.ManipulationCompleted += OnManipulationCompleted;
        }

        protected override void LoadContent()
        {
            Dictionary<ModelNames, String> modelNameToString = new Dictionary<ModelNames, String>();
            
            modelNameToString.Add(ModelNames.SMALLBOAT, "smallboat");
            modelNameToString.Add(ModelNames.SMALLSAIL, "smallsail");
            modelNameToString.Add(ModelNames.COASTGUARD, "coastguard");
            modelNameToString.Add(ModelNames.HARPOON, "harpoon");
            modelNameToString.Add(ModelNames.FISH, "fish");
            
            // for debugging (just change one model)
            //modelNameToString.Add(ModelNames.SMALLBOAT, "smallboat");
            //modelNameToString.Add(ModelNames.SMALLSAIL, "smallboat");
            //modelNameToString.Add(ModelNames.COASTGUARD, "smallboat");
            //modelNameToString.Add(ModelNames.HARPOON, "smallboat");
            //modelNameToString.Add(ModelNames.FISH, "smallboat");
            
            Dictionary<ModelNames, Model> modelNameToModel = new Dictionary<ModelNames, Model>();
            // load blender models
            foreach (KeyValuePair<ModelNames,String> keyVal in modelNameToString)
            {
                ModelNames modelName = keyVal.Key;
                String modelString = keyVal.Value;
                Model model = Content.Load<Model>(modelString);
                modelNameToModel.Add(modelName, model);
            }

            world = new World(this, modelNameToModel, selectedBoat);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Illegal Octopus Fishing";
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (isPaused)
            {
                return;
            }
            keyboardState = input.keyboardManager.GetState();
            accelerometerReading = input.accelerometer.GetCurrentReading();
            world.Update(gameTime, keyboardState, accelerometerReading);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (isPaused)
            {
                return;
            }
            // TODO
            world.Draw(gameTime);
            base.Draw(gameTime);
        }

        internal void setIsPaused(bool isPaused)
        {
            this.isPaused = isPaused;
        }

        public void Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
            if (isPaused || world == null)
            {
                return;
            }

            world.player.Fire(world);
        }

        public void OnManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {

        }

        public void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            if (isPaused || world == null)
            {
                return;
            }
            /*
            float deltaY = (float)args.Delta.Translation.Y;
            world.player.changeSlack(deltaY);
            */
            float screenDeltaY = (float)args.Delta.Translation.Y;
            float fractionalY = screenDeltaY / (this.GraphicsDevice.BackBuffer.Height);
            float swipeScale = (float)Math.PI; // swipe through half screen height to change slack by maximum amount (pi/2)
            world.player.changeSlack(swipeScale * fractionalY);
        }

        public void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            if (isPaused || world == null)
            {
                return;
            }
        }

        internal void GameOver()
        {
            this.mainPage.GameOver();
        }

        internal void updateHUD(float playerHealth, Vector3 playerDir, Vector3 windDir, float windSpeed)
        {
            float windAngle = (float)Math.Acos(Vector3.Dot(playerDir, windDir));
            if (Vector3.Cross(playerDir, windDir).Y < 0) {
                windAngle = -1 * windAngle;
            }

            mainPage.setHealthDisplay(playerHealth);
            // TODO: move this to 2d rendering
            //mainPage.setWindDisplay(windAngle, windSpeed);
        }
    }
}
