
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System;
using Android.Content;
using static Android.Views.ScaleGestureDetector;
using ZoomView.Forms.Droid;
using Android.Runtime;
using Android.Views;

[assembly: ExportRenderer(typeof(ZoomView.Forms.ZoomView), typeof(ZoomViewRenderer))]
namespace ZoomView.Forms.Droid
{
	[Preserve(AllMembers = true)]
	public class ZoomViewRenderer : ScrollViewRenderer, IOnScaleGestureListener
	{
		private const int PointerIndexMask = 65280;
		private const int PointerIndexShift = 8;
		private static int INVALID_POINTER_ID = 1;
		private int ActivePointerId = INVALID_POINTER_ID;

		private float ScaleFactor = 1;
		private ScaleGestureDetector ScaleDetector;
		private Matrix ScaleMatrix = new Matrix();
		private Matrix ScaleMatrixInverse = new Matrix();

		private float PositionOnXAxis;
		private float PositionOnYAxis;
		private Matrix TranslateMatrix = new Matrix();
		private Matrix TranslateMatrixInverse = new Matrix();

		private float PositionOfLastTouchOnXAxis;
		private float PositionOfLastTouchOnYAxis;

		private float PositionOfFocalPointOnXAxis;
		private float PositionOfFocalPointOnYAxis;

		private float[] DispatchTouchEventWorkingArray = new float[2]; // 0 is X, 1 is Y
		private float[] OnTouchEventWorkingArray = new float[2]; // 0 is X, 1 is Y

		public ZoomViewRenderer(Context context) : base(context)
		{
			ScaleDetector = new ScaleGestureDetector(context, this);
			TranslateMatrix.SetTranslate(0, 0);
			ScaleMatrix.SetScale(1, 1);
		}

		public bool OnScale(ScaleGestureDetector detector)
		{
			ScaleFactor *= detector.ScaleFactor;
			SetFocalPointWhenScaleInProgress(detector);
			ScaleFactor = Math.Max(0.1f, Math.Min(ScaleFactor, 5.0f));
			ScaleMatrix.SetScale(ScaleFactor, ScaleFactor, PositionOfFocalPointOnXAxis, PositionOfFocalPointOnYAxis);
			ScaleMatrix.Invert(ScaleMatrixInverse);
			Invalidate();
			ForceLayout();
			return true;
		}

		private void SetFocalPointWhenScaleInProgress(ScaleGestureDetector detector)
		{
			if (detector.IsInProgress)
			{
				PositionOfFocalPointOnXAxis = detector.FocusX;
				PositionOfFocalPointOnYAxis = detector.FocusY;
			}
		}

		public bool OnScaleBegin(ScaleGestureDetector detector)
		{
			return true;
		}

		public void OnScaleEnd(ScaleGestureDetector detector)
		{
		}


		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			int childCount = ChildCount;
			for (int childIndex = 0; childIndex < childCount; childIndex++)
			{
				var child = GetChildAt(childIndex);
				if (child?.Visibility != ViewStates.Gone)
				{
					child.Layout(left, top, left + child.MeasuredWidth, top + child.MeasuredHeight);
				}
			}
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			int childCount = this.ChildCount;
			for (int childIndex = 0; childIndex < childCount; childIndex++)
			{
				var child = this.GetChildAt(childIndex);
				if (child.Visibility != ViewStates.Gone)
				{
					MeasureChild(child, widthMeasureSpec, heightMeasureSpec);
				}
			}
		}

		protected override void DispatchDraw(Canvas canvas)
		{
			canvas.Save();
			canvas.Translate(PositionOnXAxis, PositionOnYAxis);
			canvas.Scale(ScaleFactor, ScaleFactor, PositionOfFocalPointOnXAxis, PositionOfFocalPointOnYAxis);
			base.DispatchDraw(canvas);
			canvas.Restore();
		}

		/// <summary>
		/// This will translate our touch event into its scaled counterpart for our zoomview, and then
		/// allow the base handling to occur.
		/// </summary>
		/// <param name="ev"> the unscaled touch Event</param>
		public override bool DispatchTouchEvent(MotionEvent ev)
		{
			DispatchTouchEventWorkingArray[0] = ev.GetX(); //Set the MotionEvent X 
			DispatchTouchEventWorkingArray[1] = ev.GetY(); //And Y for transformation

			DispatchTouchEventWorkingArray = ScreenPointsToScaledPoints(DispatchTouchEventWorkingArray); // Provides the correct scaled version 
			ev.SetLocation(DispatchTouchEventWorkingArray[0], DispatchTouchEventWorkingArray[1]); //Sets the location to be where it is properly scaled to
			return base.DispatchTouchEvent(ev);
		}

