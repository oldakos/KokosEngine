﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    /// <summary>
    /// Generates the code for a static readonly array of Bitboards that represent king moves from each square, indexed by Coordinate index
    /// </summary>
    internal static class KingMovesLookupGenerator
    {
        internal static string GenerateArraySourceCode()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("static readonly ulong[] KingMoves =");
            sb.AppendLine("{");
            for (int i = 0; i < 64; i++)
            {
                sb.Append(PrintBitboard(GenerateBitboard(i)));
                if (i < 63) sb.Append(',');
                sb.AppendLine();
            }
            sb.AppendLine("};");

            return sb.ToString();
        }
        internal static ulong GenerateBitboard(int index)
        {
            Bitboard s = (ulong)1 << index;
            ulong result = s.Shift_N() | s.Shift_NE() | s.Shift_E() | s.Shift_SE() | s.Shift_S() | s.Shift_SW() | s.Shift_W() | s.Shift_NW();
            return result;
        }
        internal static void ToConsole()
        {
            Console.WriteLine(GenerateArraySourceCode());
        }
        internal static string PrintBitboard(ulong bb)
        {
            return string.Format("0x{0:X16}", bb);
        }
    }
}
