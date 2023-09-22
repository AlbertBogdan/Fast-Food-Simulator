using FastFoodSimulatorLibrary.Customer_;
using FastFoodSimulatorLibrary.Employee_;
using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Kitchen_
{
    public interface IKitchen
    {
        public Task<OrderTicket> PlaceOrderAsync(Customer customer);
        public Task<Chef> GetNextOrderAsync(Chef chef);
    }
}
