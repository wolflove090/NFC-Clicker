using NoaDebugger.DebugCommand;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class DebugCommandScrollAreaView : ViewBase<DebugCommandViewLinker>
    {
        [SerializeField] CommandScroll _scroll;
        [SerializeField] GameObject _noCategoriesLabel;
        [SerializeField] GameObject _noCommandsLabel;


        void _OnValidateUI()
        {
            Assert.IsNotNull(_scroll);
            Assert.IsNotNull(_noCategoriesLabel);
            Assert.IsNotNull(_noCommandsLabel);
        }

        protected override void _Init()
        {
            _OnValidateUI();
        }

        protected override void _OnShow(DebugCommandViewLinker linker)
        {
            if (linker._categoryNames is {Length: <= 0})
            {
                _scroll.gameObject.SetActive(false);
                _noCategoriesLabel.SetActive(true);
                _noCommandsLabel.SetActive(false);
            }
            else if (linker._groups is {Length: <= 0})
            {
                _scroll.gameObject.SetActive(false);
                _noCategoriesLabel.SetActive(false);
                _noCommandsLabel.SetActive(true);
            }
            else
            {
                _scroll.gameObject.SetActive(true);
                _noCategoriesLabel.SetActive(false);
                _noCommandsLabel.SetActive(false);
                _UpdateScroll(linker);
            }
        }


        void _UpdateScroll(DebugCommandViewLinker linker)
        {
            if (linker.IsMatchUpdateTarget(CommandViewUpdateTarget.CommandPanel))
            {
                _scroll.Reset(linker._groups);
            }
            else
            {
                _scroll.UpdatePanelsStatus(linker);
            }
        }

        IEnumerator _WaitSetScrollPosition()
        {
            yield return null;

            ResetScrollPosition();
        }

        public void ResetScrollPosition()
        {
            _scroll.ResetScrollPosition();
        }


        public void RefreshCommandPanels()
        {
            _scroll.RefreshPanels();
        }

        public void AlignmentUI(bool isReverse)
        {
            Vector2 tmp = _scroll.content.pivot;
            tmp.y = isReverse ? 0 : 1;
            _scroll.content.pivot = tmp;

            GlobalCoroutine.Run(_WaitSetScrollPosition());
        }

        void OnDestroy()
        {
            _scroll.Dispose();
            _scroll = default;
            _noCategoriesLabel = default;
            _noCommandsLabel = default;
        }
    }
}
