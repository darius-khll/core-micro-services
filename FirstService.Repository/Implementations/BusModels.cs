using System;
using System.Collections.Generic;
using System.Text;

namespace FirstService.Repository.Implementations
{
    public interface SubmitOrder
    {
        string OrderId { get; }
    }

    public interface OrderAccepted
    {
        string OrderId { get; }
    }
}
