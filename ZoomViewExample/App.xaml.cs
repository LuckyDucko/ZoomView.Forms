using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ZoomViewExample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
			MainPage.Navigation.PushModalAsync(new MainPage());
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
