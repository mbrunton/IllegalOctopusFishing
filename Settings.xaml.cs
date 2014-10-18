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
    public sealed partial class Settings : Page
    {
        private MainMenu mainMenu;

        public Settings(MainMenu mainMenu, int difficulty, int minDifficulty, int maxDifficulty)
        {
            this.InitializeComponent();
            this.mainMenu = mainMenu;

            this.difficultySlider.Value = difficulty;
            difficultySlider.Minimum = minDifficulty;
            difficultySlider.Maximum = maxDifficulty;
        }

        private void onMainMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            int difficulty = (int)difficultySlider.Value;
            mainMenu.removeSettings(difficulty);
        }

        private void onDifficultySliderChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }
    }
}
