using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KokosEngine
{
    /// <summary>
    /// Represents the "address" of a square on the chessboard.
    /// </summary>
    internal struct Coordinate
    {
        private byte _index;

        internal Coordinate(int index)
        {
            if (index >= 0 && index <= 63) this._index = (byte)index;
            else this._index = Byte.MaxValue;
        }
        internal Coordinate(string name)
        {
            if (name == null || name.Length != 2)
            {
                _index = Byte.MaxValue;
            }
            else
            {
                char file = name[0];
                char rank = name[1];
                int fileindex = file - 'a';
                int rankindex = rank - '1';
                if (fileindex < 0 || fileindex > 7 || rankindex < 0 || rankindex > 7) _index = Byte.MaxValue;
                else _index = (byte)(8 * rankindex + fileindex);
            }
        }

        #region List of 64 static getters named after the individual squares
        internal static Coordinate a1 { get { return new Coordinate(0); } }
        internal static Coordinate b1 { get { return new Coordinate(1); } }
        internal static Coordinate c1 { get { return new Coordinate(2); } }
        internal static Coordinate d1 { get { return new Coordinate(3); } }
        internal static Coordinate e1 { get { return new Coordinate(4); } }
        internal static Coordinate f1 { get { return new Coordinate(5); } }
        internal static Coordinate g1 { get { return new Coordinate(6); } }
        internal static Coordinate h1 { get { return new Coordinate(7); } }
        internal static Coordinate a2 { get { return new Coordinate(8); } }
        internal static Coordinate b2 { get { return new Coordinate(9); } }
        internal static Coordinate c2 { get { return new Coordinate(10); } }
        internal static Coordinate d2 { get { return new Coordinate(11); } }
        internal static Coordinate e2 { get { return new Coordinate(12); } }
        internal static Coordinate f2 { get { return new Coordinate(13); } }
        internal static Coordinate g2 { get { return new Coordinate(14); } }
        internal static Coordinate h2 { get { return new Coordinate(15); } }
        internal static Coordinate a3 { get { return new Coordinate(16); } }
        internal static Coordinate b3 { get { return new Coordinate(17); } }
        internal static Coordinate c3 { get { return new Coordinate(18); } }
        internal static Coordinate d3 { get { return new Coordinate(19); } }
        internal static Coordinate e3 { get { return new Coordinate(20); } }
        internal static Coordinate f3 { get { return new Coordinate(21); } }
        internal static Coordinate g3 { get { return new Coordinate(22); } }
        internal static Coordinate h3 { get { return new Coordinate(23); } }
        internal static Coordinate a4 { get { return new Coordinate(24); } }
        internal static Coordinate b4 { get { return new Coordinate(25); } }
        internal static Coordinate c4 { get { return new Coordinate(26); } }
        internal static Coordinate d4 { get { return new Coordinate(27); } }
        internal static Coordinate e4 { get { return new Coordinate(28); } }
        internal static Coordinate f4 { get { return new Coordinate(29); } }
        internal static Coordinate g4 { get { return new Coordinate(30); } }
        internal static Coordinate h4 { get { return new Coordinate(31); } }
        internal static Coordinate a5 { get { return new Coordinate(32); } }
        internal static Coordinate b5 { get { return new Coordinate(33); } }
        internal static Coordinate c5 { get { return new Coordinate(34); } }
        internal static Coordinate d5 { get { return new Coordinate(35); } }
        internal static Coordinate e5 { get { return new Coordinate(36); } }
        internal static Coordinate f5 { get { return new Coordinate(37); } }
        internal static Coordinate g5 { get { return new Coordinate(38); } }
        internal static Coordinate h5 { get { return new Coordinate(39); } }
        internal static Coordinate a6 { get { return new Coordinate(40); } }
        internal static Coordinate b6 { get { return new Coordinate(41); } }
        internal static Coordinate c6 { get { return new Coordinate(42); } }
        internal static Coordinate d6 { get { return new Coordinate(43); } }
        internal static Coordinate e6 { get { return new Coordinate(44); } }
        internal static Coordinate f6 { get { return new Coordinate(45); } }
        internal static Coordinate g6 { get { return new Coordinate(46); } }
        internal static Coordinate h6 { get { return new Coordinate(47); } }
        internal static Coordinate a7 { get { return new Coordinate(48); } }
        internal static Coordinate b7 { get { return new Coordinate(49); } }
        internal static Coordinate c7 { get { return new Coordinate(50); } }
        internal static Coordinate d7 { get { return new Coordinate(51); } }
        internal static Coordinate e7 { get { return new Coordinate(52); } }
        internal static Coordinate f7 { get { return new Coordinate(53); } }
        internal static Coordinate g7 { get { return new Coordinate(54); } }
        internal static Coordinate h7 { get { return new Coordinate(55); } }
        internal static Coordinate a8 { get { return new Coordinate(56); } }
        internal static Coordinate b8 { get { return new Coordinate(57); } }
        internal static Coordinate c8 { get { return new Coordinate(58); } }
        internal static Coordinate d8 { get { return new Coordinate(59); } }
        internal static Coordinate e8 { get { return new Coordinate(60); } }
        internal static Coordinate f8 { get { return new Coordinate(61); } }
        internal static Coordinate g8 { get { return new Coordinate(62); } }
        internal static Coordinate h8 { get { return new Coordinate(63); } }
        #endregion

        internal static Coordinate None { get { return new Coordinate(-1); } }

        public static implicit operator Coordinate(int index) { return new Coordinate(index); }
        public static implicit operator int(Coordinate coord) { return coord._index; }

        #region List of 16 instance getters that return a new Coordinate in cardinal, diagonal and "knight" directions from "this" instance
        internal Coordinate North { get { return new Coordinate(_index + 8); } }
        internal Coordinate East { get { return new Coordinate(_index + 1); } }
        internal Coordinate South { get { return new Coordinate(_index - 8); } }
        internal Coordinate West { get { return new Coordinate(_index - 1); } }
        internal Coordinate NE { get { return new Coordinate(_index + 9); } }
        internal Coordinate SE { get { return new Coordinate(_index - 7); } }
        internal Coordinate SW { get { return new Coordinate(_index - 9); } }
        internal Coordinate NW { get { return new Coordinate(_index + 7); } }
        internal Coordinate KnightNNE { get { return new Coordinate(_index + 17); } }
        internal Coordinate KnightNEE { get { return new Coordinate(_index + 10); } }
        internal Coordinate KnightSEE { get { return new Coordinate(_index - 6); } }
        internal Coordinate KnightSSE { get { return new Coordinate(_index - 15); } }
        internal Coordinate KnightSSW { get { return new Coordinate(_index - 17); } }
        internal Coordinate KnightSWW { get { return new Coordinate(_index - 10); } }
        internal Coordinate KnightNWW { get { return new Coordinate(_index + 6); } }
        internal Coordinate KnightNNW { get { return new Coordinate(_index + 15); } }
        #endregion

        public override string ToString()
        {
            if (_index > 63) return "-";
            int rankindex = _index / 8;
            int fileindex = _index % 8;
            char rank = (char)('1' + rankindex);
            char file = (char)('a' + fileindex);
            return new string(new char[] { file, rank });
        }
        internal string GetFileString()
        {
            if (_index > 63) return "-";
            int fileindex = _index % 8;
            return new string(new char[] { (char)('a' + fileindex) });
        }
        internal string GetRankString()
        {
            if (_index > 63) return "-";
            int rankindex = _index / 8;
            return new string(new char[] { (char)('1' + rankindex) });
        }
    }
}
