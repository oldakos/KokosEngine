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
        static readonly internal ulong NotHFile = 0x7f7f7f7f7f7f7f7f;

        static internal ulong Shift_N(ulong bb) { return bb << 8; }
        static internal ulong Shift_NE(ulong bb) { return (bb << 9) & NotAFile; }
        static internal ulong Shift_E(ulong bb) { return (bb << 1) & NotAFile; }
        static internal ulong Shift_SE(ulong bb) { return (bb >> 7) & NotAFile; }
        static internal ulong Shift_S(ulong bb) { return bb >> 8; }
        static internal ulong Shift_SW(ulong bb) { return (bb >> 9) & NotHFile; }
        static internal ulong Shift_W(ulong bb) { return (bb >> 1) & NotHFile; }
        static internal ulong Shift_NW(ulong bb) { return (bb << 7) & NotHFile; }

        #endregion

        static readonly internal ulong BB_1stRank = 0x00000000000000FF;
        static readonly internal ulong BB_2ndRank = 0x000000000000FF00;
        static readonly internal ulong BB_7thRank = 0x00FF000000000000;
        static readonly internal ulong BB_8thRank = 0xFF00000000000000;

        static internal List<int> SerializeBitboard(ulong bb)
        {
            var list = new List<int>();
            while (bb != 0)
            {
                int index = BitScanForward(bb);
                list.Add(index);
                bb &= bb - 1;
            }
            return list;
        }


        static internal int BitScanForward(ulong bb)
        {
            if (bb == 0) throw new ArgumentException("you bitscanned a zero bitboard");
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
    }
}
