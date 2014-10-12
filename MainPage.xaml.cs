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
        private BoatSelection boatSelection = null;
        private Instructions instructions = null;
        private Settings settings = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void onPlayButtonClicked(object sender, RoutedEventArgs e)
        {
            boatSelection = new BoatSelection(this);
            this.Children.Add(boatSelection);
        }

        internal void removeBoatSelection()
        {
            if (boatSelection != null)
            {
                this.Children.Remove(boatSelection);
            }
        }

        private void onInstructionsButtonClicked(object sender, RoutedEventArgs e)
        {
            instructions = new Instructions(this);
            this.Children.Add(instructions);
        }

        internal void removeInstructions()
        {
            if (instructions != null)
            {
                this.Children.Remove(instructions);
            }
        }

        private void onSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            settings = new Settings(this);
            this.Children.Add(settings);
        }

        internal void removeSettings()
        {
            if (settings != null)
            {
                this.Children.Remove(settings);
            }
        }
    }
}
