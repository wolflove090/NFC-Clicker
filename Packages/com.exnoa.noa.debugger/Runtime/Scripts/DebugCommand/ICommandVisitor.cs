namespace NoaDebugger.DebugCommand
{
    interface ICommandVisitor
    {
        void Visit(GetOnlyPropertyCommand command);
        void Visit(StringPropertyCommand command);
        void Visit(ShortPropertyCommand command);
        void Visit(UShortPropertyCommand command);
        void Visit(IntPropertyCommand command);
        void Visit(UIntPropertyCommand command);
        void Visit(BytePropertyCommand command);
        void Visit(SBytePropertyCommand command);
        void Visit(LongPropertyCommand command);
        void Visit(ULongPropertyCommand command);
        void Visit(CharPropertyCommand command);
        void Visit(FloatPropertyCommand command);
        void Visit(DoublePropertyCommand command);
        void Visit(DecimalPropertyCommand command);
        void Visit(BoolPropertyCommand command);
        void Visit(EnumPropertyCommand command);
        void Visit(MethodCommand command);
        void Visit(CoroutineCommand command);
        void Visit(HandleMethodCommand command);
    }
}
