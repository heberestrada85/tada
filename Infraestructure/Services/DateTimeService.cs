using System;
using Tada.Application.Interface;

namespace Tada.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
