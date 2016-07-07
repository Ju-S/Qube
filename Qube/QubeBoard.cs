using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qube
{
    class QubeBoard
    {
        #region type

        Random rand = new Random();

        public static readonly int
            GameBoardWidth = 10,
            GameBoardHeight = 10;

        private int
            ClearCondition = 1,
            ClearCounter = 0;

        public bool
            clear,
            gameOver;

        private Color
            OldColor;

        private QubeBase[,] QubePieces =
            new QubeBase[GameBoardWidth, GameBoardHeight];

        private int[] HelpList = new int[GameBoardWidth];

        public List<Vector2>
            QubeTracker = new List<Vector2>();

        #endregion

        public void ClearBoard()
        {
            for (int x = 0; x < GameBoardWidth; x++)
                for (int y = 0; y < GameBoardHeight; y++)
                {
                    if (y == GameBoardHeight - 1)
                        HelpList[x] = 0;
                    QubePieces[x, y] = new QubeBase(Color.White);
                }
        }

        public QubeBoard()
        {
            ClearBoard();
        }

        public Color GetQube(int x, int y)
        {
            return QubePieces[x, y].Type;
        }

        public void SetQube(int x, int y, Color color)
        {
            QubePieces[x, y].SetQube(color);
        }

        public int GetHelpList(int x)
        {
            return HelpList[x];
        }

        public void SetClearCondition(int currentLv)
        {
            ClearCondition = currentLv;
        }

        public void RandomQube(int x, int y)
        {
            QubePieces[x, y].SetQube(QubeBase.ColorList[rand.Next(0, QubeBase.MaxColorIndex + 1)]);
        }

        public void FillFromAbove(int x, int y)
        {
            int rowLookup = y - 1;
            while (rowLookup >= 0)
            {
                if (GetQube(x, rowLookup) != Color.White)
                {
                    SetQube(x, y, GetQube(x, rowLookup));
                    SetQube(x, rowLookup, Color.White);
                    rowLookup = -1;
                }
                rowLookup--;
            }
        }

        public void FillFromNext(int x, int y)
        {
            QubeBase Temp;
            for (; x > 0; x--)
                for (; y >= 0; y--)
                {
                    Temp = QubePieces[x, y];
                    QubePieces[x, y] = QubePieces[x - 1, y];
                    QubePieces[x - 1, y] = Temp;
                    if (HelpList[x - 1] == 1)
                    {
                        HelpList[x] = 1;
                        HelpList[x - 1] = 0;
                    }
                }
        }

        public void DropNewPieces()
        {
            clear = true;
            for (int x = GameBoardWidth - 1; x >= 0; x--)
                for (int y = GameBoardHeight - 1; y >= 0; y--)
                {
                    if (GetQube(x, y) == Color.White)
                        FillFromAbove(x, y);
                    if (y == GameBoardHeight - 1)
                    {
                        if (GetQube(x, y) == Color.White)
                        {
                            if (HelpList[x] == 1)
                                HelpList[x] = 0;
                            FillFromNext(x, y);
                        }
                        if (HelpList[x] == 1)
                            clear = false;
                    }
                }
        }

        public void CheckGameOver()
        {
            gameOver = true;
            for (int x = GameBoardWidth - 1; x >= 0; x--)
            {
                OldColor = Color.White;
                for (int y = GameBoardHeight - 1; y >= 0; y--)
                {
                    if (GetQube(x, y) == Color.White)
                    {
                        OldColor = GetQube(x, y);
                        continue;
                    }
                    if (OldColor != GetQube(x, y))
                        OldColor = GetQube(x, y);
                    else
                        gameOver = false;
                }
            }

            for (int y = GameBoardHeight - 1; y >= 0; y--)
            {
                OldColor = Color.White;
                for (int x = GameBoardWidth - 1; x >= 0; x--)
                {
                    if (GetQube(x, y) == Color.White)
                    {
                        OldColor = GetQube(x, y);
                        continue;
                    }
                    if (OldColor != GetQube(x, y))
                        OldColor = GetQube(x, y);
                    else
                        gameOver = false;
                }
            }
        }

        public void GenerateNewPieces()
        {
            for (int y = 0; y < GameBoardHeight; y++)
                for (int x = 0; x < GameBoardWidth; x++)
                {
                    if (GetQube(x, y) == Color.White)
                        RandomQube(x, y);
                }
            for (int x = 0; ; x++)
            {
                int X = x % GameBoardWidth;
                if (ClearCounter < ClearCondition)
                {
                    if (rand.Next(0, 10) == 1 && HelpList[X] == 0)
                    {
                        ClearCounter++;
                        HelpList[X] = 1;
                    }
                }
                else
                    break;
            }
            ClearCounter = 0;
        }

        public void PropagateColor(int x, int y, Color color)
        {
            for (int i = -1; i < 2; i += 2)
                if ((y + i >= 0) && (y + i < GameBoardHeight))
                    if (GetQube(x, y + i) == color && !QubeTracker.Contains(new Vector2(x, y + i)))
                    {
                        QubeTracker.Add(new Vector2(x, y + i));
                        PropagateColor(x, y + i, GetQube(x, y + i));
                    }

            for (int i = -1; i < 2; i += 2)
                if ((x + i >= 0) && (x + i < GameBoardWidth))
                    if (GetQube(x + i, y) == color && !QubeTracker.Contains(new Vector2(x + i, y)))
                    {
                        QubeTracker.Add(new Vector2(x + i, y));
                        PropagateColor(x + i, y, GetQube(x + i, y));
                    }
        }

        public List<Vector2> GetQubeChain(int x, int y)
        {
            PropagateColor(x, y, GetQube(x, y));
            return QubeTracker;
        }
    }
}