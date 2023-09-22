using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Server_
{
    public interface IServer
    {
        public string ServeOrder(OrderTicket orderTicket);
    }
}
