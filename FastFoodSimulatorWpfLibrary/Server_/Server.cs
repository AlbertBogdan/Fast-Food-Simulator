using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Server_
{
    public class Server : IServer
    {
        public int servedCount;
        private Queue<OrderTicket> serviceQueue = new Queue<OrderTicket>();
        private object queueLock = new object();

        public string ServeOrder(OrderTicket orderTicket)
        {
            lock (queueLock)
            {
                servedCount++;
                serviceQueue.Enqueue(orderTicket);
                orderTicket.OrderReady.SetResult(true);
                return $"Order {orderTicket.OrderNumber} is ready to be served.";
                Console.WriteLine($"Order {orderTicket.OrderNumber} is ready to be served.");
            }
        }
    }
}
