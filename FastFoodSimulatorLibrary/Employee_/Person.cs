using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Employee_;

public class Person
{
    public event EventHandler PropertyChanged;
    private bool IsWork { get; set; }
    public string Name { get; set; }
    public bool isWork
    {
        get { return IsWork; }
        set
        {
            if (IsWork != value)
            {
                IsWork = value;
                OnPropertyChanged(nameof(isWork));
            }
        }
    }

    public Person(string Name)
    {
        this.Name = Name;
        isWork = false;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
