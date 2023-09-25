using FastFoodSimulatorLibrary.Customer_;
using FastFoodSimulatorLibrary.Employee_;
using FastFoodSimulatorLibrary.Events;
using FastFoodSimulatorLibrary.Kitchen_;
using FastFoodSimulatorLibrary.Order;
using FastFoodSimulatorLibrary.Server_;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Restaurant_;

public class Restaurant : IRestaurant
{
    private int customerInterval;
    private int orderInterval;
    private int orderIntervaltoKitchen;

    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private Kitchen kitchen;
    private Cook cook;
    private Server server;

    public event EventHandler<Message> StringReturned;
    public event EventHandler<Chef> Work;

    private List<Person> Chefs = new List<Person>();
    private List<Person> Traders = new List<Person>();
    private List<Customer> customers;

    private ObservableQueue<OrderTicket> orderTickets = new ObservableQueue<OrderTicket>("Order");
    private ObservableQueue<OrderTicket> takingOrders = new ObservableQueue<OrderTicket>("TakeOrder");



    public Restaurant(int customerInterval, int orderInterval,int orderIntervaltoKitchen, int chefcount, int tradecount)
    {
        this.customerInterval = customerInterval;
        this.orderInterval = orderInterval;
        this.orderIntervaltoKitchen = orderIntervaltoKitchen;

        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = cancellationTokenSource.Token;

        this.customers = new List<Customer>();
        this.kitchen = new Kitchen();
        this.cook = new Cook();
        this.server = new Server();

        Chefs = GetWorkers<Chef>(chefcount);
        Traders = GetWorkers<Trader>(tradecount);

        orderTickets.ItemEnqueued += Queue_ItemEnqueued;
        takingOrders.ItemEnqueued += Queue_ItemEnqueued;
        orderTickets.ItemDequeued += Queue_ItemDequeued;
        takingOrders.ItemDequeued += Queue_ItemDequeued;

        server.StringReturned += (sender, result) =>
        {
            StringReturned?.Invoke(this, result);
        };
        cook.StringReturned += (sender, result) =>
        {
            StringReturned?.Invoke(this, result);
        };
        kitchen.StringReturned += (sender, result) =>
        {
            StringReturned?.Invoke(this, result);
        };
    }



    public Person GetFreeWorker<T>(List<T> workers) where T : Person
    {
        Person freeWorker = null;
        while (true)
        {
            freeWorker = workers[new Random().Next(0, workers.Count)];
            if (freeWorker != null && !freeWorker.IsWork)
            {
                return freeWorker;
            }
        }
    }
    public List<Person> GetWorkers<T>(int count) where T : Person
    {
        List<Person> workers = new List<Person>();

        if (typeof(T) == typeof(Trader))
        {
            for (var i = 0; i < count; i++)
            {
                workers.Add(new Trader($"Trader N{i+1}"));
            }
        }
        else if (typeof(T) == typeof(Chef))
        {
            for (var i = 0; i < count; i++)
            {
                workers.Add(new Chef($"Chef N{i+1}"));
            }
        }

        foreach (var worker in  workers)
        {
            worker.PropertyChanged += Person_PropertyChanged;
        }

        return workers;
    }

    public void Start(CancellationToken cancellationToken)
    {
        Task.Run(() => SimulationStart(cancellationToken));
        Task.Run(() => ProcessOrdersAsync(cancellationToken));
    }

    public async Task StopAsync()
    {
        cancellationTokenSource.Cancel();

        // Wait for all tasks to complete
        while (customers.Count > server.servedCount)
        {
            await Task.Delay(100); // Wait for pending orders to be served
        }
    }

    private async Task SimulationStart(CancellationToken cancellation)
    {
        int customerCount = 1;
        while (!cancellationToken.IsCancellationRequested)
        {
            Task.Run( () => SimulateCustomersAsync(cancellationToken, customerCount));
            await Task.Delay(customerInterval, cancellationToken);
            customerCount++;
        }

    }

    private async Task SimulateCustomersAsync(CancellationToken cancellationToken, int customerCount)
    {
        var freeTrader = (Trader)GetFreeWorker(Traders);

        var customer = new Customer(customerCount);
        var orderTicket = await customer.PlaceOrderAsync(this.kitchen);

        freeTrader.ticket = orderTicket;
        freeTrader.IsWork = true;

        await Task.Delay(orderIntervaltoKitchen, cancellationToken);
        freeTrader.IsWork = false;

        orderTickets.Enqueue(orderTicket);
        var freeChefs = GetFreeWorker(Chefs);

        StringReturned?.Invoke(this, new Message("TraderDo", $"The {freeTrader.Name} passed the order N{freeTrader.ticket.OrderNumber} to the kitchen"));


        var chef = await kitchen.GetNextOrderAsync((Chef)freeChefs);

        Work?.Invoke(this, chef);
        await orderTicket.OrderReady.Task;
        chef.IsWork = false;


        var res = orderTickets.Dequeue();
        takingOrders.Enqueue(res);

        freeTrader = (Trader)GetFreeWorker(Traders);
        freeTrader.ticket = res;
        freeTrader.IsWork = true;


        await Task.Delay(customerInterval, cancellationToken);

        StringReturned?.Invoke(this, new Message("TraderDo", $"{freeTrader.Name} has transferred the order N{freeTrader.ticket.OrderNumber} to the Customer N{customer.CustomerNumber}."));
        freeTrader.IsWork = false;
        takingOrders.Dequeue();

    }

    public string GetOrderList<T>(Queue<T> orders)
    {
        var str = "";
        foreach (var item in orders)
        {
            str += $"{item.ToString()}\n";
        }
        return str;
    }

    private async Task ProcessOrdersAsync(CancellationToken cancellationToken)
    {
        Work += async (sender, result) =>
        {
            if (result != null)
            {
                await cook.PrepareOrderAsync(result, server, orderInterval);
            }
        };
    }


    private void Queue_ItemDequeued(object? sender, ItemDequeuedEventArgs<OrderTicket> e)
    {
        Check(sender);
    }
    private void Queue_ItemEnqueued(object sender, ItemEnqueuedEventArgs<OrderTicket> e)
    {
        Check(sender);
    }

    private void Check(object sender)
    {
        ObservableQueue<OrderTicket> queue = (ObservableQueue<OrderTicket>)sender;

        if (queue.QueueName == "Order")
        {
            StringReturned?.Invoke(this, new Message("Order", $"{GetOrderList(orderTickets)}"));
        }
        else
        {
            StringReturned?.Invoke(this, new Message("TakeOrder", $"{GetOrderList(takingOrders)}"));
        }
    }

    private void Person_PropertyChanged(object? sender, EventArgs e)
    {
        switch (sender)
        {
            case Trader trader:
                StringReturned?.Invoke(this, new Message("Traders", GetListWorker(Traders)));
                break;
            case Chef chef:
                StringReturned?.Invoke(this, new Message("Chefs", GetListWorker(Chefs)));
                break;
            default:
                break;
        }
    }

    public string GetListWorker(List<Person> workers)
    {
        string info = "";
        foreach (var worker in workers)
        {
            switch (worker)
            {
                case Trader trader:
                    info += $"{trader.Name} {(trader.IsWork ? $"№{trader.ticket.OrderNumber}\n" : "Free\n")}";
                    break;
                case Chef chef:
                    info += $"{chef.Name} {(chef.IsWork ? $"№{chef.ticket.OrderNumber}\n" : "Free\n")}";
                    break;
                default:
                    info += "Неверный тип работника.";
                    break;
            }
        }
        return info;
    }
}
