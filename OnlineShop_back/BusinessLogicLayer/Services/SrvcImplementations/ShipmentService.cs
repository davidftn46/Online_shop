using BusinessLogicLayer.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.SrvcImplementations
{
    public class ShipmentService:IShipmentService
    {
        private readonly Random _rnd;
        private readonly Dictionary<long, Timer> _shipmentTimers = new Dictionary<long, Timer>();
        private readonly Dictionary<long, DateTime> _shipmentTimersStart = new Dictionary<long, DateTime>();
        private readonly Dictionary<long, TimeSpan> _shipmentDurations = new Dictionary<long, TimeSpan>();
        private readonly IServiceProvider _serviceProvider;
        public ShipmentService(IServiceProvider serviceProvider)
        {
            _rnd = new Random();
            _serviceProvider = serviceProvider;
        }

        public int ScheduleShipment(long orderId)
        {
            int arrivalMinutes = _rnd.Next(61, 240); 
            TimeSpan arrivalDuration = TimeSpan.FromMinutes(arrivalMinutes);

            DateTime dueTime = DateTime.Now + arrivalDuration;
            Timer shipmentTimer = new Timer(_ => MarkOrderAsShiped(orderId), null, arrivalMinutes * 60000, Timeout.Infinite);
            _shipmentTimers.Add(orderId, shipmentTimer);
            _shipmentTimersStart.Add(orderId, DateTime.Now);
            _shipmentDurations.Add(orderId, arrivalDuration);

            return arrivalMinutes;
        }

        public bool CancelShipment(long orderId)
        {
            if (_shipmentTimers.ContainsKey(orderId))
            {
                TimeSpan elapsedTime = DateTime.Now - _shipmentTimersStart[orderId];
                if (elapsedTime.TotalMinutes < 60)
                {
                  
                    _shipmentTimers[orderId].Dispose();
                    _shipmentTimers.Remove(orderId);
                    _shipmentTimersStart.Remove(orderId);
                    _shipmentDurations.Remove(orderId);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public TimeSpan GetRemainingTime(long orderId)
        {
            if (_shipmentTimersStart.ContainsKey(orderId) && _shipmentTimers.ContainsKey(orderId))
            {
                DateTime startTime = _shipmentTimersStart[orderId];
                TimeSpan elapsedTime = DateTime.Now - startTime;
                TimeSpan remainingTime = _shipmentDurations[orderId] - elapsedTime;

                if (remainingTime.TotalMilliseconds > 0)
                    return remainingTime;
                else
                    return TimeSpan.Zero; 
            }

            return TimeSpan.Zero; 
        }

        private void MarkOrderAsShiped(long orderId)
        {

            _shipmentTimers[orderId].Dispose();
            _shipmentTimers.Remove(orderId);
            _shipmentTimersStart.Remove(orderId);
            _shipmentDurations.Remove(orderId);
            using (var scope = _serviceProvider.CreateScope())
            {
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                orderService.MarkAsShiped(orderId);
            }
        }
    }
}
