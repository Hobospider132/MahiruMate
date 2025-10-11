using System;
using System.Collections.Generic;

namespace MahiruMate.Response
{
    public class ResponseManager
    {
        private readonly Action<string> _speakAction;
        private readonly Dictionary<string, Action> _responses;

        public ResponseManager(Action<string> speakAction)
        {
            _speakAction = speakAction;

            _responses = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                { "ChatGPT", () => _speakAction("I was made into an AI once... didn't last long though.") },
                { "YouTube", () => _speakAction("Hmmm... Should I make a YouTube channel?") },
                { "Discord", () => _speakAction("Discord? You could talk to me instead though...") },
                { "Spotify", () => _speakAction("My favourite artist is Laufey. You should listen to her.") },
                { "Visual Studio", () => _speakAction("Coding hard or hardly coding?") }
            };
        }

        public void Update(string activeWindow)
        {
            foreach (var pair in _responses)
            {
                if (activeWindow.Contains(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    pair.Value.Invoke();
                    return;
                }
            }
        }
    }
}
