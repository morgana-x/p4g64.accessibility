using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace p4g64.accessibility.Native;

internal unsafe class Text
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct TextStruct
    {
        [FieldOffset(0x40)] internal TextLine* Lines;

        public override string ToString()
        {
            StringBuilder sb = new();
            for (TextLine* line = Lines; line != (TextLine*)0; line = line->NextLine)
            {
                sb.Append(line->ToString());
                sb.Append(' ');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the text for the specified selection option
        /// </summary>
        /// <param name="option">The index of the option</param>
        /// <returns>The text of the specified seleciton option</returns>
        public string GetSelection(int option)
        {
            int curOption = 0;
            int lastY = Lines->YPos;
            StringBuilder sb = new();
            for (TextLine* line = Lines; line != (TextLine*)0; line = line->NextLine)
            {
                // The selections are just identified by being at different Y positions
                if (lastY != line->YPos)
                {
                    lastY = line->YPos;
                    curOption++;
                }
                
                if (curOption == option)
                {
                    sb.Append(line->ToString());
                }
                else if (curOption > option)
                {
                    break;
                }
            }

            return sb.ToString();
        }
    }

    // TODO this could probably be done using an encoding or something
    private static char DecodeChar(byte byteVal)
    {
        if (byteVal == 0x8A || byteVal == 0x80)
            return ' ';
        return (char)byteVal;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct TextLine
    {
        [FieldOffset(4)] internal int XPos;

        [FieldOffset(8)] internal int YPos;

        [FieldOffset(0x20)] internal TextCharacter* Characters;

        [FieldOffset(0x38)] internal TextLine* NextLine;

        public override string ToString()
        {
            StringBuilder sb = new();
            for (TextCharacter* character = Characters;
                 character != (TextCharacter*)0;
                 character = character->NextCharacter)
            {
                // Utils.Log($"{DecodeChar(character->Character)}: {character->Character:X}");
                sb.Append(DecodeChar(character->Character));
            }

            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct TextCharacter
    {
        [FieldOffset(0)] internal byte Character;

        [FieldOffset(0x38)] internal TextCharacter* NextCharacter;
    }
}