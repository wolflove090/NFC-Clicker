using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Debug = UnityEngine.Debug;

namespace NoaDebugger
{
    static class JsonFormatter
    {
        public enum NewLine
        {
            Cr,

            Lf,

            CrLf
        }

        const string DEFAULT_INDENT_STRING = "  ";

        const NewLine DEFAULT_NEW_LINE = NewLine.Lf;

        public static string Format(
            string inputJson,
            string indentString = JsonFormatter.DEFAULT_INDENT_STRING,
            NewLine newLine = JsonFormatter.DEFAULT_NEW_LINE)
        {
            using var formatter = new Formatter(inputJson, indentString, newLine);
            return formatter.Format();
        }

        class Formatter : IDisposable
        {
            readonly StringReader _inputJsonReader;

            readonly StringBuilder _outputStringBuilder;

            readonly string _indentString;

            readonly string _newLineString;

            int _indentDepth;

            public Formatter(
                string inputJson,
                string indentString = JsonFormatter.DEFAULT_INDENT_STRING,
                NewLine newLine = JsonFormatter.DEFAULT_NEW_LINE)
            {
                _inputJsonReader = new StringReader(inputJson);
                _outputStringBuilder = new StringBuilder();
                _indentString = indentString;
                _newLineString = newLine switch
                {
                    NewLine.Lf => "\n",
                    NewLine.Cr => "\r",
                    NewLine.CrLf => "\r\n",
                    _ => throw new ArgumentException($"Invalid NewLine type: {newLine}")
                };
            }

            public void Dispose() => _inputJsonReader.Dispose();

            public string Format()
            {
                if (!ReadValue())
                {
                    return null;
                }

                if (_inputJsonReader.Peek() != -1)
                {
                    Formatter.ReportWarning("Having multiple top-level values is prohibited.");
                    return null;
                }

                return _outputStringBuilder.ToString();
            }

