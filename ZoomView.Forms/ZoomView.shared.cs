using System;
using System.ComponentModel;

using Xamarin.Forms;

namespace ZoomView.Forms
{
	/// <summary>
	/// ZoomView Interface
	/// </summary>
	public class ZoomView : ScrollView, INotifyPropertyChanged
	{

		/// <summary>
		/// Max Scale of ZoomView
		/// </summary>
		public float MaxZoomScale
		{
			get => (float)GetValue(MaxZoomScaleProperty);
			set => SetValue(MaxZoomScaleProperty, value);
		}

		/// <summary>
		/// Minimum Scale of ZoomView
		/// </summary>
		public float MinimumZoomScale
		{
			get => (float)GetValue(MinimumZoomScaleProperty);
			set => SetValue(MinimumZoomScaleProperty, value);
		}


		/// <summary>
		/// Current Scale of ZoomView
		/// </summary>
		public float CurrentZoomScale
		{
			get => (float)GetValue(CurrentZoomScaleProperty);
			set => SetValue(CurrentZoomScaleProperty, BoundsRestrictedZoomScaling(value));
		}


		private float BoundsRestrictedZoomScaling(float newZoomScale)
		{
			if (newZoomScale > MaxZoomScale)
			{
				return MaxZoomScale;
			}
			else if (newZoomScale < MinimumZoomScale)
			{
				return MinimumZoomScale;
			}
			else
			{
				return newZoomScale;
			}
		}

		/// <summary>
		/// Max Scale Property of ZoomView 
		/// </summary>
		public static readonly BindableProperty MaxZoomScaleProperty =
		  BindableProperty.Create(propertyName: nameof(MaxZoomScale),
			  returnType: typeof(float),
			  declaringType: typeof(ZoomView),
			  defaultValue: 10F);



		/// <summary>
		/// Minimum Scale Property of ZoomView 
		/// </summary>
		public static readonly BindableProperty MinimumZoomScaleProperty =
			BindableProperty.Create(propertyName: nameof(MinimumZoomScale),
			  returnType: typeof(float),
			  declaringType: typeof(ZoomView),
			  defaultValue: 0.5F);



		/// <summary>
		/// Current Scale property of ZoomView
		/// </summary>
		public static readonly BindableProperty CurrentZoomScaleProperty =
			BindableProperty.Create(propertyName: nameof(CurrentZoomScale),
			  returnType: typeof(float),
			  declaringType: typeof(ZoomView),
			  defaultValue: 1F);



	}
}
