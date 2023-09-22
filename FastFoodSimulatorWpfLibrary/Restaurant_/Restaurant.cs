using FastFoodSimulatorLibrary.Customer_;
using FastFoodSimulatorLibrary.Kitchen_;
using FastFoodSimulatorLibrary.Server_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FastFoodSimulatorLibrary.Restaurant_
{
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
        public ListBox lstMsg;
        public Restaurant(int customerInterval, int orderInterval, ListBox lstMsg)
        {
            this.customerInterval = customerInterval;
            this.orderInterval = orderInterval;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = cancellationTokenSource.Token;
            this.customers = new List<Customer>();
            this.kitchen = new Kitchen();
            this.cook = new Cook();
            this.server = new Server();
            this.lstMsg = lstMsg;
        }

        public void Start(CancellationToken cancellationToken)
        {
            Task.Run(() => SimulateCustomersAsync(cancellationToken));
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

        // Previous code
        private async Task SimulateCustomersAsync(CancellationToken cancellationToken)
        {
            int customerCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(customerInterval, cancellationToken);

                customerCount++;
                var customer = new Customer(customerCount);

                // Remove the following line as "Arrive" method is not defined
                // customer.Arrive();

                var orderTicket = await customer.PlaceOrderAsync(this.kitchen);

                // Simulate customer waiting for order
                await orderTicket.OrderReady.Task;
            }
        }


        private async Task ProcessOrdersAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var orderTicket = await kitchen.GetNextOrderAsync();

                if (orderTicket != null)
                {
                    await cook.PrepareOrderAsync(orderTicket, server);
                }
            }
        }
    }

}
