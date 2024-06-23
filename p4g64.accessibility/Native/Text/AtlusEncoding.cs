using System.Globalization;
using System.Text;

namespace p4g64.accessibility.Native.Text;

/// <summary>
/// Class for decoding and encoding text, based on Atlus Script Compiler's encoding stuffs
/// </summary>
public class AtlusEncoding : Encoding
{
    /// <summary>
    /// Offset from start of glyph range to start of the char table.
    /// </summary>
    private const int CHAR_TO_GLYPH_INDEX_OFFSET = 0x60;

    /// <summary>
    /// Size of a single glyph table.
    /// </summary>
    private const int GLYPH_TABLE_SIZE = 0x80;

    /// <summary>
    /// The range 0-based range of an ascii character index.
    /// </summary>
    private const int ASCII_RANGE = 0x7F;

    /// <summary>
    /// The high bit serves as a marker for a table index.
    /// </summary>
    private const int GLYPH_TABLE_INDEX_MARKER = 0x80;

    public static Encoding P4;

    private Dictionary<string, CodePoint> mCharToCodePoint;
    private Dictionary<CodePoint, string> mCodePointToChar;

    private AtlusEncoding(string tableFilePath)
    {
        var charTable = ReadCharsetFile(tableFilePath);

        // build character to codepoint table
        mCharToCodePoint = new Dictionary<string, CodePoint>(charTable.Count);

        // add the ascii range seperately
        for (int charIndex = 0; charIndex < ASCII_RANGE + 1; charIndex++)
        {
            if (!mCharToCodePoint.ContainsKey(charTable[charIndex]))
                mCharToCodePoint[charTable[charIndex]] = new CodePoint(0, (byte)charIndex);
        }

        // add extended characters, but don't re-include the ascii range
        for (int charIndex = ASCII_RANGE + 1; charIndex < charTable.Count; charIndex++)
        {
            int glyphIndex = charIndex + CHAR_TO_GLYPH_INDEX_OFFSET;
            int tableIndex = (glyphIndex / GLYPH_TABLE_SIZE) - 1;
            int tableRelativeIndex = glyphIndex - (tableIndex * GLYPH_TABLE_SIZE);

            if (!mCharToCodePoint.ContainsKey(charTable[charIndex]))
                mCharToCodePoint[charTable[charIndex]] = new CodePoint((byte)(GLYPH_TABLE_INDEX_MARKER | tableIndex),
                    (byte)(tableRelativeIndex));
        }

        // build code point to character lookup table
        mCodePointToChar = new Dictionary<CodePoint, string>(charTable.Count);

        // add the ascii range seperately
        for (int charIndex = 0; charIndex < ASCII_RANGE + 1; charIndex++)
        {
            mCodePointToChar[new CodePoint(0, (byte)charIndex)] = charTable[charIndex];
        }

        // add extended characters, and make sure to include the ascii range again due to overlap
        for (int charIndex = 0x20; charIndex < charTable.Count; charIndex++)
        {
            int glyphIndex = charIndex + CHAR_TO_GLYPH_INDEX_OFFSET;
            int tableIndex = (glyphIndex / GLYPH_TABLE_SIZE) - 1;
            int tableRelativeIndex = glyphIndex - (tableIndex * GLYPH_TABLE_SIZE);

            mCodePointToChar[new CodePoint((byte)(GLYPH_TABLE_INDEX_MARKER | tableIndex), (byte)(tableRelativeIndex))] =
                charTable[charIndex];
        }
    }

    /// <summary>
    /// Sets up the encoding to be used in the mod
    /// </summary>
    /// <param name="modDir">The directory that the mod is in, used to read charsets</param>
    public static void Initiailse(string modDir)
    {
        P4 = new AtlusEncoding(Path.Combine(modDir, "P4.tsv"));
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
        int byteCount = 0;
        for (int i = index; i < count; i++)
        {
            if (chars[i] <= ASCII_RANGE)
                byteCount += 1;
            else
                byteCount += 2;
        }

        return byteCount;
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
        int byteCount = 0;

        for (; charIndex < charCount; charIndex++)
        {
            CodePoint codePoint;
            var c = chars[charIndex].ToString();
            if (char.IsHighSurrogate(c[0]))
                c += chars[++charIndex].ToString();
            ;

            try
            {
                codePoint = mCharToCodePoint[c];
            }
            catch (KeyNotFoundException)
            {
                throw new UnsupportedCharacterException(EncodingName, c);
            }

            if (codePoint.HighSurrogate == 0)
            {
                bytes[byteIndex++] = codePoint.LowSurrogate;
                byteCount += 1;
            }
            else
            {
                bytes[byteIndex++] = codePoint.HighSurrogate;
                bytes[byteIndex++] = codePoint.LowSurrogate;
                byteCount += 2;
            }
        }

        return byteCount;
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
        int charCount = 0;
        for (; index < count; ++index, ++charCount)
        {
            if ((bytes[index] & GLYPH_TABLE_INDEX_MARKER) == GLYPH_TABLE_INDEX_MARKER)
            {
                ++index;
            }
        }

        return charCount;
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
        return GetCharsImpl(bytes, byteIndex, byteCount, chars, charIndex, out _);
    }

    private int GetCharsImpl(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex,
        out bool hasUndefinedChars)
    {
        int charCount = 0;
        hasUndefinedChars = false;

        for (; byteIndex < byteCount; byteIndex++)
        {
            CodePoint cp;
            if ((bytes[byteIndex] & GLYPH_TABLE_INDEX_MARKER) == GLYPH_TABLE_INDEX_MARKER)
            {
                cp.HighSurrogate = bytes[byteIndex++];
            }
            else
            {
                cp.HighSurrogate = 0;
            }

            cp.LowSurrogate = bytes[byteIndex];

            if (!mCodePointToChar.TryGetValue(cp, out var c))
            {
                hasUndefinedChars = true;
                continue;
            }

            for (int i = 0; i < c.Length; i++)
            {
                chars[charIndex++] = c[i];
                charCount++;
            }
        }

        return charCount;
    }

    public override int GetMaxByteCount(int charCount)
    {
        return charCount * 2;
    }

    public override int GetMaxCharCount(int byteCount)
    {
        return byteCount * 2;
    }

    public bool TryGetString(byte[] bytes, out string value)
    {
        var chars = new char[GetMaxCharCount(bytes.Length)];
        GetCharsImpl(bytes, 0, bytes.Length, chars, 0, out bool hasUndefinedChars);

        if (hasUndefinedChars)
        {
            value = null;
            return false;
        }

        value = new string(chars);
        return true;
    }

    private static List<string> ReadCharsetFile(string tableFilePath)
    {
        var charTable = new List<string>();
        using (var reader = File.OpenText(tableFilePath))
        {
            var lineNr = 1;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var charStrings = line.Split('\t');
                for (int i = 0; i < charStrings.Length; i++)
                {
                    var charString = charStrings[i];
                    if (charString.StartsWith("\\u"))
                    {
                        // Escaped unicode character
                        var charId = int.Parse(charString.Substring(2), NumberStyles.HexNumber,
                            CultureInfo.InvariantCulture);
                        charTable.Add(((char)charId).ToString());
                    }
                    else
                    {
                        if (charString.Length > 1)
                            Utils.LogDebug(
                                $"WARNING: Character in charset with more than 1 UTF16 character at line {lineNr}: {charString}");

                        charTable.Add(charString);
                    }
                }

                ++lineNr;
            }
        }

        return charTable;
    }
}