using FastFoodSimulatorLibrary.Employee_;
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
        public Task ServeOrder(Chef orderTicket, int orderinterval);
    }
}
