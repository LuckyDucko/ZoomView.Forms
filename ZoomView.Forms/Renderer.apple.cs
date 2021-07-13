
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;
using ZoomView.Forms.iOS;
using Foundation;
using CoreGraphics;
using CoreAnimation;
using ObjCRuntime;
using System.Diagnostics;
using StoreKit;

[assembly: ExportRenderer(typeof(ZoomView.Forms.ZoomView), typeof(ZoomViewRenderer))]
namespace ZoomView.Forms.iOS
{

	[Preserve(AllMembers = true)]
	public class ZoomViewRenderer : ScrollViewRenderer
	{
		public ZoomViewRenderer()
		{
			this.DidZoom += ZoomAdjustmentToCenter;
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			MaximumZoomScale = 10f;
			MinimumZoomScale = 0.5f;
			EnsureResetZoomScaleIsConnected(ref e);
			EnsureSetChildInputTransparentIsConnected(ref e);
			base.OnElementChanged(e);
			base.SetNeedsDisplay();
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

		private void ZoomAdjustmentToCenter(object sender, EventArgs e)
		{
			nfloat offsetX = new nfloat(Math.Max((this.Bounds.Size.Width - this.ContentSize.Width) * 0.5, 0.0));
			nfloat offsetY = new nfloat(Math.Max((this.Bounds.Size.Height - this.ContentSize.Height) * 0.5, 0.0));
			this.ContentInset = new UIEdgeInsets(offsetY, offsetX, 0, 0);
		}

		private void EnsureSetChildInputTransparentIsConnected(ref VisualElementChangedEventArgs e)
		{
			var zoomViewElement = (ZoomView)e.NewElement;
			ChildInputTransparent(zoomViewElement.UserInteractionEnabled);
			if (e.NewElement != null)
			{
				zoomViewElement.setChildInputTransparent += ChildInputTransparent;
			}
			if (e.OldElement != null)
			{
				zoomViewElement.setChildInputTransparent -= ChildInputTransparent;
			}
		}

		private void EnsureResetZoomScaleIsConnected(ref VisualElementChangedEventArgs e)
		{

			if (e.NewElement != null)
			{
				var zoomViewElement = (ZoomView)e.NewElement;
				zoomViewElement.ResetZoomScale += ResetZoomScale;
			}
			if (e.OldElement != null)
			{
				var zoomViewElement = (ZoomView)e.OldElement;
				zoomViewElement.ResetZoomScale -= ResetZoomScale;
			}
		}

		void ChildInputTransparent(bool userInteractionLevel)
		{
			foreach (var childView in this.Subviews)
			{
				childView.UserInteractionEnabled = userInteractionLevel;
			}
		}

		void ResetZoomScale()
		{
			SetZoomScale(1, false);
		}
	}
}
