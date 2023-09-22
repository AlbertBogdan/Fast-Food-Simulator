using FastFoodSimulatorLibrary.Kitchen_;
using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Customer_
{
    public interface ICustomer
    {
        public Task<OrderTicket> PlaceOrderAsync(Kitchen kitchen);
    }
}
