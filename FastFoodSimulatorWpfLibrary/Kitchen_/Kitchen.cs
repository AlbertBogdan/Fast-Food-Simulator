using FastFoodSimulatorLibrary.Customer_;
using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Kitchen_
{
    public class Kitchen : IKitchen
    {
        private int orderCount;
        private Queue<OrderTicket> kitchenQueue = new Queue<OrderTicket>();
        private object queueLock = new object();

        public async Task<OrderTicket> PlaceOrderAsync(Customer customer)
        {
            lock (queueLock)
            {
                orderCount++;
                var orderTicket = new OrderTicket(orderCount);
                kitchenQueue.Enqueue(orderTicket);
                Console.WriteLine($"Customer {customer.CustomerNumber} placed an order (Order {orderTicket.OrderNumber}).");
                return orderTicket;
            }
        }

        public async Task<OrderTicket> GetNextOrderAsync()
        {
            lock (queueLock)
            {
                if (kitchenQueue.Count > 0)
                {
                    var orderTicket = kitchenQueue.Dequeue();
                    Console.WriteLine($"Cook started preparing Order {orderTicket.OrderNumber}.");
                    return orderTicket;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
