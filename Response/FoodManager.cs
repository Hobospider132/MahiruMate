using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MahiruMate.RandTalk;
using System.Threading.Tasks;

namespace MahiruMate.FoodMenu
{
    public class FoodManager
    {
        private readonly Action<string> _speakAction;
        private Random random = new Random();

        public FoodManager(Action<string> speakAction, Action quitAction)
        {
            _speakAction = speakAction;
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
            };
            return item;
        }
    }
}
