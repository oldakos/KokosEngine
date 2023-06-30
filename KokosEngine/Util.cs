using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    internal static class Util
    {
        static internal int bitScanForward(ulong bb)
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
            int index = bitScanForward(bitboard);
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
