using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Sensors;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class IllegalOctopusFishingGame : Game
    {
        private MainPage mainPage;
        private bool isPaused;

        private GraphicsDeviceManager graphicsDeviceManager;
        private KeyboardManager keyboardManager;
        private KeyboardState keyboardState;
        private Accelerometer accelerometer;
        private AccelerometerReading accelerometerReading;

        public IllegalOctopusFishingGame(MainPage mainPage)
        {
            this.mainPage = mainPage;
            this.isPaused = true;

            graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        protected override void LoadContent()
        {
            // TODO
            base.LoadContent();
        }

        protected override void Initialize()
        {
            // TODO
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (isPaused)
            {
                return;
            }
            keyboardState = keyboardManager.GetState();
            accelerometerReading = accelerometer.GetCurrentReading();

            // TODO
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (isPaused)
            {
                return;
            }
            // TODO
            base.Draw(gameTime);
        }

        internal void setIsPaused(bool isPaused)
        {
            this.isPaused = isPaused;
        }
    }
}
