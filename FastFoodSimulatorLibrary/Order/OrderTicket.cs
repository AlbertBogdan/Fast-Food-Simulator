using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Order;

public class OrderTicket
{
    public int OrderNumber { get; private set; }
    public TaskCompletionSource<bool> OrderReady { get; } = new TaskCompletionSource<bool>();

    public OrderTicket(int orderNumber)
    {
        OrderNumber = orderNumber;
    }

    public override string ToString()
    {
        return $"N{OrderNumber}\n";
    }
}
