using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Information for one unit of snapshot logs
    /// </summary>
    public sealed class SnapshotLogRecordInformation
    {
        /// <summary>
        /// Log identification ID
        /// It does not affect the display at all and is used only for management
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Name of the log (user can input)
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// The elapsed time when the log was inserted
        /// </summary>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// The elapsed time since the last time the log was inserted
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// Whether it is highlighted or not
        /// </summary>
        internal bool IsHighlighted { get; private set; }

        /// <summary>
        /// Whether it is selected or not
        /// </summary>
        internal bool IsSelected { get; private set; }

        /// <summary>
        /// State of the toggle
        /// </summary>
        internal SnapshotModel.ToggleState ToggleState { get; private set; }

        /// <summary>
        /// Snapshot data
        /// </summary>
        public ProfilerSnapshotData Snapshot { get; private set; }

        /// <summary>
        /// Background color
        /// </summary>
        public Color? BackgroundColor { get; private set; }

        /// <summary>
        /// Additional information
        /// </summary>
        public Dictionary<string, NoaSnapshotCategory> AdditionalInfo { get; private set; }

        /// <summary>
        /// Generates a SnapshotLogRecordInformation
        /// </summary>
        /// <param name="id">Specifies the log ID</param>
        /// <param name="label">Specifies the log label</param>
        /// <param name="time">Specifies the elapsed time at the timing of log insertion</param>
        /// <param name="elapsedTime">Specifies the elapsed time since the previous log</param>
        /// <param name="snapshotData">Specifies the snapshot data</param>
        /// <param name="backgroundColor">Specifies the background color</param>
        /// <param name="additionalInfo">Specifies the additional information</param>
        internal SnapshotLogRecordInformation(int id, string label, TimeSpan time, TimeSpan elapsedTime,
                                              ProfilerSnapshotData snapshotData, Color? backgroundColor,
                                              Dictionary<string, NoaSnapshotCategory> additionalInfo)
        {
            Id = id;
            Label = label;
            Time = time;
            ElapsedTime = elapsedTime;
            Snapshot = snapshotData;
            BackgroundColor = backgroundColor;
            AdditionalInfo = additionalInfo;
        }

        /// <summary>
        /// Changes the highlight information
        /// </summary>
        /// <param name="highlighted">Specifies the highlight information to change</param>
        internal void SetHighlighted(bool highlighted)
        {
            IsHighlighted = highlighted;
        }

        /// <summary>
        /// Changes the selection state
        /// </summary>
        /// <param name="selected">Specifies the selection state to change</param>
        internal void SetSelected(bool selected)
        {
            IsSelected = selected;
        }

        /// <summary>
        /// Changes the toggle state
        /// </summary>
        /// <param name="state">Specifies the toggle state to change</param>
        internal void SetToggleState(SnapshotModel.ToggleState state)
        {
            ToggleState = state;
        }

        /// <summary>
        /// Changes the label
        /// </summary>
        /// <param name="text">Specifies the label name to change</param>
        internal void SetLabel(string text)
        {
            Label = text;
        }
    }
}
