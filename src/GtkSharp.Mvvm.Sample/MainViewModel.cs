namespace GtkSharp.Mvvm
{
    public class MainViewModel : BaseViewModel
    {
        private string text = "asdf";
        private int counter = 0;
        private string entry = "inital";

        public string Text
        {
            get => text;
            set => Set(ref text, value);
        }

        public int Counter
        {
            get => counter;
            set
            {
                if (Set(ref counter, value))
                {
                    this.Text = $"Counter changed to {value}";
                }
            }
        }

        public string Entry
        {
            get => entry;
            set => Set(ref entry, value);
        }
    }
}
