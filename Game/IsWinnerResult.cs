// File Name:     IsWinnerResult.cs
// By:            Saidi Tarik
// Date:          16, 09, 2022

using System.Collections.Generic;
using System.Drawing;

namespace ConnectFour.Game
{
    public sealed class IsWinnerResult
    {
        public bool PlayerWon { get; private set; }


        public HashSet<Point> WinningLocations { get; private set; }

        public IsWinnerResult(bool playerWon, HashSet<Point> winningLocations)
        {
            PlayerWon = playerWon;
            WinningLocations = winningLocations;
        }
    }
}