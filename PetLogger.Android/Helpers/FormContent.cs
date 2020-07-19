using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace PetLogger.Droid.Helpers
{
    public enum FormContentTypes
    {
        Text,
        Toggle,
        Dropdown
    }

    public class FormContent
    {
        private View _contentView;

        public FormContent(string label, FormContentTypes contentType)
        {
            Label = label;
            ContentType = contentType;
        }

        public string Label { get; }
        public FormContentTypes ContentType { get; }

        public string GetTextValue()
        {
            if (_contentView != null)
            {
                if (ContentType == FormContentTypes.Text)
                {
                    if (_contentView is EditText editText)
                    {
                        return editText.Text;
                    }
                }
                else if (ContentType == FormContentTypes.Dropdown)
                {
                    if (_contentView is DropDownListView dropDown)
                    {
                        
                    }
                }
            }

            return null;
        }

        public bool? GetToggleValue()
        {
            if (ContentType == FormContentTypes.Toggle && _contentView is ToggleButton toggle)
            {
                return toggle.Checked;
            }

            return null;
        }

        public void AddToLayout(LinearLayout linearLayout)
        {
            var labelText = new TextView(linearLayout.Context)
            {
                Text = Label,
                TextSize = 12.0f
            };
            labelText.SetTextColor(ColorHelper.TextGray);

            _contentView = CreateContentView(linearLayout.Context);

            linearLayout.AddView(labelText);
            linearLayout.AddView(_contentView);
        }

        private View CreateContentView(Context context)
        {
            switch (ContentType)
            {
                case FormContentTypes.Text:
                    var inputText = new EditText(context);
                    inputText.SetTextColor(ColorHelper.TextGray);
                    return inputText;
                case FormContentTypes.Toggle:
                    return new ToggleButton(context);
            }

            throw new ArgumentOutOfRangeException("Could not handle ContentType " + ContentType);
        }
    }
}