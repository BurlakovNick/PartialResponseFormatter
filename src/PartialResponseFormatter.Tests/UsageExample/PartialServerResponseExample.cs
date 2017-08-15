using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace PartialResponseFormatter.Tests.UsageExample
{
    [TestFixture]
    public class PartialServerResponseExample
    {
        [Test]
        public void TestSerialization()
        {
            var client = new Client();
            var actual = client.Select(Guid.NewGuid());
            Console.WriteLine(JsonConvert.SerializeObject(actual, Formatting.Indented));
        }
        
        public class Client
        {
            private readonly Server server;

            public Client()
            {
                server = new Server();
            }
            
            public ClientProspectiveSaleModel Select(Guid saleId)
            {
                var responseSpecification = ResponseSpecification.Create<ClientProspectiveSaleModel>();
                var response = server.Select(saleId, responseSpecification);
                return JsonConvert.DeserializeObject<ClientProspectiveSaleModel>(response);
            }
        }

        public class Server
        {
            private readonly PartialResponseFormatter partialResponseFormatter;

            public Server()
            {
                partialResponseFormatter = new PartialResponseFormatter();
            }
            
            public string Select(Guid saleId, ResponseSpecification responseSpecification)
            {
                var response = Select(saleId);
                var partialResponse = partialResponseFormatter.Format(response, responseSpecification);
                return JsonConvert.SerializeObject(partialResponse);
            }

            private ServerProspectiveSaleModel Select(Guid saleId)
            {
                return new ServerProspectiveSaleModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Name",
                    CreateDate = DateTime.Now,
                    Contacts = new[]
                    {
                        new ServerProspectiveSaleContactModel
                        {
                            Id = Guid.NewGuid(),
                            Name = "Contact name 1",
                            Number = "Contact number 1",
                            BirthDate = DateTime.Now.AddYears(-20),
                            Position = "Boss"
                        },
                        new ServerProspectiveSaleContactModel
                        {
                            Id = Guid.NewGuid(),
                            Name = "Contact name 2",
                            Number = "Contact number 2",
                            BirthDate = DateTime.Now.AddYears(-40),
                            Position = "Accountant"
                        },
                    }
                };
            }
        }

        [MapFromContract(typeof(ServerProspectiveSaleModel))]
        public class ClientProspectiveSaleModel
        {
            public string Name { get; set; }
            public DateTime CreateDate { get; set; }
            public ClientProspectiveSaleContactModel[] Contacts { get; set; }
        }

        public class ClientProspectiveSaleContactModel
        {
            public string Name { get; set; }
            public string Number { get; set; }
            public string Position { get; set; }
        }

        public class ServerProspectiveSaleModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateDate { get; set; }
            public ServerProspectiveSaleContactModel[] Contacts { get; set; }
        }

        public class ServerProspectiveSaleContactModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Number { get; set; }
            public string Position { get; set; }
            public DateTime BirthDate { get; set; }
        }
    }
}