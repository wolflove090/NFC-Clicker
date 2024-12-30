namespace NoaDebugger
{
    static class DataUnitConverterModel
    {
        public static float ByteToMB(float byteValue)
        {
            return byteValue / 1024 / 1024;
        }
        
        public static float KBToByte(float kbValue)
        {
            return kbValue * 1024;
        }
        
        public static float MBToByte(float mbValue)
        {
            return mbValue * 1024 * 1024;
        }

        static readonly string[] SizeUnits = { "Byte", "KB", "MB", "GB", "TB" };

        public static string ToHumanReadableBytes(long bytes)
        {
            double displayBytes = bytes;
            int order = 0;
            while (displayBytes >= 1024 && order < DataUnitConverterModel.SizeUnits.Length - 1)
            {
                ++order;
                displayBytes /= 1024;
            }
            return $"{displayBytes:0.#} {DataUnitConverterModel.SizeUnits[order]}";
        }
    }
}
