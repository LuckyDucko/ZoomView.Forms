using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace ZoomView.Forms
{
	/// <summary>
	/// ZoomView Interface
	/// </summary>
	public class ZoomView : ScrollView, INotifyPropertyChanged
	{

		public delegate void ResetZoomScaleDelegate();
		public ResetZoomScaleDelegate ResetZoomScale;

		/// <summary>
		/// Overridden version, to reset zoom scale so errors dont arise
		/// </summary>
		public new bool InputTransparent
		{
			get => (bool)GetValue(InputTransparentProperty);
			set
			{
				ResetZoomScale();
				SetValue(InputTransparentProperty, value);
			}
		}


		public bool AllowEasyZoomInteraction
		{
			get => (bool)GetValue(AllowEasyZoomInteractionProperty);
			set
			{
				this.Content.InputTransparent = value;
			}
		}

		public static readonly BindableProperty AllowEasyZoomInteractionProperty =
		BindableProperty.Create(propertyName: nameof(AllowEasyZoomInteraction),
		  returnType: typeof(bool),
		  declaringType: typeof(ZoomView),
		  defaultValue: true);

		public ZoomView()
		{
		}
	}

}
