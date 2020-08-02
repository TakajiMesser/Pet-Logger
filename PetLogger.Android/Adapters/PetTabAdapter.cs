using Android.Runtime;
using Android.Support.V4.App;
using PetLogger.Droid.Fragments;
using PetLogger.Droid.Utilities;
using PetLogger.Shared.Data;
using System.Collections.Generic;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace PetLogger.Droid.Adapters
{
    public class PetTabAdapter : FragmentPagerAdapter
    {
        private IList<Pet> _pets;

        public PetTabAdapter(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public PetTabAdapter(FragmentManager fragmentManager, IList<Pet> pets) : base(fragmentManager) => _pets = pets;

        public override int Count => _pets.Count;

        public override Fragment GetItem(int position) => PetFragment.Instantiate(_pets[position]);

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) => _pets[position].Name.ToJavaString();

        public string GetTabTitle(int position) => _pets[position].Name;
    }
}