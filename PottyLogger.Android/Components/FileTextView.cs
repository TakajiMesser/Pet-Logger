using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PottyLogger.Droid.Components
{
    [Register("com.pottylogger.android.components.FileTextView")]
    public class FileTextView : ViewGroup
    {
        //private LinearLayout _headers;
        private RecyclerView _rows;
        private TextRowAdapter _adapter;
        private int _offset = 0;
        private bool _gettingRows = false;
        private string _filePath;

        public bool LazyLoad { get; private set; }
        public int LazyLoadLimit { get; private set; }
        public ChoiceMode ChoiceMode { get; private set; }
        public float RowTextSize { get; private set; }
        public Color RowTextColor { get; private set; }
        public Typeface RowTypeface { get; private set; }
        public Drawable RowBackground { get; private set; }
        public float LineNumberTextSize { get; private set; }
        public Color LineNumberColor { get; private set; }
        public Typeface LineNumberTypeface { get; private set; }
        public Drawable LineNumberBackground { get; private set; }
        public Drawable HorizontalRowDivider { get; private set; }
        public Drawable VerticalRowDivider { get; private set; }

        //public IEnumerable<int> SelectedIDs => _adapter.SelectedIDs;

        public EventHandler<TextRowAdapter.ItemClickEventArgs> ItemClick;
        public EventHandler<TextRowAdapter.ItemLongClickEventArgs> ItemLongClick;

        public FileTextView(Context context) : base(context) { }
        public FileTextView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public FileTextView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public FileTextView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var attr = context.ObtainStyledAttributes(attrs, Resource.Styleable.FileTextView, 0, 0);

            LazyLoad = attr.GetBoolean(Resource.Styleable.FileTextView_lazyLoad, false);
            LazyLoadLimit = attr.GetInteger(Resource.Styleable.FileTextView_lazyLoadLimit, 0);
            ChoiceMode = (ChoiceMode)attrs.GetAttributeIntValue(Android.Resource.Attribute.ChoiceMode, 0);
            RowTextSize = attr.GetFloat(Resource.Styleable.FileTextView_rowTextSize, TextRowAdapter.DEFAULT_TEXT_SIZE);
            RowTextColor = attr.GetColor(Resource.Styleable.FileTextView_rowTextColor, TextRowAdapter.DEFAULT_TEXT_COLOR);

            var rowFontID = attr.GetResourceId(Resource.Styleable.FileTextView_rowFontFamily, -1);
            /*if (rowFontID != -1)
            {
                RowTypeface = (true) ? ResourcesCompat.Get(Context, rowFontID) : Context.Resources.GetFont(rowFontID);
            }*/

            RowBackground = attr.GetDrawable(Resource.Styleable.FileTextView_rowBackground);

            LineNumberTextSize = attr.GetFloat(Resource.Styleable.FileTextView_lineNumberTextSize, TextRowAdapter.DEFAULT_NUMBER_SIZE);
            LineNumberColor = attr.GetColor(Resource.Styleable.FileTextView_lineNumberTextColor, TextRowAdapter.DEFAULT_NUMBER_COLOR);

            var lineNumberFontID = attr.GetResourceId(Resource.Styleable.FileTextView_lineNumberFontFamily, -1);
            /*if (lineNumberFontID != -1)
            {
                LineNumberTypeface = (true) ? ResourcesCompat.Get(Context, lineNumberFontID) : Context.Resources.GetFont(lineNumberFontID);
            }*/

            LineNumberBackground = attr.GetDrawable(Resource.Styleable.FileTextView_lineNumberBackground);

            HorizontalRowDivider = attr.GetDrawable(Resource.Styleable.FileTextView_horizontalRowDivider)
                ?? ResourcesCompat.GetDrawable(Resources, Resource.Drawable.horizontal_divider_semi, null);
            VerticalRowDivider = attr.GetDrawable(Resource.Styleable.FileTextView_verticalRowDivider)
                ?? ResourcesCompat.GetDrawable(Resources, Resource.Drawable.vertical_divider_semi, null);

            CreateRowView();
        }

        private void CreateRowView()
        {
            _rows = new RecyclerView(Context);
            ((SimpleItemAnimator)_rows.GetItemAnimator()).SupportsChangeAnimations = false;

            var layoutManager = new LinearLayoutManager(Context);
            _rows.SetLayoutManager(layoutManager);

            var dividerDecoration = new DividerItemDecoration(Context, layoutManager.Orientation);
            dividerDecoration.SetDrawable(HorizontalRowDivider);
            _rows.AddItemDecoration(dividerDecoration);

            if (LazyLoad)
            {
                _rows.ScrollChange += (s, e) =>
                {
                    int firstVisibleItemPosition = layoutManager.FindFirstVisibleItemPosition();

                    if (!_gettingRows && firstVisibleItemPosition > 0 && _adapter.ItemCount % LazyLoadLimit == 0 && firstVisibleItemPosition + _rows.ChildCount >= _adapter.ItemCount)
                    {
                        _gettingRows = true;

                        var dialog = new ProgressDialog(Context, Resource.Style.ProgressDialogTheme)
                        {
                            Indeterminate = true
                        };
                        dialog.SetCancelable(false);
                        dialog.Show();

                        _adapter.AddRows(GetNextRowSet());

                        dialog.Dismiss();
                        _gettingRows = false;
                    }
                };
            }
        }

        public void Adapter_ItemClick(object sender, TextRowAdapter.ItemClickEventArgs e) => ItemClick?.Invoke(sender, e);

        public void Adapter_ItemLongClick(object sender, TextRowAdapter.ItemLongClickEventArgs e) => ItemLongClick?.Invoke(sender, e);

        public void SetFile(string filePath)
        {
            _filePath = filePath;
            _adapter = new TextRowAdapter(Context)
            {
                RowTextSize = RowTextSize,
                RowTextColor = RowTextColor,
                RowTypeface = RowTypeface,
                RowBackground = RowBackground,
                LineNumberTextSize = LineNumberTextSize,
                LineNumberColor = LineNumberColor,
                LineNumberTypeface = LineNumberTypeface,
                LineNumberBackground = LineNumberBackground,
                VerticalDivider = VerticalRowDivider
            };
            _adapter.ItemClick += Adapter_ItemClick;
            _adapter.ItemLongClick += Adapter_ItemLongClick;
            _adapter.AddRows(GetNextRowSet());

            _rows.SetAdapter(_adapter);
        }

        public IEnumerable<string> GetNextRowSet()
        {
            if (LazyLoad)
            {
                foreach (var row in File.ReadLines(_filePath).Skip(_offset).Take(LazyLoadLimit))
                {
                    yield return row;
                }

                _offset += LazyLoadLimit;
            }
            else
            {
                foreach (var row in File.ReadLines(_filePath))
                {
                    yield return row;
                }
            }
        }

        public void Filter(string text) => _adapter.Filter.InvokeFilter(text);

        public void SelectAllItems() => _adapter.SetAllItemsChecked(true);

        public void DeleteSelectedItems() => _adapter.DeleteSelectedItems();

        public void SetMultiChoiceModeListener(AbsListView.IMultiChoiceModeListener listener) => _adapter.SetMultiChoiceModeListener(listener);

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

            FocusableInTouchMode = true;

            AddView(_rows, GenerateDefaultLayoutParams());
            BringChildToFront(_rows);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            int width = _rows.MeasuredWidth + PaddingLeft + PaddingRight;
            int height = GetDefaultSize(SuggestedMinimumHeight, heightMeasureSpec);

            SetMeasuredDimension(width, height);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int rowLeft = PaddingLeft;
            int rowRight = rowLeft + _rows.MeasuredWidth + PaddingRight;
            int rowTop = PaddingTop;
            int rowBottom = rowTop + _rows.MeasuredHeight + PaddingBottom;

            _rows.Layout(rowLeft, rowTop, rowRight, rowBottom);
        }
    }
}
