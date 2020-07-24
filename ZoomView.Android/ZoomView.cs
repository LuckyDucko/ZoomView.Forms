using Android.Views;
using Xamarin.Forms.Platform.Android;
using static Android.Views.ScaleGestureDetector;
using Xamarin.Forms;
using Android.Content;
using Android.Graphics;
using Java.Lang;
using ZoomView.Forms.Android;

[assembly: ExportRenderer(typeof(ZoomView.Forms.ZoomView), typeof(ZoomViewRenderer))]
namespace ZoomView.Forms.Android
{
	public class ZoomViewRenderer : ScrollViewRenderer, IOnScaleGestureListener
	{
		private const int PointerIndexMask = 65280;
		private const int PointerIndexShift = 8;
		private static int INVALID_POINTER_ID = 1;
		private int mActivePointerId = INVALID_POINTER_ID;

		private float mScaleFactor = 1;
		private ScaleGestureDetector mScaleDetector;
		private Matrix mScaleMatrix = new Matrix();
		private Matrix mScaleMatrixInverse = new Matrix();

		private float mPosX;
		private float mPosY;
		private Matrix mTranslateMatrix = new Matrix();
		private Matrix mTranslateMatrixInverse = new Matrix();

		private float mLastTouchX;
		private float mLastTouchY;

		private float mFocusY;
		private float mFocusX;

		private float[] mDispatchTouchEventWorkingArray = new float[2];
		private float[] mOnTouchEventWorkingArray = new float[2];

		public ZoomViewRenderer(Context context) : base(context)
		{
			mScaleDetector = new ScaleGestureDetector(context, this);
			mTranslateMatrix.SetTranslate(0, 0);
			mScaleMatrix.SetScale(1, 1);
		}

		public bool OnScale(ScaleGestureDetector detector)
		{
			mScaleFactor *= detector.ScaleFactor;
			if (detector.IsInProgress)
			{
				mFocusX = detector.FocusX;
				mFocusY = detector.FocusY;
			}
			mScaleFactor = Math.Max(0.1f, Math.Min(mScaleFactor, 5.0f));
			mScaleMatrix.SetScale(mScaleFactor, mScaleFactor, mFocusX, mFocusY);
			mScaleMatrix.Invert(mScaleMatrixInverse);
			Invalidate();
			ForceLayout();
			return true;
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


			for (int i = 0; i < childCount; i++)
			{
				var child = GetChildAt(i);
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
			for (int i = 0; i < childCount; i++)
			{
				var child = this.GetChildAt(i);
				if (child.Visibility != ViewStates.Gone)
				{
					MeasureChild(child, widthMeasureSpec, heightMeasureSpec);
				}
			}
		}

		protected override void DispatchDraw(Canvas canvas)
		{
			canvas.Save();
			canvas.Translate(mPosX, mPosY);
			canvas.Scale(mScaleFactor, mScaleFactor, mFocusX, mFocusY);
			base.DispatchDraw(canvas);
			canvas.Restore();
		}

		public override bool DispatchTouchEvent(MotionEvent ev)
		{
			mDispatchTouchEventWorkingArray[0] = ev.GetX();
			mDispatchTouchEventWorkingArray[1] = ev.GetY();

			mDispatchTouchEventWorkingArray = ScreenPointsToScaledPoints(mDispatchTouchEventWorkingArray);
			ev.SetLocation(mDispatchTouchEventWorkingArray[0], mDispatchTouchEventWorkingArray[1]);
			return base.DispatchTouchEvent(ev);
		}

		private float[] ScaledPointsToScreenPoints(float[] a)
		{
			mScaleMatrix.MapPoints(a);
			mTranslateMatrix.MapPoints(a);
			return a;
		}

		private float[] ScreenPointsToScaledPoints(float[] a)
		{
			mTranslateMatrixInverse.MapPoints(a);
			mScaleMatrixInverse.MapPoints(a);
			return a;
		}

		public override bool OnTouchEvent(MotionEvent ev)
		{
			try
			{
				mOnTouchEventWorkingArray[0] = ev.GetX();
				mOnTouchEventWorkingArray[1] = ev.GetY();

				mOnTouchEventWorkingArray = ScaledPointsToScreenPoints(mOnTouchEventWorkingArray);

				ev.SetLocation(mOnTouchEventWorkingArray[0], mOnTouchEventWorkingArray[1]);
				mScaleDetector.OnTouchEvent(ev);

				MotionEventActions action = ev.Action;
				switch (action & ev.ActionMasked)
				{
					case MotionEventActions.Down:
						{
							float x = ev.GetX();
							float y = ev.GetY();

							mLastTouchX = x;
							mLastTouchY = y;

							// Save the ID of this pointer
							mActivePointerId = ev.GetPointerId(0);
							break;
						}

					case MotionEventActions.Move:
						{
							// Find the index of the active pointer and fetch its position
							int pointerIndex = ev.FindPointerIndex(mActivePointerId);
							float x = ev.GetX(pointerIndex);
							float y = ev.GetY(pointerIndex);

							float dx = x - mLastTouchX;
							float dy = y - mLastTouchY;

							mPosX += dx;
							mPosY += dy;
							mTranslateMatrix.PreTranslate(dx, dy);
							mTranslateMatrix.Invert(mTranslateMatrixInverse);

							mLastTouchX = x;
							mLastTouchY = y;

							Invalidate();
							break;
						}

					case MotionEventActions.Up:
						{
							mActivePointerId = INVALID_POINTER_ID;
							break;
						}

					case MotionEventActions.Cancel:
						{
							mActivePointerId = INVALID_POINTER_ID;
							break;
						}

					case MotionEventActions.PointerUp:
						{
							// Extract the index of the pointer that left the touch sensor
							int pointerIndex = (ev.ActionIndex & PointerIndexMask) >> PointerIndexShift;
							int pointerId = ev.GetPointerId(pointerIndex);
							if (pointerId == mActivePointerId)
							{
								// This was our active pointer going up. Choose a new
								// active pointer and adjust accordingly.
								int newPointerIndex = pointerIndex == 0 ? 1 : 0;
								mLastTouchX = ev.GetX(newPointerIndex);
								mLastTouchY = ev.GetY(newPointerIndex);
								mActivePointerId = ev.GetPointerId(newPointerIndex);
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
