using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Employee_;

public class Trader:Person
{
    public OrderTicket ticket { get; set; }

    public Trader(string Name) : base(Name)
    {
        this.Name = Name;
        isWork = false;
    }
}
