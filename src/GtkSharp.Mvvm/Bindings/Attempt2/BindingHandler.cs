// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Linq;
// using System.Linq.Expressions;
// using System.Reflection;
// using System.Windows.Input;
// using Nito.Disposables;

// namespace GtkSharp.Mvvm.Bindings
// {
//     public static class BindingDefaults
//     {
//         public static IList<INotificationTypeHandler> NotificationTypeHandlers { get; } = new List<INotificationTypeHandler>();
//     }

//     public delegate void ValueChangeHandler(object source, object value);

//     public interface INotificationTypeHandler
//     {
//         bool CanHandle(object source, string propertyName);
//         IDisposable Subscribe(object source, string propertyName, ValueChangeHandler handler);
//     }

//     public class GlibNotificationTypeHandler : INotificationTypeHandler
//     {
//         public bool CanHandle(object source, string propertyName)
//         {
//             if (source is not GLib.Object glibSource)
//             {
//                 return false;
//             }

//             var prop = glibSource.GetType().GetProperty(propertyName);
//             if (!prop.CanRead)
//             {
//                 return false;
//             }

//             var propAttribute = prop.GetCustomAttribute<GLib.PropertyAttribute>(true);
//             return propAttribute != null;
//         }

//         public IDisposable Subscribe(object source, string propertyName, ValueChangeHandler handler)
//         {
//             var glibSource = (GLib.Object)source;
//             var prop = glibSource.GetType().GetProperty(propertyName);
//             var propAttribute = prop.GetCustomAttribute<GLib.PropertyAttribute>(true);

//             GLib.NotifyHandler notifyHandler = (sender, args) =>
//             {
//                 if (args.Property == propAttribute.Name)
//                 {
//                     var val = prop.GetValue(glibSource);
//                     handler(glibSource, val);
//                 }
//             };

//             glibSource.AddNotification(propAttribute.Name, notifyHandler);
//             return new Disposable(() => glibSource.RemoveNotification(propAttribute.Name, notifyHandler));
//         }
//     }

//     public class PropertyChangedNotificationTypeHandler : INotificationTypeHandler
//     {
//         public bool CanHandle(object source, string propertyName)
//         {
//             return source is INotifyPropertyChanged;
//         }

//         public IDisposable Subscribe(object source, string propertyName, ValueChangeHandler handler)
//         {
//             var notifySource = (INotifyPropertyChanged)source;
//             var prop = notifySource.GetType().GetProperty(propertyName);
//             PropertyChangedEventHandler propHandler = (sender, args) =>
//             {
//                 if (args.PropertyName == propertyName)
//                 {
//                     var val = prop.GetValue(notifySource);
//                     handler(notifySource, val);
//                 }
//             };

//             notifySource.PropertyChanged += propHandler;
//             return new Disposable(() => notifySource.PropertyChanged -= propHandler);
//         }
//     }

//     public class CommandCanExecuteNotificationTypeHandler : INotificationTypeHandler
//     {
//         public bool CanHandle(object source, string propertyName)
//         {
//             return source is ICommand && propertyName == nameof(ICommand.CanExecute);
//         }

//         public IDisposable Subscribe(object source, string propertyName, ValueChangeHandler handler)
//         {
//             var cmd = (ICommand)source;

//             EventHandler notifyHandler = (sender, args) =>
//             {
//                 var val = cmd.CanExecute(null);
//                 handler(cmd, val);
//             };

//             cmd.CanExecuteChanged += notifyHandler;
//             return new Disposable(() => cmd.CanExecuteChanged -= notifyHandler);
//         }
//     }

//     public class ErrorInfoChangedNotificationTypeHandler : INotificationTypeHandler
//     {
//         public bool CanHandle(object source, string propertyName)
//         {
//             return source is INotifyDataErrorInfo;
//         }

//         public IDisposable Subscribe(object source, string propertyName, ValueChangeHandler handler)
//         {
//             var errorSource = (INotifyDataErrorInfo)source;

//             EventHandler<DataErrorsChangedEventArgs> notifyHandler = (sender, args) =>
//             {
//                 if (args.PropertyName == propertyName)
//                 {
//                     var val = errorSource.GetErrors(propertyName);
//                     handler(errorSource, val);
//                 }
//             };

//             errorSource.ErrorsChanged += notifyHandler;
//             return new Disposable(() => errorSource.ErrorsChanged -= notifyHandler);
//         }
//     }

//     public class PropertyDescriptorNotificationTypeHandler // : INotificationTypeHandler
//     {
//     }

//     public interface ITracker<TValue> : IObservable<TValue>, IDisposable
//     {

//         public object CurrentValue { get; }
//         public void Reevaluate();
//     }

//     public class Tracker<TValue> : ITracker<TValue>
//     {
//         private readonly object root;
//         private readonly string[] propertyNameChain;
//         private readonly INotificationTypeHandler[] availableHandlers;
//         private readonly List<IObserver<TValue>> subscribers;

//         private readonly object[] subscribedObjects;
//         private readonly IDisposable[] subscriptions;

//         public object CurrentValue => this.GetValueAt(^1);

//         public Tracker(object source, string[] propertyNameChain, IEnumerable<INotificationTypeHandler> handlers)
//         {
//             this.root = source;
//             this.propertyNameChain = propertyNameChain;
//             this.availableHandlers = handlers.ToArray();
//             this.subscribers = new List<IObserver<TValue>>();

//             this.subscribedObjects = new object[propertyNameChain.Length];
//             this.subscriptions = new IDisposable[propertyNameChain.Length];

