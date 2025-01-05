using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class SnapshotLogCellView : ViewBase<SnapshotLogCellViewLinker>
    {
        [SerializeField]
        GameObject _highlightedObject;

        [SerializeField]
        LongTapButton _cellButton;

        [SerializeField]
        InputTextAutoScroller _cellLabel;

        [SerializeField]
        TextMeshProUGUI _timeText;

        [SerializeField]
        TextMeshProUGUI _elapsedText;

        [SerializeField]
        ToggleButtonBase _toggleButton;

        [SerializeField]
        Image _backGround;

        [SerializeField]
        Image _checkMark;

        SnapshotLogCellViewLinker _linker;

        protected override void _Init()
        {
            base._Init();

            _cellButton.onClick.RemoveListener(_OnClickCell);
            _cellButton.onClick.AddListener(_OnClickCell);
            _toggleButton._onClick.RemoveListener(_OnToggleChange);
            _toggleButton._onClick.AddListener(_OnToggleChange);
            _cellButton._onLongTap.RemoveListener(_OnLongTapCell);
            _cellButton._onLongTap.AddListener(_OnLongTapCell);
            _cellLabel.SetOnSelect(_OnClickCellLabel);
            _cellLabel.SetOnEndEdit(_OnUpdateLabel);
            _cellLabel.SetCharacterLimit(NoaDebuggerDefine.SnapshotLogMaxLabelCharNum);
        }

        protected override void _OnShow(SnapshotLogCellViewLinker linker)
        {
            base._OnShow(linker);

            _linker = linker;

            _highlightedObject.SetActive(_linker._isHighlighted);

            _cellLabel.SetText(_linker._label);
            _cellLabel.SetIsScroll(_linker._isHighlighted);

            _timeText.text = SnapshotPresenter.TimeSpanToHourTimeFormatString(_linker._time);
            _timeText.color = NoaDebuggerDefine.TextColors.Default;

            _elapsedText.text = SnapshotPresenter.TimeSpanToHourTimeFormatString(_linker._elapsedTime);

            switch (linker._toggleState)
            {
                case SnapshotModel.ToggleState.SelectedFirst:
                case SnapshotModel.ToggleState.SelectedSecond:
                    _toggleButton.Interactable = true;
                    if (!_toggleButton.IsOn)
                    {
                        _toggleButton.Init(true);
                    }
                    break;
                case SnapshotModel.ToggleState.Disabled:
                    _toggleButton.Interactable = false;
                    break;
                default:
                    _toggleButton.Init(false);
                    _toggleButton.Interactable = true;
                    break;
            }

            if (linker._toggleState == SnapshotModel.ToggleState.SelectedFirst)
            {
                _checkMark.color = NoaDebuggerDefine.ImageColors.SnapshotFirstSelected;
            }
            else if (linker._toggleState == SnapshotModel.ToggleState.SelectedSecond)
            {
                _checkMark.color = NoaDebuggerDefine.ImageColors.SnapshotSecondSelected;
            }

            if (_linker._backgroundColor != null)
            {
                _backGround.color = _linker._backgroundColor.Value;
            }
            else if (_linker._viewIndex % 2 == 0)
            {
                _backGround.color = NoaDebuggerDefine.BackgroundColors.LogBright;
            }
            else
            {
                _backGround.color = NoaDebuggerDefine.BackgroundColors.LogDark;
            }
        }

        void _OnClickCell()
        {
            _linker?._onClickCell?.Invoke(_linker._id);
        }

        void _OnClickCellLabel(string text)
        {
            _linker?._onClickCell?.Invoke(_linker._id);
        }

        void _OnLongTapCell()
        {
            _linker?._onLongTapCell?.Invoke(_linker._id);
        }

        void _OnUpdateLabel(string text)
        {
            _linker?._onUpdateLabel?.Invoke(_linker._id, text);
        }

        void _OnToggleChange(bool isOn)
        {
            _linker?._onToggleChanged?.Invoke(_linker._id);
        }
    }

    sealed class SnapshotLogCellViewLinker : ViewLinkerBase
    {
        public int _id;

        public int _viewIndex;

        public string _label;

        public TimeSpan _time;

        public TimeSpan _elapsedTime;

        public bool _isHighlighted;

        public UnityAction<int> _onClickCell;

        public UnityAction<int> _onLongTapCell;

        public UnityAction<int, string> _onUpdateLabel;

        public UnityAction<int> _onToggleChanged;

        public SnapshotModel.ToggleState _toggleState;

        public Color? _backgroundColor = null;
    }
}
