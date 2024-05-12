using Microsoft.Maui.Controls;

namespace BTGOpcoes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            var window = new Window(new AppShell())
            {
                Width = 1000,
                Height = 800,
            };

            window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
            window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;

            return window;

        }
        
    }
}
