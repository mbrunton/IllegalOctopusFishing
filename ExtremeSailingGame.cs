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
        private MainPage mainPage;
        private bool isPaused;

        public static List<String> modelNames = new List<String>() {"smallboat", "smallsail", "harpoon", "coastguard"};
        public Dictionary<String, Model> nameToModel;
        public Player.BoatSize selectedBoat;

        private GraphicsDeviceManager graphicsDeviceManager;
        private KeyboardState keyboardState;
        private AccelerometerReading accelerometerReading;
        private GameInput input;

        private World world;

        public ExtremeSailingGame(MainPage mainPage, Player.BoatSize selectedBoat)
        {
            this.mainPage = mainPage;
            this.isPaused = true;
            this.selectedBoat = selectedBoat;

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
            // load blender models
            nameToModel = new Dictionary<String, Model>();
            // TODO: these are getting AssetNotFoundExceptions
            
            foreach (String name in modelNames)
            {
                Model model = Content.Load<Model>(name);
                nameToModel.Add(name, model);
            }

            world = new World(this, selectedBoat);

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
            //accelerometerReading = input.accelerometer.GetCurrentReading();
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
            
        }

        public void OnManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {

        }

        public void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            
        }

        public void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {

        }

        internal void GameOver()
        {
            this.mainPage.GameOver();
        }
    }
}
