using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using OperationInvoker.CustomServiceBehaviors;
using OperationInvoker.Services;
using Microsoft.ApplicationServer.Caching;

namespace OperationInvoker.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(OrganizitionService)))
            {
                host.Open();
                var server = new List<DataCacheServerEndpoint>();
                server.Add(new DataCacheServerEndpoint("localhost", 22233));
                // Set up the DataCacheFactory configuration
                var conf = new DataCacheFactoryConfiguration();
                conf.Servers = server;
                DataCacheFactory factory = new DataCacheFactory(conf);
                //foreach (var operation in host.Description.Endpoints[0].Contract.Operations)
                //{
                //    operation.Behaviors.Add(new FabricCacheOperationInvoker(factory));
                //}

                Console.WriteLine("Service is running");
                Console.Write("Press ENTER to close the host");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}
