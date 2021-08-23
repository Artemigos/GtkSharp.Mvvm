namespace GtkSharp.Mvvm
{
    public class MainViewModel : BaseViewModel
    {
        private string text = "asdf";
        private int counter = 0;

        public string Text
        {
            get => text;
            set => Set(ref text, value);
        }

        public int Counter
        {
            get => counter;
            set => Set(ref counter, value);
        }
    }
}
