using FastFoodSimulatorLibrary.Employee_;
using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Server_;

public class Server : IServer
{
    public event EventHandler<Message> StringReturned;
    public int servedCount;
    private Queue<OrderTicket> serviceQueue = new Queue<OrderTicket>();
    private object queueLock = new object();

    public async Task ServeOrder(Chef orderTicket, int orderinterval)
    {
        await Task.Delay(orderinterval); // Simulate order preparation time
        lock (queueLock)
        {
            servedCount++;
            serviceQueue.Enqueue(orderTicket.ticket);
            orderTicket.ticket.OrderReady.SetResult(true);
        }
        StringReturned?.Invoke(this, new Message("Server" , $"Order {orderTicket.ticket.OrderNumber} is ready to be served."));

    }
    public string GetOrderList(int n)
    {
        var str = "";
        foreach (var item in serviceQueue)
        {
            str += $"{item.ToString()}\n";
        }
        return str;
    }
}
