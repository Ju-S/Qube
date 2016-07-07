using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Qube
{
    public class Game1 : Game
    {
        #region type
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D
            Background,
            Qube,
            Character,
            Health;

        QubeBoard qubeBoard = new QubeBoard();

        QubeBase qubeBase = new QubeBase(Color.White);

        Point qubeBoardDisplayOrigin;

        Vector2 currentLevelPosition = new Vector2(120, 40),
            scorePosition = new Vector2(410, 40),
            clearPosition = new Vector2(180, 500);

        Rectangle healthPosition = new Rectangle(119, 148, 631, 50);

        float
            MaxHealthTime = 60.0f,
            healthTime = 0.0f,
            healthAcceleration = 2.0f,
            gameOverCheck = 0.3f,
            titleCheck = 0.1f,
            gameOverTime = 0.0f;

        int playerScore = 0,
            currentLv,
            ScreenWidth = 800,
            ScreenHeight;

        enum GameState { TitleScreen, Playing, gameOver };

        GameState gameState = GameState.TitleScreen;

        Rectangle EmptyPiece;

        Random rand = new Random();

        bool IsMouseDown = false;

        SpriteFont
            currentLevel,
            clearFont;

        const int BlockCondition = 1;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            ScreenHeight = Convert.ToInt32(ScreenWidth * 1.5);
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Qube = Content.Load<Texture2D>("Textures/QubeBase");
            Background = Content.Load<Texture2D>("Textures/Background");
            Character = Content.Load<Texture2D>("Textures/Character");
            Health = Content.Load<Texture2D>("Textures/Air");
            EmptyPiece = new Rectangle(0, 0, 70, 70);
            qubeBoardDisplayOrigin = new Point(50, 249);
            currentLevel = Content.Load<SpriteFont>("Fonts/Pericle");
            clearFont = Content.Load<SpriteFont>("Fonts/Clear");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            switch (gameState)
            {
                case GameState.TitleScreen:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        qubeBoard.ClearBoard();
                        playerScore = 0;
                        currentLv = 1;
                        gameOverTime = 0.0f;
                        qubeBoard.SetClearCondition(currentLv);
                        healthPosition.Width = 631;
                        qubeBoard.GenerateNewPieces();
                        gameState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    if (currentLv == 6)
                        gameState = GameState.gameOver;
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !IsMouseDown)
                    {
                        IsMouseDown = true;
                        HandleMouseInput(Mouse.GetState());
                    }
                    else if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        IsMouseDown = false;
                    }

                    qubeBoard.DropNewPieces();

                    if (qubeBoard.clear)
                    {
                        qubeBoard.SetClearCondition(++currentLv);
                        healthAcceleration -= 0.3f;
                        healthPosition.Width = 631;
                        qubeBoard.clear = false;
                        qubeBoard.ClearBoard();
                        qubeBoard.GenerateNewPieces();
                    }

                    gameOverTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (gameOverTime >= gameOverCheck)
                    {
                        gameOverTime = 0.0f;
                        qubeBoard.CheckGameOver();
                        if (qubeBoard.gameOver)
                            gameState = GameState.gameOver;
                    }

                    healthTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (healthTime >= MaxHealthTime)
                    {
                        healthTime = 0.0f;
                        healthPosition.Width -= (int)healthAcceleration;
                        if (healthPosition.Width <= 0)
                            gameState = GameState.gameOver;
                    }
                    break;
                case GameState.gameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        gameState = GameState.TitleScreen;
                    break;
            }
            base.Update(gameTime);
        }

        private Color randomColor()
        {
            return QubeBase.ColorList[rand.Next(0, QubeBase.MaxColorIndex + 1)];
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            if (gameState == GameState.TitleScreen)
            {
                spriteBatch.Begin();
                gameOverTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (gameOverTime >= titleCheck)
                {
                    spriteBatch.Draw(
                    Character, new Rectangle(ScreenWidth - 650, ScreenHeight - 1000, 512, 650), Color.White);
                    spriteBatch.DrawString(currentLevel, "Press Space to Start", new Vector2(150, 800), Color.Black);
                }
                spriteBatch.End();
            }
            if (gameState == GameState.Playing || gameState == GameState.gameOver)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(
                    Background, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
                for (int x = 0; x < QubeBoard.GameBoardWidth; x++)
                {
                    for (int y = 0; y < QubeBoard.GameBoardHeight; y++)
                    {
                        int pixelX = (int)qubeBoardDisplayOrigin.X +
                            (x * EmptyPiece.Width);
                        int pixelY = (int)qubeBoardDisplayOrigin.Y +
                            (y * EmptyPiece.Height);
                        spriteBatch.Draw(
                            Qube,
                            new Rectangle(pixelX, pixelY, EmptyPiece.Width, EmptyPiece.Height),
                            EmptyPiece, qubeBoard.GetQube(x, y));
                        if (y == QubeBoard.GameBoardHeight - 1 && qubeBoard.GetHelpList(x) == 1)
                        {
                            spriteBatch.Draw(Character,
                                new Rectangle(pixelX - 10,
                                Convert.ToInt32(pixelY + EmptyPiece.Height * 1.5),
                                EmptyPiece.Width + 20, EmptyPiece.Height + 20),
                                Color.White);
                        }
                    }
                }
                spriteBatch.Draw(Health, healthPosition, Color.White);
                spriteBatch.DrawString(
                    currentLevel, currentLv.ToString(), currentLevelPosition, Color.White);
                spriteBatch.DrawString(
                    currentLevel, playerScore.ToString(), scorePosition, Color.White);
                if (gameState == GameState.gameOver)
                {
                    spriteBatch.DrawString(clearFont, "GAME OVER", clearPosition, Color.CornflowerBlue);
                    spriteBatch.DrawString(currentLevel, "Press Space to Restart", new Vector2(110, 600), Color.CornflowerBlue);
                }
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        private int DetermineScore(int QubeCount)
        {
            return (int)((Math.Pow((QubeCount / 5), 2) + QubeCount) * 77);
        }

        private void CheckScoringChain(List<Vector2> QubeChain)
        {
            if (QubeChain.Count > BlockCondition)
            {
                playerScore += DetermineScore(QubeChain.Count);

                foreach (Vector2 ScoringQube in QubeChain)
                {
                    qubeBoard.SetQube((int)ScoringQube.X, (int)ScoringQube.Y, Color.White);
                }
            }
        }

        private void HandleMouseInput(MouseState mouseState)
        {
            int x = ((mouseState.X -
                qubeBoardDisplayOrigin.X - EmptyPiece.X) / EmptyPiece.Width);
            int y = ((mouseState.Y -
                qubeBoardDisplayOrigin.Y + 30) / (EmptyPiece.Height - 10));

            if ((x >= 0) && (x < QubeBoard.GameBoardWidth) &&
                (y >= 0) && (y < QubeBoard.GameBoardHeight))
            {
                if (qubeBoard.GetQube(x, y) != Color.White)
                {
                    qubeBoard.QubeTracker.Clear();
                    CheckScoringChain(qubeBoard.GetQubeChain(x, y));
                }
            }
            qubeBoard.CheckGameOver();
        }
    }
}