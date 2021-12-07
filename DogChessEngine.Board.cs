using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DogChessEngine
{
    namespace Board
    {
        /// <summary>
        /// Used to describe squares on a chess board.
        /// </summary>
        enum Pieces
        {
            //Empty square
            Empty,

            //White pieces
            WPawn,
            WRook,
            WKnight,
            WBishop,
            WQueen,
            WKing,

            //Black pieces
            BPawn,
            BRook,
            BKnight,
            BBishop,
            BQueen,
            BKing
        }

        /// <summary>
        /// Contains methods for the Pieces enum
        /// </summary>
        static class PiecesMethods
        {
            /// <summary>
            /// Returns true if the given piece is black.
            /// </summary>
            /// <param name="piece"></param>
            /// <returns></returns>
            public static bool IsBlack(this Pieces piece)
            {
                if (piece == Pieces.BPawn || piece == Pieces.BRook || piece == Pieces.BKnight || piece == Pieces.BBishop || piece == Pieces.BQueen 
                    || piece == Pieces.BKing)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Returns true if the given piece is white.
            /// </summary>
            /// <param name="piece"></param>
            /// <returns></returns>
            public static bool IsWhite(this Pieces piece)
            {
                if (piece == Pieces.WPawn || piece == Pieces.WRook || piece == Pieces.WKnight || piece == Pieces.WBishop || piece == Pieces.WQueen 
                    || piece == Pieces.WKing)
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
        /// Describes if the last move was a castle and how the castle occured
        /// </summary>
        enum Castle
        {
            //Castle did not happen
            Empty,

            //Castle did happen
            WKCastle,
            WQCastle,
            BKCastle,
            BQCastle
        }

        /// <summary>
        /// Contains methods for the Castle enum
        /// </summary>
        static class CastleMethods
        {
            /// <summary>
            /// Returns true if controlling given position would prevent castling.
            /// </summary>
            /// <param name="castle">Describes how a king was castled if a king did castle</param>
            /// <param name="pos">Position on the board to be checked</param>
            /// <returns></returns>
            public static bool CastleVurn(this Castle castle, ChessCoord pos)
            {
                //If castle did not occur.
                if (castle == Castle.Empty)
                {
                    return false;
                }
                //If white castled king side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.WKCastle && ((pos.rank == 0 && pos.file == 4) || (pos.rank == 0 && pos.file == 5)))
                {
                    return true;
                }
                //If white caslted queen side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.WQCastle && ((pos.rank == 0 && pos.file == 4) || (pos.rank == 0 && pos.file == 3)))
                {
                    return true;
                }
                //If black castled king side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.BKCastle && ((pos.rank == 7 && pos.file == 4) || (pos.rank == 7 && pos.file == 5)))
                {
                    return true;
                }
                //If black castled queen side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.BQCastle && ((pos.rank == 7 && pos.file == 4) || (pos.rank == 7 && pos.file == 3)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Returns true if controlling given position would prevent castling.
            /// </summary>
            /// <param name="castle"></param>
            /// <param name="pos">Position on chess board represented by the index of the board array.</param>
            /// <returns></returns>
            public static bool CastleVurn(this Castle castle, int pos)
            {
                //If castle did not occur.
                if (castle == Castle.Empty)
                {
                    return false;
                }
                //If white castled king side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.WKCastle && (pos == 4 || pos == 5))
                {
                    return true;
                }
                //If white caslted queen side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.WQCastle && ((pos == 4) || (pos == 3)))
                {
                    return true;
                }
                //If black castled king side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.BKCastle && ((pos == 60) || (pos == 61)))
                {
                    return true;
                }
                //If black castled queen side and if given position controls a square that would prevent a castle.
                else if (castle == Castle.BQCastle && ((pos == 60) || (pos == 59)))
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
        /// Stores a coordinate on a chess board
        /// </summary>
        struct ChessCoord
        {
            /// <summary>
            /// Gets a ChessCoord from a string.
            /// </summary>
            /// <param name="str">String used to get ChessCoord. </param>
            /// <exception cref="FormatException"></exception>
            public ChessCoord(string str)
            {
                //Gets the file
                file = str[0] switch
                {
                    'a' => 0,
                    'b' => 1,
                    'c' => 2,
                    'd' => 3,
                    'e' => 4,
                    'f' => 5,
                    'g' => 6,
                    'h' => 7,
                    'A' => 0,
                    'B' => 1,
                    'C' => 2,
                    'D' => 3,
                    'E' => 4,
                    'F' => 5,
                    'G' => 6,
                    'H' => 7,
                    _ => throw new FormatException("First character does not represent a file."),
                };

                //Gets the rank
                rank = (byte)(str[1] - '0');
                if (rank > 7)
                {
                    throw new FormatException("Second character does not represent a rank");
                }
            }

            /// <summary>
            /// Gets a ChessCoord from file number and rank number. 
            /// </summary>
            /// <param name="f">Number representation of file. </param>
            /// <param name="r">Rank of chess coordinate. </param>
            /// <exception cref="Exception"></exception>
            public ChessCoord(byte f, byte r)
            {
                if (f > 7 || r > 7)
                {
                    throw new Exception("Coordate given is out of range of the coords on the chess board");
                }
                file = f;
                rank = r;
            }

            /// <summary>
            /// Gets a ChessCoord from file number and rank number. 
            /// </summary>
            /// <param name="f">Number representation of a file. </param>
            /// <param name="r">Rank of chess coordinate. </param>
            /// <exception cref="Exception"></exception>
            public ChessCoord(int f, int r)
            {
                if (f > 7 || r > 7)
                {
                    throw new Exception("Coordate given is out of range of the coords on the chess board");
                }
                file = (byte)f;
                rank = (byte)r;
            }

            /// <summary>
            /// Number representation of a file. 
            /// </summary>
            public byte file;

            /// <summary>
            /// Rank of chess coordinate. 
            /// </summary>
            public byte rank;

            public override string ToString()
            {
                return file switch
                {
                    0 => 'a' + ('0' + rank).ToString(),
                    1 => 'b' + ('0' + rank).ToString(),
                    2 => 'c' + ('0' + rank).ToString(),
                    3 => 'd' + ('0' + rank).ToString(),
                    4 => 'e' + ('0' + rank).ToString(),
                    5 => 'f' + ('0' + rank).ToString(),
                    6 => 'g' + ('0' + rank).ToString(),
                    7 => 'h' + ('0' + rank).ToString(),
                    _ => "",
                };
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

        /// <summary>
        /// Represents the status of a game, contaning information on if the game is ongoing, drawn, or won/lost. 
        /// </summary>
        enum GameStatus
        {
            //Game is ongoing
            Ongoing,

            //Game is over: Drawn
            Draw,
            DrawAborted,
            DrawRepetition,
            DrawWhiteInsufficientMaterial,
            DrawBlackInsufficientMaterial,
            DrawAgreement,

            //Gave is over: White won
            White,
            WhiteCheckmate,
            WhiteTime,
            WhiteResignation,
            WhiteIllegalMove,

            //Game is over: Black won
            Black,
            BlackCheckmate,
            BlackTime,
            BlackResignation,
            BlackIllegalMove
        }

        /// <summary>
        /// Contains methods for the GameStatus enum.
        /// </summary>
        static class GameStatusMethods
        {
            /// <summary>
            /// Returns true if the game status is drawn.
            /// </summary>
            /// <param name="gs"></param>
            /// <returns></returns>
            public static bool IsDraw(this GameStatus gs)
            {
                if (gs == GameStatus.DrawRepetition || gs == GameStatus.DrawWhiteInsufficientMaterial || gs == GameStatus.DrawBlackInsufficientMaterial
                    || gs == GameStatus.DrawAgreement || gs == GameStatus.DrawAborted || gs == GameStatus.Draw)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns true if white won.
            /// </summary>
            /// <param name="gs"></param>
            /// <returns></returns>
            public static bool IsWonWhite(this GameStatus gs)
            {
                if (gs == GameStatus.WhiteCheckmate || gs == GameStatus.WhiteTime || gs == GameStatus.WhiteResignation || gs == GameStatus.WhiteIllegalMove
                    || gs == GameStatus.White)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns true if black won.
            /// </summary>
            /// <param name="gs"></param>
            /// <returns></returns>
            public static bool IsWonBlack(this GameStatus gs)
            {
                if (gs == GameStatus.BlackCheckmate || gs == GameStatus.BlackTime || gs == GameStatus.BlackResignation || gs == GameStatus.BlackIllegalMove
                    || gs == GameStatus.Black)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Contatins information about a given position, including game status, material evaluation, control evaluation, and overall evaluation. 
        /// </summary>
        struct Eval
        {
            /// <summary>
            /// Status of current position. 
            /// </summary>
            public GameStatus gameStatus;

            /// <summary>
            /// Stores material evaluation of current position if available. 
            /// </summary>
            public double? materialEval;

            /// <summary>
            /// Stores control evaluation of current position if available. 
            /// </summary>
            public double? controlEval;

            /// <summary>
            /// Stores overall evaulation of current position if available. 
            /// </summary>
            public double? eval;
        }

        [Serializable]
        class Board
        {
            /// <summary>
            /// Stores information about the given position, including game status and evaluation. 
            /// </summary>
            Eval Evaluation;

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
            /// Stores if and how a castle occured one move before the current position.
            /// </summary>
            public Castle Castled;

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
            
            /// <summary>
            /// Stores all of the pieces on the board
            /// </summary>
            public Pieces[] _board = new Pieces[64];

            /// <summary>
            /// Two dimensional array of pieces
            /// </summary>
            /// <param name="files">Number representing the file of a chess coordinate. </param>
            /// <param name="ranks">Rank of chess coordinate. </param>
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
            /// <param name="pieces">Array of pieces representing a chess board. </param>
            /// <param name="turn">Set to true if it is white's turn to move, false if it is black's turn to move. </param>
            public Board(Pieces[] pieces, bool turn, bool wkCastle = true, bool wqCastle = true, bool bkCastle = true, bool bqCastle = true, ChessCoord? enPassant = null, byte halfMoveClock = 0, int fullMoveCount = 0, GameStatus gameStatus = GameStatus.Ongoing, Castle castled = Castle.Empty)
            {
                if (pieces.Length != 64)
                {
                    throw new Exception("Array of pieces is not 64.");
                }

                //Set game status. 
                Evaluation.gameStatus = gameStatus;
                Evaluation.materialEval = null;
                Evaluation.controlEval = null;
                Evaluation.eval = null;

                Castled = castled;

                _board = pieces;

                Turn = turn;
                WKCastle = wkCastle;
                WQCastle = wqCastle;
                BKCastle = bkCastle;
                BQCastle = bqCastle;
                EnPassant = enPassant;
                HalfmoveClock = halfMoveClock;
                FullMoveCount = fullMoveCount;
            }

            /// <summary>
            /// Creates a board object using a two-dimensional array of 8 by 8 pieces.
            /// </summary>
            /// <param name="pieces">Two dimensional array of pieces representing a chess board</param>
            /// <param name="turn">Set to true if it is white's turn to move, false if it is black's turn to move. </param>
            public Board(Pieces[,] pieces, bool turn, bool wkCastle = true, bool wqCastle = true, bool bkCastle = true, bool bqCastle = true, ChessCoord? enPassant = null, byte halfMoveClock = 0, int fullMoveCount = 0, GameStatus gameStatus = GameStatus.Ongoing, Castle castled = Castle.Empty)
            {
                if (pieces.GetLength(0) != 8 || pieces.GetLength(1) != 8)
                {
                    throw new Exception("Dimensions of array of pieces is not 8 by 8.");
                }

                //Converts two dimensional array to one dimensional array
                //Loops through ranks
                for (int i = 0; i < 8; i++)
                {
                    //Loops through files
                    for (int j = 0; j < 8; j++)
                    {
                        _board[j + i * 8] = pieces[i, j];
                    }
                }

                //Sets the game status. 
                Evaluation.gameStatus = gameStatus;
                Evaluation.materialEval = null;
                Evaluation.controlEval = null;
                Evaluation.eval = null;

                Castled = castled;

                Turn = turn;
                WKCastle = wkCastle;
                WQCastle = wqCastle;
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
            /// Converts FEN string into a Board object. 
            /// </summary>
            /// <param name="fen">FEN string to be converted into a Board object. </param>
            /// <returns></returns>
            public static Board FromFEN(string fen)
            {
                //Splits sections of fen string into substrings. 
                string[] fields = fen.Split(' ');
                string[] ranks = fields[0].Split('/');

                //Inits the board and current board position
                Pieces[] board = new Pieces[64];
                int boardPos = 0;

                //Whos turn is it
                bool turn;

                //Castling rights
                bool wkCastle = false;
                bool wqCastle = false;
                bool bkCastle = false;
                bool bqCastle = false;

                //En passant
                ChessCoord? enPassant;

                //Halfmove clock
                int halfmoveClock;

                //Fullmove count;
                int fullmoveCount;

                //Stores number of empty squares on a rank
                int empty;

                //Converts pieces on fen string into array of chess pieces. 
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

                //Gets the current turn of the position
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
            /// Returns the chess piece at the requested chess coordinate. 
            /// </summary>
            /// <param name="coord">Chess coordinate</param>
            /// <returns></returns>
            public Pieces GetPiece(ChessCoord coord)
            {
                return _board[coord.file + coord.rank * 8];
            }

            /// <summary>
            /// Returns the chess piece at the requested chess coordinate. 
            /// </summary>
            /// <param name="file">Integer representing the file on the chess board. </param>
            /// <param name="rank">The rank on the chess board. </param>
            /// <returns></returns>
            public Pieces GetPiece(byte file, byte rank)
            {
                return _board[file + rank * 8];
            }

            /// <summary>
            /// Returns the chess piece at the requested chess coordinate. 
            /// </summary>
            /// <param name="file">Integer representing the file on the chess board. </param>
            /// <param name="rank">The rank on the chess board. </param>
            /// <returns></returns>
            public Pieces GetPiece(int file, int rank)
            {
                return _board[file + rank * 8];
            }

            /// <summary>
            /// Converts Board object into an FEN string. 
            /// </summary>
            /// <returns></returns>
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

                        //Adds piece to FEN
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

            /// <summary>
            /// Returns an array of all possible positions that come from all possible moves in the current position. 
            /// </summary>
            /// <returns></returns>
            public Board[] Moves()
            {
                List<Board> boards = new List<Board>();

                //Adds the possible moves for all of the pieces on the board. 
                for (int i = 0; i < 64; i++)
                {
                    boards.AddRange(Moves(i));
                }

                return boards.ToArray();
            }

            /// <summary>
            /// Returns an array of all possible positions that come from the moves of the piece at the given chess coordinate. 
            /// </summary>
            /// <param name="chessCoord">Chess coordinate</param>
            /// <returns></returns>
            public Board[] Moves(ChessCoord chessCoord)
            {
                return Moves(chessCoord.file + chessCoord.rank * 8);
            }

            /// <summary>
            /// Returns an array of all possible positions that come from the moves of the piece at the given index.
            /// </summary>
            /// <param name="index">Board index</param>
            /// <returns></returns>
            public Board[] Moves(int index)
            {
                if (index > 63)
                {
                    throw new ArgumentException("Index cannot be greater than 63. ");
                }

                List<Board> boards = new List<Board>();
                Board temp;
                Board temp2;

                int tempIndex;

                //If it is white's turn
                if (Turn)
                {
                    switch (_board[index])
                    {
                        case Pieces.Empty:
                            break;

                        case Pieces.WPawn:

                            //Set up temp board
                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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
                                    temp._board[index + 8] = Pieces.WKnight;
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

                            //Taking to the left
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

                            //If black illegally castled
                            if (Castled.CastleVurn(index + 9) || Castled.CastleVurn(index + 7))
                            {
                                temp2.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                return new Board[] { temp2.DeepCopy() };
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

                            //Set up temp board
                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
                            temp2.Turn = false;
                            temp2.EnPassant = null;
                            temp2.HalfmoveClock++;

                            //Moving rook to the left (if not on the far left side of the board)
                            if (index % 8 != 0 && !_board[index - 1].IsWhite())
                            {
                                tempIndex = index;

                                //If index is the starting location for the white rook on the king side, dissable castling on the king side. 
                                if (index == 7)
                                {
                                    temp2.WKCastle = false;
                                }

                                //Loops until white piece is found, edge of board is found, or black piece is found
                                do
                                {
                                    tempIndex -= 1;

                                    temp = temp2.DeepCopy();
                                    temp._board[tempIndex] = Pieces.WRook;
                                    
                                    //If current location contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                //If index is the starting location for the white rook on the queen side, dissable castling on the queen side. 
                                if (index == 0)
                                {
                                    temp2.WQCastle = false;
                                }

                                //Loops untill white piece is found, edge of board is found, or black piece is found
                                do
                                {
                                    tempIndex += 1;

                                    temp = temp2.DeepCopy();
                                    temp._board[tempIndex] = Pieces.WRook;

                                    //If current location contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                //If index is the starting location for the white rook on the queen side, dissable castling on the queen side. 
                                if (index == 0)
                                {
                                    temp2.WQCastle = false;
                                }
                                //If index is the starting location for the white rook on the king side, dissable castling on the king side. 
                                else if (index == 7)
                                {
                                    temp2.WKCastle = false;
                                }

                                //Loops untill white piece is found, edge of board is found, or black piece is found
                                do
                                {
                                    tempIndex += 8;

                                    temp = temp2.DeepCopy();
                                    temp._board[tempIndex] = Pieces.WRook;

                                    //If current location contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current location contains a black pieces
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

                                        temp.HalfmoveClock = 0;
                                        boards.Add(temp.DeepCopy());
                                        break;
                                    }

                                    boards.Add(temp.DeepCopy());

                                } while (index / 8 != 0 && !_board[index - 8].IsWhite());
                            }

                            break;

                        case Pieces.WKnight:

                            //Set up temp board
                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
                            temp2.Turn = false;
                            temp2.EnPassant = null;
                            temp2.HalfmoveClock++;

                            // Quad 2 lower
                            if (((index - 2) % 8 < index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 6].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 6] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index + 6].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 2 higher
                            if (((index - 1) % 8 < index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 15].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 15] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index + 15].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 1 higher
                            if (((index + 1) % 8 > index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 17].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 17] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index + 17].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 1 lower
                            if (((index + 2) % 8 > index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 10].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 10] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index + 10].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 4 higher
                            if (((index + 2) % 8 > index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 6].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 6] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index - 6].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 4 lower
                            if (((index + 1) % 8 > index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 15].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 15] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index - 15].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 3 lower
                            if (((index - 1) % 8 < index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 17].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 17] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index - 17].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 3 higher
                            if (((index - 2) % 8 < index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 10].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 10] = Pieces.WKnight;

                                //If current index contains a black piece
                                if (_board[index - 10].IsBlack())
                                {
                                    if (Castled.CastleVurn(index))
                                    {
                                        //If black illegally castled
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            break;

                        case Pieces.WBishop:

                            //Sets up temp board
                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

                                        temp.HalfmoveClock = 0;
                                        boards.Add(temp.DeepCopy());
                                        break;
                                    }

                                    boards.Add(temp.DeepCopy());

                                } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsWhite());
                            }

                            break;

                        case Pieces.WQueen:

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If current index contains a black piece
                                    if (_board[tempIndex].IsBlack())
                                    {
                                        //If black illegally castled
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

                                        temp.HalfmoveClock = 0;
                                        boards.Add(temp.DeepCopy());
                                        break;
                                    }

                                    boards.Add(temp.DeepCopy());

                                } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsWhite());
                            }

                            break;

                        case Pieces.WKing:

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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

                                //If current index contains a black piece
                                if (_board[index + 8].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king to the left
                            if (index % 8 != 0 && !_board[index - 1].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 1] = Pieces.WKing;

                                //If current index contains a black piece
                                if (_board[index - 1].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king to down
                            if (index / 8 != 0 && !_board[index - 8].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 8] = Pieces.WKing;

                                //If current index contains a black piece
                                if (_board[index - 8].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king to the right
                            if (index % 8 != 7 && !_board[index + 1].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 1] = Pieces.WKing;

                                //If current index contains a black piece
                                if (_board[index + 1].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 1
                            if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 9] = Pieces.WKing;

                                //If current index contains a black piece
                                if (_board[index + 9].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 2
                            if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 7] = Pieces.WKing;

                                //If current index contains a black piece
                                if (_board[index + 7].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 3
                            if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 9] = Pieces.WKing;

                                //If current index contains a black piece
                                if (_board[index - 9].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 4
                            if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsWhite())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 7] = Pieces.WKing;

                                //If current index contains a black piece
                                if (_board[index - 7].IsBlack())
                                {
                                    //If black illegally castled
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.WhiteIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

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
                                temp.Castled = Castle.WKCastle;
                                boards.Add(temp.DeepCopy());
                            }

                            //Castleing queen side
                            if (_board[index - 1] == Pieces.Empty && _board[index - 2] == Pieces.Empty && _board[index - 3] == Pieces.Empty && WQCastle)
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 2] = Pieces.WKing;
                                temp._board[index - 1] = Pieces.WRook;
                                temp._board[index - 4] = Pieces.Empty;
                                temp.Castled = Castle.WQCastle;
                                boards.Add(temp.DeepCopy());
                            }

                            break;

                        default:
                            break;
                    }
                }
                //If it is black's turn
                else
                {
                    switch (_board[index])
                    {
                        case Pieces.Empty:
                            break;

                        case Pieces.BPawn:

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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
                                    temp._board[index - 7] = Pieces.WKnight;
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

                            //If white illegally castled
                            if (Castled.CastleVurn(index + 9) || Castled.CastleVurn(index + 7))
                            {
                                temp2.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                return new Board[] { temp2.DeepCopy() };
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

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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

                                    //If rook is in the starting position for the black, king side rook. 
                                    if (index == 63)
                                    {
                                        temp.BKCastle = false;
                                    }

                                    //If rook can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If rook is at the starting location for the black, queen side rook. 
                                    if (index == 56)
                                    {
                                        temp.BQCastle = false;
                                    }

                                    //If rook can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If rook can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If rook is at the starting position of the black, queen side rook. 
                                    if (index == 56)
                                    {
                                        temp.BQCastle = false;
                                    }
                                    //If rook is at the starting position of the black, king side rook. 
                                    else if (index == 63)
                                    {
                                        temp.BKCastle = false;
                                    }

                                    //If rook can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

                                        temp.HalfmoveClock = 0;
                                        boards.Add(temp.DeepCopy());
                                        break;
                                    }

                                    boards.Add(temp.DeepCopy());

                                } while (index / 8 != 0 && !_board[index - 8].IsBlack());
                            }

                            break;

                        case Pieces.BKnight:

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
                            temp2.Turn = true;
                            temp2.EnPassant = null;
                            temp2.FullMoveCount++;
                            temp2.HalfmoveClock++;

                            // Quad 2 lower
                            if (((index - 2) % 8 < index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 6].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 6] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index + 6].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 2 higher
                            if (((index - 1) % 8 < index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 15].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 15] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index + 15].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 1 higher
                            if (((index + 1) % 8 > index % 8) && ((index + 16) / 8 % 8 > index / 8) && !_board[index + 17].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 17] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index + 17].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 1 lower
                            if (((index + 2) % 8 > index % 8) && ((index + 8) / 8 % 8 > index / 8) && !_board[index + 10].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 10] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index + 10].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 4 higher
                            if (((index + 2) % 8 > index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 6].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 6] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index - 6].IsWhite())
                                {
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 4 lower
                            if (((index + 1) % 8 > index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 15].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 15] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index - 15].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 3 lower
                            if (((index - 1) % 8 < index % 8) && ((index - 16) / 8 % 8 < index / 8) && !_board[index - 17].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 17] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index - 17].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            // Quad 3 higher
                            if (((index - 2) % 8 < index % 8) && ((index - 8) / 8 % 8 < index / 8) && !_board[index - 10].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 10] = Pieces.BKnight;

                                //If knight can take a white piece. 
                                if (_board[index - 10].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            break;

                        case Pieces.BBishop:

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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

                                    //If bishop can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If bishop can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If bishop can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If bishop can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

                                        temp.HalfmoveClock = 0;
                                        boards.Add(temp.DeepCopy());
                                        break;
                                    }

                                    boards.Add(temp.DeepCopy());

                                } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsBlack());
                            }

                            break;

                        case Pieces.BQueen:

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

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

                                    //If queen can take a white piece. 
                                    if (_board[tempIndex].IsWhite())
                                    {
                                        //If white castled illegally. 
                                        if (Castled.CastleVurn(tempIndex))
                                        {
                                            temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                            return new Board[] { temp.DeepCopy() };
                                        }

                                        temp.HalfmoveClock = 0;
                                        boards.Add(temp.DeepCopy());
                                        break;
                                    }

                                    boards.Add(temp.DeepCopy());

                                } while (tempIndex % 8 != 7 && tempIndex / 8 != 0 && !_board[tempIndex - 7].IsBlack());
                            }

                            break;

                        case Pieces.BKing:

                            temp2 = DeepCopy();
                            temp2._board[index] = Pieces.Empty;
                            temp2.Castled = Castle.Empty;
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

                                //If king can take a white piece. 
                                if (_board[index + 8].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king to the left
                            if (index % 8 != 0 && !_board[index - 1].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 1] = Pieces.BKing;

                                //If king can take a white piece. 
                                if (_board[index - 1].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king to down
                            if (index / 8 != 0 && !_board[index - 8].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 8] = Pieces.BKing;

                                //If king can take a white piece. 
                                if (_board[index - 8].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king to the right
                            if (index % 8 != 7 && !_board[index + 1].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 1] = Pieces.BKing;

                                //If king can take a white piece. 
                                if (_board[index + 1].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 1
                            if (index % 8 != 7 && index / 8 != 7 && !_board[index + 9].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 9] = Pieces.BKing;

                                //If king can take a white piece. 
                                if (_board[index + 9].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 2
                            if (index % 8 != 0 && index / 8 != 7 && !_board[index + 7].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index + 7] = Pieces.BKing;

                                //If king can take a white piece. 
                                if (_board[index + 7].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 3
                            if (index % 8 != 0 && index / 8 != 0 && !_board[index - 9].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 9] = Pieces.BKing;

                                //If king can take a white piece. 
                                if (_board[index - 9].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

                                    temp.HalfmoveClock = 0;
                                }

                                boards.Add(temp.DeepCopy());
                            }

                            //Moving king into quad 4
                            if (index % 8 != 7 && index / 8 != 0 && !_board[index - 7].IsBlack())
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 7] = Pieces.BKing;

                                //If king can take a white piece. 
                                if (_board[index - 7].IsWhite())
                                {
                                    //If white castled illegally. 
                                    if (Castled.CastleVurn(index))
                                    {
                                        temp.Evaluation.gameStatus = GameStatus.BlackIllegalMove;
                                        return new Board[] { temp.DeepCopy() };
                                    }

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
                                temp.Castled = Castle.BKCastle;
                                boards.Add(temp.DeepCopy());
                            }
                            //Castleing queen side
                            if (_board[index - 1] == Pieces.Empty && _board[index - 2] == Pieces.Empty && _board[index - 3] == Pieces.Empty && WQCastle)
                            {
                                temp = temp2.DeepCopy();
                                temp._board[index - 2] = Pieces.BKing;
                                temp._board[index - 1] = Pieces.BRook;
                                temp._board[index - 4] = Pieces.Empty;
                                temp.Castled = Castle.BQCastle;
                                boards.Add(temp.DeepCopy());
                            }

                            break;

                        default:
                            break;
                    }
                }

                return boards.ToArray();
            }

            /// <summary>
            /// Deep copies this board object and returns the copy.. 
            /// </summary>
            /// <returns></returns>
            public Board DeepCopy()
            {
                //https://stackoverflow.com/questions/1031023/copy-a-class-c-sharp
                using MemoryStream ms = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;
                return (Board)formatter.Deserialize(ms);
            }

            public override string ToString()
            {
                return ToFEN();
            }
        }
    }
}