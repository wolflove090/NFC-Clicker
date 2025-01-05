using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using System;
using AOT;
#endif

namespace NoaDebugger
{
    sealed class NoaDebuggerInputField : TMP_InputField
    {
        OnValidateInput _validate = null;

        bool _activateOnPointerDown = true;
        GameObject _dragTarget;

        public void SetActivateOnPointerDown(bool activate)
        {
            _activateOnPointerDown = activate;
        }

        public void SetDragTarget(GameObject dragTarget)
        {
            _dragTarget = dragTarget;
        }

        protected override void Awake()
        {
            base.Awake();

            if (_validate == null)
            {
                UseCustomValidation();
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            NoaDebuggerPasteStr(RegisterToUnityClipboard);
#endif
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (_activateOnPointerDown)
            {
                base.OnPointerDown(eventData);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == gameObject)
            {
                OnSelect(eventData);
                OnPointerClick(eventData);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (_dragTarget != null)
            {
                ExecuteEvents.ExecuteHierarchy(_dragTarget, eventData, ExecuteEvents.beginDragHandler);
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_dragTarget != null)
            {
                ExecuteEvents.ExecuteHierarchy(_dragTarget, eventData, ExecuteEvents.dragHandler);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_dragTarget != null)
            {
                ExecuteEvents.ExecuteHierarchy(_dragTarget, eventData, ExecuteEvents.endDragHandler);
            }
            else
            {
                base.OnEndDrag(eventData);
            }
        }

        public void UseCustomValidation(OnValidateInput validate = null)
        {
            _validate = validate;
            onValidateInput = _OnValidateInput;
        }

        public void UseContentType(
            ContentType newContentType,
            InputType newInputType = InputType.Standard,
            TouchScreenKeyboardType newKeyboardType = TouchScreenKeyboardType.Default,
            CharacterValidation newCharacterValidation = CharacterValidation.None)
        {
            inputType = newInputType;
            keyboardType = newKeyboardType;
            characterValidation = newCharacterValidation;

            contentType = newContentType;

            onValidateInput = null;
        }

        char _OnValidateInput(string text, int charIndex, char addedChar)
        {
            return _validate?.Invoke(text, charIndex, addedChar) ?? addedChar;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void NoaDebuggerPasteStr(Action<string> pasteAction);

        static string _jslibText = "";

        [MonoPInvokeCallback(typeof(Action<string>))]
        static void RegisterToUnityClipboard(string pasteValue)
        {
            if(pasteValue != _jslibText){
                _jslibText = pasteValue;
                GUIUtility.systemCopyBuffer = _jslibText;
            }
        }
#endif
    }
}
