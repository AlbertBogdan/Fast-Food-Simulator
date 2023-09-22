using FastFoodSimulatorLibrary.Order;
using FastFoodSimulatorLibrary.Server_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Kitchen_
{
    public class Cook : ICook
    {
        public async Task PrepareOrderAsync(OrderTicket orderTicket, Server server)
        {
            await Task.Delay(1000); // Simulate order preparation time
            Console.WriteLine($"Cook finished preparing Order {orderTicket.OrderNumber}.");
            server.ServeOrder(orderTicket);
        }
    }
}
