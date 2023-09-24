using FastFoodSimulatorLibrary.Customer_;
using FastFoodSimulatorLibrary.Employee_;
using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Kitchen_;

public class Kitchen : IKitchen
{
    public event EventHandler<Message> StringReturned;
    private int orderCount;
    private Queue<OrderTicket> kitchenQueue = new Queue<OrderTicket>();
    private object queueLock = new object();


    public Kitchen()
    {

    }

    public async Task<OrderTicket> PlaceOrderAsync(Customer customer)
    {
        lock (queueLock)
        {
            orderCount++;
            var orderTicket = new OrderTicket(orderCount);
            kitchenQueue.Enqueue(orderTicket);
            StringReturned?.Invoke(this, new Message("Kitchen", $"Customer {customer.CustomerNumber} placed an order (Order {orderTicket.OrderNumber})."));
            return orderTicket;
        }
    }

    public async Task<Chef> GetNextOrderAsync(Chef chef)
    {
        lock (queueLock)
        {
            if (kitchenQueue.Count > 0)
            {
                var orderTicket = kitchenQueue.Dequeue();
                chef.ticket = orderTicket;
                chef.IsWork = true;
                StringReturned?.Invoke(this, new Message("Kitchen", $"Cook started preparing Order {orderTicket.OrderNumber}."));
                return chef;
            }
            else
            {
                return null;
            }
        }
    }
}
