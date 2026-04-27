using Simulation.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Service
{
    public interface IApiService
    {
        Task<Case> GetActiveCase();
        Task SendAction(ActionLog action);
    }
}
