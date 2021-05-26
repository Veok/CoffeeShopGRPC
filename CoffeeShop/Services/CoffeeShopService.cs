using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Services
{
    public class CoffeeShopService : CoffeeShop.CoffeeShopBase
    {
        private readonly ILogger<CoffeeShopService> _logger;

        public CoffeeShopService(ILogger<CoffeeShopService> logger)
        {
            _logger = logger;
        }

        public override async Task OrderCoffee(IAsyncStreamReader<OrderRequest> requestStream, IServerStreamWriter<CoffeeResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var current = requestStream.Current;
                var isCoffeeInMenu = CoffeeMenu().TryGetValue(current.Coffee, out var price);
                _logger.LogInformation("Received order with coffee: {Coffee}", current.Coffee);

                if (isCoffeeInMenu)
                {
                    var coffeeOrder = new CoffeeResponse
                    {
                        Coffee = current.Coffee,
                        Receipt = $"{current.Coffee}: {price}$"
                    };

                    await responseStream.WriteAsync(coffeeOrder);
                }
                else
                {
                    const string message = "Invalid Coffee: {Coffee}";
                    _logger.LogError(message, current.Coffee);
                    throw new ArgumentOutOfRangeException(nameof(current.Coffee), current.Coffee, message);
                }
            }
        }

        public override async Task GetMenu(Empty request, IServerStreamWriter<MenuRecord> responseStream, ServerCallContext context)
        {
            var index = 1;
            foreach (var (key, value) in CoffeeMenu())
            {
                var menuRecord = new MenuRecord {Coffee = key, Price = value, Position = index};
                index++;
                await responseStream.WriteAsync(menuRecord);
            }
        }

        private static Dictionary<Coffee, int> CoffeeMenu()
        {
            return new()
            {
                {Coffee.Americano, 11},
                {Coffee.Espresso, 8},
                {Coffee.Latte, 14},
                {Coffee.FlatWhite, 14}
            };
        }
    }
}