using FastFoodSimulatorLibrary.Employee_;
using FastFoodSimulatorLibrary.Order;
using FastFoodSimulatorLibrary.Server_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Kitchen_
{
    public interface ICook
    {
        public Task PrepareOrderAsync(Chef orderTicket, Server server, int orderinterval);
    }
}
