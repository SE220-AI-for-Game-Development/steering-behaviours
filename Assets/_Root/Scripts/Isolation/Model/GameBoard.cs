namespace Ai4Gamedev.MiniMax.Isolation
{
    using Cysharp.Threading.Tasks;

    public class GameBoard : IGameBoard
    {
        public Cell[,] Cells { get; set; }
        
        private IGameBoardView view;

        public GameBoard(IGameBoardView view)
        {
            Cells = new Cell[5, 5];
            for (int x = 0; x < Cells.GetLength(0); x++)
            {
                for (int y = 0; y < Cells.GetLength(0); y++)
                {
                    Cells[x, y] = new Cell();
                    if (x == 2 && y == 0)
                    {
                        Cells[x, y].Occupy(1);
                    }
                    if (x == 2 && y == 4)
                    {
                        Cells[x, y].Occupy(2);
                    }
                }
            }
            
            this.view = view;
            view.Initialize(this);
        }


        public bool IsValidMove(Move move)
        {
            throw new System.NotImplementedException();
        }

        public UniTask ApplyMove(Move move)
        {
            throw new System.NotImplementedException();
        }

        public IGameBoard Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}