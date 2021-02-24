using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ZoomViewExample
{
	public partial class MyPage : ContentPage
	{
		public MyPage()
		{
			InitializeComponent();
			Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, false);
			NavigationPage.SetHasBackButton(this, false);
			NavigationPage.SetHasNavigationBar(this, true);
			this.BackgroundColor = Color.FromHex("F5F5F5");
		}

		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			this.griddle.AllowEasyZoomInteraction = true;
		}

		void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
		{
			this.griddle.AllowEasyZoomInteraction = false;
		}
	}
}
