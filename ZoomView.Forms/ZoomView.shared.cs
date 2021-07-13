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

		public delegate void SetChildInputTransparentDelegate(bool userInteractionLevel);
		public SetChildInputTransparentDelegate setChildInputTransparent;

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


		public bool UserInteractionEnabled
		{
			get => (bool)GetValue(UserInteractionEnabledProperty);
			set
			{
				SetValue(UserInteractionEnabledProperty, value);
				setChildInputTransparent(value);
			}
		}

		private static void OnUserInteractionPropertyChanged(BindableObject bindable, object oldVal, object newVal)
		{
			var zoomview = bindable as ZoomView;
			bool newValResult;
			if (bool.TryParse(newVal.ToString(), out newValResult))
			{
				zoomview.Content.InputTransparent = newValResult;
			}
		}

		public static readonly BindableProperty UserInteractionEnabledProperty =
		BindableProperty.Create(propertyName: nameof(UserInteractionEnabled),
		  returnType: typeof(bool),
		  declaringType: typeof(ZoomView),
		  defaultValue: true,
		  propertyChanged: OnUserInteractionPropertyChanged
		  );

		public ZoomView()
		{
		}
	}

}
