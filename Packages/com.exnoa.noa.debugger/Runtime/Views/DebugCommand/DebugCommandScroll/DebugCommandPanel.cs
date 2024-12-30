using System;
using NoaDebugger.DebugCommand;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class DebugCommandPanel : MonoBehaviour
    {
        public ICommand _command;

        [Header("Root")]
        public GameObject _actions;
        public LayoutGroup _nameLayout;

        [Header("DisplayName")]
        public TextMeshProUGUI _displayName;

        [Header("Actions")]
        public Button _button;
        public ToggleButtonBase _toggleButton;
        public NoaDebuggerScrollableInputComponent _inputField;
        public TMP_Dropdown _dropdown;
        public NumericCommandDragSlider _swipe;
        public NoaDebuggerScrollableInputComponent _inputSwipeInput;
        public LongPressButton _leftArrowButton;
        public LongPressButton _rightArrowButton;
        public NoaDebuggerText _valueText;
        public GameObject _grayOut;
        public ToggleButtonBase _detailToggleButton;

        [Serializable]
        struct BackgroundColors
        {
            public Color interactable;
            public Color nonInteractable;
        }

        [SerializeField]
        Image _background;
        [SerializeField]
        BackgroundColors _backgroundColors;

        [Header("ActionRoot")]
        public GameObject _toggle;
        public GameObject _inputRoot;
        public GameObject _inputSwipeRoot;
        public GameObject _valueRoot;

        ICommandPanel _commandPanel;
        UnityAction<ICommand, bool> _onClickDetail;

        public CommandScroll ParentScroll { get; private set; }


        void Awake()
        {
            Assert.IsNotNull(_actions);
            Assert.IsNotNull(_nameLayout);
            Assert.IsNotNull(_displayName);
            Assert.IsNotNull(_button);
            Assert.IsNotNull(_toggleButton);
            Assert.IsNotNull(_inputField);
            Assert.IsNotNull(_dropdown);
            Assert.IsNotNull(_swipe);
            Assert.IsNotNull(_inputSwipeInput);
            Assert.IsNotNull(_leftArrowButton);
            Assert.IsNotNull(_rightArrowButton);
            Assert.IsNotNull(_valueText);
            Assert.IsNotNull(_grayOut);
            Assert.IsNotNull(_detailToggleButton);
            Assert.IsNotNull(_background);
            Assert.IsNotNull(_toggle);
            Assert.IsNotNull(_inputRoot);
            Assert.IsNotNull(_inputSwipeRoot);
            Assert.IsNotNull(_valueRoot);

            _detailToggleButton.gameObject.SetActive(false);
            _detailToggleButton._onClick.AddListener(_OnClickDetailToggle);
        }

        public void Init(ICommand command, float maxContentWidth, DetailToggleInfo detailToggleInfo, CommandScroll scrollRect)
        {
            _ResetActionActive();
            _command = command;
            ParentScroll = scrollRect;
            _displayName.text = command.DisplayName;
            _SetInteractable(command.IsInteractable);
            _commandPanel = _CreateCommandPanel(command, this, maxContentWidth);
            _onClickDetail = detailToggleInfo._onSelectCommand;
            _SetDetailInfo(command, detailToggleInfo);
        }

        void _ResetActionActive()
        {
            _nameLayout.childAlignment = TextAnchor.MiddleCenter;

            _actions.gameObject.SetActive(false);

            _toggle.SetActive(false);
            _button.gameObject.SetActive(false);
            _toggleButton.gameObject.SetActive(false);
            _inputRoot.gameObject.SetActive(false);
            _dropdown.gameObject.SetActive(false);
            _swipe.gameObject.SetActive(false);
            _inputSwipeRoot.gameObject.SetActive(false);
            _inputSwipeInput.gameObject.SetActive(false);
            _valueRoot.gameObject.SetActive(false);
            _SetInteractable(true);
        }

        void _SetInteractable(bool interactable)
        {
            _grayOut.SetActive(!interactable);
            _background.color = interactable ? _backgroundColors.interactable : _backgroundColors.nonInteractable;
        }

        void _SetDetailInfo(ICommand command, DetailToggleInfo detailToggleInfo)
        {
            _detailToggleButton.gameObject.SetActive(detailToggleInfo._isDetailMode);
            _detailToggleButton.Init(detailToggleInfo.IsShowDetail(command.GroupName, command.DisplayName));
        }


        public void OnUpdateWidth(float maxContentWidth) => _commandPanel.OnUpdateWidth(maxContentWidth);

        public void Refresh() => _commandPanel.Refresh();

        public void UpdateData(ICommand command, DetailToggleInfo detailToggleInfo)
        {
            _commandPanel.UpdateData(command);
            _SetDetailInfo(command, detailToggleInfo);
            _commandPanel.Refresh();
        }


        void _OnClickDetailToggle(bool isOn)
        {
            _onClickDetail?.Invoke(_command, isOn);
        }


        static ICommandPanel _CreateCommandPanel(ICommand type, DebugCommandPanel panel, float maxContentWidth)
        {
            CommandPanelVisitor visitor = new CommandPanelVisitor(panel, maxContentWidth);
            type.Accept(visitor);

            if (!visitor.IsSuccess)
            {
                throw new Exception($"Unsupported type. ==> {type}");
            }

            return visitor.Result;
        }

        void OnDestroy()
        {
            _command = default;
            _actions = default;
            _nameLayout = default;
            _displayName = default;
            _button = default;
            _toggleButton = default;
            _inputField = default;
            _dropdown = default;
            _swipe = default;
            _inputSwipeInput = default;
            _leftArrowButton = default;
            _rightArrowButton = default;
            _valueText = default;
            _grayOut = default;
            _detailToggleButton = default;
            _background = default;
            _toggle = default;
            _inputRoot = default;
            _inputSwipeRoot = default;
            _valueRoot = default;
            _commandPanel = default;
        }

        class CommandPanelVisitor : ICommandVisitor
        {
            readonly DebugCommandPanel _panel;
            readonly float _maxContentWidth;

            public ICommandPanel Result { get; private set; }

            public bool IsSuccess => Result != null;

            public CommandPanelVisitor(DebugCommandPanel panel, float maxContentWidth)
            {
                _panel = panel;
                _maxContentWidth = maxContentWidth;
            }

            public void Visit(GetOnlyPropertyCommand command) => Result = new CommandPanelGetOnlyProperty(_panel, _maxContentWidth);
            public void Visit(StringPropertyCommand command) => Result = new CommandPanelStringProperty(_panel, _maxContentWidth);
            public void Visit(ShortPropertyCommand command) => Result = new CommandPanelShortProperty(_panel);
            public void Visit(UShortPropertyCommand command) => Result = new CommandPanelUShortProperty(_panel);
            public void Visit(IntPropertyCommand command) => Result = new CommandPanelIntProperty(_panel);
            public void Visit(UIntPropertyCommand command) => Result = new CommandPanelUIntProperty(_panel);
            public void Visit(BytePropertyCommand command) => Result = new CommandPanelByteProperty(_panel);
            public void Visit(SBytePropertyCommand command) => Result = new CommandPanelSByteProperty(_panel);
            public void Visit(LongPropertyCommand command) => Result = new CommandPanelLongProperty(_panel);
            public void Visit(ULongPropertyCommand command) => Result = new CommandPanelULongProperty(_panel);
            public void Visit(CharPropertyCommand command) => Result = new CommandPanelCharProperty(_panel);
            public void Visit(FloatPropertyCommand command) => Result = new CommandPanelFloatProperty(_panel);
            public void Visit(DoublePropertyCommand command) => Result = new CommandPanelDoubleProperty(_panel);
            public void Visit(DecimalPropertyCommand command) => Result = new CommandPanelDecimalProperty(_panel, _maxContentWidth);
            public void Visit(BoolPropertyCommand command) => Result = new CommandPanelBoolProperty(_panel);
            public void Visit(EnumPropertyCommand command) => Result = new CommandPanelEnumProperty(_panel, _maxContentWidth);
            public void Visit(MethodCommand command) => Result = new CommandPanelMethod(_panel);
            public void Visit(CoroutineCommand command) => Result = new CommandPanelCoroutine(_panel);
            public void Visit(HandleMethodCommand command) => Result = new CommandPanelHandleMethod(_panel);
        }
    }
}
