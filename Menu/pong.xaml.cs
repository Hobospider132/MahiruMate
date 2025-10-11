using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MahiruMate.Fun
{
    public partial class PongWindow : Window
    {
        private double ballX = 280, ballY = 180;
        private double ballDX = 4, ballDY = 4;
        private double playerY = 150, aiY = 150;
        private double playerDY = 0;
        private const double PlayerPaddleSpeed = 10.0;
        private DispatcherTimer timer;
        private int playerScore = 0;
        private int aiScore = 0;
        private const int MaxScore = 3;

        private readonly Random rnd = new Random();
        private const double AITolerance = 7.0;
        private const double MaxAISpeed = 10.0;

        private readonly Action<bool> _gameEnded;

        public PongWindow(Action<string> speakAction, Action<bool> gameEnded)
        {
            InitializeComponent();
            _gameEnded = gameEnded;

            playerY = GameCanvas.Height / 2 - PlayerPaddle.Height / 2;
            aiY = GameCanvas.Height / 2 - AIPaddle.Height / 2;

            Canvas.SetLeft(PlayerPaddle, 10);
            Canvas.SetTop(PlayerPaddle, playerY);
            Canvas.SetLeft(AIPaddle, GameCanvas.Width - 35);
            Canvas.SetTop(AIPaddle, aiY);
            Canvas.SetLeft(Ball, ballX);
            Canvas.SetTop(Ball, ballY);

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += GameLoop;
            timer.Start();

            this.KeyDown += PongWindow_KeyDown;
            this.KeyUp += PongWindow_KeyUp;
        }

        public PongWindow() : this(_ => { }, _ => { }) { }

        private void PongWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                playerDY = -PlayerPaddleSpeed;
            }
            else if (e.Key == Key.S)
            {
                playerDY = PlayerPaddleSpeed;
            }
        }

        private void PongWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.W && playerDY < 0) || (e.Key == Key.S && playerDY > 0))
            {
                playerDY = 0;
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerY += playerDY;
            playerY = Math.Max(0, Math.Min(GameCanvas.Height - PlayerPaddle.Height, playerY));
            Canvas.SetTop(PlayerPaddle, playerY);

            ballX += ballDX;
            ballY += ballDY;

            if (ballY <= 0)
            {
                ballY = 0;
                ballDY *= -1;
            }

            if (ballY >= GameCanvas.Height - Ball.Height)
            {
                ballY = GameCanvas.Height - Ball.Height;
                ballDY *= -1;
            }

            // ball speed increase multiplier
            const double speedIncrease = 1.1;

            if (ballX <= Canvas.GetLeft(PlayerPaddle) + PlayerPaddle.Width &&
                ballY + Ball.Height >= Canvas.GetTop(PlayerPaddle) &&
                ballY <= Canvas.GetTop(PlayerPaddle) + PlayerPaddle.Height)
            {
                if (ballDX < 0)
                {
                    ballDX *= -1 * speedIncrease;
                    ballDY *= speedIncrease;
                }
            }

            if (ballX + Ball.Width >= Canvas.GetLeft(AIPaddle) &&
                ballY + Ball.Height >= Canvas.GetTop(AIPaddle) &&
                ballY <= Canvas.GetTop(AIPaddle) + AIPaddle.Height)
            {
                if (ballDX > 0)
                {
                    ballDX = Math.Abs(ballDX) * -1;
                    ballDX *= speedIncrease;
                    ballDY *= speedIncrease;
                }
            }

            // ai difficulty controls + placement
            double aiCenter = aiY + AIPaddle.Height / 2;
            double aiSpeed = Math.Min(Math.Abs(ballDY) * 0.6 + 2, MaxAISpeed);
            double error = rnd.NextDouble() * 10 - 5;

            double targetY = ballY + error;
            double distTarget = targetY - aiCenter;

            if (distTarget > 0)
            {
                aiY += Math.Min(aiSpeed, distTarget);
            } 
            else
            {
                aiY += Math.Max(-aiSpeed, distTarget);
            }

                aiY = Math.Max(0, Math.Min(GameCanvas.Height - AIPaddle.Height, aiY));
            Canvas.SetTop(AIPaddle, aiY);
            
            if (ballX < 0)
            {
                aiScore++;
                AIScore.Text = aiScore.ToString();
                CheckGameOver();
                return;
            }

            if (ballX > GameCanvas.Width - Ball.Width)
            {
                playerScore++;
                PlayerScore.Text = playerScore.ToString();
                CheckGameOver();
                return;
            }

            Canvas.SetLeft(Ball, ballX);
            Canvas.SetTop(Ball, ballY);
        }

        private void CheckGameOver()
        {
            if (playerScore >= MaxScore || aiScore >= MaxScore)
            {
                bool playerWon = playerScore >= MaxScore;
                timer.Stop();
                _gameEnded(playerWon);

                DispatcherTimer closeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                closeTimer.Tick += (s, e) =>
                {
                    closeTimer.Stop();
                    this.Close();
                };
                closeTimer.Start();
            }
            else
            {
                ResetBall();
            }
        }

        private void ResetBall()
        {
            ballX = GameCanvas.Width / 2 - Ball.Width / 2;
            ballY = GameCanvas.Height / 2 - Ball.Height / 2;

            ballDX = 4 * (rnd.NextDouble() > 0.5 ? 1 : -1);
            ballDY = 4 * (rnd.NextDouble() > 0.5 ? 1 : -1);

            playerY = GameCanvas.Height / 2 - PlayerPaddle.Height / 2;
            aiY = GameCanvas.Height / 2 - AIPaddle.Height / 2;

            Canvas.SetTop(PlayerPaddle, playerY);
            Canvas.SetTop(AIPaddle, aiY);
        }
    }
}