using System;
using System.Numerics;

namespace DogChessEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    enum Pieces
    {
        Empty,

        WPawn,
        WRook,
        WKnight,
        WBishop,
        WQueen,
        WKing,

        BPawn,
        BRook,
        BKnight,
        BBishop,
        BQueen,
        BKing
    }

    /// <summary>
    /// Stores a coordinate
    /// </summary>
    struct ChessCoord
    {
        public ChessCoord(string str)
        {
            switch (str[0])
            {
                case 'a':
                    file = 0;
                    break;
                case 'b':
                    file = 1;
                    break;
                case 'c':
                    file = 2;
                    break;
                case 'd':
                    file = 3;
                    break;
                case 'e':
                    file = 4;
                    break;
                case 'f':
                    file = 5;
                    break;
                case 'g':
                    file = 6;
                    break;
                case 'h':
                    file = 7;
                    break;
                case 'A':
                    file = 0;
                    break;
                case 'B':
                    file = 1;
                    break;
                case 'C':
                    file = 2;
                    break;
                case 'D':
                    file = 3;
                    break;
                case 'E':
                    file = 4;
                    break;
                case 'F':
                    file = 5;
                    break;
                case 'G':
                    file = 6;
                    break;
                case 'H':
                    file = 7;
                    break;
                default:
                    throw new FormatException("First character does not represent a file.");
            }

            rank = (byte)(str[1] - '0');
        }

        public ChessCoord(byte f, byte r)
        {
            if (f > 7 || r > 7)
            {
                throw new Exception("Coordate given is out of range of the coords on the chess board");
            }
            file = f;
            rank = r;
        }

        public ChessCoord(int f, int r)
        {
            if (f > 7 || r > 7)
            {
                throw new Exception("Coordate given is out of range of the coords on the chess board");
            }
            file = (byte)f;
            rank = (byte)r;
        }

        byte file;
        byte rank;
    }

    class Board
    {
        /// <summary>
        /// If it is white's turn to move, "Turn" is set to true.
        /// </summary>
        public bool Turn;

        /// <summary>
        /// True if white can castle king-side
        /// </summary>
        public bool WKCastle;

        /// <summary>
        /// True if white can castle queen-side
        /// </summary>
        public bool WQCastle;

        /// <summary>
        /// True if black can castle king-side
        /// </summary>
        public bool BKCastle;

        /// <summary>
        /// True if black can castle queen-side
        /// </summary>
        public bool BQCastle;

        /// <summary>
        /// If EnPassant is available the target square coord will be stored here
        /// </summary>
        public ChessCoord? EnPassant;

        /// <summary>
        /// Number of halfmoves since the last capture or pawn advance. 
        /// </summary>
        public byte HalfmoveClock;

        /// <summary>
        /// Counts the number of full moves. 
        /// </summary>
        public int FullMoveCount;

        //Stores all of the pieces on the board
        private Pieces[] _board = new Pieces[64];

        /// <summary>
        /// Two dimensional array of pieces
        /// </summary>
        /// <param name="files"></param>
        /// <param name="ranks"></param>
        /// <returns></returns>
        public Pieces this[int files, int ranks]
        {
            get
            {
                return (_board[files + ranks * 8]);
            }
            set
            {
                _board[files + ranks * 8] = value;
            }
        }

        /// <summary>
        /// Creates a board object using an one-dimensional array of 64 pieces.
        /// </summary>
        /// <param name="pieces"></param>
        public Board(Pieces[] pieces, bool turn, bool wkCaslte = true, bool wqCastle = true, bool bkCastle = true, bool bqCastle = true, ChessCoord? enPassant = null, byte halfMoveClock = 0, int fullMoveCount = 0)
        {
            if (pieces.Length != 64)
            {
                throw new Exception("Array of pieces is not 64.");
            }

            _board = pieces;

            Turn = turn;
            WKCastle = wkCaslte;
            WQCastle = wkCaslte;
            BKCastle = bkCastle;
            BQCastle = bqCastle;
            EnPassant = enPassant;
            HalfmoveClock = halfMoveClock;
            FullMoveCount = fullMoveCount;
        }

        /// <summary>
        /// Creates a board object using a two-dimensional array of 8 by 8 pieces.
        /// </summary>
        /// <param name="pieces"></param>
        public Board(Pieces[,] pieces, bool turn, bool wkCaslte = true, bool wqCastle = true, bool bkCastle = true, bool bqCastle = true, ChessCoord? enPassant = null, byte halfMoveClock = 0, int fullMoveCount = 0)
        {
            if (pieces.GetLength(0) != 8 || pieces.GetLength(1) != 8)
            {
                throw new Exception("Dimensions of array of pieces is not 8 by 8.");
            }

            //Loops through ranks
            for (int i = 0; i < 8; i++)
            {
                //Loops through files
                for (int j = 0; j < 8; j++)
                {
                    _board[j + i * 8] = pieces[i, j];
                }
            }

            Turn = turn;
            WKCastle = wkCaslte;
            WQCastle = wkCaslte;
            BKCastle = bkCastle;
            BQCastle = bqCastle;
            EnPassant = enPassant;
            HalfmoveClock = halfMoveClock;
            FullMoveCount = fullMoveCount;
        }

        /// <summary>
        /// Returns the starting position
        /// </summary>
        /// <returns></returns>
        public static Board StartingPosition()
        {
            return new Board(new Pieces[64] { 
                Pieces.WRook, Pieces.WKnight, Pieces.WBishop, Pieces.WQueen, Pieces.WKing, Pieces.WBishop, Pieces.WKnight, Pieces.WRook,
                Pieces.WPawn, Pieces.WPawn, Pieces.WPawn, Pieces.WPawn, Pieces.WPawn, Pieces.WPawn, Pieces.WPawn, Pieces.WPawn,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.BPawn, Pieces.BPawn, Pieces.BPawn, Pieces.BPawn, Pieces.BPawn, Pieces.BPawn, Pieces.BPawn, Pieces.BPawn,
                Pieces.BRook, Pieces.BKnight, Pieces.BBishop, Pieces.BQueen, Pieces.BKing, Pieces.BBishop, Pieces.BKnight, Pieces.BRook }, true);
        }

        /// <summary>
        /// Returns an empty board
        /// </summary>
        /// <returns></returns>
        public static Board Empty()
        {
            return new Board(new Pieces[64] {
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty,
                Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty, Pieces.Empty }, true);
        }

        public Pieces GetPiece(ChessCoord coord)
        {

        }

        public Pieces GetPiece(byte file, byte rank)
        {

        }

        public Pieces GetPiece(int file, int rank)
        {

        }

        public static Board FromFEN(string fen)
        {

        }

        public string ToFEN()
        {

        }
    }
}
