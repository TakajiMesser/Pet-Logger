using Android.Runtime;
using Android.Support.V4.App;
using PetLogger.Droid.Utilities;
using System;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace PetLogger.Droid.Adapters
{
    public abstract class TabAdapter : FragmentPagerAdapter
    {
        private string[] _tabNames;

        public TabAdapter(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public TabAdapter(FragmentManager fragmentManager, string[] tabNames) : base(fragmentManager)
        {
            _tabNames = tabNames;
        }

        public override int Count => _tabNames.Length;

        /*public override Fragment GetItem(int position)
        {
            if (position < 0 || position >= _tabNames.Length) throw new ArgumentOutOfRangeException("Could not handle position " + position);


        }*/

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            if (position < 0 || position >= _tabNames.Length) throw new ArgumentOutOfRangeException("Could not handle position " + position);

            return _tabNames[position].ToJavaString();
        }
    }
}