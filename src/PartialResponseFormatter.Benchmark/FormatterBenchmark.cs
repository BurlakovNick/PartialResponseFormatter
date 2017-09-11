using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using PartialResponseFormatter.Benchmark.ClientContracts;
using PartialResponseFormatter.Benchmark.ServerContracts;

namespace PartialResponseFormatter.Benchmark
{
    public class FormatterBenchmark
    {
        private readonly PartialResponseFormatter partialResponseFormatter;
        private readonly ServerUserAccount serverModel;
        private readonly Random random;
        private ResponseSpecification responseSpecification;
        private const string Alphabet = "1234567890qwertyuiopasdfghjklzxcvbnm";

        public FormatterBenchmark()
        {
            responseSpecification = ResponseSpecification.Create<ClientUserAccount>();

            random = new Random();
            partialResponseFormatter = new PartialResponseFormatter();
            serverModel = new ServerUserAccount
            {
                UserId = Guid.NewGuid(),
                UserName = GenString(20),
                LastVisitTime = DateTime.Today,
                UserEmail = GenString(30),
                ContactPhone = Enumerable.Range(0, 5).Select(i => GenPhone()).ToArray(),
                CreditCards = Enumerable.Range(0, 3).Select(i => GenCreditCard()).ToArray(),
                PreviousPurchases = Enumerable.Range(0, 10).Select(i => GenPreviousPurchase()).ToArray(),
                UserBag = new ServerUserBag
                {
                    ValidUntil = DateTime.Now,
                    PromoCode = GenString(6),
                    Items = Enumerable.Range(0, 10).Select(i => Guid.NewGuid()).ToDictionary(i => i, i => GenBagItem())
                }
            };
        }

        private string GenString(int length)
        {
            return new string(Enumerable.Range(0, length).Select(i => Alphabet[random.Next(0, Alphabet.Length)])
                .ToArray());
        }

        private ServerContactPhone GenPhone()
        {
            return new ServerContactPhone {Number = GenString(10)};
        }

        private ServerCreditCard GenCreditCard()
        {
            return new ServerCreditCard
            {
                ValidUntil = DateTime.UtcNow,
                Number = GenString(16),
                CardHolder = GenString(20)
            };
        }

        private ServerPreviousPurchase GenPreviousPurchase()
        {
            return new ServerPreviousPurchase
            {
                IsDelivered = false,
                IsPaid = true,
                PurchaseDate = DateTime.Now,
                Items = Enumerable.Range(0, 10).Select(i => GenPreviousPurchaseItem()).ToArray()
            };
        }

        private ServerPreviousPurchaseItem GenPreviousPurchaseItem()
        {
            return new ServerPreviousPurchaseItem
            {
                Discount = random.Next(0, 100),
                Price = random.Next(0, 100000),
                VatRate = random.Next(0, 20),
            };
        }

        private ServerUserBagItem GenBagItem()
        {
            return new ServerUserBagItem
            {
                Discount = random.Next(0, 100),
                Price = random.Next(0, 100000),
                VatRate = random.Next(0, 20),
                GoodId = Guid.NewGuid(),
                ItemName = GenString(20)
            };
        }

        [Benchmark]
        public object FormatWithPartialResponseFormatter()
        {
            return partialResponseFormatter.Format(serverModel, responseSpecification);
        }

        [Benchmark]
        public object FormatWithNaiveFormatter()
        {
            return new ClientUserAccount
            {
                UserName = serverModel.UserName,
                LastVisitTime = serverModel.LastVisitTime,
                CreditCards = serverModel
                    .CreditCards
                    .Select(creditCard => new ClientCreditCard
                    {
                        CardHolder = creditCard.CardHolder,
                        ValidUntil = creditCard.ValidUntil
                    })
                    .ToArray(),
                PreviousPurchases = serverModel
                    .PreviousPurchases
                    .Select(prevPurchase => new ClientPreviousPurchase
                    {
                        PurchaseDate = prevPurchase.PurchaseDate,
                        Items = prevPurchase
                            .Items
                            .Select(item => new ClientPreviousPurchaseItem
                            {
                                Price = item.Price,
                                VatRate = item.VatRate
                            })
                            .ToArray()
                    })
                    .ToArray(),
                UserBag = new ClientUserBag
                {
                    ValidUntil = serverModel.UserBag.ValidUntil,
                    Items = serverModel.UserBag
                        .Items
                        .ToDictionary(item => item.Key, item => new ClientUserBagItem
                        {
                            ItemName = item.Value.ItemName,
                            Discount = item.Value.Discount,
                            Price = item.Value.Price,
                            VatRate = item.Value.VatRate,
                        })
                }
            };
        }
    }
}