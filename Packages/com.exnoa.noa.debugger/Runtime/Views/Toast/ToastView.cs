using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class ToastView : ViewBase<ToastViewLinker>
    {
        [SerializeField]
        TextMeshProUGUI _label;

        [SerializeField]
        Animation _animation;

        bool _playAnim;

        protected override void _Init()
        {
            Assert.IsNotNull(_label);
            Assert.IsNotNull(_animation);
        }
        
        protected override void _OnShow(ToastViewLinker linker)
        {
            _label.text = linker._label;
            gameObject.SetActive(true);
            _animation.Stop();
            _animation.Play("show_toast");
            _playAnim = true;
        }
        
        protected override void _OnHide()
        {
            gameObject.SetActive(false);
            GameObject.Destroy(gameObject);
        }

        void Update()
        {
            if (!_playAnim)
            {
                return;
            }

            if (!_animation.isPlaying)
            {
                Hide();
            }
        }
    }

    sealed class ToastViewLinker : ViewLinkerBase
    {
        public string _label;
    }
}


