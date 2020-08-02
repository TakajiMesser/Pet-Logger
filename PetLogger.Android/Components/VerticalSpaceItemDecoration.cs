using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace PetLogger.Droid.Components
{
    public class VerticalSpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private int _height;

        public VerticalSpaceItemDecoration(int height) => _height = height;

        public bool ShouldShowBeforeFirst { get; set; }
        public bool ShouldShowAfterLast { get; set; }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            var position = parent.GetChildAdapterPosition(view);

            if (ShouldShowBeforeFirst && position == 0)
            {
                outRect.Top = _height;
            }

            if (ShouldShowAfterLast || position < parent.GetAdapter().ItemCount - 1)
            {
                outRect.Bottom = _height;
            }
        }
    }
}
