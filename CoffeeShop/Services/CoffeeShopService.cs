using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
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

        public override Task<CoffeeResponse> OrderCoffee(OrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received order with coffee: {Coffee}", request.Coffee);
            var isCoffeeInMenu = CoffeeMenu().TryGetValue(request.Coffee, out var price);

            if (isCoffeeInMenu)
            {
                return Task.FromResult(new CoffeeResponse
                {
                    Coffee = request.Coffee,
                    Receipt = $"{request.Coffee}: {price}$"
                });
            }

            const string message = "Invalid Coffee: {Coffee}";
            _logger.LogError(message, request.Coffee);
            throw new ArgumentOutOfRangeException(nameof(request.Coffee), request.Coffee, message);
        }

        public override Task<MenuResponse> GetMenu(Empty request, ServerCallContext context)
        {
            var menuRecords = CoffeeMenu()
                .Select(x => new MenuRecord
                {
                    Coffee = x.Key,
                    Price = x.Value
                }).ToList();

            return Task.FromResult(new MenuResponse
            {
                MenuRecords = {menuRecords}
            });
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