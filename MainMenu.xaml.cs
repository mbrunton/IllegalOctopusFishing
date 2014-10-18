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
    public sealed partial class MainMenu : Page
    {
        private MainPage mainPage;
        private BoatSelection boatSelection = null;
        private Instructions instructions = null;
        private Settings settings = null;
        private int difficulty;
        private int defaultDifficulty;
        private int minDifficulty, maxDifficulty;

        public MainMenu(MainPage mainPage, bool displayGameOver)
        {
            this.InitializeComponent();
            this.mainPage = mainPage;
            if (displayGameOver)
            {
                this.gameOverTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                this.gameOverTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed; 
            }

            this.defaultDifficulty = 3;
            this.minDifficulty = 1;
            this.maxDifficulty = 5;
            this.difficulty = defaultDifficulty;
        }

        private void onInstructionsButtonClicked(object sender, RoutedEventArgs e)
        {
            instructions = new Instructions(this);
            mainPage.Children.Add(instructions);
        }

        internal void removeInstructions()
        {
            if (instructions != null)
            {
                mainPage.Children.Remove(instructions);
            }
        }

        private void onPlayButtonClicked(object sender, RoutedEventArgs e)
        {
            boatSelection = new BoatSelection(this);
            mainPage.Children.Add(boatSelection);
        }

        internal void removeBoatSelection()
        {
            if (boatSelection != null)
            {
                mainPage.Children.Remove(boatSelection);
            }
        }

        private void onSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            settings = new Settings(this, difficulty, minDifficulty, maxDifficulty);
            mainPage.Children.Add(settings);
        }

        internal void removeSettings(int difficulty)
        {
            if (difficulty < minDifficulty || difficulty > maxDifficulty)
            {
                throw new Exception("difficulty outside of expected range");
            }

            this.difficulty = difficulty;
            if (settings != null)
            {
                mainPage.Children.Remove(settings);
            }
        }

        internal void startGame(Player.BoatSize selectedBoat)
        {
            mainPage.Children.Remove(boatSelection);
            mainPage.startGame(selectedBoat, difficulty);
        }
    }
}
