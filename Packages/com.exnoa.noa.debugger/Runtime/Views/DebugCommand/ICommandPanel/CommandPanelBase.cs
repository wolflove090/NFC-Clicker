using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    abstract class CommandPanelBase<TCommand> : ICommandPanel where TCommand : CommandBase
    {
        protected readonly DebugCommandPanel _panel;
        protected TCommand _command;

        protected readonly TextAlignmentOptions _alignmentOptionCenter = TextAlignmentOptions.Midline;
        protected readonly TextAlignmentOptions _alignmentOptionLeft = TextAlignmentOptions.MidlineLeft;

        protected CommandPanelBase(DebugCommandPanel panel)
        {
            _panel = panel;
            UpdateData(panel._command);
        }

        public virtual void OnUpdateWidth(float maxContentWidth) { }

        public void UpdateData(ICommand command)
        {
            _command = command as TCommand;
        }

        public virtual void Refresh() { }

        protected TextAlignmentOptions GetDisplayNameAlignmentFromDisplayFormat()
        {
            CommandDisplayFormat format = DebugCommandPresenter.GetCurrentFormat();
            return format == CommandDisplayFormat.Panel
                ? _alignmentOptionCenter
                : _alignmentOptionLeft;
        }

        protected void SetPreferredWidthFromText(LayoutElement targetLayout, float horizontalPadding, TMP_Text textComponent, float maxWidth)
        {
            if (targetLayout == null)
            {
                throw new Exception("LayoutElement could not be find");
            }

            float width = textComponent.preferredWidth + horizontalPadding;
            if (width >= maxWidth)
            {
                width = maxWidth;
            }

            targetLayout.preferredWidth = width;
        }
    }
}
