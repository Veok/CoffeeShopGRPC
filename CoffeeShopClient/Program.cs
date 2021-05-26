using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace CoffeeShopClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new CoffeeShop.CoffeeShopClient(channel);
            
            var orders = await GetOrdersFromMenu(client);
            await OrderCoffee(client, orders);

            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
            await channel.ShutdownAsync();
        }

        private static async Task OrderCoffee(CoffeeShop.CoffeeShopClient client, IEnumerable<OrderRequest> orders)
        {
            using var call = client.OrderCoffee();
            
            var orderResponseTask = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var order = call.ResponseStream.Current;
                    Console.WriteLine($"\nYour order: {order.Coffee}. \nReceipt: {order.Receipt}");
                }
            });

            foreach (var order in orders)
            {
                await call.RequestStream.WriteAsync(order);
            }

            await call.RequestStream.CompleteAsync();
            await orderResponseTask;
        }

        private static async Task<List<OrderRequest>> GetOrdersFromMenu(CoffeeShop.CoffeeShopClient client)
        {
            var orders = new List<OrderRequest>();
            Console.WriteLine("Menu:");

            using var call = client.GetMenu(new Empty());

            while (await call.ResponseStream.MoveNext())
            {
                var menuItem = call.ResponseStream.Current;
                Console.WriteLine($"{menuItem.Position}.{menuItem.Coffee}: {menuItem.Price}$");
                orders.Add(new OrderRequest {Coffee = menuItem.Coffee});
            }

            return orders;
        }
    }
}