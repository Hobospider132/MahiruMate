using MahiruMate.Menu;
using MahiruMate.Response;
using MahiruMate.RandTalk;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MahiruMate
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private int frameIndex = 0;
        private BitmapImage[] walkFrames, idleFrames, dragFrames, currentFrames;

        private double x = 100;

        private int walkDir = 1;
        private double walkSpeed = 120.0;

        private string lastActiveWindow;
        private ResponseManager responseManager;

        private double vy = 0.0;
        private const double gravity = 1500.0;

        private enum MahiruState { Idle, Walking, Dragging }
        private MahiruState state = MahiruState.Idle;
        private double stateTimer = 0;
        private Random random = new Random();

        private readonly SpeechViewModel viewModel = new();
        private readonly DispatcherTimer messageTimer = new();
        private readonly DispatcherTimer moodTimer = new();

        private bool dragging = false;
        private bool dragMsgShown = false;

        private Point initialMouseScreen;
        private double initialLeft;
        private double initialTop;

        private const double FixedDeltaTime = 1.0 / 60.0;
        private DateTime lastUpdate = DateTime.Now;
        private double timeAccumulator = 0.0;
        private const double MaxDeltaTime = 0.25;
        private readonly Action _quitAction;

        private DispatcherTimer _currentHideTimer;

        public int Happiness { get; set; } = 5;
        public int Thirst { get; set; } = 5;
        public int Hunger { get; set; } = 5;
        public bool LookAfter { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;

            LoadFrames();

            responseManager = new ResponseManager(msg => Speak(msg));
            var menuManager = new MenuManager(msg => Speak(msg), () => Application.Current.Shutdown(), this);
            MahiruImage.ContextMenu = menuManager.CreateMenu(this);

            Loaded += MainWindow_Loaded;

            stateTimer = random.Next(3000, 60000);
            messageTimer.Interval = TimeSpan.FromSeconds(random.Next(10, 120));
            messageTimer.Tick += (s, e) => RandMSG();
            messageTimer.Start();

            moodTimer.Interval = TimeSpan.FromSeconds(random.Next(600, 1800));
            moodTimer.Tick += (s, e) => RandDrop();
            moodTimer.Start();

            MahiruImage.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            MahiruImage.MouseLeftButtonUp += MainWindow_MouseLeftButtonUp;
            MahiruImage.MouseMove += MainWindow_MouseMove;

            CompositionTarget.Rendering += OnRendering;
            lastUpdate = DateTime.Now;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double windowOffsetX = Math.Max(0, (this.Width - MahiruImage.Width) / 2);
            double windowOffsetY = Math.Max(0, (this.Height - MahiruImage.Height) / 2);
            Canvas.SetLeft(MahiruImage, windowOffsetX);
            Canvas.SetTop(MahiruImage, windowOffsetY);

            this.Left = x;
            this.Top = SystemParameters.WorkArea.Bottom - this.Height + windowOffsetY;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragging = true;
            initialMouseScreen = PointToScreen(e.GetPosition(this));
            initialLeft = this.Left;
            initialTop = this.Top;
            MahiruImage.CaptureMouse();
            vy = 0;

            e.Handled = true;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging) return;
            var currentMouseScreen = PointToScreen(e.GetPosition(this));
            double dxScreen = currentMouseScreen.X - initialMouseScreen.X;
            double dyScreen = currentMouseScreen.Y - initialMouseScreen.Y;

            this.Left = initialLeft + dxScreen;
            this.Top = initialTop + dyScreen;
        }

        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!dragging) return;
            dragging = false;
            MahiruImage.ReleaseMouseCapture();
            vy = 0;
        }

        private void RandMSG()
        {
            if (Hunger != 0 || Thirst != 0 || Happiness != 0)
            {
                var msg = rdmMsg.Messages[random.Next(rdmMsg.Messages.Length)];
                Speak(msg);
                messageTimer.Interval = TimeSpan.FromSeconds(random.Next(30, 120));
            }
        }

        private void RandDrop()
        {
            if (random.Next(2) == 0 && LookAfter)
            {
                Hunger--;
            }
            else
            {
                Thirst--;
            }
            mood();
            moodTimer.Interval = TimeSpan.FromSeconds(random.Next(600, 1800));
        }

        private string GetActive()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            if (GetWindowText(handle, buff, nChars) > 0)
                return buff.ToString();
            return string.Empty;
        }

        private void LoadFrames()
        {

            // Waiting on animations, these are just test images which will be switched out eventually
            walkFrames = new BitmapImage[]
            {
                new BitmapImage(new Uri("Assets/indiballs.png", UriKind.Relative))
            };

            idleFrames = new BitmapImage[]
            {
                new BitmapImage(new Uri("Assets/indiballs.png", UriKind.Relative))
            };

            dragFrames = new BitmapImage[]
            {
                new BitmapImage(new Uri("Assets/indiballs.png", UriKind.Relative))
            };

            currentFrames = idleFrames;
            if (currentFrames.Length > 0)
            {
                MahiruImage.Source = currentFrames[0];
            }

        }

        private void mood()
        {
            string message = null;
            if (Happiness == 0) message = "Hmph! I want to go home!";
            else if (Happiness == 1) message = "Hey! I'm getting pretty unhappy!\nIf you annoy me one more time I'll go home!";
            else if (Hunger == 1) message = "Hey... I'm getting pretty hungry.\nI might go home soon if I don't get something to eat";
            else if (Thirst == 1) message = "Hey... I'm getting pretty thirsty.\nI might go home soon if I don't get something to drink";
            else if (Hunger == 0) message = "Hey. I'm getting hungry. I'll come back and play later ok?";
            else if (Thirst == 0) message = "Hey. I'm getting thirsty. I'll come back and play later ok?";

            if (message == null) return;

            Speak(message);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                Application.Current.Shutdown();
            };
            timer.Start();
        }
        private void SetAnimation(MahiruState newstate)
        {
            if (state == newstate) return;

            state = newstate;
            frameIndex = 0;

            switch (state)
            {
                case MahiruState.Idle:
                    currentFrames = idleFrames;
                    break;
                case MahiruState.Walking:
                    currentFrames = walkFrames;
                    break;
                case MahiruState.Dragging:
                    currentFrames = dragFrames;
                    break;
            }

            if (currentFrames != null && currentFrames.Length > 0)
            {
                MahiruImage.Source = currentFrames[0];
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            double real_dt = (DateTime.Now - lastUpdate).TotalSeconds;
            lastUpdate = DateTime.Now;

            if (real_dt > MaxDeltaTime) real_dt = MaxDeltaTime;
            timeAccumulator += real_dt;

            while (timeAccumulator >= FixedDeltaTime)
            {
                UpdatePhysics(FixedDeltaTime);
                timeAccumulator -= FixedDeltaTime;
            }

            if (currentFrames != null && currentFrames.Length > 0)
            {
                frameIndex = (frameIndex + 1) % currentFrames.Length;
                MahiruImage.Source = currentFrames[frameIndex];
            }

            string active = GetActive();
            if (!string.Equals(active, lastActiveWindow, StringComparison.OrdinalIgnoreCase))
            {
                lastActiveWindow = active;
                responseManager.Update(active);
            }

            Dispatcher.BeginInvoke((Action)UpdateSpeechBubblePosition, DispatcherPriority.Render);
        }

        private void UpdatePhysics(double dt)
        {
            stateTimer -= dt * 1000;
            if (stateTimer <= 0)
            {
                if (state == MahiruState.Idle)
                {
                    state = MahiruState.Walking;
                    walkDir = random.Next(0, 2) == 0 ? -1 : 1;
                    stateTimer = random.Next(3000, 7000);
                }
                else
                {
                    state = MahiruState.Idle;
                    stateTimer = random.Next(2000, 60000);
                }
            }

            double windowOffsetY = Math.Max(0, (this.Height - MahiruImage.Height) / 2);
            double groundY = SystemParameters.WorkArea.Bottom - this.Height + windowOffsetY;

            if (dragging)
            {
                SetAnimation(MahiruState.Dragging);
                vy = 0;

                if (!dragMsgShown)
                {
                    var dragmsg = DragMsg.DragMessages[random.Next(DragMsg.DragMessages.Length)];
                    Speak(dragmsg);
                    if (LookAfter)
                    {
                        Happiness--;
                        mood();
                    }
                    dragMsgShown = true;
                }
            }
            else
            {
                vy += gravity * dt;
                double newTop = this.Top + vy * dt;

                if (newTop >= groundY)
                {
                    this.Top = groundY;
                    vy = 0;
                    dragMsgShown = false;
                }
                else
                {
                    this.Top = newTop;
                }

                if (state == MahiruState.Walking)
                    SetAnimation(MahiruState.Walking);
                else if (state == MahiruState.Idle)
                    SetAnimation(MahiruState.Idle);
            }


            if (!dragging && state == MahiruState.Walking && Math.Abs(this.Top - (SystemParameters.WorkArea.Bottom - this.Height + windowOffsetY)) < 0.5)
            {
                double newLeft = this.Left + walkDir * walkSpeed * dt;

                double leftLimit = 0;
                double rightLimit = SystemParameters.WorkArea.Width - this.Width;

                if (newLeft <= leftLimit)
                {
                    newLeft = leftLimit;
                    walkDir = 1;
                }
                else if (newLeft >= rightLimit)
                {
                    newLeft = rightLimit;
                    walkDir = -1;
                }

                this.Left = newLeft;
            }
        }

        private void UpdateSpeechBubblePosition()
        {
            double imageLeft = Canvas.GetLeft(MahiruImage);
            double imageTop = Canvas.GetTop(MahiruImage);

            double bubbleX = imageLeft + (MahiruImage.Width / 2) - (SpeechBubble.ActualWidth / 2);
            double bubbleY = imageTop - SpeechBubble.ActualHeight - 10;

            if (bubbleX < 0) bubbleX = 0;

            Canvas.SetLeft(SpeechBubble, bubbleX);
            Canvas.SetTop(SpeechBubble, bubbleY);
        }

        public void Speak(string message, int durationMs = 4000)
        {

            if (_currentHideTimer != null)
            {
                _currentHideTimer.Stop();
            }

            viewModel.Message = message;
            SpeechBubble.Visibility = Visibility.Visible;

            Dispatcher.BeginInvoke((Action)UpdateSpeechBubblePosition, DispatcherPriority.Render);

            var newHideTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(durationMs) };
            newHideTimer.Tick += (s, e) =>
            {
                SpeechBubble.Visibility = Visibility.Hidden;
                ((DispatcherTimer)s).Stop();

                if (s == _currentHideTimer)
                {
                    _currentHideTimer = null;
                }
            };

            _currentHideTimer = newHideTimer; 
            _currentHideTimer.Start();
        }
    }
}
