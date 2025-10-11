using MahiruMate.FoodMenu;
using MahiruMate.Fun;
using MahiruMate.RandTalk;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MahiruMate.Menu
{
    public class MenuManager
    {
        private Random random = new Random();
        private readonly Action<string> _speakAction;
        private readonly Action _quitAction;
        private MenuItem? _pongMenuItem;

        public MenuManager(Action<string> speakAction, Action quitAction)
        {
            _speakAction = speakAction;
            _quitAction = quitAction;
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

        private void Pong_Click()
        {
            _speakAction("Use W and S, see if you can beat me! First to 3 wins!");
            if (_pongMenuItem != null)
                _pongMenuItem.IsEnabled = false;

            var pongWindow = new PongWindow(_speakAction, HandleGame);
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

        public ContextMenu CreateMenu()
        {
            var menu = new ContextMenu();

            var feedItem = new MenuItem { Header = "Feed" };
            var pongItem = new MenuItem { Header = "Play Pong" };
            _pongMenuItem = pongItem;
            var chatItem = new MenuItem { Header = "Chat" };
            var goodbyeItem = new MenuItem { Header = "Goodbye!" };
            var msgItem = new MenuItem { Header = "Where's Mahiru?" };

            var foodManager = new FoodManager(_speakAction, _quitAction);
            var foodItems = foodManager.CreateFoodItems();

            foreach (var food in foodItems)
                feedItem.Items.Add(food);

            menu.Items.Add(feedItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(pongItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(chatItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(msgItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(goodbyeItem);


            pongItem.Click += (s, e) => Pong_Click();
            chatItem.Click += (s, e) => randSay();
            goodbyeItem.Click += (s, e) => Goodbye();
            msgItem.Click += (s, e) => _speakAction("Hi, Sorry, Mahiru isn't here right now. I don't have the animations yet so please enjoy IndiBalls\n- Hobo");

            return menu;
        }
    }
}
