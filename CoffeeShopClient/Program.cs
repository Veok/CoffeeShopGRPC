using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace CoffeeShopClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new CoffeeShop.CoffeeShopClient(channel);

            var menu = await client.GetMenuAsync(new Empty());
            
            Console.WriteLine("Menu:");
            Console.WriteLine(string.Join('\n', menu.MenuRecords.Select((c,i) => $"{i + 1}.{c.Coffee}: {c.Price}$")));
            Console.WriteLine("What you like to order. Type a number: ");
            
            var key = Console.ReadKey();
            
            var orderRequest = new OrderRequest
            {
                Coffee = (Coffee) int.Parse(key.KeyChar.ToString())
            };
          
            var order = await client.OrderCoffeeAsync(orderRequest);
            
            Console.WriteLine($"\nYour order: {order.Coffee}. \nReceipt: {order.Receipt}");
            Console.ReadKey();
        }
    }
}