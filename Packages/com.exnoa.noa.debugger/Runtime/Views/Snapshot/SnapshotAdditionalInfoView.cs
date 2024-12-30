using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class SnapshotAdditionalInfoView : ViewBase<SnapshotAdditionalInfoViewLinker>
    {
        [SerializeField] NoaDebuggerText _category;

        [SerializeField] GameObject _keyValueRoot;

        [SerializeField] CategoryKeyValue _keyValuePrefab;

        protected override void _OnShow(SnapshotAdditionalInfoViewLinker linker)
        {
            gameObject.SetActive(true);

            _category.text = linker._category;
            foreach (var categoryItem in linker._categoryItems)
            {
                var categoryKeyValue = Instantiate(_keyValuePrefab, _keyValueRoot.transform);
                categoryKeyValue.Init(categoryItem);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        protected override void _OnHide()
        {
            base._OnHide();
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            _category = default;
            _keyValueRoot = default;
            _keyValuePrefab = default;
        }
    }

    sealed class SnapshotAdditionalInfoViewLinker : ViewLinkerBase
    {
        public string _category;
        public List<NoaSnapshotCategoryItem> _categoryItems;

    }
}
