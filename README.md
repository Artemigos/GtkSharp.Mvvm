# GtkSharp.Mvvm

[![nuget](https://img.shields.io/nuget/v/GtkSharp.Mvvm?style=flat-square)](https://www.nuget.org/packages/GtkSharp.Mvvm/)

Base tools for using the MVVM pattern with GtkSharp. The goal is to provide easy to use methods, that allow binding your typical view models to the GtkSharp views.

The project is in it's very early stage. I might reach the necessary feature set soon, but extensive testing should follow (hopefully in some smaller apps first).

Currently supported:

- `INotifyPropertyChanged`
- `glib`'s property change tracking
- `ICommand` binding with `CanExecute` tracking
- `INotifyDataErrorInfo`
- long property paths, e.g.

  ```csharp
  label.Bind(
      x => x.Text,
      vm,
      x => x.Property.Inner.GetErrors("ValidatedProperty");
  ```

Planned support:

- `IObservableCollection`
- command parameters
- **maybe** multi-bindings - this can be achieved with [Reactive Extensions] and `CombineLatest` and I'm not sure I want to reimplement that

## Installing

The library can be installed from NuGet:

```sh
dotnet add package GtkSharp.Mvvm
```

## Using

### Property binding

```csharp
label.Bind(
    x => x.Text,              // widget property path
    vm,                       // view model instance
    x => x.Path.To.Property); // view model property path
```

or

```csharp
label.Bind(
    vm,                       // view model instance
    x => x.Path.To.Property,  // view model property path
    val => label.Text = val); // action handling every new value
```

### Two way binding (return binding)

```csharp
entry.BindBack(
    x => x.Text,                // widget property path
    () => vm.Path.To.Property); // view model property path
```

or

```csharp
entry.BindBack(
    x => x.Text,                       // widget property path
    val => vm.Path.To.Property = val); // action handling every new value
```

### Command binding

```csharp
button.BindCommand(
    vm,                      // view model instance
    x => x.Path.To.Command); // view model command path
```

### Error info binding

```csharp
label.Bind(
    x => x.Text,                               // widget property path
    vm,                                        // view model instance
    x => x.Path.To.GetErrors("PropertyName")); // view model error info path
```

or

```csharp
label.Bind(
    vm,                                       // view model instance
    x => x.Path.To.GetErrors("PropertyName"), // view model error info path
    val => label.Text = val);                 // action handling every new value
```

### Lower level API

The library is based on `IObservable<>` and `IObserver<>`. When necessary, it's possible to use the extension methods that expose those observable objects.

- `vm.ObserveProperty(x => x.Path.To.Property)` - it returns an observable that can track a multi-level path with support for all the path features mentioned above. This is the core building block for the entire library.
- `propertyObservable.ObserveInnerProperty(x => x.Property)` - lets you drill down the property path, returns an observable, e.g.

  ```csharp
  vm.ObserveProperty(x => x.Path.To)
      .ObserveInnerProperty(x => x.Property);
  vm.ObserveProperty(x => x.Path.To.Command)
      .ObserveInnerProperty(x => x.CanExecute(null));
  ```

- `observable.Subscribe(val => {...})` - allows subscribing with callbacks, without the need to implement `IObserver<>` yourself.
- `disposable.AttachToWidgetLifetime(widget)` - subscriptions are disposable (and so are bindings) and should be disposed when no longer necessary. This takes care of that when the widget is destroyed. **All of the `Bind*` methods already do this for you.** They return the disposable in case you need to dispose the binding early.
- `observable.ResendLastOnSubscribe(initialValue)` - very similar to what `BehaviorSubject<>` from [Reactive Extensions] does and very useful for immediately forwarding property values to controls after binding.

An interesting side effect of this is the ability to use [Reactive Extensions] in conjuction with the bindings. Examples of that:

- multi-binding

  ```csharp
  vm.ObservePath(x => x.Path.To.Property)
      .CombineLatest(
          vm.ObservePath(x => x.Path.To.OtherProperty),
          (val1, val2) => $"{val1} {val2}")
      .ResendLastOnSubscribe(string.Empty)
      .Subscribe(val => label.Text = val)
      .AttachToWidgetLifetime(label);
  ```

- debounce input

  ```csharp
  entry.ObservePath(x => x.Text)
      .Throttle(TimeSpan.FromMilliseconds(200))
      .ResendLastOnSubscribe(entry.Text)
      .Subscribe(val => vm.Path.To.Property = val)
      .AttachToWidgetLifetime(entry);
  ```

## Contributing

First and foremost, use it and report any issues you find. The project needs some feedback from real world usage.

If you want to contribute code, please hold off at least until the code style is defined.

[Reactive Extensions]: https://github.com/dotnet/reactive
