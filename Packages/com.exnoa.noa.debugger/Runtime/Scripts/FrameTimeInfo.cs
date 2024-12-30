using System.Collections.Generic;

namespace NoaDebugger
{
    sealed class FrameTimeInfo
    {
        public bool IsEnabled { get; set; }

        public bool IsActive { get; set; }

        public RingBuffer<float[]> HistoryBuffer { get; }

        readonly List<float[]> _historyValueBuffer;

        public FrameTimeInfo(int historyCapacity)
        {
            HistoryBuffer = new RingBuffer<float[]>(historyCapacity);
            _historyValueBuffer = new List<float[]>(historyCapacity);

            for (var i = 0; i < _historyValueBuffer.Capacity; ++i)
            {
                _historyValueBuffer.Add(new float[3]);
            }
        }

        public void AddHistory(float updateMilliseconds, float renderingMilliseconds, float othersMilliseconds)
        {
            _historyValueBuffer[HistoryBuffer.Tail][0] = updateMilliseconds;
            _historyValueBuffer[HistoryBuffer.Tail][1] = renderingMilliseconds;
            _historyValueBuffer[HistoryBuffer.Tail][2] = othersMilliseconds;
            HistoryBuffer.Append(_historyValueBuffer[HistoryBuffer.Tail]);
        }

        public void ClearHistory()
        {
            HistoryBuffer.Clear();
        }
    }
}
