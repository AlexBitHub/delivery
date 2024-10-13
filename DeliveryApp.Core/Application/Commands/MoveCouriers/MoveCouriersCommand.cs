using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.MoveCouriers
{
    public class MoveCouriersCommand : IRequest<bool> 
    {
    }
}
