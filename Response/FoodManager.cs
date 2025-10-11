using MahiruMate.RandTalk;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MahiruMate.FoodMenu
{
    public class FoodManager
    {
        private readonly Action<string> _speakAction;
        private Random random = new Random();
        private MainWindow _mainWindow;
        private BitmapImage[] hungerImage, thirstImage, happinessImage;


        public FoodManager(Action<string> speakAction, MainWindow window)
        {
            _speakAction = speakAction;
            _mainWindow = window;
            StatImages();
        }

        public List<MenuItem> CreateFoodItems()
        {
            var items = new List<MenuItem>();

            items.Add(CreateFoodItem("Egg Tart", "Yum~! Egg Tart!"));
            items.Add(CreateFoodItem("Beef Noodles", "Beef noodles!"));
            items.Add(CreateFoodItem("Omurice", "Haa! Omurice~!"));

            return items;
        }

        private MenuItem CreateFoodItem(string header, string message)
        {
            var item = new MenuItem { Header = header };

            item.Click += async (s, e) =>
            {
                _speakAction("For me? Itadakimasu!");
                await Task.Delay(2000);
                _speakAction(message);
                await Task.Delay(2000);
                var msg = FoodMsg.FoodMessages[random.Next(FoodMsg.FoodMessages.Length)];
                _speakAction(msg);
                if (_mainWindow.Hunger < 5) _mainWindow.Hunger++;
            };
            return item;
        }

        public List<MenuItem> CreateDrinkItems()
        {
            var drinks = new List<MenuItem>();

            drinks.Add(CreateDrinkItem("Water", "That's better."));
            drinks.Add(CreateDrinkItem("Coco goat milk", "... Wait...\nWhere did you get this from?"));
            drinks.Add(CreateDrinkItem("Coffee", "Personally I like espressos more"));

            return drinks;
        }

        private MenuItem CreateDrinkItem(string header, string message)
        {
            var item = new MenuItem { Header = header };

            item.Click += async (s, e) =>
            {
                _speakAction("Thanks! I was getting thirsty");
                await Task.Delay(2000);
                _speakAction(message);
                await Task.Delay(2000);
                var msg = DrinkMsg.DrinkMessages[random.Next(DrinkMsg.DrinkMessages.Length)];
                _speakAction(msg);
                if (_mainWindow.Thirst < 5) _mainWindow.Thirst++;
            };
            return item;
        }

        public List<MenuItem> CreateGiftItems()
        {
            var gifts = new List<MenuItem>();

            gifts.Add(CreateGiftItem("Eggs", "Eggs? I'll make some omurice later."));
            gifts.Add(CreateGiftItem("Teddy bear", "Don't tell Amane-Kun but I'll name the bear after him..."));
            gifts.Add(CreateGiftItem("Chocolate", "Thanks! I'll have some later."));

            return gifts;
        }

        private MenuItem CreateGiftItem(string header, string message)
        {
            var item = new MenuItem { Header = header };

            item.Click += async (s, e) =>
            {
                _speakAction("Hm? A gift? For me?");
                await Task.Delay(2000);
                _speakAction(message);
                await Task.Delay(2000);
                var msg = GiftMsg.GiftMessages[random.Next(GiftMsg.GiftMessages.Length)];
                _speakAction(msg);
                if (_mainWindow.Happiness < 5) _mainWindow.Happiness++;
            };
            return item;
        }
        // Yes I know it's a food manager but I can't be asked to make a whole new file just for the stats. I'll just lump it in
        // ignore the file name


        public List<MenuItem> CreateStatItems()
        {
            var stats = new List<MenuItem>();

            stats.Add(CreateStatItem("Happiness"));
            stats.Add(CreateStatItem("Hunger"));
            stats.Add(CreateStatItem("Thirst"));

            return stats;
        }

        private void StatImages()
        {

            hungerImage = new BitmapImage[]
            {
                new BitmapImage(new Uri("Assets/hunger/hunger1.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/hunger/hunger2.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/hunger/hunger3.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/hunger/hunger4.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/hunger/hunger5.png", UriKind.Relative)),
            };

            thirstImage = new BitmapImage[]
            {
                new BitmapImage(new Uri("Assets/thirst/thirst1.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/thirst/thirst2.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/thirst/thirst3.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/thirst/thirst4.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/thirst/thirst5.png", UriKind.Relative)),
            };

            happinessImage = new BitmapImage[]
            {
                new BitmapImage(new Uri("Assets/happiness/happiness1.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/happiness/happiness2.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/happiness/happiness3.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/happiness/happiness4.png", UriKind.Relative)),
                new BitmapImage(new Uri("Assets/happiness/happiness5.png", UriKind.Relative)),
            };
        }

        private MenuItem CreateStatItem(string header)
        {
            var item = new MenuItem { Header = header };

            item.Click += (s, e) =>
            {
                int level = 0;
                ImageSource selectedImage = null;

                switch (header)
                {
                    case "Happiness":
                        level = _mainWindow.Happiness;
                        selectedImage = happinessImage[level - 1];
                        break;

                    case "Hunger":
                        level = _mainWindow.Hunger;
                        selectedImage = hungerImage[level - 1];
                        break;

                    case "Thirst":
                        level = _mainWindow.Thirst;
                        selectedImage = thirstImage[level - 1];
                        break;
                }

                if (selectedImage != null)
                {
                    _mainWindow.StatImage.Source = selectedImage;
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
                    timer.Tick += (sender, args) =>
                    {
                        _mainWindow.StatImage.Source = null; 
                        timer.Stop();
                    };
                    timer.Start();
                }
            };

            return item;
        }

    }
}
