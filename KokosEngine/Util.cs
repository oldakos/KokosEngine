using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    internal static class Util
    {
        #region BB Shifting

        static readonly internal ulong NotAFile = 0xfefefefefefefefe;
        static readonly internal ulong NotABFiles = 0xfcfcfcfcfcfcfcfc;
        static readonly internal ulong NotHFile = 0x7f7f7f7f7f7f7f7f;
        static readonly internal ulong NotGHFiles = 0x3f3f3f3f3f3f3f3f;

        static internal ulong Shift_N(ulong bb) { return bb << 8; }
        static internal ulong Shift_NE(ulong bb) { return (bb << 9) & NotAFile; }
        static internal ulong Shift_E(ulong bb) { return (bb << 1) & NotAFile; }
        static internal ulong Shift_SE(ulong bb) { return (bb >> 7) & NotAFile; }
        static internal ulong Shift_S(ulong bb) { return bb >> 8; }
        static internal ulong Shift_SW(ulong bb) { return (bb >> 9) & NotHFile; }
        static internal ulong Shift_W(ulong bb) { return (bb >> 1) & NotHFile; }
        static internal ulong Shift_NW(ulong bb) { return (bb << 7) & NotHFile; }
        static internal ulong Shift_NNE(ulong bb) { return (bb << 17) & NotAFile; }
        static internal ulong Shift_NEE(ulong bb) { return (bb << 10) & NotABFiles; }
        static internal ulong Shift_SEE(ulong bb) { return (bb >> 6) & NotABFiles; }
        static internal ulong Shift_SSE(ulong bb) { return (bb >> 15) & NotAFile; }
        static internal ulong Shift_SSW(ulong bb) { return (bb >> 17) & NotHFile; }
        static internal ulong Shift_SWW(ulong bb) { return (bb >> 10) & NotGHFiles; }
        static internal ulong Shift_NWW(ulong bb) { return (bb << 6) & NotGHFiles; }
        static internal ulong Shift_NNW(ulong bb) { return (bb << 15) & NotHFile; }

        #endregion

        #region BB Propagation
        // Occluded Fill contains the generator and excludes blockers.
        // Attack Fill does not contain the generator and includes blockers.

        static internal ulong OccludedFill_N(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_N(generator) & propagator;
            }
            return result;
        }
        static internal ulong OccludedFill_NE(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_NE(generator) & propagator;
            }
            return result;
        }
        static internal ulong OccludedFill_E(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_E(generator) & propagator;
            }
            return result;
        }
        static internal ulong OccludedFill_SE(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_SE(generator) & propagator;
            }
            return result;
        }
        static internal ulong OccludedFill_S(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_S(generator) & propagator;
            }
            return result;
        }
        static internal ulong OccludedFill_SW(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_SW(generator) & propagator;
            }
            return result;
        }
        static internal ulong OccludedFill_W(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_W(generator) & propagator;
            }
            return result;
        }
        static internal ulong OccludedFill_NW(ulong generator, ulong propagator)
        {
            ulong result = 0;
            while (generator != 0)
            {
                result |= generator;
                generator = Shift_NW(generator) & propagator;
            }
            return result;
        }
        static internal ulong AttackFill_N(ulong generator, ulong propagator)
        {
            return Shift_N(OccludedFill_N(generator, propagator));
        }
        static internal ulong AttackFill_NE(ulong generator, ulong propagator)
        {
            return Shift_NE(OccludedFill_NE(generator, propagator));
        }
        static internal ulong AttackFill_E(ulong generator, ulong propagator)
        {
            return Shift_E(OccludedFill_E(generator, propagator));
        }
        static internal ulong AttackFill_SE(ulong generator, ulong propagator)
        {
            return Shift_SE(OccludedFill_SE(generator, propagator));
        }
        static internal ulong AttackFill_S(ulong generator, ulong propagator)
        {
            return Shift_S(OccludedFill_S(generator, propagator));
        }
        static internal ulong AttackFill_SW(ulong generator, ulong propagator)
        {
            return Shift_SW(OccludedFill_SW(generator, propagator));
        }
        static internal ulong AttackFill_W(ulong generator, ulong propagator)
        {
            return Shift_W(OccludedFill_W(generator, propagator));
        }
        static internal ulong AttackFill_NW(ulong generator, ulong propagator)
        {
            return Shift_NW(OccludedFill_NW(generator, propagator));
        }

        static internal ulong AttackFill_Bishop(ulong g, ulong p)
        {
            return AttackFill_NE(g, p) | AttackFill_SE(g, p) | AttackFill_SW(g, p) | AttackFill_NW(g, p);
        }
        static internal ulong AttackFill_Rook(ulong g, ulong p)
        {
            return AttackFill_N(g, p) | AttackFill_E(g, p) | AttackFill_S(g, p) | AttackFill_W(g, p);
        }
        static internal ulong AttackFill_Queen(ulong g, ulong p)
        {
            return AttackFill_Bishop(g, p) | AttackFill_Rook(g, p);
        }

        #endregion

        static readonly internal ulong BB_1stRank = 0x00000000000000FF;
        static readonly internal ulong BB_2ndRank = 0x000000000000FF00;
        static readonly internal ulong BB_7thRank = 0x00FF000000000000;
        static readonly internal ulong BB_8thRank = 0xFF00000000000000;

        static readonly internal ulong BB_e1f1g1 = 0x0000000000000070;
        static readonly internal ulong BB_f1g1 = 0x0000000000000060;
        static readonly internal ulong BB_e1d1c1 = 0x000000000000001C;
        static readonly internal ulong BB_d1c1b1 = 0x000000000000000E;
        static readonly internal ulong BB_e8f8g8 = 0x7000000000000000;
        static readonly internal ulong BB_f8g8 = 0x6000000000000000;
        static readonly internal ulong BB_e8d8c8 = 0x1C00000000000000;
        static readonly internal ulong BB_d8c8b8 = 0x0E00000000000000;

        static readonly internal ulong[] KnightMoves =
        {
        0x0000000000020400,
        0x0000000000050800,
        0x00000000000A1100,
        0x0000000000142200,
        0x0000000000284400,
        0x0000000000508800,
        0x0000000000A01000,
        0x0000000000402000,
        0x0000000002040004,
        0x0000000005080008,
        0x000000000A110011,
        0x0000000014220022,
        0x0000000028440044,
        0x0000000050880088,
        0x00000000A0100010,
        0x0000000040200020,
        0x0000000204000402,
        0x0000000508000805,
        0x0000000A1100110A,
        0x0000001422002214,
        0x0000002844004428,
        0x0000005088008850,
        0x000000A0100010A0,
        0x0000004020002040,
        0x0000020400040200,
        0x0000050800080500,
        0x00000A1100110A00,
        0x0000142200221400,
        0x0000284400442800,
        0x0000508800885000,
        0x0000A0100010A000,
        0x0000402000204000,
        0x0002040004020000,
        0x0005080008050000,
        0x000A1100110A0000,
        0x0014220022140000,
        0x0028440044280000,
        0x0050880088500000,
        0x00A0100010A00000,
        0x0040200020400000,
        0x0204000402000000,
        0x0508000805000000,
        0x0A1100110A000000,
        0x1422002214000000,
        0x2844004428000000,
        0x5088008850000000,
        0xA0100010A0000000,
        0x4020002040000000,
        0x0400040200000000,
        0x0800080500000000,
        0x1100110A00000000,
        0x2200221400000000,
        0x4400442800000000,
        0x8800885000000000,
        0x100010A000000000,
        0x2000204000000000,
        0x0004020000000000,
        0x0008050000000000,
        0x00110A0000000000,
        0x0022140000000000,
        0x0044280000000000,
        0x0088500000000000,
        0x0010A00000000000,
        0x0020400000000000
        };
        static readonly internal ulong[] KingMoves =
        {
        0x0000000000000302,
        0x0000000000000705,
        0x0000000000000E0A,
        0x0000000000001C14,
        0x0000000000003828,
        0x0000000000007050,
        0x000000000000E0A0,
        0x000000000000C040,
        0x0000000000030203,
        0x0000000000070507,
        0x00000000000E0A0E,
        0x00000000001C141C,
        0x0000000000382838,
        0x0000000000705070,
        0x0000000000E0A0E0,
        0x0000000000C040C0,
        0x0000000003020300,
        0x0000000007050700,
        0x000000000E0A0E00,
        0x000000001C141C00,
        0x0000000038283800,
        0x0000000070507000,
        0x00000000E0A0E000,
        0x00000000C040C000,
        0x0000000302030000,
        0x0000000705070000,
        0x0000000E0A0E0000,
        0x0000001C141C0000,
        0x0000003828380000,
        0x0000007050700000,
        0x000000E0A0E00000,
        0x000000C040C00000,
        0x0000030203000000,
        0x0000070507000000,
        0x00000E0A0E000000,
        0x00001C141C000000,
        0x0000382838000000,
        0x0000705070000000,
        0x0000E0A0E0000000,
        0x0000C040C0000000,
        0x0003020300000000,
        0x0007050700000000,
        0x000E0A0E00000000,
        0x001C141C00000000,
        0x0038283800000000,
        0x0070507000000000,
        0x00E0A0E000000000,
        0x00C040C000000000,
        0x0302030000000000,
        0x0705070000000000,
        0x0E0A0E0000000000,
        0x1C141C0000000000,
        0x3828380000000000,
        0x7050700000000000,
        0xE0A0E00000000000,
        0xC040C00000000000,
        0x0203000000000000,
        0x0507000000000000,
        0x0A0E000000000000,
        0x141C000000000000,
        0x2838000000000000,
        0x5070000000000000,
        0xA0E0000000000000,
        0x40C0000000000000
        };

        static internal void SerializeBitboard(ulong bb, List<int> list)
        {
            while (bb != 0)
            {
                int index = BitScanForward(bb);
                list.Add(index);
                bb &= bb - 1;
            }
        }


        static internal int BitScanForward(ulong bb)
        {
            const ulong debruijn64 = 0x03f79d71b4cb0a89;
            ulong negative = (ulong)(-(long)bb);
            return index64[((bb & negative) * debruijn64) >> 58];
        }
        static readonly int[] index64 = {
             0,  1, 48,  2, 57, 49, 28,  3,
            61, 58, 50, 42, 38, 29, 17,  4,
            62, 55, 59, 36, 53, 51, 43, 22,
            45, 39, 33, 30, 24, 18, 12,  5,
            63, 47, 56, 27, 60, 41, 37, 16,
            54, 35, 52, 21, 44, 32, 23, 11,
            46, 26, 40, 15, 34, 20, 31, 10,
            25, 14, 19,  9, 13,  8,  7,  6
        };

        /// <summary>
        /// Return algebraic notation of the LSB1 square in given bitboard.
        /// </summary>
        /// <returns>Algebraic notation of a square, or "-" if bitboard is empty.</returns>
        static internal string LSB1ToAlgebraic(ulong bitboard)
        {
            if (bitboard == 0)
            {
                return "-";
            }
            int index = BitScanForward(bitboard);
            return IndexToAlgebraic(index);
        }
        static internal string IndexToAlgebraic(int index)
        {
            if (index < 0 || index > 63) return "-";
            int rankindex = index / 8;
            int fileindex = index % 8;
            char rank = (char)('1' + rankindex);
            char file = (char)('a' + fileindex);
            return new string(new char[] { file, rank });
        }
        static internal int AlgebraicToIndex(string alg)
        {
            char file = alg[0];
            char rank = alg[1];
            int fileindex = file - 'a';
            int rankindex = rank - '1';
            return 8 * rankindex + fileindex;
        }

        static internal string BitboardToString(ulong bb)
        {
            StringBuilder sb = new StringBuilder();

            List<int> indices = new List<int>();
            SerializeBitboard(bb, indices);
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file < 8; file++)
                {
                    int i = 8 * rank + file;
                    if (indices.Contains(i)) sb.Append('#');
                    else sb.Append('.');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
