using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Employee_;

public class Person
{
    public bool isWork { get; set; }
    public string Name { get; set; }

    public Person(string Name)
    {
        this.Name = Name;
        isWork = false;
    }
}
