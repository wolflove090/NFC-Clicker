using System.Collections.Generic;
using System.Linq;

namespace NoaDebugger
{
    /// <summary>
    /// Category for the snapshot
    /// </summary>
    public class NoaSnapshotCategory
    {
        List<NoaSnapshotCategoryItem> _categoryItems;

        public List<NoaSnapshotCategoryItem> CategoryItems
        {
            get { return _categoryItems; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NoaSnapshotCategory()
        {
            _categoryItems = new List<NoaSnapshotCategoryItem>();
        }

        /// <summary>
        /// Adds an element to include in the category
        /// </summary>
        /// <param name="item">Element of the category</param>
        public void Add(NoaSnapshotCategoryItem item)
        {
            NoaSnapshotCategoryItem categoryItem =
                _categoryItems.FirstOrDefault(categoryItem => categoryItem.Key == item.Key);

            if (categoryItem != null)
            {
                _categoryItems.Remove(categoryItem);
            }

            _categoryItems.Add(item);
        }
    }
}
