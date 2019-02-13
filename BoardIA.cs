/// -----------------------------------------------------------------------------------------------------
/// Authors : Haldenwang Thibault && Malik Fleury
/// Date : 12.02.2019
/// Update : 13.02.2019
/// 
/// Description : AlphaBeta algo for reversi.
/// -----------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace IAOthelloFH
{
    class BoardIA : OthelloLogic, IPlayable.IPlayable
    {
        /// <summary>
        /// Default constructor: create a grid of size 9x7
        /// </summary>
        public BoardIA(): base()
        {
        }
        
        /// <summary>
        /// Return the current black score (number of pawns)
        /// </summary>
        /// <returns>Black score</returns>
        public int GetBlackScore()
        {
            UpdatePlayersScore();
            return GetBlackPlayerData().NumberOfPawns;
        }

        /// <summary>
        /// Return the internal board
        /// </summary>
        /// <returns>Board</returns>
        public int[,] GetBoard()
        {
            return GameBoard;
        }

        /// <summary>
        /// Return the name of the IA
        /// </summary>
        /// <returns>Name of the IA</returns>
        public string GetName()
        {
            return "ArcOthelloFH";
        }

        /// <summary>
        /// Asks the game engine next (valid) move given a game position
        /// The board assumes following standard move notation:
        ///                             v
        ///             A B C D E F G H I
        ///            [0 1 2 3 4 5 6 7 8]    (first index)
        ///       1[0]
        ///       2[1]
        ///       3[2]        w K
        ///       4[3]        K w 
        ///       5[4]
        ///       6[5]
        ///      >7[6]                  x
        ///       
        ///   {Column;Line}
        ///  E.g.:  'w' on D3 in game notation will map to {3,2}, and 'x' on I7  to {8,6}
        /// </summary>
        /// <param name="game">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA, 5 normally</param>
        /// <param name="whiteTurn">true if white players turn, false otherwise</param>
        /// <returns>The column and line indices. Will return {-1,-1} as PASS if no possible move </returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            int[,] gameState = new int[9, 7];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    gameState[i, j] = game[i, j];
                }
            }

            BoardIA logic = new BoardIA
            {
                GameBoard = gameState,
                PlayerTurn = whiteTurn ? Player.White : Player.Black
            };

            List<IntPosition> possibleMoves = logic.GetAllPossibleMoves();
            if (possibleMoves.Count == 0)
            {
                return new Tuple<int, int>(-1, -1);
            }

            var nextMove = AlphaBeta(logic, level, int.MaxValue, whiteTurn);
            return new Tuple<int, int>(nextMove.Item2.Column, nextMove.Item2.Row);
        }

        /// <summary>
        /// Check whether a state is terminal
        /// </summary>
        /// <returns>True if state is terminal</returns>
        private bool IsTerminal()
        {
            var possibleMoves = GetAllPossibleMoves();
            if (possibleMoves.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Implementation of negamax algorithm with alpha beta pruning. A node is represented by a game board. 
        /// </summary>
        /// <param name="nodeBoard">A particular game state</param>
        /// <param name="depth">The maximum tree depth</param>
        /// <param name="parentValue">The parent node value</param>
        /// <param name="maximizingPlayer">Whether node is a maximizimer or minimizer. True if maximizer. </param>
        /// <returns>A tuple containing the value as it's first item and the position corresponding to the best
        /// predicted play as it's second item. </returns>
        private Tuple<int, IntPosition> AlphaBeta(BoardIA nodeBoard, int depth, int parentValue, bool maximizingPlayer)
        {
            if (depth == 0 || nodeBoard.IsTerminal())
            {
                var heuristicValue = nodeBoard.GetHeuristicValue();
                return new Tuple<int, IntPosition>(heuristicValue, new IntPosition(-1, -1));
            }
            else
            {
                int bestValue = maximizingPlayer ? -int.MaxValue : int.MaxValue;
                IntPosition bestMove = new IntPosition(-1, -1);

                var childPositions = nodeBoard.GetAllPossibleMoves();
                foreach (var child in childPositions)
                {
                    var childValue = AlphaBeta(PosToBoard(child, nodeBoard), depth - 1, bestValue, !maximizingPlayer);
                    int minOrMax = maximizingPlayer ? 1 : -1;
                    if (childValue.Item1 * minOrMax > bestValue * minOrMax)
                    {
                        bestValue = childValue.Item1;
                        bestMove = child;
                        if (bestValue * minOrMax > parentValue * minOrMax)
                        {
                            break;
                        }
                    }
                }
                return new Tuple<int, IntPosition>(bestValue, bestMove);
            }
        }

        /// <summary>
        /// Compute the heuristic with the following values : coin parity, mobility, corners captured
        /// </summary>
        /// <returns>Heuristic value</returns>
        private int GetHeuristicValue()
        {

            var coinParity = 100 * (GetWhiteScore() - GetBlackScore()) / (GetWhiteScore() + GetBlackScore());
            if (PlayerTurn == Player.Black)
            {
                SwitchPlayer();
            }
            var possibleWhiteMoves = GetAllPossibleMoves().Count();
            SwitchPlayer();
            var possibleBlackMoves = GetAllPossibleMoves().Count();
            int mobility = 0;
            if (possibleBlackMoves + possibleWhiteMoves != 0)
            {
                mobility = (int)(100 * ((double)(possibleWhiteMoves - possibleBlackMoves) / (double)(possibleWhiteMoves + possibleBlackMoves)));
            }
            int nbCorners = 0;
            int nbCornersWhite = GetNbCoinsInCorners(true);
            int nbCornersBlack = GetNbCoinsInCorners(false);
            if (nbCornersWhite + nbCornersBlack != 0)
            {
                nbCorners = 10 * (nbCornersWhite - nbCornersBlack) / (nbCornersWhite + nbCornersBlack);
            }
            return (int)(coinParity + mobility + nbCorners);
        }

        /// <summary>
        /// Get the number of coins in the corners for black or white player
        /// </summary>
        /// <param name="whitePlayer">True : white player, False : Black player</param>
        /// <returns>Number of coins in the corners</returns>
        private int GetNbCoinsInCorners(bool whitePlayer)
        {
            int nbCorners = 0;
            int playerInt = whitePlayer ? 0 : 1;
            List<IntPosition> listCorners = new List<IntPosition>();
            listCorners.Add(new IntPosition(0, 0));
            listCorners.Add(new IntPosition(0, 6));
            listCorners.Add(new IntPosition(8, 0));
            listCorners.Add(new IntPosition(8, 6));
            foreach (var corner in listCorners)
            {
                var slotValue = GameBoard[corner.Column, corner.Row];
                if (slotValue == playerInt)
                {
                    nbCorners++;
                }
            }
            return nbCorners;
        }

        /// <summary>
        /// Creates an AIBoard class given the position of a move to play
        /// and the board where the move has to be played.
        /// </summary>
        /// <param name="position">The position of the pawn to play on the board</param>
        /// <param name="sourceBoard">The board where the move takes place</param>
        /// <returns>A new AIBoard with the updated game state</returns>
        private BoardIA PosToBoard(IntPosition position, BoardIA sourceBoard)
        {
            int[,] gameState = new int[9, 7];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    gameState[i, j] = sourceBoard.GameBoard[i, j];
                }
            }

            BoardIA newBoard = new BoardIA
            {
                GameBoard = gameState,
                PlayerTurn = sourceBoard.PlayerTurn
            };
            newBoard.PlayMove(position.Column, position.Row, sourceBoard.PlayerTurn == Player.White);
            return newBoard;
        }

        /// <summary>
        /// Get the current white score (number of pawns)
        /// </summary>
        /// <returns>White score</returns>
        public int GetWhiteScore()
        {
            UpdatePlayersScore();
            return GetWhitePlayerData().NumberOfPawns;
        }


        /// <summary>
        /// Check whether the given position is legal.
        /// </summary>
        /// <param name="column">Column number of the position to check</param>
        /// <param name="line">Line number of the position to check</param>
        /// <param name="isWhite">True if player turn is white</param>
        /// <returns>True if move is legal</returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            Player currentPlayer = PlayerTurn;
            PlayerTurn = isWhite ? Player.White : Player.Black;
            IntPosition positionToCheck = new IntPosition(column, line);
            List<IntPosition> playableSlots = GetAllPossibleMoves();
            PlayerTurn = currentPlayer;
            return playableSlots.Contains(positionToCheck);
        }


        /// <summary>
        /// Play a move and alter the board accordingly. Returns true if the move was legal.
        /// </summary>
        /// <param name="column">The column corresponding to the position of the move</param>
        /// <param name="line">The line corresponding to the position of the move</param>
        /// <param name="isWhite">True if the current player turn is white</param>
        /// <returns>True if move was legal.</returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            PlayerTurn = isWhite ? Player.White : Player.Black;
            bool IsMoveValid = IsPlayable(column, line, isWhite);
            if (IsMoveValid)
            {
                IntPosition slotPosition = new IntPosition(column, line);
                List<IntPosition> pawnsToFlip = GetPawnsToFlip(slotPosition);
                pawnsToFlip.Add(slotPosition);
                UpdateSlots(pawnsToFlip);
            }
            return IsMoveValid;
        }
    }
}
