namespace GtkSharp.Mvvm.Sample
{
    public class MainViewModel : BaseViewModel
    {
        private string text = string.Empty;
        private int counter = 0;
        private string entry = "inital";

        public MainViewModel()
        {
            this.IncrementCounter = new RelayCommand(
                _ => this.Counter++,
                _ => this.Counter < 10
            );
            this.DecrementCounter = new RelayCommand(
                _ => this.Counter--,
                _ => this.Counter > 0
            );
        }

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
                    this.IncrementCounter.RaiseCanExecuteChanged();
                    this.DecrementCounter.RaiseCanExecuteChanged();
                }
            }
        }

        public string Entry
        {
            get => entry;
            set
            {
                if (Set(ref entry, value))
                {
                    this.ClearErrors();
                    if (value.Length % 2 == 1)
                        this.AddError("Length of text expected to be even.");
                }
            }
        }

        public RelayCommand IncrementCounter { get; }

        public RelayCommand DecrementCounter { get; }
    }
}