		/// <summary>
		/// Returns our scaled X/Y to the what they are on the screen
		/// </summary>
		/// <param name="onTouchEventWorkingArrayWithMotionEventPositions"></param>
		/// <returns></returns>
		private float[] ScaledPointsToScreenPoints(float[] onTouchEventWorkingArrayWithMotionEventPositions)
		{
			ScaleMatrix.MapPoints(onTouchEventWorkingArrayWithMotionEventPositions); // Applies our scalematrix in place
			TranslateMatrix.MapPoints(onTouchEventWorkingArrayWithMotionEventPositions);//applies thr translation in place
			return onTouchEventWorkingArrayWithMotionEventPositions;
		}

		/// <summary>
		///  returns our screen points if they were scaled according to our translate/scale matrix'
		/// </summary>
		/// <param name="dispatchTouchEventWorkingArrayWithMotionEventPositions"></param>
		/// <returns></returns>
		private float[] ScreenPointsToScaledPoints(float[] dispatchTouchEventWorkingArrayWithMotionEventPositions)
		{
			TranslateMatrixInverse.MapPoints(dispatchTouchEventWorkingArrayWithMotionEventPositions);
			ScaleMatrixInverse.MapPoints(dispatchTouchEventWorkingArrayWithMotionEventPositions);
			return dispatchTouchEventWorkingArrayWithMotionEventPositions;
		}

		public override bool OnTouchEvent(MotionEvent ev)
		{
			try
			{
				OnTouchEventWorkingArray[0] = ev.GetX(); //Set the  MotionEvent X 
				OnTouchEventWorkingArray[1] = ev.GetY(); //And Y for transformation

				OnTouchEventWorkingArray = ScaledPointsToScreenPoints(OnTouchEventWorkingArray); //Will Transform the OnTouchEventWrkingArray In place.

				ev.SetLocation(OnTouchEventWorkingArray[0], OnTouchEventWorkingArray[1]);
				ScaleDetector.OnTouchEvent(ev);

				MotionEventActions action = ev.Action;
				switch (action & ev.ActionMasked)
				{
					case MotionEventActions.Down:
						{
							float x = ev.GetX();
							float y = ev.GetY();

							PositionOfLastTouchOnXAxis = x;
							PositionOfLastTouchOnYAxis = y;

							// Save the ID of this pointer
							ActivePointerId = ev.GetPointerId(0);
							break;
						}

					case MotionEventActions.Move:
						{
							// Find the index of the active pointer and fetch its position
							int pointerIndex = ev.FindPointerIndex(ActivePointerId);
							float x = ev.GetX(pointerIndex);
							float y = ev.GetY(pointerIndex);

							float dx = x - PositionOfLastTouchOnXAxis;
							float dy = y - PositionOfLastTouchOnYAxis;

							PositionOnXAxis += dx;
							PositionOnYAxis += dy;
							TranslateMatrix.PreTranslate(dx, dy);
							TranslateMatrix.Invert(TranslateMatrixInverse);

							PositionOfLastTouchOnXAxis = x;
							PositionOfLastTouchOnYAxis = y;

							Invalidate();
							break;
						}

					case MotionEventActions.Up:
						{
							ActivePointerId = INVALID_POINTER_ID;
							break;
						}

					case MotionEventActions.Cancel:
						{
							ActivePointerId = INVALID_POINTER_ID;
							break;
						}

					case MotionEventActions.PointerUp:
						{
							// Extract the index of the pointer that left the touch sensor
							int pointerIndex = (ev.ActionIndex & PointerIndexMask) >> PointerIndexShift;
							int pointerId = ev.GetPointerId(pointerIndex);
							if (pointerId == ActivePointerId)
							{
								// This was our active pointer going up. Choose a new
								// active pointer and adjust accordingly.
								int newPointerIndex = pointerIndex == 0 ? 1 : 0;
								PositionOfLastTouchOnXAxis = ev.GetX(newPointerIndex);
								PositionOfLastTouchOnYAxis = ev.GetY(newPointerIndex);
								ActivePointerId = ev.GetPointerId(newPointerIndex);
							}
							break;
						}
				}
				return true;
			}
			catch (Exception)
			{
				return true;
			}
		}
	}
}