            [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
            static void ReportWarning(string message) => Debug.LogWarning(message);

            bool ReadValue()
            {
                bool success;
                SkipWhitespaces();
                switch (_inputJsonReader.Peek())
                {
                    case '{':
                        success = ReadObject();
                        break;

                    case '[':
                        success = ReadArray();
                        break;

                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        success = ReadNumber();
                        break;

                    case '"':
                        success = ReadString();
                        break;

                    case 't':
                        success = ReadLiteral("true");
                        break;

                    case 'f':
                        success = ReadLiteral("false");
                        break;

                    case 'n':
                        success = ReadLiteral("null");
                        break;

                    default:
                        Formatter.ReportWarning("Expected value.");
                        return false;
                }
                SkipWhitespaces();
                return success;
            }

            bool ReadObject()
            {
                if (_inputJsonReader.Peek() != '{')
                {
                    Formatter.ReportWarning("Expected opening '{'.");
                    return false;
                }
                ReadCharacter();
                SkipWhitespaces();
                if (_inputJsonReader.Peek() == '}')
                {
                    ReadCharacter();
                    return true;
                }
                IncreaseIndent();
                NextLine();

                while (true)
                {
                    if (!ReadObjectMember())
                    {
                        return false;
                    }
                    switch (_inputJsonReader.Peek())
                    {
                        case ',':
                            ReadCharacter();
                            NextLine();
                            break;

                        case '}':
                            DecreaseIndent();
                            NextLine();
                            ReadCharacter();
                            return true;

                        case -1:
                            Formatter.ReportWarning("Expected closing '}' at EOF.");
                            return false;

                        default:
                            Formatter.ReportWarning("Expected separator between members.");
                            return false;
                    }
                }
            }

            bool ReadObjectMember()
            {
                SkipWhitespaces();
                if (!ReadString())
                {
                    Formatter.ReportWarning("Expected object-name.");
                    return false;
                }
                SkipWhitespaces();

                if (_inputJsonReader.Peek() != ':')
                {
                    Formatter.ReportWarning("Expected separator between name and value.");
                    return false;
                }
                ReadCharacter();
                _outputStringBuilder.Append(' ');

                return ReadValue();
            }

            bool ReadArray()
            {
                if (_inputJsonReader.Peek() != '[')
                {
                    Formatter.ReportWarning("Expected opening '['.");
                    return false;
                }
                ReadCharacter();
                SkipWhitespaces();
                if (_inputJsonReader.Peek() == ']')
                {
                    ReadCharacter();
                    return true;
                }
                IncreaseIndent();
                NextLine();

                while (true)
                {
                    if (!ReadValue())
                    {
                        return false;
                    }
                    switch (_inputJsonReader.Peek())
                    {
                        case ',':
                            ReadCharacter();
                            NextLine();
                            break;

                        case ']':
                            DecreaseIndent();
                            NextLine();
                            ReadCharacter();
                            return true;

                        case -1:
                            Formatter.ReportWarning("Expected closing ']' at EOF.");
                            return false;

                        default:
                            Formatter.ReportWarning("Expected separator between array elements.");
                            return false;
                    }
                }
            }

            bool ReadNumber()
            {
                return ReadInteger()
                       && ReadFraction()
                       && ReadExponent();
            }

            bool ReadInteger()
            {
                if (_inputJsonReader.Peek() == '-')
                {
                    ReadCharacter();
                }
                if (_inputJsonReader.Peek() != '0')
                {
                    return ReadDigits();
                }

                ReadCharacter();
                return true;
            }

            bool ReadFraction()
            {
                if (_inputJsonReader.Peek() != '.')
                {
                    return true;
                }
                ReadCharacter();
                return ReadDigits();
            }

            bool ReadExponent()
            {
                if (_inputJsonReader.Peek() is not ('E' or 'e'))
                {
                    return true;
                }
                ReadCharacter();
                if (_inputJsonReader.Peek() is '+' or '-')
                {
                    ReadCharacter();
                }
                return ReadDigits();
            }

            bool ReadDigits()
            {
                var hasDigits = false;
                while (_inputJsonReader.Peek() is >= '0' and <= '9')
                {
                    ReadCharacter();
                    hasDigits = true;
                }
                if (hasDigits)
                {
                    return true;
                }
                Formatter.ReportWarning("Expected digits.");
                return false;
            }

            bool ReadString()
            {
                if (_inputJsonReader.Peek() != '"')
                {
                    Formatter.ReportWarning(@"Expected opening '""'.");
                    return false;
                }
                ReadCharacter();

                while (true)
                {
                    switch (_inputJsonReader.Peek())
                    {
                        case '"':
                            ReadCharacter();
                            return true;

                        case '\\':
                            if (!ReadEscapeSequence())
                            {
                                return false;
                            }
                            break;

                        case -1:
                            Formatter.ReportWarning("Unfinished string at EOF.");
                            return false;

                        default:
                            ReadCharacter();
                            break;
                    }
                }
            }

            bool ReadEscapeSequence()
            {
                if (_inputJsonReader.Peek() != '\\')
                {
                    return false;
                }
                ReadCharacter();
                switch (_inputJsonReader.Peek())
                {
                    case '"':
                    case '/':
                    case '\\':
                    case 'b':
                    case 'f':
                    case 'n':
                    case 'r':
                    case 't':
                        ReadCharacter();
                        return true;

                    case 'u':
                        ReadCharacter();
                        for (var i = 0; i < 4; ++i)
                        {
                            int ch = _inputJsonReader.Peek();
                            if (ch is >= '0' and <= '9'
                                or >= 'A' and <= 'F'
                                or >= 'a' and <= 'f')
                            {
                                ReadCharacter();
                            }
                            else
                            {
                                Formatter.ReportWarning("Invalid escape.");
                                return false;
                            }
                        }
                        return true;

                    default:
                        Formatter.ReportWarning("Invalid escape.");
                        return false;
                }
            }

            bool ReadLiteral(string expectedLiteral)
            {
                foreach (char expectedCharacter in expectedLiteral)
                {
                    if (_inputJsonReader.Peek() != expectedCharacter)
                    {
                        Formatter.ReportWarning("Invalid literal.");
                        return false;
                    }
                    ReadCharacter();
                }
                return true;
            }

            void ReadCharacter() => _outputStringBuilder.Append((char)_inputJsonReader.Read());

            void SkipWhitespaces()
            {
                while (true)
                {
                    switch (_inputJsonReader.Peek())
                    {
                        case ' ':
                        case '\t':
                        case '\n':
                        case '\r':
                            _inputJsonReader.Read();
                            break;

                        default:
                            return;
                    }
                }
            }

            void IncreaseIndent() => ++_indentDepth;

            void DecreaseIndent() => --_indentDepth;

            void NextLine()
            {
                _outputStringBuilder.Append(_newLineString);
                for (var i = 0; i < _indentDepth; ++i)
                {
                    _outputStringBuilder.Append(_indentString);
                }
            }
        }
    }
}
