using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;

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

    static class PiecesMethods
    {
        public static bool IsBlack(this Pieces piece)
        {
            if (piece == Pieces.BPawn || piece == Pieces.BRook || piece == Pieces.BKnight || piece == Pieces.BBishop || piece == Pieces.BQueen || piece == Pieces.BKing)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsWhite(this Pieces piece)
        {
            if (piece == Pieces.WPawn || piece == Pieces.WRook || piece == Pieces.WKnight || piece == Pieces.WBishop || piece == Pieces.WQueen || piece == Pieces.WKing)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
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

        public byte file;
        public byte rank;

        public override string ToString()
        {
            switch (file)
            {
                case 0:
                    return 'a' + ('0' + rank).ToString();
                case 1:
                    return 'b' + ('0' + rank).ToString();
                case 2:
                    return 'c' + ('0' + rank).ToString();
                case 3:
                    return 'd' + ('0' + rank).ToString();
                case 4:
                    return 'e' + ('0' + rank).ToString();
                case 5:
                    return 'f' + ('0' + rank).ToString();
                case 6:
                    return 'g' + ('0' + rank).ToString();
                case 7:
                    return 'h' + ('0' + rank).ToString();
                default:
                    return "";
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(file, rank);
        }

        public override bool Equals(object obj)
        {
            return obj is ChessCoord coord &&
                   file == coord.file &&
                   rank == coord.rank;
        }
    }

    [Serializable]
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
        /// True if white castled king-side
        /// </summary>
        public bool WKCastled;

        /// <summary>
        /// True if white castled queen-side
        /// </summary>
        public bool WQCastled;

        /// <summary>
        /// True if black castled king-side
        /// </summary>
        public bool BKCastled;

        /// <summary>
        /// True if black castled queen-side
        /// </summary>
        public bool BQCastled;

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
        public Pieces[] _board = new Pieces[64];

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public static Board FromFEN(string fen)
        {
            string[] fields = fen.Split(' ');
            string[] ranks = fields[0].Split('/');

            //Stores the board and current board position
            Pieces[] board = new Pieces[64];
            int boardPos = 0;

            //Whos turn is it
            bool turn;

            //Castling rights
            bool wkCastle = true;
            bool wqCastle = true;
            bool bkCastle = true;
            bool bqCastle = true;

            //En passant
            ChessCoord? enPassant = null;

            //Halfmove clock
            int halfmoveClock;

            //Fullmove count;
            int fullmoveCount;

            //Stores number of empty squares on a rank
            int empty;

            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < ranks[i].Length; j++)
                {
                    switch (ranks[i][j])
                    {
                        case 'R':
                            board[boardPos] = Pieces.WRook;
                            break;
                        case 'N':
                            board[boardPos] = Pieces.WKnight;
                            break;
                        case 'B':
                            board[boardPos] = Pieces.WBishop;
                            break;
                        case 'Q':
                            board[boardPos] = Pieces.WQueen;
                            break;
                        case 'K':
                            board[boardPos] = Pieces.WKing;
                            break;
                        case 'P':
                            board[boardPos] = Pieces.WPawn;
                            break;
                        case 'r':
                            board[boardPos] = Pieces.BRook;
                            break;
                        case 'n':
                            board[boardPos] = Pieces.BKnight;
                            break;
                        case 'b':
                            board[boardPos] = Pieces.BBishop;
                            break;
                        case 'q':
                            board[boardPos] = Pieces.BQueen;
                            break;
                        case 'k':
                            board[boardPos] = Pieces.BKing;
                            break;
                        case 'p':
                            board[boardPos] = Pieces.BPawn;
                            break;
                        default:
                            empty = ranks[i][j] - '0';

                            if (empty < 0 || empty > 8)
                            {
                                throw new FormatException("FEN string is not correct");
                            }

                            boardPos += empty;
                            break;
                    }

                    boardPos++;
                }
            }

            //Gets to active color
            if (fields[1][0] == 'w')
            {
                turn = true;
            }
            else if (fields[1][0] == 'b')
            {
                turn = false;
            }
            else
            {
                throw new FormatException("FEN string is not formated correctly");
            }

            //Gets castling rights
            if (fields[2][0] == '-')
            {
                wkCastle = false;
                wqCastle = false;
                bkCastle = false;
                bqCastle = false;
            }
            else
            {
                for (int i = 0; i < fields[2].Length; i++)
                {
                    if (fields[2][i] == 'W')
                    {
                        wkCastle = true;
                    }
                    else if (fields[2][i] == 'Q')
                    {
                        wqCastle = true;
                    }
                    else if (fields[2][i] == 'w')
                    {
                        bkCastle = true;
                    }
                    else if (fields[2][i] == 'q')
                    {
                        bqCastle = true;
                    }
                    else
                    {
                        throw new FormatException("FEN string is not formated correctly");
                    }
                }
            }

            //Gets en passant availability
            if (fields[3][0] == '-')
            {
                enPassant = null;
            }
            else
            {
                enPassant = new ChessCoord(fields[3]);
            }

            halfmoveClock = Convert.ToInt32(fields[4]);
            fullmoveCount = Convert.ToInt32(fields[5]);

            return new Board(board, turn, wkCastle, wqCastle, bkCastle, bqCastle, enPassant, (byte)halfmoveClock, fullmoveCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public Pieces GetPiece(ChessCoord coord)
        {
            return _board[coord.file + coord.rank * 8];
        }

        public Pieces GetPiece(byte file, byte rank)
        {
            return _board[file + rank * 8];
        }

        public Pieces GetPiece(int file, int rank)
        {
            return _board[file + rank * 8];
        }

        public string ToFEN()
        {
            string fen = "";

            int emptyCount = 0;
            //Loops through ranks
            for (int i = 7; i >= 0; i--)
            {
                if (i != 7)
                {
                    fen += '/';
                }

                //Loops through files
                for (int j = 0; j < 8; j++)
                {
                    if (emptyCount != 0 && GetPiece(j, i) != Pieces.Empty)
                    {
                        fen += '0' + emptyCount;
                        emptyCount = 0;
                    }

                    switch (GetPiece(j, i))
                    {
                        case Pieces.Empty:
                            emptyCount++;
                            break;
                        case Pieces.WPawn:
                            emptyCount += 'P';
                            break;
                        case Pieces.BPawn:
                            emptyCount += 'p';
                            break;
                        case Pieces.WRook:
                            emptyCount += 'R';
                            break;
                        case Pieces.WKnight:
                            emptyCount += 'N';
                            break;
                        case Pieces.WBishop:
                            emptyCount += 'B';
                            break;
                        case Pieces.WQueen:
                            emptyCount += 'Q';
                            break;
                        case Pieces.WKing:
                            emptyCount += 'K';
                            break;
                        case Pieces.BRook:
                            emptyCount += 'r';
                            break;
                        case Pieces.BKnight:
                            emptyCount += 'n';
                            break;
                        case Pieces.BBishop:
                            emptyCount += 'b';
                            break;
                        case Pieces.BQueen:
                            emptyCount += 'q';
                            break;
                        case Pieces.BKing:
                            emptyCount += 'k';
                            break;
                    }
                }
            }

            //Active color
            fen += ' ';
            if (Turn)
            {
                fen += 'w';
            }
            else
            {
                fen += 'b';
            }

            //Castling rights
            fen += ' ';
            if (WKCastle)
            {
                fen += 'K';
            }
            if (WQCastle)
            {
                fen += 'Q';
            }
            if (BKCastle)
            {
                fen += 'k';
            }
            if (BQCastle)
            {
                fen += 'q';
            }

            //EnPassant?
            fen += ' ';
            if (EnPassant == null)
            {
                fen += '-';
            }
            else
            {
                fen += EnPassant.ToString();
            }

            //Half move count and fullmove count
            fen += ' ' + HalfmoveClock.ToString() + ' ' + FullMoveCount.ToString();

            return fen;
        }

        public Board[] Moves()
        {

        }

        public Board[] Moves(ChessCoord chessCoord)
        {

        }

        public Board[] Moves(int index)
        {
            List<Board> boards = new List<Board>();
            Board temp;
            Board temp2;

            int tempIndex;
            //Board temp2;

            switch (_board[index])
            {
                case Pieces.Empty:
                    break;
                case Pieces.WPawn:
                    //If not white's turn
                    if (!Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.BKCastled = false;
                    temp2.BQCastled = false;
                    temp2.Turn = false;
                    temp2.EnPassant = null;
                    temp2.HalfmoveClock = 0;
                    
                    //Moving pawn one square up
                    if (_board[index + 8] == Pieces.Empty)
                    {
                        temp = temp2.DeepCopy();

                        //Pawn promotion
                        if (index > 55)
                        {
                            temp._board[index + 8] = Pieces.WBishop;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 8] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 8] = Pieces.WRook;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 8] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }
                        //If not promoting
                        else
                        {
                            temp._board[index + 8] = Pieces.WPawn;
                            boards.Add(temp.DeepCopy());
                        }
                    }
                    
                    //Taking to the right
                    if (_board[index + 9].IsBlack() && (index % 8) != 7)
                    {
                        temp = temp2.DeepCopy();

                        //Pawn promotion
                        if (index > 47)
                        {

                            temp._board[index + 9] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 9] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }//If not promoting
                        else
                        {
                            temp._board[index + 9] = Pieces.WPawn;
                            boards.Add(temp.DeepCopy());
                        }
                    }

                    //Taking to the right en passant
                    if (index + 9 == EnPassant.Value.file + EnPassant.Value.rank * 8)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 9] = Pieces.WPawn;
                        temp._board[index + 1] = Pieces.Empty;
                        boards.Add(temp.DeepCopy());
                    }
                    
                    //Taking to the left + en passant
                    if (_board[index + 7].IsBlack() && (index % 8) != 0)
                    {
                        temp = temp2.DeepCopy();

                        //Pawn promotion
                        if (index > 47)
                        {
                            temp._board[index + 7] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 7] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }
                        //If not promoting
                        else
                        {
                            temp._board[index + 7] = Pieces.WPawn;
                            boards.Add(temp.DeepCopy());
                        }
                    }

                    //Taking to the left en passant
                    if (index + 9 == EnPassant.Value.file + EnPassant.Value.rank * 8)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 7] = Pieces.WPawn;
                        temp._board[index - 1] = Pieces.Empty;
                        boards.Add(temp.DeepCopy());
                    }
                    
                    //Moving up two squares
                    if (index < 16 && _board[index + 16] == Pieces.Empty && _board[index + 8] == Pieces.Empty)
                    {
                        temp = temp2.DeepCopy();

                        temp._board[index + 16] = Pieces.WPawn;
                        temp.EnPassant = new ChessCoord(index % 8, (index + 8) / 8);

                        boards.Add(temp.DeepCopy());
                    }
                    
                    break;

                case Pieces.WRook:
                    //If not white's turn
                    if (!Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.BKCastled = false;
                    temp2.BQCastled = false;
                    temp2.Turn = false;
                    temp2.EnPassant = null;
                    temp2.HalfmoveClock++;

                    //Moving rook to the left (if not on the far left side of the board)
                    if (index % 8 != 0 && !_board[index - 1].IsWhite())
                    {
                        tempIndex = index;

                        //Loops until white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WRook;
                            if (index == 7)
                            {
                                temp.WKCastle = false;
                            }

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 0 && !_board[index - 1].IsWhite());
                    }
                    
                    //Moving to the right
                    if (index % 8 != 7 && !_board[index + 1].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WRook;
                            if (index == 0)
                            {
                                temp.WQCastle = false;
                            }

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 7 && !_board[index + 1].IsWhite());
                    }
                    
                    //Moving up
                    if (index / 8 != 7 && !_board[index + 8].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WRook;
                            if (index == 0)
                            {
                                temp.WQCastle = false;
                            }
                            else if (index == 7)
                            {
                                temp.WKCastle = false;
                            }
                            
                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 7 && !_board[index + 8].IsWhite());
                    }
                    
                    //Moving down
                    if (index / 8 != 0 && !_board[index - 8].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WRook;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 0 && !_board[index - 8].IsWhite());
                    }
                    
                    break;

                case Pieces.WKnight:
                    //If not white's turn
                    if (!Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.BKCastled = false;
                    temp2.BQCastled = false;
                    temp2.Turn = false;
                    temp2.EnPassant = null;
                    temp2.HalfmoveClock++;

                    // Quad 2 lower
                    if (((index - 2) % 8 < index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 6].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 6] = Pieces.WKnight;

                        if (_board[index + 6].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }
                    
                    // Quad 2 higher
                    if (((index - 1) % 8 < index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 15].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 15] = Pieces.WKnight;

                        if (_board[index + 15].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }
                    
                    // Quad 1 higher
                    if (((index + 1) % 8 > index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 17].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 17] = Pieces.WKnight;

                        if (_board[index + 17].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }
                    
                    // Quad 1 lower
                    if (((index + 2) % 8 > index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 10].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 10] = Pieces.WKnight;

                        if (_board[index + 10].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }
                    
                    // Quad 4 higher
                    if (((index + 2) % 8 > index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 6].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 6] = Pieces.WKnight;

                        if (_board[index - 6].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }
                    
                    // Quad 4 lower
                    if (((index + 1) % 8 > index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 15].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 15] = Pieces.WKnight;

                        if (_board[index - 15].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }
                    
                    // Quad 3 lower
                    if (((index - 1) % 8 < index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 17].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 17] = Pieces.WKnight;

                        if (_board[index - 17].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }
                    
                    // Quad 3 higher
                    if (((index - 2) % 8 < index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 10].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 10] = Pieces.WKnight;

                        if (_board[index - 10].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    break;

                case Pieces.WBishop:
                    //If not white's turn
                    if (!Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.BKCastled = false;
                    temp2.BQCastled = false;
                    temp2.Turn = false;
                    temp2.EnPassant = null;
                    temp2.HalfmoveClock++;

                    //Moving bishop in quad 1 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WBishop;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 7 && !_board[tempIndex + 9].IsWhite());
                    }

                    //Moving bishop in quad 2 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WBishop;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 7 && !_board[tempIndex + 7].IsWhite());
                    }

                    //Moving bishop in quad 3 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WBishop;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 0 && !_board[tempIndex - 9].IsWhite());
                    }

                    //Moving bishop in quad 1 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WBishop;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsWhite());
                    }

                    break;

                case Pieces.WQueen:
                    //If not white's turn
                    if (!Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.BKCastled = false;
                    temp2.BQCastled = false;
                    temp2.Turn = false;
                    temp2.EnPassant = null;
                    temp2.HalfmoveClock++;

                    //Moving queen to the left (if not on the far left side of the board)
                    if (index % 8 != 0 && !_board[index - 1].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 0 && !_board[index - 1].IsWhite());
                    }

                    //Moving to the right
                    if (index % 8 != 7 && !_board[index + 1].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 7 && !_board[index + 1].IsWhite());
                    }

                    //Moving up
                    if (index / 8 != 7 && !_board[index + 8].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 7 && !_board[index + 8].IsWhite());
                    }

                    //Moving down
                    if (index / 8 != 0 && !_board[index - 8].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 0 && !_board[index - 8].IsWhite());
                    }

                    //Moving queen in quad 1 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 7 && !_board[tempIndex + 9].IsWhite());
                    }

                    //Moving queen in quad 2 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 7 && !_board[tempIndex + 7].IsWhite());
                    }

                    //Moving queen in quad 3 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 0 && !_board[tempIndex - 9].IsWhite());
                    }

                    //Moving queen in quad 4 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsWhite())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.WQueen;

                            if (_board[tempIndex].IsBlack())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsWhite());
                    }

                    break;

                case Pieces.WKing:
                    //If not white's turn
                    if (!Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.BKCastled = false;
                    temp2.BQCastled = false;
                    temp2.Turn = false;
                    temp2.EnPassant = null;
                    temp2.HalfmoveClock++;
                    temp2.WKCastle = false;
                    temp2.WQCastle = false;

                    //Moving king up
                    if (index / 8 != 7 && !_board[index + 8].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 8] = Pieces.WKing;

                        if (_board[index + 8].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king to the left
                    if (index % 8 != 0 && !_board[index - 1].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 1] = Pieces.WKing;

                        if (_board[index - 1].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king to down
                    if (index / 8 != 0 && !_board[index - 8].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 8] = Pieces.WKing;

                        if (_board[index - 8].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king to the right
                    if (index % 8 != 7 && !_board[index + 1].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 1] = Pieces.WKing;

                        if (_board[index + 1].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 1
                    if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 9] = Pieces.WKing;

                        if (_board[index + 9].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 2
                    if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 7] = Pieces.WKing;

                        if (_board[index + 7].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 3
                    if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 9] = Pieces.WKing;

                        if (_board[index - 9].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 4
                    if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsWhite())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 7] = Pieces.WKing;

                        if (_board[index - 7].IsBlack())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Castleing king side
                    if (_board[index + 1] == Pieces.Empty && _board[index + 2] == Pieces.Empty && WKCastle)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 2] = Pieces.WKing;
                        temp._board[index + 1] = Pieces.WRook;
                        temp._board[index + 3] = Pieces.Empty;
                        temp.WKCastled = true;
                        boards.Add(temp.DeepCopy());
                    }

                    //Castleing queen side
                    if (_board[index - 1] == Pieces.Empty && _board[index - 2] == Pieces.Empty && _board[index - 3] == Pieces.Empty && WQCastle)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 2] = Pieces.WKing;
                        temp._board[index - 1] = Pieces.WRook;
                        temp._board[index - 4] = Pieces.Empty;
                        temp.WQCastled = true;
                        boards.Add(temp.DeepCopy());
                    }

                    break;

                case Pieces.BPawn:
                    //If not blacks's turn
                    if (Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.WKCastled = false;
                    temp2.WQCastled = false;
                    temp2.Turn = true;
                    temp2.EnPassant = null;
                    temp2.FullMoveCount++;
                    temp2.HalfmoveClock = 0;

                    //Moving pawn one square down
                    if (_board[index - 8] == Pieces.Empty)
                    {
                        temp = temp2.DeepCopy();

                        //Pawn promotion
                        if (index < 8)
                        {
                            temp._board[index - 8] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index - 8] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }
                        //If not promoting
                        else
                        {
                            temp._board[index - 8] = Pieces.WPawn;
                            boards.Add(temp.DeepCopy());
                        }
                    }
                    
                    //Taking to the left
                    if (_board[index - 9].IsBlack() && (index % 8) != 0)
                    {
                        temp = temp2.DeepCopy();

                        //Pawn promotion
                        if (index < 8)
                        {
                            temp._board[index - 9] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index - 9] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }//If not promoting
                        else
                        {
                            temp._board[index - 9] = Pieces.WPawn;
                            boards.Add(temp.DeepCopy());
                        }
                    }

                    //Taking to the left en passant
                    if (index - 9 == EnPassant.Value.file + EnPassant.Value.rank * 8)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 9] = Pieces.WPawn;
                        temp._board[index - 1] = Pieces.Empty;
                        boards.Add(temp.DeepCopy());
                    }
                    
                    //Taking to the right
                    if (_board[index - 7].IsBlack() && (index % 8) != 7)
                    {
                        temp = temp2.DeepCopy();

                        //Pawn promotion
                        if (index < 8)
                        {
                            temp._board[index - 7] = Pieces.WBishop;
                            boards.Add(temp.DeepCopy());

                            temp._board[index - 7] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index - 7] = Pieces.WRook;
                            boards.Add(temp.DeepCopy());

                            temp._board[index - 7] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }
                        //If not promoting
                        else
                        {
                            temp._board[index - 7] = Pieces.WPawn;
                            boards.Add(temp.DeepCopy());
                        }
                    }
                    
                    //Taking to the right en passant
                    if (index - 7 == EnPassant.Value.file + EnPassant.Value.rank * 8)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 7] = Pieces.WPawn;
                        temp._board[index + 1] = Pieces.Empty;
                        boards.Add(temp.DeepCopy());
                    }

                    //Moving up two squares
                    if (index < 16 && _board[index + 16] == Pieces.Empty && _board[index + 8] == Pieces.Empty)
                    {
                        temp = temp2.DeepCopy();

                        temp._board[index + 16] = Pieces.WPawn;
                        temp.EnPassant = new ChessCoord(index % 8, (index + 8) / 8);

                        boards.Add(temp.DeepCopy());
                    }

                    break;

                case Pieces.BRook:
                    //If not blacks's turn
                    if (Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.WKCastled = false;
                    temp2.WQCastled = false;
                    temp2.Turn = true;
                    temp2.EnPassant = null;
                    temp2.FullMoveCount++;
                    temp2.HalfmoveClock++;

                    //Moving rook to the left (if not on the far left side of the board)
                    if (index % 8 != 0 && !_board[index - 1].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BRook;

                            if (index == 63)
                            {
                                temp.BKCastle = false;
                            }

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 0 && !_board[index - 1].IsBlack());
                    }

                    //Moving to the right
                    if (index % 8 != 7 && !_board[index + 1].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BRook;

                            if (index == 56)
                            {
                                temp.BQCastle = false;
                            }

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 7 && !_board[index + 1].IsBlack());
                    }

                    //Moving up
                    if (index / 8 != 7 && !_board[index + 8].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BRook;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 7 && !_board[index + 8].IsBlack());
                    }

                    //Moving down
                    if (index / 8 != 0 && !_board[index - 8].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BRook;

                            if (index == 56)
                            {
                                temp.BQCastle = false;
                            }
                            else if (index == 63)
                            {
                                temp.BKCastle = false;
                            }

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 0 && !_board[index - 8].IsBlack());
                    }

                    break;

                case Pieces.BKnight:
                    //If not blacks's turn
                    if (Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.WKCastled = false;
                    temp2.WQCastled = false;
                    temp2.Turn = true;
                    temp2.EnPassant = null;
                    temp2.FullMoveCount++;
                    temp2.HalfmoveClock++;

                    // Quad 2 lower
                    if (((index - 2) % 8 < index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 6].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 6] = Pieces.BKnight;

                        if (_board[index + 6].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    // Quad 2 higher
                    if (((index - 1) % 8 < index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 15].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index +  15] = Pieces.BKnight;

                        if (_board[index + 15].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    // Quad 1 higher
                    if (((index + 1) % 8 > index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 17].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 17] = Pieces.BKnight;

                        if (_board[index + 17].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    // Quad 1 lower
                    if (((index + 2) % 8 > index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 10].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 10] = Pieces.BKnight;

                        if (_board[index + 10].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    // Quad 4 higher
                    if (((index + 2) % 8 > index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 6].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 6] = Pieces.BKnight;

                        if (_board[index - 6].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    // Quad 4 lower
                    if (((index + 1) % 8 > index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 15].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 15] = Pieces.BKnight;

                        if (_board[index - 15].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    // Quad 3 lower
                    if (((index - 1) % 8 < index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 17].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 17] = Pieces.BKnight;

                        if (_board[index - 17].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    // Quad 3 higher
                    if (((index - 2) % 8 < index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 10].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 10] = Pieces.BKnight;

                        if (_board[index - 10].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    break;

                case Pieces.BBishop:
                    //If not blacks's turn
                    if (Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.WKCastled = false;
                    temp2.WQCastled = false;
                    temp2.Turn = true;
                    temp2.EnPassant = null;
                    temp2.FullMoveCount++;
                    temp2.HalfmoveClock++;

                    //Moving bishop in quad 1 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BBishop;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 7 && !_board[tempIndex + 9].IsBlack());
                    }

                    //Moving bishop in quad 2 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BBishop;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 7 && !_board[tempIndex + 7].IsBlack());
                    }

                    //Moving bishop in quad 3 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BBishop;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 0 && !_board[tempIndex - 9].IsBlack());
                    }

                    //Moving bishop in quad 1 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BBishop;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsBlack());
                    }

                    break;

                case Pieces.BQueen:
                    //If not blacks's turn
                    if (Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.WKCastled = false;
                    temp2.WQCastled = false;
                    temp2.Turn = true;
                    temp2.EnPassant = null;
                    temp2.FullMoveCount++;
                    temp2.HalfmoveClock++;

                    //Moving queen to the left (if not on the far left side of the board)
                    if (index % 8 != 0 && !_board[index - 1].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 0 && !_board[index - 1].IsBlack());
                    }

                    //Moving to the right
                    if (index % 8 != 7 && !_board[index + 1].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 1;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index % 8 != 7 && !_board[index + 1].IsBlack());
                    }

                    //Moving up
                    if (index / 8 != 7 && !_board[index + 8].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 7 && !_board[index + 8].IsBlack());
                    }

                    //Moving down
                    if (index / 8 != 0 && !_board[index - 8].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 8;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (index / 8 != 0 && !_board[index - 8].IsBlack());
                    }

                    //Moving queen in quad 1 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 7 && !_board[tempIndex + 9].IsBlack());
                    }

                    //Moving queen in quad 2 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex += 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 7 && !_board[tempIndex + 7].IsBlack());
                    }

                    //Moving queen in quad 3 (if not on the far left side of the board)
                    if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 9;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 0 && tempIndex / 8 != 0 && !_board[tempIndex - 9].IsBlack());
                    }

                    //Moving queen in quad 4 (if not on the far left side of the board)
                    if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsBlack())
                    {
                        tempIndex = index;

                        //Loops untill white piece is found, edge of board is found, or black piece is found
                        do
                        {
                            tempIndex -= 7;

                            temp = temp2.DeepCopy();
                            temp._board[tempIndex] = Pieces.BQueen;

                            if (_board[tempIndex].IsWhite())
                            {
                                temp.HalfmoveClock = 0;
                                boards.Add(temp.DeepCopy());
                                break;
                            }

                            boards.Add(temp.DeepCopy());

                        } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsBlack());
                    }

                    break;
                case Pieces.BKing:
                    //If not blacks's turn
                    if (Turn)
                    {
                        break;
                    }

                    temp2 = DeepCopy();
                    temp2._board[index] = Pieces.Empty;
                    temp2.WKCastled = false;
                    temp2.WQCastled = false;
                    temp2.Turn = true;
                    temp2.EnPassant = null;
                    temp2.FullMoveCount++;
                    temp2.HalfmoveClock++;
                    temp2.BKCastle = false;
                    temp2.BQCastle = false;

                    //Moving king up
                    if (index / 8 != 7 && !_board[index + 8].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 8] = Pieces.BKing;

                        if (_board[index + 8].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king to the left
                    if (index % 8 != 0 && !_board[index - 1].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 1] = Pieces.BKing;

                        if (_board[index - 1].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king to down
                    if (index / 8 != 0 && !_board[index - 8].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 8] = Pieces.BKing;

                        if (_board[index - 8].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king to the right
                    if (index % 8 != 7 && !_board[index + 1].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 1] = Pieces.BKing;

                        if (_board[index + 1].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 1
                    if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 9] = Pieces.BKing;

                        if (_board[index + 9].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 2
                    if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 7] = Pieces.BKing;

                        if (_board[index + 7].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 3
                    if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 9] = Pieces.BKing;

                        if (_board[index - 9].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Moving king into quad 4
                    if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsBlack())
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 7] = Pieces.BKing;

                        if (_board[index - 7].IsWhite())
                        {
                            temp.HalfmoveClock = 0;
                        }

                        boards.Add(temp.DeepCopy());
                    }

                    //Castleing king side
                    if (_board[index + 1] == Pieces.Empty && _board[index + 2] == Pieces.Empty && WKCastle)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index + 2] = Pieces.BKing;
                        temp._board[index + 1] = Pieces.BRook;
                        temp._board[index + 3] = Pieces.Empty;
                        temp.BKCastled = true;
                        boards.Add(temp.DeepCopy());
                    }
                    //Castleing queen side
                    if (_board[index - 1] == Pieces.Empty && _board[index - 2] == Pieces.Empty && _board[index - 3] == Pieces.Empty && WQCastle)
                    {
                        temp = temp2.DeepCopy();
                        temp._board[index - 2] = Pieces.BKing;
                        temp._board[index - 1] = Pieces.BRook;
                        temp._board[index - 4] = Pieces.Empty;
                        temp.BQCastled = true;
                        boards.Add(temp.DeepCopy());
                    }

                    break;

                default:
                    break;

                //Update other 
            }
        }

        public int MaterialEval()
        {

        }

        public double ControlEval()
        {

        }

        public Board DeepCopy()
        {
            //https://stackoverflow.com/questions/1031023/copy-a-class-c-sharp
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;
                return (Board)formatter.Deserialize(ms);
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}