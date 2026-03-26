using System.Collections.Generic;

namespace Ai4Gamedev.MiniMax.Isolation
{
    public class NykytaKasianenkoMinimaxStrategy : IMinimaxStrategy
    {
        public string Name => name;
        private readonly string name;

        public int SearchDepth { get; }
        
        private readonly IPossibleMovesProvider movesProvider;
        private IGameBoard board1;

        public NykytaKasianenkoMinimaxStrategy(IGameBoard board, string name = "BotNykyta")
        {
            board1 = board;
            this.name = name;
            SearchDepth = 5;
            movesProvider = new PossibleMovesProvider();
        }

        public int EvaluateBoard(IGameBoard board, int playerId)
        {
            return movesProvider.GetPossibleMovesFor(board, playerId).Count;
        }

        public List<Move> Sort(IGameBoard board, List<Move> moves)
        {
            var cells = board1.Cells;
            
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);
            
            Position oponentPosition = null;
            Position ourPosition = null;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if(cells[i,j].State == CellState.Occupied && cells[i,j].PlayerId == 2)
                    {
                        oponentPosition = new Position { Column = i, Row = j };
                    }

                    if (cells[i, j].State == CellState.Occupied && cells[i, j].PlayerId == 1)
                    {
                        ourPosition = new Position { Column = i, Row = j };
                    }
                }
            }
            
            Position bestMove = null;
            int count = -1;

            if (ourPosition != null)
            {
                int currentFree;
                Position p; 

                p = new Position { Column = ourPosition.Column + 1, Row = ourPosition.Row + 1 };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }

                p = new Position { Column = ourPosition.Column - 1, Row = ourPosition.Row - 1 };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }

                p = new Position { Column = ourPosition.Column + 1, Row = ourPosition.Row - 1 };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }

                p = new Position { Column = ourPosition.Column - 1, Row = ourPosition.Row + 1 };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }

                p = new Position { Column = ourPosition.Column + 1, Row = ourPosition.Row };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }

                p = new Position { Column = ourPosition.Column - 1, Row = ourPosition.Row };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }

                p = new Position { Column = ourPosition.Column, Row = ourPosition.Row + 1 };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }

                p = new Position { Column = ourPosition.Column, Row = ourPosition.Row - 1 };
                if (IsCellFree(p)) { currentFree = FreeCellsAround(p); if (currentFree > count) { count = currentFree; bestMove = p; } }   
            }
            
            Position bestBlock = null;
            int blockCount = -1;

            if (oponentPosition != null)
            {
                int opponentFree;
                Position p;

                p = new Position { Column = oponentPosition.Column + 1, Row = oponentPosition.Row + 1 };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }

                p = new Position { Column = oponentPosition.Column - 1, Row = oponentPosition.Row - 1 };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }

                p = new Position { Column = oponentPosition.Column + 1, Row = oponentPosition.Row - 1 };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }

                p = new Position { Column = oponentPosition.Column - 1, Row = oponentPosition.Row + 1 };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }

                p = new Position { Column = oponentPosition.Column + 1, Row = oponentPosition.Row };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }

                p = new Position { Column = oponentPosition.Column - 1, Row = oponentPosition.Row };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }

                p = new Position { Column = oponentPosition.Column, Row = oponentPosition.Row + 1 };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }

                p = new Position { Column = oponentPosition.Column, Row = oponentPosition.Row - 1 };
                if (IsCellFree(p)) { opponentFree = FreeCellsAround(p); if (opponentFree > blockCount) { blockCount = opponentFree; bestBlock = p; } }
            }
            
            
            Move perfectMove = null;
            // our perfect move
            if (bestMove != null && bestBlock != null)
            {
                perfectMove = moves.Find(m => 
                    m.DestinationPosition.Column == bestMove.Column && m.DestinationPosition.Row == bestMove.Row &&
                    m.BlockPosition.Column == bestBlock.Column && m.BlockPosition.Row == bestBlock.Row);
            }
            
            if (perfectMove != null)
            {
                moves.Remove(perfectMove);
                moves.Insert(0, perfectMove);
            }
            else
            {
                
                
                // best block
                if (bestBlock != null)
                {
                    var blockMove = moves.Find(m => m.BlockPosition.Column == bestBlock.Column && m.BlockPosition.Row == bestBlock.Row);
                    if (blockMove != null)
                    {
                        moves.Remove(blockMove);
                        moves.Insert(0, blockMove); 
                    }
                }

                // best move
                if (bestMove != null)
                {
                    var topMove = moves.Find(m => m.DestinationPosition.Column == bestMove.Column && m.DestinationPosition.Row == bestMove.Row);
                    if (topMove != null)
                    {
                        moves.Remove(topMove);
                        moves.Insert(0, topMove);
                    }
                }
            }

            return moves;
        }
        
        private bool IsCellFree(Position position)
        {
            if (position == null) return false;
            
            int w = board1.Cells.GetLength(0);
            int h = board1.Cells.GetLength(1);
            
            if (position.Column < 0 || position.Column >= w || 
                position.Row < 0 || position.Row >= h)
            {
                return false;
            }

           
            return board1.Cells[position.Column, position.Row].State == CellState.Free;
        }

        private int FreeCellsAround(Position position)
        {
            if (position == null) return 0;
            
            int w = board1.Cells.GetLength(0);
            int h = board1.Cells.GetLength(1);

            if (position.Column < 0 || position.Column >= w || 
                position.Row < 0 || position.Row >= h)
            {
                return 0;
            }

            var freeCells = 0;

            if (position.Column + 1 < w && board1.Cells[position.Column + 1, position.Row].State == CellState.Free)
                freeCells++;
                
            if (position.Column - 1 >= 0 && board1.Cells[position.Column - 1, position.Row].State == CellState.Free)
                freeCells++;

            if (position.Row + 1 < h && board1.Cells[position.Column, position.Row + 1].State == CellState.Free)
                freeCells++;

            if (position.Row - 1 >= 0 && board1.Cells[position.Column, position.Row - 1].State == CellState.Free)
                freeCells++;
            
            if (position.Column + 1 < w && position.Row + 1 < h && board1.Cells[position.Column + 1, position.Row + 1].State == CellState.Free)
                freeCells++;

            if (position.Column - 1 >= 0 && position.Row - 1 >= 0 && board1.Cells[position.Column - 1, position.Row - 1].State == CellState.Free)
                freeCells++;
            
            if (position.Column + 1 < w && position.Row - 1 >= 0 && board1.Cells[position.Column + 1, position.Row - 1].State == CellState.Free)
                freeCells++;
            
            if (position.Column - 1 >= 0 && position.Row + 1 < h && board1.Cells[position.Column - 1, position.Row + 1].State == CellState.Free)
                freeCells++;
            
            return freeCells;
        }
    }
}