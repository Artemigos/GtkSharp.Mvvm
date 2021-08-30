using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GtkSharp.Mvvm.Sample
{
    public class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public bool HasErrors => this.errors.Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return this.errors.TryGetValue(propertyName, out var e) ? e : Array.Empty<string>();
        }

        protected void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ClearErrors([CallerMemberName] string propertyName = null)
        {
            this.errors.Remove(propertyName);
            this.RaiseErrorsChanged(propertyName);
        }

        protected void AddError(string error, [CallerMemberName] string propertyName = null)
        {
            if (!this.errors.TryGetValue(propertyName, out var e))
            {
                this.errors[propertyName] = e = new List<string>();
            }

            e.Add(error);
            this.RaiseErrorsChanged(propertyName);
        }

        protected void SetErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = null)
        {
            this.errors[propertyName] = errors.ToList();
            this.RaiseErrorsChanged(propertyName);
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
