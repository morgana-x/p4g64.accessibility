﻿using System;
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
        [FieldOffset(0x40)]
        internal TextLine* Lines;

        public override string ToString()
        {
            StringBuilder sb = new();
            for (TextLine* line = Lines; line != (TextLine*)0; line = line->NextLine)
            {
                for (TextCharacter* character = line->Characters; character != (TextCharacter*)0; character = character->NextCharacter)
                {
                    //Utils.Log($"{DecodeChar(character->Character)}: {character->Character:X}");
                    sb.Append(DecodeChar(character->Character));
                }
                sb.Append(' ');
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
        [FieldOffset(0x20)]
        internal TextCharacter* Characters;

        [FieldOffset(0x38)]
        internal TextLine* NextLine;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct TextCharacter
    {
        [FieldOffset(0)]
        internal byte Character;

        [FieldOffset(0x38)]
        internal TextCharacter* NextCharacter;
    }
}