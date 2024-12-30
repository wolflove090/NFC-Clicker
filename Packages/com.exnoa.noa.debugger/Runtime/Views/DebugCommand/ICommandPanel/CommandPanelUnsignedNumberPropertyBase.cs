using TMPro;

namespace NoaDebugger.DebugCommand
{
    abstract class CommandPanelUnsignedNumberPropertyBase<TCommand, TNumericType>
        : CommandPanelNumericInputPropertyBase<TCommand, TNumericType>
        where TCommand : MutableNumericPropertyCommandBase<TNumericType>
        where TNumericType : struct
    {
        protected CommandPanelUnsignedNumberPropertyBase(DebugCommandPanel panel) : base(panel)
        {
            panel._inputSwipeInput.UpdateContentType(TMP_InputField.ContentType.IntegerNumber);

            panel._inputSwipeInput.UpdateValidation(ValidateUnsignedInput);
        }

        char ValidateUnsignedInput(string text, int charIndex, char addedChar)
        {
            if (!char.IsDigit(addedChar))
            {
                return '\0';
            }

            return addedChar;
        }
    }
}
