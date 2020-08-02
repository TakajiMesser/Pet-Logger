using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Fragments
{
    public class PetsFragment : Fragment
    {
        public static PetsFragment Instantiate() => new PetsFragment();

        private PetTabAdapter _tabAdapter;
        private ViewPager _viewPager;
        private TabLayout _tabLayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_pets, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Pets");
            ToolbarHelper.HideToolbarBackButton(Activity);

            _viewPager = view.FindViewById<ViewPager>(Resource.Id.pet_pager);
            _tabLayout = view.FindViewById<TabLayout>(Resource.Id.pet_tabs);

            var pets = GetPets().ToList();
            if (pets.Count > 0)
            {
                SetUpTabLayout(pets);
            }
            else
            {
                // TODO - Handle scenario where there are no pets
            }

            var fabAddPet = view.FindViewById<FloatingActionButton>(Resource.Id.fab_add_pet);
            fabAddPet.Click += (s, args) => FragmentHelper.Push(Activity, AddEntityFragment<Pet>.Instantiate("Pet"));
        }

        private void SetUpTabLayout(IList<Pet> pets)
        {
            _tabAdapter = new PetTabAdapter(ChildFragmentManager, pets);
            _viewPager.Adapter = _tabAdapter;

            _tabLayout.SetupWithViewPager(_viewPager);
            _tabLayout.TabSelected += (s, args) => ToolbarHelper.ShowToolbar(Activity, args.Tab.Text);

            ToolbarHelper.ShowToolbar(Activity, _tabAdapter.GetPageTitle(0));
        }

        // TODO - Order pet tabs
        private IEnumerable<Pet> GetPets() => DBTable.GetAll<Pet>();
    }
}
