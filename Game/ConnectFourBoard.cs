// File Name:     ConnectFourBoard.cs
// By:            Saidi Tarik
// Date:          16, 09, 2022

using ConnectFour.Game.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace ConnectFour.Game
{
    public sealed class ConnectFourBoard
    {

        public readonly int Columns;

        public readonly int Rows;

        private Chip _firstPlayerChip;
        public Chip FirstPlayerChip
        {
            get
            {
                return _firstPlayerChip;
            }
            set
            {
                if (value == Chip.None)
                {
                    throw new Exception("Invalid chip!");
                }

                _firstPlayerChip = value;
            }
        }


        public Chip CurrentChipTurn { get; private set; }

        public Dictionary<Chip, uint> Scores { get; private set; } = new Dictionary<Chip, uint>()
        {
            { Chip.Red, 0 },
            { Chip.Yellow, 0 }
        };


        public bool IsOpponentComputer { get; set; }


        public bool IsComputerTurn => IsOpponentComputer && CurrentChipTurn == ComputerPlayerChip;


        public bool IsGameOver => CurrentGameStatus != GameStatus.OngoingGame;


        public bool IsFilled
        {
            get
            {
                for (int col = 0; col < gameBoardChips.GetLength(1); col++)
                {
                    if (gameBoardChips[0, col] == Chip.None)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public Chip this[int row, int col] => gameBoardChips[row, col];

        public Chip ComputerPlayerChip => FirstPlayerChip == Chip.Red ? Chip.Yellow : Chip.Red;


        public GameStatus CurrentGameStatus
        {
            get
            {
                if (IsWinner(Chip.Red).PlayerWon)
                {
                    return GameStatus.RedChipWon;
                }
                else if (IsWinner(Chip.Yellow).PlayerWon)
                {
                    return GameStatus.YellowChipWon;
                }
                else if (IsFilled)
                {
                    return GameStatus.TiedGame;
                }
                else
                {
                    return GameStatus.OngoingGame;
                }
            }
        }


        public delegate void OnChipPlacedHandler(object sender);
        public event OnChipPlacedHandler OnChipPlaced;

        public delegate void OnGameOverHandler(object sender, GameStatus endResult);
        public event OnGameOverHandler OnGameOver;


        public delegate void OnGameResetHandler(object sender);
        public event OnGameResetHandler OnGameReset;


        public delegate void OnNewGameHandler(object sender);
        public event OnNewGameHandler OnNewGame;


        public delegate void OnSwitchTurnHandler(object sender);
        public event OnSwitchTurnHandler OnSwitchTurn;

        private Chip[,] gameBoardChips;


        private Random random;

        public ConnectFourBoard(int columns, int rows, Chip firstPlayerChip, bool isOpponentComputer)
        {
            if (columns < 7)
            {
                throw new Exception("The number of columns must be greater than or equal to 7.");
            }

            if (rows < 6)
            {
                throw new Exception("The number of columns must be greater than or equal to 6.");
            }

            if (firstPlayerChip == Chip.None)
            {
                throw new Exception("Invalid chip type! The first player chip can be either red or yellow.");
            }

            Columns = columns;
            Rows = rows;
            CurrentChipTurn = FirstPlayerChip = firstPlayerChip;
            IsOpponentComputer = isOpponentComputer;

            gameBoardChips = new Chip[rows, columns];
            random = new Random();

            StartNewGame(false);
        }


        public HashSet<Point> GetWinLocations()
        {
            IsWinnerResult result;

            return (result = IsWinner(Chip.Red)).PlayerWon || (result = IsWinner(Chip.Yellow)).PlayerWon ? result.WinningLocations : null;
        }

        public bool IsColumnAvailable(int column)
        {
            return gameBoardChips[0, column] == Chip.None;
        }

        public List<int> GetAvailableColumns()
        {
            List<int> availableColumns = new List<int>();

            for (int col = 0; col < Columns; col++)
            {
                if (IsColumnAvailable(col))
                {
                    availableColumns.Add(col);
                }
            }

            return availableColumns;
        }

        public void PlaceChip(int column, Chip chip)
        {
            if (!IsColumnAvailable(column))
            {
                throw new Exception($"The column {column} is filled in completely!");
            }

            if (chip == Chip.None)
            {
                throw new InvalidEnumArgumentException("The chip is invalid!");
            }

            gameBoardChips[GetNextAvailableRow(column), column] = chip;

            OnChipPlaced?.Invoke(this);

            if (IsGameOver)
            {
                UpdateScore();

                OnGameOver?.Invoke(this, CurrentGameStatus);
            }
            else
            {
                SwitchTurns();
            }
        }


        private void UpdateScore()
        {
            GameStatus gameResult = CurrentGameStatus;

            if (gameResult == GameStatus.RedChipWon || gameResult == GameStatus.YellowChipWon)
            {
                Scores[gameResult == GameStatus.RedChipWon ? Chip.Red : Chip.Yellow]++;
            }
        }

        public void PerformComputeMove()
        { 
            if (!IsOpponentComputer || CurrentChipTurn != ComputerPlayerChip)
            {
                throw new Exception(!IsOpponentComputer ? "The opponent is not a computer player!" : "It's not the opponents turn!");
            }

            List<int> possibleMoves = GetAvailableColumns();

            if (possibleMoves.Count > 0)
            {
                Task.Run(async() =>
                {
                    int column = possibleMoves[random.Next(0, possibleMoves.Count - 1)];

                    await Task.Delay(random.Next(500, 1500));

                    PlaceChip(column, ComputerPlayerChip);
                });
            }
        }

        public void StartNewGame(bool resetScores)
        {
            OnGameReset?.Invoke(this);

            for (int row = 0; row < gameBoardChips.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoardChips.GetLength(1); col++)
                {
                    gameBoardChips[row, col] = Chip.None;
                }
            }

            if (resetScores)
            {
                Scores[Chip.Red] = Scores[Chip.Yellow] = 0;
            }

            if (IsComputerTurn)
            {
                PerformComputeMove();
            }

            OnNewGame?.Invoke(this);
        }


        private int GetNextAvailableRow(int column)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (gameBoardChips[row, column] == Chip.None)
                {
                    return row;
                }
            }

            return -1; 
        }


        private void SwitchTurns()
        {
            CurrentChipTurn = CurrentChipTurn == Chip.Red ? Chip.Yellow : Chip.Red;

            OnSwitchTurn?.Invoke(this);

            if (IsComputerTurn)
            {
                PerformComputeMove();
            }
        }

        private IsWinnerResult IsWinner(Chip chip)
        {
            if (chip == Chip.None)
            {
                throw new InvalidEnumArgumentException("The chip is invalid!");
            }

            HashSet<Point> winningLocations = new HashSet<Point>();


            for (int row = 0; row < gameBoardChips.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoardChips.GetLength(1) - 3; col++)
                {
                    if (gameBoardChips[row, col] == chip
                        && gameBoardChips[row, col + 1] == chip
                        && gameBoardChips[row, col + 2] == chip
                        && gameBoardChips[row, col + 3] == chip)
                    {
                        for (int i = 0; i <= 3; i++)
                        {
                            winningLocations.Add(new Point(col + i, row));
                        }
                    }
                }
            }


            for (int row = 0; row < gameBoardChips.GetLength(0) - 3; row++)
            {
                for (int col = 0; col < gameBoardChips.GetLength(1); col++)
                {
                    if (gameBoardChips[row, col] == chip
                        && gameBoardChips[row + 1, col] == chip
                        && gameBoardChips[row + 2, col] == chip
                        && gameBoardChips[row + 3, col] == chip)
                    {
                        for (int i = 0; i <= 3; i++)
                        {
                            winningLocations.Add(new Point(col, row + i));
                        }
                    }
                }
            }


            for (int row = 0; row < gameBoardChips.GetLength(0) - 3; row++)
            {
                for (int col = 0; col < gameBoardChips.GetLength(1) - 3; col++)
                {
                    if (gameBoardChips[row, col] == chip
                        && gameBoardChips[row + 1, col + 1] == chip
                        && gameBoardChips[row + 2, col + 2] == chip
                        && gameBoardChips[row + 3, col + 3] == chip)
                    {
                        for (int i = 0; i <= 3; i++)
                        {
                            winningLocations.Add(new Point(col + i, row + i));
                        }
                    }
                }
            }


            for (int row = 0; row < gameBoardChips.GetLength(0) - 3; row++)
            {
                for (int col = gameBoardChips.GetLength(1) - 1; col >= 3; col--)
                {
                    if (gameBoardChips[row, col] == chip
                        && gameBoardChips[row + 1, col - 1] == chip
                        && gameBoardChips[row + 2, col - 2] == chip
                        && gameBoardChips[row + 3, col - 3] == chip)
                    {
                        for (int i = 0; i <= 3; i++)
                        {
                            winningLocations.Add(new Point(col - i, row + i));
                        }
                    }
                }
            }

            return new IsWinnerResult(winningLocations.Count > 0, winningLocations);
        }
    }
}