namespace Ai4Gamedev.MiniMax.Isolation
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public interface IPossibleMovesProvider
    {
        List<Move> GetPossibleMovesFor(IGameBoard board, int playerId);
    }

    public class PossibleMovesProvider : IPossibleMovesProvider
    {
        public List<Move> GetPossibleMovesFor(IGameBoard board, int playerId)
        {
            // Isolation rule variant:
            // - Destination: king8 adjacent to current player's occupied cell.
            // - Destination must be free.
            // - Extra block: any free cell on the board except destination.

            var cells = board.Cells;

            var boardWidth = cells.GetLength(0);
            var boardHeight = cells.GetLength(1);

            Position occupiedPosition = null;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (cells[x, y].State == CellState.Occupied &&
                        cells[x, y].PlayerId == playerId)
                    {
                        occupiedPosition = new Position { Column = x, Row = y };
                    }
                }
            }

            if (occupiedPosition == null)
            {
                return new List<Move>();
            }

            var moves = new List<Move>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    var destinationColumn = occupiedPosition.Column + dx;
                    var destinationRow = occupiedPosition.Row + dy;

                    if (destinationColumn < 0 || destinationColumn >= boardWidth)
                    {
                        continue;
                    }

                    if (destinationRow < 0 || destinationRow >= boardHeight)
                    {
                        continue;
                    }

                    if (cells[destinationColumn, destinationRow].State != CellState.Free)
                    {
                        continue;
                    }

                    // Extra block: any free cell except destination.
                    for (int bx = 0; bx < boardWidth; bx++)
                    {
                        for (int by = 0; by < boardHeight; by++)
                        {
                            if (bx == destinationColumn && by == destinationRow)
                            {
                                continue;
                            }

                            if (cells[bx, by].State != CellState.Free)
                            {
                                continue;
                            }

                            moves.Add(new Move
                            {
                                PlayerId = playerId,
                                DestinationPosition = new Position
                                {
                                    Column = destinationColumn,
                                    Row = destinationRow
                                },
                                BlockPosition = new Position
                                {
                                    Column = bx,
                                    Row = by
                                }
                            });
                        }
                    }
                }
            }

            return moves;
        }
    }
}