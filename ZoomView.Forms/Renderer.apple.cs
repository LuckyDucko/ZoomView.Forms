﻿
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;
using ZoomView.Forms.iOS;
using Foundation;

[assembly: ExportRenderer(typeof(ZoomView.Forms.ZoomView), typeof(ZoomViewRenderer))]
namespace ZoomView.Forms.iOS
{
	/// <summary>
	/// ImageCircle Implementation
	/// </summary>
	[Preserve(AllMembers = true)]
	public class ZoomViewRenderer : ScrollViewRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			MaximumZoomScale = 10f;
			MinimumZoomScale = 0.5f;
			base.SizeToFit();
			base.Center = this.Subviews.FirstOrDefault().Center;
			base.TranslatesAutoresizingMaskIntoConstraints = true;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (Subviews.Length > 0)
			{
				ViewForZoomingInScrollView += GetViewForZooming;
			}
			else
			{
				ViewForZoomingInScrollView -= GetViewForZooming;
			}
		}

		public UIView GetViewForZooming(UIScrollView sv)
		{
			return this.Subviews.FirstOrDefault();
		}
	}
}