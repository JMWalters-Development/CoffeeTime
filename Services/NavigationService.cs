using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using CoffeeTime.Interfaces;
using ReactiveUI;

namespace CoffeeTime.Services;

public class NavigationService : INavigationService
{
    #region Private fields and properties

    private readonly BehaviorSubject<ReactiveObject?> _currentVmSubject = new(null);
    private readonly Stack<ReactiveObject> _stack = [];
    
    #endregion

    #region Public fields and properties
    
    public ReactiveObject? CurrentVm => _currentVmSubject.Value;
    public IObservable<ReactiveObject?> CurrentVmChanged => _currentVmSubject;
    
    #endregion
    
    #region Methods

    public void NavigateBack()
    {
        if (_stack.Count > 0)
        {
            _currentVmSubject.OnNext(_stack.Pop());
        }
    }

    public void NavigateTo(ReactiveObject vm)
    {
        if (CurrentVm != null)
        {
            _stack.Push(CurrentVm);
        }

        _currentVmSubject.OnNext(vm);
    }
    
    #endregion
}