#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed　class EditorCommandComponentVisitor : ICommandVisitor
    {
        public IEditorCommandComponent Result { get; private set; }

        public bool IsSuccess => Result != null;

        public void Visit(GetOnlyPropertyCommand command) => Result = new GetOnlyPropertyEditorCommandComponent(command);
        public void Visit(StringPropertyCommand command) => Result = new StringPropertyEditorCommandComponent(command);
        public void Visit(ShortPropertyCommand command) => Result = new ShortPropertyEditorCommandComponent(command);
        public void Visit(UShortPropertyCommand command) => Result = new UShortPropertyEditorCommandComponent(command);
        public void Visit(IntPropertyCommand command) => Result = new IntPropertyEditorCommandComponent(command);
        public void Visit(UIntPropertyCommand command) => Result = new UIntPropertyEditorCommandComponent(command);
        public void Visit(BytePropertyCommand command) => Result = new BytePropertyEditorCommandComponent(command);
        public void Visit(SBytePropertyCommand command) => Result = new SBytePropertyEditorCommandComponent(command);
        public void Visit(LongPropertyCommand command) => Result = new LongPropertyEditorCommandComponent(command);
        public void Visit(ULongPropertyCommand command) => Result = new ULongPropertyEditorCommandComponent(command);
        public void Visit(CharPropertyCommand command) => Result = new CharPropertyEditorCommandComponent(command);
        public void Visit(FloatPropertyCommand command) => Result = new FloatPropertyEditorCommandComponent(command);
        public void Visit(DoublePropertyCommand command) => Result = new DoublePropertyEditorCommandComponent(command);
        public void Visit(DecimalPropertyCommand command) => Result = new DecimalPropertyEditorCommandComponent(command);
        public void Visit(BoolPropertyCommand command) => Result = new BoolPropertyEditorCommandComponent(command);
        public void Visit(EnumPropertyCommand command) => Result = new EnumPropertyEditorCommandComponent(command);
        public void Visit(MethodCommand command) => Result = new MethodEditorCommandComponent(command);
        public void Visit(CoroutineCommand command) => Result = new CoroutineEditorCommandComponent(command);
        public void Visit(HandleMethodCommand command) => Result = new HandleMethodEditorCommandComponent(command);
    }
}
#endif
