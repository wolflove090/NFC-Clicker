using TMPro;

namespace NoaDebugger.DebugCommand
{
    abstract class CommandPanelFloatingPointNumberPropertyBase<TCommand, TNumericType>
        : CommandPanelNumericInputPropertyBase<TCommand, TNumericType>
        where TCommand : MutableNumericPropertyCommandBase<TNumericType>
        where TNumericType : struct
    {
        protected CommandPanelFloatingPointNumberPropertyBase(DebugCommandPanel panel) : base(panel)
        {
            panel._inputSwipeInput.UpdateContentType(TMP_InputField.ContentType.DecimalNumber);
        }
    }
}
