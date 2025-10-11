using MahiruMate.FoodMenu;
using MahiruMate.Fun;
using MahiruMate.RandTalk;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MahiruMate.Menu
{
    public class MenuManager
    {
        private Random random = new Random();
        private readonly Action<string> _speakAction;
        private readonly Action _quitAction;
        private MenuItem? _pongMenuItem;
        private readonly MainWindow _mainWindow;

        public MenuManager(Action<string> speakAction, Action quitAction, MainWindow window)
        {
            _speakAction = speakAction;
            _quitAction = quitAction;
            _mainWindow = window;
        }

        private void HandleGame(bool playerWon)
        {
            if (playerWon)
            {
                _speakAction("Guess you beat me... I won't go easy next time though!");
            }
            else
            {
                _speakAction("Heheh, looks like it's my win!");
            }
        }

        private void Pong_Click(MainWindow mainWindow)
        {
            _speakAction("Use W and S, see if you can beat me! First to 3 wins!");
            if (_pongMenuItem != null)
                _pongMenuItem.IsEnabled = false;

            var pongWindow = new PongWindow(_speakAction, HandleGame, mainWindow);
            pongWindow.Closed += (s, e) =>
            {
                if (_pongMenuItem != null)
                    _pongMenuItem.IsEnabled = true;
            };
            pongWindow.Show();
        }

        private void Goodbye()
        {
            _speakAction("Bye! See you next time!");
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                _quitAction();
            };
            timer.Start();
        }

        private void randSay()
        {
            var msg = ChatMsg.ChatMessages[random.Next(ChatMsg.ChatMessages.Length)];
            _speakAction(msg);
        }

        public ContextMenu CreateMenu(MainWindow mainWindow)
        {
            var menu = new ContextMenu();

            var feedItem = new MenuItem { Header = "Feed" };
            var giftItem = new MenuItem { Header = "Gift" };
            var drinkItem = new MenuItem { Header = "Give drink" };
            var statsItem = new MenuItem { Header = "Stats" };
            var pongItem = new MenuItem { Header = "Play Pong" };
            _pongMenuItem = pongItem;
            var chatItem = new MenuItem { Header = "Chat" };
            var goodbyeItem = new MenuItem { Header = "Goodbye!" };
            var msgItem = new MenuItem { Header = "Where's Mahiru?" };
            var lookAfterToggle = new MenuItem { Header = "Toggle mood drops" };

            var foodManager = new FoodManager(_speakAction, mainWindow);
            var foodItems = foodManager.CreateFoodItems();

            foreach (var food in foodItems)
                feedItem.Items.Add(food);

            var drinkManager = new FoodManager(_speakAction, mainWindow);
            var drinkItems = foodManager.CreateDrinkItems();

            foreach (var drink in drinkItems)
                drinkItem.Items.Add(drink);

            var giftManager = new FoodManager(_speakAction, mainWindow);
            var giftItems = foodManager.CreateGiftItems();

            foreach (var gift in giftItems)
                giftItem.Items.Add(gift);


            // Yes I know it's a food manager but I can't be asked to make a whole new file just for the stats. I'll just lump it in
            // ignore the file name
            var statsManager = new FoodManager(_speakAction, mainWindow);
            var statsItems = statsManager.CreateStatItems();
            foreach (var stats in statsItems)
                statsItem.Items.Add(stats);

            menu.Items.Add(feedItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(giftItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(drinkItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(statsItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(pongItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(lookAfterToggle);
            menu.Items.Add(new Separator());
            menu.Items.Add(chatItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(msgItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(goodbyeItem);


            pongItem.Click += (s, e) => Pong_Click(mainWindow);
            chatItem.Click += (s, e) => randSay();
            goodbyeItem.Click += (s, e) => Goodbye();
            msgItem.Click += (s, e) => _speakAction("Hi, Sorry, Mahiru isn't here right now. I don't have the animations yet so please enjoy IndiBalls\n- Hobo");
            lookAfterToggle.Click += (s, e) =>
            {
                if (_mainWindow.LookAfter)
                {
                    _speakAction("My mood will not drop");
                    _mainWindow.LookAfter = false;
                }
                else
                {
                    _speakAction("My mood will now drop");
                    _mainWindow.LookAfter = true;
                }
            };
            return menu;
        }
    }
}
