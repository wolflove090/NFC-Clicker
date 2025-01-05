using TMPro;
using UnityEngine;

namespace NoaDebugger.DebugCommand
{
    abstract class CommandPanelSignedNumberPropertyBase<TCommand, TNumericType>
        : CommandPanelNumericInputPropertyBase<TCommand, TNumericType>
        where TCommand : MutableNumericPropertyCommandBase<TNumericType>
        where TNumericType : struct
    {
        protected CommandPanelSignedNumberPropertyBase(DebugCommandPanel panel) : base(panel)
        {
            panel._inputSwipeInput.UpdateContentType(
                TMP_InputField.ContentType.Custom,
                keyboardType: TouchScreenKeyboardType.NumbersAndPunctuation,
                characterValidation: TMP_InputField.CharacterValidation.Integer);
        }
    }
}
