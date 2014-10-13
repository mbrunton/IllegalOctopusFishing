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
    public sealed partial class BoatSelection
    {
        private MainMenu mainMenu;
        private Player.BoatSize selectedBoat;
        private bool isBoatSelected = false;
        private double unselectedOpacity = 0.5f;

        public BoatSelection(MainMenu mainMenu)
        {
            this.InitializeComponent();
            this.mainMenu = mainMenu;
        }

        private void onBasicBoatButtonClicked(object sender, RoutedEventArgs e)
        {
            isBoatSelected = true;
            advancedBoatButton.Opacity = unselectedOpacity;
            basicBoatButton.Opacity = 1f;
            selectedBoat = Player.BoatSize.SMALL;
        }

        private void onAdvancedBoatButtonClicked(object sender, RoutedEventArgs e)
        {
            isBoatSelected = true;
            basicBoatButton.Opacity = unselectedOpacity;
            advancedBoatButton.Opacity = 1f;
            selectedBoat = Player.BoatSize.LARGE;
        }

        private void onMainMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            mainMenu.removeBoatSelection();
        }

        private void onSetSailButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!isBoatSelected)
            {
                selectBoatPromptTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                return;
            }

            mainMenu.startGame(selectedBoat);
        }
    }
}
