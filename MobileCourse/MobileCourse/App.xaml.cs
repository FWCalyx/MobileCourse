using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCourse
{
    public partial class App : Application
    {
        public static string dbPath;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }
        public App(string path)
        {
            InitializeComponent();
            dbPath = path;
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
