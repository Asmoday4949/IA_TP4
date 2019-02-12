using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAOthelloFH
{
    class BoardIA : OthelloLogic, IPlayable.IPlayable
    {
        public BoardIA(): base()
        {
        }

        public int GetBlackScore()
        {
            UpdatePlayersScore();
            return GetBlackPlayerData().NumberOfPawns;
        }

        public int[,] GetBoard()
        {
            return GameBoard;
        }

        public string GetName()
        {
            return "ArcOthelloFH";
        }

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
