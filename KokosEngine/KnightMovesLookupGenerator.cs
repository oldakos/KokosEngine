using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    /// <summary>
    /// Generates the code for a static readonly array of Bitboards that represent knight moves from each square, indexed by Coordinate index
    /// </summary>
    internal static class KnightMovesLookupGenerator
    {
        internal static string GenerateArraySourceCode()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("static readonly ulong[] KnightMoves =");
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
            ulong result = (s.Shift_NNE()) | (s.Shift_NEE()) | (s.Shift_SEE()) | (s.Shift_SSE()) | (s.Shift_SSW()) | (s.Shift_SWW()) | (s.Shift_NWW()) | (s.Shift_NNW());
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
