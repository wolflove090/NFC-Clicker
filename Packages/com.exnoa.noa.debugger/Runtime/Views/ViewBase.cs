using System;
using UnityEngine;

namespace NoaDebugger
{
    class ViewBase<T> : MonoBehaviour where T : ViewLinkerBase
    {
        const string DESTROY_TARGET_VIEW = "FloatingWindow";
        
        void Awake()
        {
            try
            {
                _Init();
            }
            catch (Exception e)
            {
                _CaughtThrow(e);
            }
        }

        void Start ()
        {
            try
            {
                _OnStart();
            }
            catch (Exception e)
            {
                _CaughtThrow(e);
            }
        }

        protected virtual void _Init() { }

        protected virtual void _OnStart() { }
        
        public void Show(T linker)
        {
            try
            {
                _OnShow(linker);
            }
            catch (Exception e)
            {
                _CaughtThrow(e);
            }
        }

        protected virtual void _OnShow(T linker){}

        public void Hide()
        {
            try
            {
                _OnHide();
            }
            catch (Exception e)
            {
                _CaughtThrow(e);
            }
        }

        protected virtual void _OnHide() {}

        void _CaughtThrow(Exception e)
        {
            LogModel.CollectNoaDebuggerErrorLog(e.Message, e.StackTrace);

            if (e.StackTrace.Contains(DESTROY_TARGET_VIEW))
            {
                GameObject.Destroy(gameObject);
            }
            NoaDebuggerManager.DetectError();
            throw new Exception(e.Message, e);
        }
    }

    class ViewLinkerBase
    {

    }
}
