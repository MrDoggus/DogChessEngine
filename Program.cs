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
            int tempIndex;
            //Board temp2;

            switch (_board[index])
            {
                case Pieces.Empty:
                    return boards.ToArray();
                case Pieces.WPawn:
                    //If not white's turn
                    if (!Turn)
                    {
                        return boards.ToArray();
                    }
                    if (_board[index + 8] == Pieces.Empty)
                    {
                        //Removes pawn from its current position
                        temp = DeepCopy();
                        temp._board[index] = Pieces.Empty;

                        //Pawn promotion
                        if (index > 47)
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
                        else
                        {
                            
                            temp._board[index + 8] = Pieces.WPawn;
                            boards.Add(temp.DeepCopy());
                        }
                    }
                    //Dont forget enPassant
                    if ((_board[index + 9] != Pieces.Empty || index + 9 == EnPassant.Value.file + EnPassant.Value.rank * 8) && (index % 8) != 7)
                    {
                        //Removes pawn from its current position
                        temp = DeepCopy();
                        temp._board[index] = Pieces.Empty;

                        //Pawn promotion
                        if (index > 47)
                        {
                            temp._board[index + 9] = Pieces.WBishop;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 9] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 9] = Pieces.WRook;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 9] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }
                        else
                        {
                            temp._board[index + 9] = Pieces.WPawn;
                            if (index + 9 == EnPassant.Value.file + EnPassant.Value.rank * 8)
                            {
                                temp._board[index + 1] = Pieces.Empty;
                            }
                            boards.Add(temp.DeepCopy());
                        }
                    }
                    if ((_board[index + 7] != Pieces.Empty || index + 9 == EnPassant.Value.file + EnPassant.Value.rank * 8) && (index % 8) != 0)
                    {
                        //Removes pawn from its current position
                        temp = DeepCopy();
                        temp._board[index] = Pieces.Empty;

                        //Pawn promotion
                        if (index > 47)
                        {
                            temp._board[index + 7] = Pieces.WBishop;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 7] = Pieces.WKnight;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 7] = Pieces.WRook;
                            boards.Add(temp.DeepCopy());

                            temp._board[index + 7] = Pieces.WQueen;
                            boards.Add(temp.DeepCopy());
                        }
                        else
                        {
                            temp._board[index + 7] = Pieces.WPawn;
                            if (index + 7 == EnPassant.Value.file + EnPassant.Value.rank * 8)
                            {
                                temp._board[index - 1] = Pieces.Empty;
                            }
                            boards.Add(temp.DeepCopy());
                        }
                    }
                    if (index < 16 && _board[index + 16] == Pieces.Empty && _board[index + 8] == Pieces.Empty)
                    {
                        //Removes pawn from its current position
                        temp = DeepCopy();
                        temp._board[index] = Pieces.Empty;
                        temp._board[index + 16] = Pieces.WPawn;
                        temp.EnPassant = new ChessCoord(index % 8, (index + 8) / 8);

                        boards.Add(temp.DeepCopy());
                    }
                    break;
                case Pieces.WRook:
                    //If not white's turn
                    if (!Turn)
                    {
                        return boards.ToArray();
                    }
                    //Moving rook to the left
                    if (index % 8 != 0)
                    {
                        /*
                        temp = DeepCopy();
                        temp._board[index] = Pieces.Empty;

                        tempIndex = index;
                        do
                        {
                            index -= 1;
                        } while (true);
                        */
                    }
                    break;
                case Pieces.WKnight:
                    //If not white's turn
                    if (!Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.WBishop:
                    //If not white's turn
                    if (!Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.WQueen:
                    //If not white's turn
                    if (!Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.WKing:
                    //If not white's turn
                    if (!Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.BPawn:
                    //If not blacks's turn
                    if (Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.BRook:
                    //If not blacks's turn
                    if (Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.BKnight:
                    //If not blacks's turn
                    if (Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.BBishop:
                    //If not blacks's turn
                    if (Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.BQueen:
                    //If not blacks's turn
                    if (Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                case Pieces.BKing:
                    //If not blacks's turn
                    if (Turn)
                    {
                        return boards.ToArray();
                    }
                    break;
                default:
                    break;
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