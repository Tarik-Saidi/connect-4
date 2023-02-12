// File Name:     ConnectFourContainer.cs
// By:            Saidi Tarik
// Date:          16, 09, 2022

using ConnectFour.Game.Enums;
using ConnectFour.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace ConnectFour.Game.Controls
{
    public sealed partial class ConnectFourContainer : Control
    {

        public const int LineThickness = 8;

        public const int TableHorizontalOffset = 150;

        public bool IsSoundMuted { get; set; } = false;

        [Browsable(false)]
        public int GridSquareWidth { get; private set; }

        [Browsable(false)]
        public int GridSquareHeight { get; private set; }

        [Browsable(false)]
        public int BoardHorizontalPadding { get; private set; }

        [Browsable(false)]
        public int BoardVerticalPadding { get; private set; }

        [Browsable(false)]
        public ConnectFourBoard GameBoard { get; private set; }

        [Browsable(false)]
        public int CurrentHoveredColumn { get; private set; }

        public delegate void OnClickedFullColumnHandler(object sender);
        public event OnClickedFullColumnHandler OnClickedFullColumn;

        public ConnectFourContainer() : this(Chip.Red, true)
        {

        }
        public ConnectFourContainer(Chip firstPlayerChip, bool isOpponentComputer)
        {
            Cursor = Cursors.Default;
            DoubleBuffered = true;
            Font = new Font("Arial", 18, FontStyle.Bold);
            GameBoard = new ConnectFourBoard(7, 6, firstPlayerChip, isOpponentComputer);

            SubscribeToEvents();
            OnResize(null);
        }


        private void SubscribeToEvents()
        {
            GameBoard.OnChipPlaced += GameBoard_OnChipPlaced;
            GameBoard.OnGameOver += GameBoard_OnGameOver;
            GameBoard.OnNewGame += GameBoard_OnNewGame;
        }

        private void GameBoard_OnChipPlaced(object sender)
        {
            PlaySoundEffect(Resources.Pop_Sound_Effect);

            Invalidate();
        }

        private void GameBoard_OnNewGame(object sender)
        {
            Invalidate();
        }

        private void GameBoard_OnGameOver(object sender, GameStatus gameOutcome)
        {
            PlaySoundEffect(Resources.Game_Over_Sound_Effect);
        }

        private void PlaySoundEffect(UnmanagedMemoryStream soundResource)
        {
            if (!IsSoundMuted)
            {
                using (SoundPlayer soundEffect = new SoundPlayer(soundResource))
                {
                    new Thread(() =>
                    {
                        soundEffect.Play();
                    }).Start();
                }
            }
        }

        private void DrawChip(Graphics g, Chip chip, int column, int row)
        {
            int chipPaddingX = (int)(GridSquareWidth * 0.70);
            int chipPaddingY = (int)(GridSquareHeight * 0.70); 

            Rectangle chipBounds = new Rectangle(BoardHorizontalPadding + (column * GridSquareWidth) + chipPaddingX + LineThickness,
                BoardVerticalPadding + row * GridSquareHeight + chipPaddingY + LineThickness,
                GridSquareWidth - (chipPaddingX * 2) - (LineThickness * 2),
                GridSquareHeight - (chipPaddingY * 2) - (LineThickness * 2));

            Color topColor;
            Color bottomColor;

            switch (chip) 
            {
                default:
                case Chip.None:
                    topColor = Color.FromArgb(215, 215, 215);
                    bottomColor = Color.FromArgb(255, 255, 255);
                    break;
                case Chip.Red:
                    topColor = Color.FromArgb(155, 0, 0);
                    bottomColor = Color.FromArgb(245, 0, 0);
                    break;
                case Chip.Yellow:
                    topColor = Color.FromArgb(203, 197, 49);
                    bottomColor = Color.FromArgb(227, 229, 123);
                    break;
            }

            using (var br = new LinearGradientBrush(chipBounds, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                g.FillEllipse(br, chipBounds);

                using (var pen = new Pen(Color.Black, LineThickness))
                {
                    g.DrawEllipse(pen, chipBounds);
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (GameBoard.IsGameOver)
            {
                GameBoard.StartNewGame(false);
            }
            else if (!GameBoard.IsColumnAvailable(CurrentHoveredColumn) && !GameBoard.IsComputerTurn)
            {
                OnClickedFullColumn?.Invoke(this);
            }
            else
            {
                if (!GameBoard.IsComputerTurn)
                {
                    GameBoard.PlaceChip(CurrentHoveredColumn, GameBoard.CurrentChipTurn);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int hoverColumn = (e.X - BoardHorizontalPadding) / GridSquareWidth;

            if (hoverColumn < 0 || hoverColumn >= GameBoard.Columns)
            {
                hoverColumn = hoverColumn < 0 ? 0 : GameBoard.Columns - 1;
            }

            if (CurrentHoveredColumn != hoverColumn)
            {
                CurrentHoveredColumn = hoverColumn;
                
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GridSquareHeight == 0 || GridSquareWidth == 0)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle backgroundBounds = new Rectangle(-2, -2, Width + 2, Height + 2);
            using (LinearGradientBrush br = new LinearGradientBrush(backgroundBounds, Color.FromArgb(3, 78, 146), Color.FromArgb(3, 5, 40), 45))
            {
                g.FillRectangle(br, backgroundBounds);
            }

            using (var br = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(126, 85, 63), Color.FromArgb(64, 42, 29), 90))
            {
                Point[] floorPointsArray =
                {
                    new Point(0, Height + LineThickness),
                    new Point(TableHorizontalOffset, Height - BoardVerticalPadding - (GridSquareHeight / 2)),
                    new Point(Width - TableHorizontalOffset, Height - BoardVerticalPadding - (GridSquareHeight / 2)),
                    new Point(Width, Height + LineThickness)
                };

                g.FillPolygon(br, floorPointsArray);

                using (var p = new Pen(Color.Black, LineThickness))
                {
                    g.DrawPolygon(p, floorPointsArray);
                }
            }

            Rectangle gameBoardBounds = new Rectangle(BoardHorizontalPadding, BoardVerticalPadding, (GridSquareWidth * GameBoard.Columns), (GridSquareHeight * GameBoard.Rows));
            using (LinearGradientBrush br = new LinearGradientBrush(gameBoardBounds, Color.FromArgb(64, 138, 196), Color.FromArgb(18, 82, 129), 90))
            {
                g.FillRectangle(br, gameBoardBounds);
            }

            HashSet<Point> winLocations = GameBoard.GetWinLocations();
            if (winLocations != null)
            {
                foreach (Point winLocation in winLocations)
                {
                    Rectangle winGridBoxBounds = new Rectangle(BoardHorizontalPadding + winLocation.X * GridSquareWidth,
                        BoardVerticalPadding + winLocation.Y * GridSquareHeight,
                        GridSquareWidth,
                        GridSquareHeight);

                    using (LinearGradientBrush br = new LinearGradientBrush(winGridBoxBounds, Color.FromArgb(168, 224, 99), Color.FromArgb(86, 171, 47), 90))
                    {
                        g.FillRectangle(br, winGridBoxBounds);
                    }
                }
            }

            using (Pen pen = new Pen(Color.Black, LineThickness))
            {
                for (int i = 1; i < GameBoard.Rows; i++)
                {
                    g.DrawLine(pen,
                        BoardHorizontalPadding,
                        BoardVerticalPadding + (i * GridSquareHeight),
                        BoardHorizontalPadding + (GridSquareWidth * GameBoard.Columns),
                        BoardVerticalPadding + (i * GridSquareHeight));
                }

                for (int i = 1; i < GameBoard.Columns; i++)
                {
                    g.DrawLine(pen,
                        BoardHorizontalPadding + (i * GridSquareWidth),
                        BoardVerticalPadding,
                        BoardHorizontalPadding + (i * GridSquareWidth),
                        BoardVerticalPadding + (GridSquareHeight * GameBoard.Rows));
                }

                g.DrawRectangle(pen, gameBoardBounds);
            }

            for (int row = 0; row < GameBoard.Rows; row++)
            {
                for (int col = 0; col < GameBoard.Columns; col++)
                {
                    DrawChip(g, GameBoard[row, col], col, row);
                }
            }

            if (!DesignMode && !GameBoard.IsGameOver)
            {
                if (!GameBoard.IsComputerTurn)
                {
                    DrawChip(g, GameBoard.CurrentChipTurn, CurrentHoveredColumn, -1);
                }
            }

            using (StringFormat sf = new StringFormat())
            {
                Rectangle textBounds = new Rectangle(0,
                    BoardVerticalPadding + (GridSquareHeight * GameBoard.Rows),
                    Width,
                    (int)(GridSquareHeight * 1.5) - ((int)Font.Size / 2));

                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                g.DrawString(GameBoard.IsGameOver ? "Click anywhere to start a new round..." : $"{GameBoard.Scores[Chip.Red]} : {GameBoard.Scores[Chip.Yellow]}",
                    Font,
                    Brushes.White,
                    textBounds,
                    sf);
            }
        }


        protected override void OnResize(EventArgs e)
        {
            BoardHorizontalPadding = (int)(Width * 0.25);
            BoardVerticalPadding = (int)(Height * 0.15);

            GridSquareWidth = (Width - BoardHorizontalPadding * 2) / GameBoard.Columns;
            GridSquareHeight = (Height - BoardVerticalPadding * 2) / GameBoard.Rows;

            Invalidate();
        }
    }
}