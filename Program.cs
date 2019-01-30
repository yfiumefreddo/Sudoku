using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku {
  internal class Chrono : System.Windows.Forms.Label {
    public Chrono() {
      this.timer.Interval = 31;
      this.Text = "00:00:00";
      this.timer.Tick += delegate (object sender, System.EventArgs e) {
        if (this.showMilliseconds)
          this.Text = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", this.chrono.Elapsed.Hours, this.chrono.Elapsed.Minutes, this.chrono.Elapsed.Seconds, this.chrono.Elapsed.Milliseconds);
        else
          this.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", this.chrono.Elapsed.Hours, this.chrono.Elapsed.Minutes, this.chrono.Elapsed.Seconds);
      };
    }

    public bool ShowMilliseconds {
      get { return this.showMilliseconds; }
      set {
        this.showMilliseconds = value;
        if (!this.isRunning)
          this.Reset();
      }
    }

    public bool IsRunning {
      get { return this.isRunning; }
    }

    public void Start() {
      this.isRunning = true;
      this.chrono.Start();
      this.timer.Enabled = true;
    }

    public void Pause() { this.timer.Enabled = false; }

    public void Resume() { this.timer.Enabled = true; }

    public void Stop() {
      this.isRunning = false;
      this.chrono.Stop();
      this.timer.Enabled = false;
    }

    public void Reset() {
      this.Stop();
      this.chrono.Reset();
      this.Text = this.showMilliseconds ? "00:00:00.000" : "00:00:00";
    }

    private System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();
    private System.Windows.Forms.Timer timer = new Timer();
    private bool isRunning = false;
    private bool showMilliseconds = false;
  }

  class MenuPanel : System.Windows.Forms.Panel {
    public MenuPanel() { }

    public Menu.MenuItemCollection Items {
      get { return this.items; }
      set {
        this.items = value;
      }
    }

    private Menu.MenuItemCollection items = new Menu.MenuItemCollection(null);
    private System.Collections.Generic.List<Label> values = new System.Collections.Generic.List<Label>();
  }

  class FormSudoku : Form {
    public static void Main(string[] args) {
      Application.Run(new FormSudoku());
    }

    public FormSudoku() {
      this.Text = "Sudoku";
      this.StartPosition = FormStartPosition.Manual;
      this.Location = new Point(300, 100);
      this.ClientSize = new Size(333, 383);
      this.KeyPreview = true;
      this.Icon = Sudoku.Properties.Resources.Sudoku;

      this.SizeChanged += delegate (object sender, EventArgs e) {
        this.menuPanel.Size = this.ClientSize;

        this.newGameLabel.Location = new Point(0, 0);
        this.newGameLabel.Size = new Size(this.menuPanel.Size.Width, this.menuPanel.Size.Height / 6);
        this.newGameLabel.Font = new Font("Courier New", (float)this.menuPanel.Size.Width / 10, System.Drawing.FontStyle.Bold);

        this.resumeLabel.Location = new Point(0, this.newGameLabel.Location.Y + this.newGameLabel.Size.Height);
        this.resumeLabel.Size = new Size(this.menuPanel.Size.Width, this.menuPanel.Size.Height / 6);
        this.resumeLabel.Font = new Font("Courier New", (float)this.menuPanel.Size.Width / 10, System.Drawing.FontStyle.Bold);

        this.setingsLabel.Location = new Point(0, this.resumeLabel.Location.Y + this.resumeLabel.Size.Height);
        this.setingsLabel.Size = new Size(this.menuPanel.Size.Width, this.menuPanel.Size.Height / 6);
        this.setingsLabel.Font = new Font("Courier New", (float)this.menuPanel.Size.Width / 10, System.Drawing.FontStyle.Bold);

        this.statisticsLabel.Location = new Point(0, this.setingsLabel.Location.Y + this.setingsLabel.Size.Height);
        this.statisticsLabel.Size = new Size(this.menuPanel.Size.Width, this.menuPanel.Size.Height / 6);
        this.statisticsLabel.Font = new Font("Courier New", (float)this.menuPanel.Size.Width / 10, System.Drawing.FontStyle.Bold);

        this.helpLabel.Location = new Point(0, this.statisticsLabel.Location.Y + this.statisticsLabel.Size.Height);
        this.helpLabel.Size = new Size(this.menuPanel.Size.Width, this.menuPanel.Size.Height / 6);
        this.helpLabel.Font = new Font("Courier New", (float)this.menuPanel.Size.Width / 10, System.Drawing.FontStyle.Bold);

        this.exitLabel.Location = new Point(0, this.helpLabel.Location.Y + this.helpLabel.Size.Height);
        this.exitLabel.Size = new Size(this.menuPanel.Size.Width, this.menuPanel.Size.Height / 6);
        this.exitLabel.Font = new Font("Courier New", (float)this.menuPanel.Size.Width / 10, System.Drawing.FontStyle.Bold);

        this.levelPanel.Size = this.ClientSize;

        System.Drawing.Point l = new Point();
        foreach (Label levelLabel in this.levelLabels) {
          levelLabel.Location = l;
          levelLabel.Size = new Size(this.levelPanel.Size.Width, this.levelPanel.Size.Height / 6);
          levelLabel.Font = new Font("Courier New", (float)this.levelPanel.Size.Width / 10, System.Drawing.FontStyle.Bold);
          l = new Point(0, levelLabel.Location.Y + levelLabel.Size.Height);
        }

        this.gamePanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 50);
        for (int i = 0; i < 81; i++) {
          cells[i / 9, i % 9].Size = new Size((int)((float)this.gamePanel.Size.Width / 9) - 2, (int)((float)this.gamePanel.Size.Height / 9) - 2);
          cells[i / 9, i % 9].Location = new Point(5 * (i % 9 / 3 + 1) + ((this.gamePanel.Size.Width / 9) - 2) * (i % 9), 5 * (i / 27 + 1) + ((this.gamePanel.Size.Height / 9) - 2) * (i / 9));
          cells[i / 9, i % 9].Invalidate();
        }
        this.winLabel.Location = new Point(0, 0);
        this.winLabel.Size = new Size(this.gamePanel.Size.Width, this.gamePanel.Size.Height / 6);
        this.winLabel.Font = new Font("Courier New", (float)this.gamePanel.Size.Width / 10, System.Drawing.FontStyle.Bold);

        this.boardPanel.Location = new Point(0, this.ClientSize.Height - 50);
        this.boardPanel.Size = new Size(this.ClientSize.Width, 50);
        this.chrono.Location = new Point(this.boardPanel.Size.Width - this.chrono.Size.Width, 8);
      };

      this.KeyDown += delegate (object sender, KeyEventArgs e) {
        if (e.KeyData == (Keys.Control | Keys.Shift | Keys.S))
          SolveGame();
        else if (e.KeyData == (Keys.Control | Keys.Shift | Keys.R))
          RestartGame();
        else if (e.KeyCode == Keys.Escape) {
          if (this.levelPanel.Visible) {
            this.levelPanel.Visible = false;
            this.menuPanel.Visible = true;
          } else if (this.menuPanel.Visible) {
            this.menuPanel.Visible = false;
            this.gamePanel.Visible = true;
            this.boardPanel.Visible = true;
          } else {
            this.menuPanel.Visible = true;
            this.gamePanel.Visible = false;
            this.boardPanel.Visible = false;
          }
        }
      };

      this.winLabel.Parent = this.gamePanel;
      this.winLabel.Text = "You win";
      this.winLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.winLabel.ForeColor = System.Drawing.Color.LightGreen;
      this.winLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.winLabel.Visible = false;

      this.menuPanel.Parent = this;
      this.menuPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.menuPanel.Visible = false;

      this.newGameLabel.Parent = this.menuPanel;
      this.newGameLabel.Text = "New Game";
      this.newGameLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.newGameLabel.ForeColor = System.Drawing.SystemColors.Window;
      this.newGameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.newGameLabel.Cursor = Cursors.Hand;
      this.newGameLabel.MouseEnter += delegate (object sender, EventArgs e) { this.newGameLabel.ForeColor = System.Drawing.SystemColors.Highlight; };
      this.newGameLabel.MouseLeave += delegate (object sender, EventArgs e) { this.newGameLabel.ForeColor = System.Drawing.SystemColors.Window; };
      this.newGameLabel.Click += delegate (object sender, EventArgs e) {
        this.menuPanel.Visible = false;
        this.levelPanel.Visible = true;
      };

      this.resumeLabel.Parent = this.menuPanel;
      this.resumeLabel.Text = "Resume";
      this.resumeLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.resumeLabel.ForeColor = System.Drawing.SystemColors.Window;
      this.resumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.resumeLabel.Cursor = Cursors.Hand;
      this.resumeLabel.MouseEnter += delegate (object sender, EventArgs e) { this.resumeLabel.ForeColor = System.Drawing.SystemColors.Highlight; };
      this.resumeLabel.MouseLeave += delegate (object sender, EventArgs e) { this.resumeLabel.ForeColor = System.Drawing.SystemColors.Window; };
      this.resumeLabel.Click += delegate (object sender, EventArgs e) {
        this.menuPanel.Visible = false;
        this.gamePanel.Visible = true;
        this.boardPanel.Visible = true;
      };

      this.setingsLabel.Parent = this.menuPanel;
      this.setingsLabel.Text = "Setings";
      this.setingsLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.setingsLabel.ForeColor = System.Drawing.SystemColors.Window;
      this.setingsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.setingsLabel.Cursor = Cursors.Hand;
      this.setingsLabel.MouseEnter += delegate (object sender, EventArgs e) { this.setingsLabel.ForeColor = System.Drawing.SystemColors.Highlight; };
      this.setingsLabel.MouseLeave += delegate (object sender, EventArgs e) { this.setingsLabel.ForeColor = System.Drawing.SystemColors.Window; };
      this.setingsLabel.Click += delegate (object sender, EventArgs e) {
      };

      this.statisticsLabel.Parent = this.menuPanel;
      this.statisticsLabel.Text = "Statistics";
      this.statisticsLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.statisticsLabel.ForeColor = System.Drawing.SystemColors.Window;
      this.statisticsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.statisticsLabel.Cursor = Cursors.Hand;
      this.statisticsLabel.MouseEnter += delegate (object sender, EventArgs e) { this.statisticsLabel.ForeColor = System.Drawing.SystemColors.Highlight; };
      this.statisticsLabel.MouseLeave += delegate (object sender, EventArgs e) { this.statisticsLabel.ForeColor = System.Drawing.SystemColors.Window; };
      this.statisticsLabel.Click += delegate (object sender, EventArgs e) {
      };

      this.helpLabel.Parent = this.menuPanel;
      this.helpLabel.Text = "Help";
      this.helpLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.helpLabel.ForeColor = System.Drawing.SystemColors.Window;
      this.helpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.helpLabel.Cursor = Cursors.Hand;
      this.helpLabel.Enabled = false;
      this.helpLabel.MouseEnter += delegate (object sender, EventArgs e) { this.helpLabel.ForeColor = System.Drawing.SystemColors.Highlight; };
      this.helpLabel.MouseLeave += delegate (object sender, EventArgs e) { this.helpLabel.ForeColor = System.Drawing.SystemColors.Window; };
      this.helpLabel.Click += delegate (object sender, EventArgs e) {
      };

      this.exitLabel.Parent = this.menuPanel;
      this.exitLabel.Text = "Exit";
      this.exitLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.exitLabel.ForeColor = System.Drawing.SystemColors.Window;
      this.exitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.exitLabel.Cursor = Cursors.Hand;
      this.exitLabel.MouseEnter += delegate (object sender, EventArgs e) { this.exitLabel.ForeColor = System.Drawing.SystemColors.Highlight; };
      this.exitLabel.MouseLeave += delegate (object sender, EventArgs e) { this.exitLabel.ForeColor = System.Drawing.SystemColors.Window; };
      this.exitLabel.Click += delegate (object sender, EventArgs e) {
        this.Close();
      };

      this.levelPanel.Parent = this;
      this.levelPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.levelPanel.Visible = false;

      for (int index = 0; index < this.levelLabels.Length; index++) {
        this.levelLabels[index] = new Label();
        this.levelLabels[index].Parent = this.levelPanel;
        this.levelLabels[index].Text = Enum.GetNames(typeof(Level))[index];
        this.levelLabels[index].BackColor = System.Drawing.SystemColors.ControlDarkDark;
        this.levelLabels[index].ForeColor = System.Drawing.SystemColors.Window;
        this.levelLabels[index].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.levelLabels[index].Cursor = Cursors.Hand;
        this.levelLabels[index].MouseEnter += delegate (object sender, EventArgs e) { (sender as Label).ForeColor = System.Drawing.SystemColors.Highlight; };
        this.levelLabels[index].MouseLeave += delegate (object sender, EventArgs e) { (sender as Label).ForeColor = System.Drawing.SystemColors.Window; };
        this.levelLabels[index].Click += delegate (object sender, EventArgs e) { this.NewGame((Level)Enum.Parse(typeof(Level), (sender as Label).Text)); };
      }

      this.boardPanel.Parent = this;
      this.boardPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      //this.boardPanel.Dock = DockStyle.Bottom;

      this.homePicture.Parent = this.boardPanel;
      this.homePicture.Size = new Size(32, 32);
      this.homePicture.Image = Sudoku.Properties.Resources.Home;
      this.homePicture.Location = new Point(9, 9);
      this.homePicture.Cursor = Cursors.Hand;
      this.homePicture.MouseEnter += delegate (object sender, EventArgs e) { this.homePicture.Image = Sudoku.Properties.Resources.HomeHighlight; };
      this.homePicture.MouseLeave += delegate (object sender, EventArgs e) { this.homePicture.Image = Sudoku.Properties.Resources.Home; };
      this.homePicture.Click += delegate (object sender, EventArgs e) {
        this.gamePanel.Visible = false;
        this.boardPanel.Visible = false;
        this.menuPanel.Visible = true;
      };

      this.chrono.Parent = this.boardPanel;
      this.chrono.Font = new Font("Courier New", 23, System.Drawing.FontStyle.Bold);
      this.chrono.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.chrono.ForeColor = System.Drawing.SystemColors.Window;
      this.chrono.AutoSize = true;
      this.chrono.Reset();

      this.gamePanel.Parent = this;
      this.gamePanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      //this.boardPanel.Dock = DockStyle.Fill;

      for (int i = 0; i < 81; i++) {
        this.cells[i / 9, i % 9] = new Cell();
        this.cells[i / 9, i % 9].Parent = this.gamePanel;
        this.cells[i / 9, i % 9].ValueChanged += delegate (object sender, EventArgs e) {
          if (!this.chrono.IsRunning)
            this.chrono.Start();
          this.CheckGame();
        };
      }

      this.OnSizeChanged(EventArgs.Empty);

      LoadGame();
    }

    private void NewGame(Level level) {
      this.level = level;
      this.Text = string.Format("Sudoku - {0}", this.level);

      for (int i = 0; i < this.game.GetLength(0); i++)
        for (int j = 0; j < this.game.GetLength(1); j++) {
          cells[i, j].ReadOnly = true;
          cells[i, j].Cursor = Cursors.Default;
        }

      Random rand = new Random(System.DateTime.Now.GetHashCode());
      this.game = GenerateGrid(rand);

      System.Collections.Generic.SortedDictionary<Level, int> levelToNumberOfRemovdCells = new System.Collections.Generic.SortedDictionary<Level, int>();
      levelToNumberOfRemovdCells.Add(Level.Easy, 26);
      levelToNumberOfRemovdCells.Add(Level.Medium, 37);
      levelToNumberOfRemovdCells.Add(Level.Hard, 48);
      levelToNumberOfRemovdCells.Add(Level.Impossible, 59);
      for (int i = 0; i < levelToNumberOfRemovdCells[this.level]; i++) {
        int row = rand.Next(0, 9);
        int col = rand.Next(0, 9);
        cells[row, col].ReadOnly = false;
        cells[row, col].Cursor = Cursors.IBeam;
      }

      this.RestartGame();
      this.Focus();
      this.chrono.Reset();
    }

    private void RestartGame() {
      this.menuPanel.Visible = false;
      this.levelPanel.Visible = false;
      this.gamePanel.Visible = true;
      this.boardPanel.Visible = true;

      for (int i = 0; i < this.game.GetLength(0); i++)
        for (int j = 0; j < this.game.GetLength(1); j++)
          cells[i, j].Value = cells[i, j].ReadOnly ? this.game[i, j] : 0;
      this.winLabel.Visible = false;
    }

    void SolveGame() {
      for (int i = 0; i < this.game.GetLength(0); i++)
        for (int j = 0; j < this.game.GetLength(1); j++)
          cells[i, j].Value = this.game[i, j];
    }

    private void LoadGame() { NewGame(Level.Medium); }

    private void SaveGame() { }

    private void CheckGame() {
      for (int i = 0; i < cells.GetLength(0); i++) {
        for (int j = 0; j < cells.GetLength(1); j++) {
          if (cells[i, j].Value != this.game[i, j]) {
            return;
          }
        }
      }

      this.chrono.Stop();
      this.winLabel.Visible = true;
      this.Invalidate(true);
    }

    private new bool Focus() {
      for (int i = 0; i < cells.GetLength(0); i++) {
        for (int j = 0; j < cells.GetLength(1); j++) {
          if (!cells[i, j].ReadOnly) {
            cells[i, j].Focus();
            return true;
          }
        }
      }
      return false;
    }

    private int[,] GenerateGrid(System.Random rand) {
      int[,] values = new int[9, 9];
      Array.Clear(values, 0, values.Length);

      for (int j = 0; j < 9; j += 3) {
        for (int k = 0; k < 9; k += 3) {
          for (int t = 1; t <= 9; t++) {
            int count;
            for (count = 0; count < 20; count++) {
              int m = j + rand.Next(0, 3);
              int n = k + rand.Next(0, 3);
              if (m >= 0 && m < 9 && n >= 0 && n < 9 && values[m, n] == 0) {
                int mm;
                for (mm = 0; mm < m; mm++)
                  if (values[mm, n] == t) break;
                if (mm < m) continue;

                int nn;
                for (nn = 0; nn < n; nn++)
                  if (values[m, nn] == t) break;
                if (nn < n) continue;

                values[m, n] = t;
                break;
              }
            }

            if (count == 20) {
              k = 9;
              j = -3;
              Array.Clear(values, 0, values.Length);
              //Array.Clear(values, 0, 9 * 9);
            }
          }
        }
      }
      return values;
    }

    private int[,] game = new int[9, 9];
    private MainMenu mainMenu = new MainMenu();
    private Label winLabel = new Label();
    private Panel gamePanel = new Panel();
    private Cell[,] cells = new Cell[9, 9];
    private Panel boardPanel = new Panel();
    private PictureBox homePicture = new PictureBox();
    private Chrono chrono = new Chrono();
    private Panel menuPanel = new Panel();
    private Label newGameLabel = new Label();
    private Label resumeLabel = new Label();
    private Label setingsLabel = new Label();
    private Label statisticsLabel = new Label();
    private Label helpLabel = new Label();
    private Label exitLabel = new Label();
    private Panel levelPanel = new Panel();
    private Label[] levelLabels = new Label[Enum.GetValues(typeof(Level)).Length];
    private Level level;
  }
}
