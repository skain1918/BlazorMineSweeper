
namespace BlazorMineSweeper.Client.Code;

public enum GameState { Loading, Playing, Won, Lost }
public class MineField {
    public Cell[,] Cells { get; private set; }
    public int Width { get; set; }  //cells count in X direction
    public int Height { get; set; } //cells count in Y direction
    public int MineCount { get; set; } //number of mines in the field
    public GameState CurrentGameState { get; set; } = GameState.Loading;

    public void CreateField() {
        CurrentGameState = GameState.Loading;
        // Initialize the cell array
        Cells = new Cell[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                Cells[i, j] = new Cell(i, j, OnCellRevealed);


        // Place mines randomly
        for (int m = 0; m < MineCount; m++) {
            int x, y;
            do {
                x = Random.Shared.Next(Width);
                y = Random.Shared.Next(Height);
            } while (Cells[x, y].CellType == 9); // Ensure no duplicate mines
            Cells[x, y].CellType = 9; // 9 represents a mine
            // Update adjacent cells' mine counts
            for (int dx = Math.Max(y - 1, 0); dx <= Math.Min(y + 1, Height - 1); dx++)
                for (int dy = Math.Max(x - 1, 0); dy <= Math.Min(x + 1, Width - 1); dy++)
                    if (Cells[dy, dx].CellType != 9)
                        Cells[dy, dx].CellType++;
        }


        CurrentGameState = GameState.Playing;
    }
    // Queue for cells to be revealed 
    Queue<Cell> queue = new Queue<Cell>();
    private void OnCellRevealed(object? sender, EventArgs e) {
        // Handle cell reveal logic here
        var cell = sender as Cell;
        // Click on celltype 0 reveals neighboring cells
        if (cell.CellType == 0) {
            queue.Clear();
            queue.Enqueue(cell);
            RevealCellAndNeighbors();
        }

        // Click on mine ends game and reveals all mines
        else if (cell.CellType == 9) {
            foreach (var c in Cells) {
                if (!c.IsRevealed && c.CellType == 9)
                    c.Reveal(false);
            }
            CurrentGameState = GameState.Lost;
            return;
        }
        // Check for win condition
        foreach (var c in Cells) {
            if (!c.IsRevealed && c.CellType != 9)
                return; // Still cells to reveal
        }
        CurrentGameState = GameState.Won;
    }

    private void RevealCellAndNeighbors() {
        while (queue.Count > 0) {
            var c = queue.Dequeue();
            if (c.CellType == 0) {
                c.Reveal(false);
                for (int dx = Math.Max(c.Y - 1, 0); dx <= Math.Min(c.Y + 1, Height - 1); dx++)
                    for (int dy = Math.Max(c.X - 1, 0); dy <= Math.Min(c.X + 1, Width - 1); dy++)
                        if (Cells[dy, dx].IsClickAble && !queue.Contains(Cells[dy, dx]))
                            queue.Enqueue(Cells[dy, dx]);
            }
            else {
                c.Reveal(false);
            }
        }
    }
}