//             this.Resubscribe();
//         }

//         public void Dispose()
//         {
//             for (int i = 0; i < this.subscribedObjects.Length; ++i)
//             {
//                 this.subscribedObjects[i] = null;
//                 this.subscriptions[i]?.Dispose();
//                 this.subscriptions[i] = null;
//             }
//         }

//         public IDisposable Subscribe(IObserver<TValue> observer)
//         {
//             this.subscribers.Add(observer);
//             observer.OnNext((TValue)this.CurrentValue);
//             return new Disposable(() => this.subscribers.Remove(observer));
//         }

//         private void HandleChange(object source, object value)
//         {
//             if (source != this.subscribedObjects[^1])
//             {
//                 // middle property changed - resubscribe and send new value
//                 this.Resubscribe();
//                 value = this.CurrentValue;
//             }

//             this.PushValue(value);
//         }

//         private void Resubscribe()
//         {
//             var expected = this.root;
//             for (int i = 0; i < this.subscribedObjects.Length; ++i)
//             {
//                 var current = this.subscribedObjects[i];
//                 if (!ReferenceEquals(expected, current))
//                 {
//                     this.subscriptions[i]?.Dispose();
//                     this.subscriptions[i] = null;
//                     this.subscribedObjects[i] = null;

//                     var handler = this.GetHandlerFor(expected, this.propertyNameChain[i]);
//                     if (handler != null)
//                     {
//                         this.subscribedObjects[i] = expected;
//                         this.subscriptions[i] = handler.Subscribe(expected, this.propertyNameChain[i], this.HandleChange);
//                     }
//                     else
//                     {
//                         // TODO: log binding problems
//                     }
//                 }

//                 expected = this.GetValueAt(i);
//             }
//         }

//         private object GetValueAt(Index i)
//         {
//             // TODO: handle errors
//             var obj = this.subscribedObjects[i];
//             var prop = obj.GetType().GetProperty(this.propertyNameChain[i]);
//             return prop.GetValue(obj);
//         }

//         private INotificationTypeHandler GetHandlerFor(object source, string propertyName)
//         {
//             foreach (var handler in this.availableHandlers)
//             {
//                 if (handler.CanHandle(source, propertyName))
//                 {
//                     return handler;
//                 }
//             }

//             return null;
//         }

//         private void PushValue(object value)
//         {
//             foreach (var observer in this.subscribers)
//             {
//                 observer.OnNext((TValue)value);
//             }
//         }

//         public void Reevaluate()
//         {
//             this.Resubscribe();
//             this.PushValue(this.CurrentValue);
//         }
//     }

//     public class DelegatingObserver<TValue> : IObserver<TValue>
//     {
//         private readonly Action<TValue> onNext;
//         private readonly Action<Exception> onError;
//         private readonly Action onCompleted;

//         public DelegatingObserver(Action<TValue> onNext, Action<Exception> onError = null, Action onCompleted = null)
//         {
//             this.onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
//             this.onError = onError;
//             this.onCompleted = onCompleted;
//         }

//         public void OnCompleted()
//         {
//             this.onCompleted?.Invoke();
//         }

//         public void OnError(Exception error)
//         {
//             this.onError?.Invoke(error);
//         }

//         public void OnNext(TValue value)
//         {
//             this.onNext(value);
//         }
//     }

//     public static class BinExt
//     {
//         public static IDisposable Do<TValue>(this IObservable<TValue> observable, Action<TValue> handler)
//         {
//             var observer = new DelegatingObserver<TValue>(handler);
//             return observable.Subscribe(observer);
//         }

//         public static ITracker<TValue> Track<TValue>(this object source, params string[] propertyNameChain)
//         {
//             return Track<TValue>(source, BindingDefaults.NotificationTypeHandlers, propertyNameChain);
//         }

//         public static ITracker<TValue> Track<TValue>(this object source, IEnumerable<INotificationTypeHandler> notificationTypeHandlers, params string[] propertyNameChain)
//         {
//             return new Tracker<TValue>(source, propertyNameChain, notificationTypeHandlers);
//         }

//         public static IObservable<TValue> Track<TSource, TValue>(this TSource source, Expression<Func<TSource, TValue>> selector)
//         {
//             return Track(source, BindingDefaults.NotificationTypeHandlers, selector);
//         }

//         public static IObservable<TValue> Track<TSource, TValue>(this TSource source, IEnumerable<INotificationTypeHandler> notificationTypeHandlers, Expression<Func<TSource, TValue>> selector)
//         {
//             var names = new List<string>();

//             Expression current = selector.Body;
//             while (current is MemberExpression mem && mem.Member is PropertyInfo prop)
//             {
//                 names.Add(prop.Name);
//                 current = mem.Expression;
//             }

//             if (current is not ParameterExpression par)
//             {
//                 throw new ArgumentException("The only supported expressions are ones accessing properties on the input parameter.", nameof(selector));
//             }

//             names.Reverse();
//             return new Tracker<TValue>(source, names.ToArray(), notificationTypeHandlers);
//         }
//     }

//     // initial idea, maybe not necessary
//     public sealed class BindingHandler<TContext> : SingleDisposable<CollectionDisposable>
//     {
//         public BindingHandler(TContext context = default)
//             : base(new CollectionDisposable())
//         {
//             this.Context = context;
//         }

//         public TContext Context { get; set; }

//         protected override void Dispose(CollectionDisposable context)
//         {
//             context.Dispose();
//         }
//     }
// }

