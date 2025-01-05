using System;
using System.Text.RegularExpressions;

namespace NoaDebugger
{
    struct SemanticVersion : IComparable<SemanticVersion>
    {
        const string VERSION_PATTERN = @"\d+(\.\d+){0,2}";

        const int MAX_DIGIT_COUNT = 3;

        public int major;
        public int minor;
        public int patch;

        public static string ExtractSemanticVersionString(string input)
        {
            return Regex.Match(input, SemanticVersion.VERSION_PATTERN).ToString();
        }

        public static SemanticVersion Parse(string input)
        {
            string[] digits = input.Split('.');
            if (SemanticVersion.MAX_DIGIT_COUNT < digits.Length)
            {
                throw new FormatException("Number of elements must be less than 3.");
            }
            SemanticVersion version = new SemanticVersion
            {
                major = (0 < digits.Length) ? Int32.Parse(digits[0]) : 0,
                minor = (1 < digits.Length) ? Int32.Parse(digits[1]) : 0,
                patch = (2 < digits.Length) ? Int32.Parse(digits[2]) : 0
            };
            if ((version.major < 0) || (version.minor < 0) || (version.patch < 0))
            {
                throw new ArgumentException("Each number must be zero or positive.");
            }
            return version;
        }

        public int CompareTo(SemanticVersion another)
        {
            if (major < another.major) { return -1; }
            if (major > another.major) { return 1; }
            if (minor < another.minor) { return -1; }
            if (minor > another.minor) { return 1; }
            if (patch < another.patch) { return -1; }
            if (patch > another.patch) { return 1; }
            return 0;
        }

        public static bool operator ==(SemanticVersion one, SemanticVersion another) => one.CompareTo(another) == 0;

        public static bool operator !=(SemanticVersion one, SemanticVersion another) => one.CompareTo(another) != 0;

        public static bool operator <(SemanticVersion one, SemanticVersion another) => one.CompareTo(another) < 0;

        public static bool operator >(SemanticVersion one, SemanticVersion another) => one.CompareTo(another) > 0;

        public static bool operator <=(SemanticVersion one, SemanticVersion another) => one.CompareTo(another) <= 0;

        public static bool operator >=(SemanticVersion one, SemanticVersion another) => one.CompareTo(another) >= 0;

        public override int GetHashCode() => (major, minor, patch).GetHashCode();

        public override bool Equals(object another)
        {
            return (another != null)
                   && (GetType() == another.GetType())
                   && (this == (SemanticVersion) another);
        }
    }
}
