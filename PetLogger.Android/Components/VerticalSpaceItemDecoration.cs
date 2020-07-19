using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace PetLogger.Droid.Components
{
    public class VerticalSpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private int _verticalSpaceHeight;

        public VerticalSpaceItemDecoration(int verticalSpaceHeight) => _verticalSpaceHeight = verticalSpaceHeight;

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            if (parent.GetChildAdapterPosition(view) != parent.GetAdapter().ItemCount - 1)
            {
                outRect.Bottom = _verticalSpaceHeight;
            }
        }
    }
}
