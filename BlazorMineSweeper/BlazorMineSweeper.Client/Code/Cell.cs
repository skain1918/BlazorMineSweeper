namespace BlazorMineSweeper.Client.Code;
public class Cell {
    public bool IsRevealed { get; private set; }  // Indicates if the cell has been revealed
    public bool IsFlagged { get; private set; }  // Indicates if the cell is flagged
    public bool IsClickAble => !IsRevealed && !IsFlagged; // Indicates if the cell can be clicked
    public bool Fire { get; private set; } = false; // Indicates if the player clicked on a mine
    public string CssClass => (Fire ? " fire" : "") +
        (CellType == 9 ? " mine" : "") +
        (CellType > 0 ? $" n-{CellType}" : "");
    public int X { get; init; } // X coordinate of the cell
    public int Y { get; init; } // Y coordinate of the cell
    private int cellType = 0; // 0 = empty, 1-8 = number of adjacent mines, 9 = mine
    // Property to get or set the cell type with validation
    public int CellType {
        get => cellType;
        set {
            if (value >= 0 && value <= 9) {
                cellType = value;
            }
        }
    }
    // Event triggered when the cell is revealed
    public event EventHandler CellReveal;

    //Constructor to initialize the cell with coordinates and an event handler for revealing the cell
    public Cell(int x, int y, EventHandler cellReveal) {
        X = x;
        Y = y;
        CellReveal += cellReveal;
    }
    // Method to reveal the cell and optionally run the CellReveal event
    public void Reveal(bool runEvent) {
        if (!IsClickAble) return;
        IsRevealed = true;
        if (runEvent) {
            if (CellType == 9) {
                Fire = true;
            }
            CellReveal?.Invoke(this, EventArgs.Empty);
        }
    }
    // Method to toggle the flagged state of the cell
    public void ToggleFlag() {
        if (IsRevealed) return;
        IsFlagged = !IsFlagged;
    }

}
