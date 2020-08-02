using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.HorizontalDividerItemDecoration")]
    public class HorizontalDividerItemDecoration : DividerItemDecoration
    {
        private Drawable _dividerDrawable;
        private int _drawableHeight;

        public HorizontalDividerItemDecoration(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public HorizontalDividerItemDecoration(Context context, int orientation, int resourceID) : base(context, orientation)
        {
            _dividerDrawable = ResourcesCompat.GetDrawable(context.Resources, resourceID, null);
            _drawableHeight = _dividerDrawable.IntrinsicHeight;

            SetDrawable(_dividerDrawable);
        }

        public bool ShouldShowBeforeFirst { get; set; }
        public bool ShouldShowAfterLast { get; set; }

        public int PaddingLeft { get; set; }
        public int PaddingTop { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingBottom { get; set; }

        public int IndexOffset { get; set; }

        public void SetPadding(int left, int top, int right, int bottom)
        {
            PaddingLeft = left;
            PaddingTop = top;
            PaddingRight = right;
            PaddingBottom = bottom;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            var position = parent.GetChildAdapterPosition(view);

            if (position == IndexOffset && ShouldShowBeforeFirst)
            {
                outRect.Top = _drawableHeight;
            }
            
            if (position >= IndexOffset)
            {
                outRect.Bottom = _drawableHeight;
            }
        }

        public override void OnDraw(Canvas canvas, RecyclerView parent, RecyclerView.State state)
        {
            var dividerLeft = parent.PaddingLeft + PaddingLeft;
            var dividerRight = parent.Width - parent.PaddingRight + PaddingRight;

            for (var i = 0; i < parent.ChildCount; i++)
            {
                var child = parent.GetChildAt(i);
                var layoutParams = (RecyclerView.LayoutParams)child.LayoutParameters;

                var index = i - IndexOffset;

                if (index >= 0)
                {
                    if (index == 0 && ShouldShowBeforeFirst)
                    {
                        var dividerTop = child.Top + layoutParams.TopMargin - _drawableHeight;
                        var dividerBottom = dividerTop + _drawableHeight;

                        _dividerDrawable.SetBounds(dividerLeft, dividerTop, dividerRight, dividerBottom);
                        _dividerDrawable.Draw(canvas);
                    }

                    if (index < parent.ChildCount - 1 || ShouldShowAfterLast)
                    {
                        var dividerTop = child.Bottom + layoutParams.BottomMargin;
                        var dividerBottom = dividerTop + _drawableHeight;

                        _dividerDrawable.SetBounds(dividerLeft, dividerTop, dividerRight, dividerBottom);
                        _dividerDrawable.Draw(canvas);
                    }
                }
            }
        }
    }
}
