using FastFoodSimulatorLibrary.Customer_;
using FastFoodSimulatorLibrary.Employee_;
using FastFoodSimulatorLibrary.Kitchen_;
using FastFoodSimulatorLibrary.Order;
using FastFoodSimulatorLibrary.Server_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Restaurant_;

public class Restaurant : IRestaurant
{
    private int customerInterval;
    private int orderInterval;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private List<Customer> customers;
    private Kitchen kitchen;
    private Cook cook;
    private Server server;
    public event EventHandler<Message> StringReturned;
    public event EventHandler<Chef> Work;
    private List<Person> Chefs;
    private List<Person> Traders;
    private Queue<OrderTicket> orderTickets = new Queue<OrderTicket>();
    private Queue<OrderTicket> order = new Queue<OrderTicket>();



    public Restaurant(int customerInterval, int orderInterval, int chefcount, int tradecount)
    {
        this.customerInterval = customerInterval;
        this.orderInterval = orderInterval;
        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = cancellationTokenSource.Token;
        this.customers = new List<Customer>();
        this.kitchen = new Kitchen();
        this.cook = new Cook();
        this.server = new Server();
        Chefs = GetWorkers<Chef>(chefcount);
        Traders = GetWorkers<Trader>(tradecount);
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
            if (freeWorker != null && !freeWorker.isWork)
            {
                return freeWorker;
            }
        }
    }
    public List<Person> GetWorkers<T>(int count) where T : Person
    {
        // Создаем пустой список
        List<Person> workers = new List<Person>();

        // Добавляем в список работников нужного типа
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

        // Возвращаем список работников
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

    // Previous code
    private async Task SimulateCustomersAsync(CancellationToken cancellationToken, int customerCount)
    {
        var customer = new Customer(customerCount);
        var orderTicket = await customer.PlaceOrderAsync(this.kitchen);
        orderTickets.Enqueue(orderTicket);
        //var freeChefs = Chefs.Where(chef => !chef.isWork).ToList();
        var freeChefs = GetFreeWorker(Chefs);
        var freeTraders = GetFreeWorker(Traders);

        var chef = await kitchen.GetNextOrderAsync((Chef)freeChefs);
        StringReturned?.Invoke(this, new Message("Chefs", GetListWorker(Chefs)));
        StringReturned?.Invoke(this, new Message("Traders", GetListWorker(Traders)));

        Work?.Invoke(this, chef);
        // Simulate customer waiting for order
        await orderTicket.OrderReady.Task;
        chef.isWork = false;
        //kitchen.Dequeue();
        StringReturned?.Invoke(this, new Message("Chefs", GetListWorker(Chefs)));
        var served = orderTickets.Dequeue();
        StringReturned?.Invoke(this, new Message("Order", $"{GetOrderList(1)}"));
        await Task.Delay(customerInterval, cancellationToken);
    }

    public string GetOrderList(int n)
    {
        var str = "";
        foreach (var item in orderTickets)
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

    public string GetListWorker(List<Person> workers)
    {
        string info = "";
        foreach (var worker in workers)
        {
            switch (worker)
            {
                case Trader trader:
                    info += $"{trader.Name} ";
                    if (trader.isWork)
                    {
                        info += $"№{trader.ticket.OrderNumber}\n";
                    }
                    else
                    {
                        info += "Free\n";
                    }
                    break;
                case Chef chef:
                    info += $"{chef.Name} ";
                    if (chef.isWork)
                    {
                        info += $"№{chef.ticket.OrderNumber}\n";
                    }
                    else
                    {
                        info += "Free\n";
                    }
                    break;
                default:
                    info += "Неверный тип работника.";
                    break;
            }
        }
        // Возвращаем строку текста
        return info;
    }
}
