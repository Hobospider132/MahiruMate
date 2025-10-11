namespace MahiruMate.Speak
{
    public class Speaker
    {
        private readonly Action<string> _speakAction;

        public Speaker(Action<string> speakAction)
        {
            _speakAction = speakAction;
        }

        public void Speak(string message)
        {
            _speakAction(message);
        }
    }
}
