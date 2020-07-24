
using System.ComponentModel;

using Xamarin.Forms;

namespace ZoomView.Forms
{
	public class ZoomView : ScrollView, INotifyPropertyChanged
	{
		public static float MaxZoomScale { get; set; }
		public static float MinZoomScale { get; set; }

		public float ZoomScale
		{
			get => (float)GetValue(ZoomScaleProperty);
			set => SetValue(ZoomScaleProperty, value);
		}

		public static readonly BindableProperty ZoomScaleProperty = BindableProperty.Create(nameof(ZoomScale), typeof(float), typeof(ZoomView), 1f, propertyChanged: (bindable, oldValue, newValue) =>
		{
			BoundsRestrictedZoomScaling(bindable, newValue);
		});

		private static void BoundsRestrictedZoomScaling(BindableObject bindable, object newValue)
		{
			if ((float)newValue > MaxZoomScale)
			{
				((ZoomView)bindable).ZoomScale = MaxZoomScale;
			}
			else if ((float)newValue < MinZoomScale)
			{
				((ZoomView)bindable).ZoomScale = MinZoomScale;
			}
			else
			{
				((ZoomView)bindable).ZoomScale = (float)newValue;
			}
		}

		public ZoomView()
		{
			MaxZoomScale = 10f;
			MinZoomScale = 0.5f;
		}
	}
}
