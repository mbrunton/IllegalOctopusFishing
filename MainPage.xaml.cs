using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IllegalOctopusFishing
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private MainMenu mainMenu;
        private ExtremeSailingGame game;

        public MainPage()
        {
            this.InitializeComponent();
            AddMainMenu();
        }

        private void AddMainMenu(bool displayGameOver = false)
        {
            this.mainMenu = new MainMenu(this, displayGameOver);
            this.Children.Add(mainMenu);
        }

        internal void startGame(Player.BoatSize selectedBoat, int difficulty)
        {
            this.Children.Remove(mainMenu);
            game = new ExtremeSailingGame(this, selectedBoat, difficulty);
            game.Run(this);
            game.setIsPaused(false);
        }

        internal void GameOver()
        {
            game.Exit();
            AddMainMenu(displayGameOver: true);
        }
    }
}
