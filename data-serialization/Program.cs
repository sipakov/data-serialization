using System;
using System.IO;
using System.Threading.Tasks;
using data_serialization.Implementations;
using data_serialization.Interfaceses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace data_serialization
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            using var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;

            var pathToTargetXml = Path.Combine(Environment.CurrentDirectory, "test.xml");
            var pathToOutputJson = Path.Combine(Environment.CurrentDirectory, "output.json");

            Console.Write("To start process press any key");
            Console.ReadKey();

            if (!File.Exists(pathToTargetXml))
            {
                Console.WriteLine("Target xml file not found");
            }

            if (File.Exists(pathToOutputJson))
            {
                File.Delete(pathToOutputJson);
            }

            var isSuccessConverted = await StartConverting(pathToTargetXml, pathToOutputJson, provider);
            if (isSuccessConverted)
                Console.WriteLine("Xml converted to json successfully");
            
            Console.ReadLine();
        }

        private static async Task<bool> StartConverting(string pathToTargetXml, string pathToOutputJson, IServiceProvider provider)
        {
            try
            {
                var isSuccess = await provider.GetRequiredService<IXmlToJsonConverter>().Convert(pathToTargetXml, pathToOutputJson);
                return isSuccess;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddTransient<IXmlToJsonConverter, XmlToJsonConverter>());
    }
}