using System;
using ReactiveUI;

namespace CoffeeTime.Interfaces;

public interface INavigation
{
    public ReactiveObject? CurrentVm { get; }
    public IObservable<ReactiveObject?> CurrentVmChanged { get; }
    public void NavigateBack();
    public void NavigateTo(ReactiveObject vm);
}