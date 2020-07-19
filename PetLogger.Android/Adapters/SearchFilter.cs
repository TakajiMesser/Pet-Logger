using Android.Widget;
using PetLogger.Droid.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace PetLogger.Droid.Adapters
{
    public abstract class SearchFilter<T> : Filter
    {
        private IList<T> _originalItems;

        protected abstract IList<T> GetItemsToFilter();
        protected abstract string GetSearchStringFromItem(T item);
        protected abstract void SetItemsFromResults(List<T> results);

        protected override FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        {
            if (_originalItems == null)
            {
                _originalItems = GetItemsToFilter();
            }

            var filterResults = new FilterResults();

            if (constraint == null)
            {
                return filterResults;
            }

            var results = new List<T>();

            if (_originalItems != null && _originalItems.Count > 0)
            {
                var constraintString = constraint.ToString();
                if (constraintString != null)
                {
                    results.AddRange(PerformSearch(constraintString));
                }
            }

            filterResults.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            filterResults.Count = results.Count;

            constraint.Dispose();

            return filterResults;
        }

        protected override void PublishResults(Java.Lang.ICharSequence constraint, FilterResults results)
        {
            using (var values = results.Values)
            {
                SetItemsFromResults(values.ToArray<Java.Lang.Object>().Select(a => a.ToNetObject<T>()).ToList());
            }

            constraint.Dispose();
            results.Dispose();
        }

        private IEnumerable<T> PerformSearch(string constraint)
        {
            return _originalItems.Where(r => GetSearchStringFromItem(r).ToLower().Contains(constraint.ToLower()));

            /*foreach (var item in _originalItems)
            {
                var itemWords = GetSearchStringFromItem(item).ToLower().Split(' ');
                var constraintWords = constraint.ToLower().Split(' ');

                foreach (var word in itemWords)
                {
                    if (word.Contains(constraint))
                    {
                        yield return item;
                    }
                }
            }*/
        }
    }
}