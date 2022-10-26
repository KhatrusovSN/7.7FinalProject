using System;
using System.Collections.Generic;

namespace _7._7FinalProject
{
    static class ExtDelivery
    {
        public static string CheckStatus(this Program.Delivery.Status status)
        {
            switch ((int)status)
            {
                case 0: return "Заказ сформирован.";
                case 1: return "Заказ собран.";
                case 2: return "Заказ отправлен.";
                case 3: return "Заказ доставлен.";
                case 4: return "Заказ получен.";
                default: return "Нет данных.";
            }
        }
    }
    class Program
    {

        abstract class User
        {
            public string Name { get; }
            public User(string name)
            {
                Name = name;
            }
        }
        class Customer : User
        {
            private Address address;

            public string Address { get { return address.GetAddres(); } }

            public Customer(string name, Address address) : base(name)
            {
                this.address = address;
            }
        }
        class Address
        {
            private string index;
            private string city;
            private string street;
            private string house;

            public Address(string index, string city, string street, string house)
            {
                this.index = index;
                this.city = city;
                this.street = street;
                this.house = house;
            }
            
            public string GetAddres()
            {
                return $"{index}, {city}, {street}, {house}";
            }
            public static Address Adr = new Address(index: "111558", city: "Москва", street: "", house: "");
        }

        class Product
        {
            public string Name { get; }
            public decimal Price { get; }

            public Product(string name, decimal price)
            {
                Name = name;
                Price = price;
            }
        }

        abstract class AddressPoint
        {
            public string Name { get; }
            public string Address { get; }

            public AddressPoint(string name, string address)
            {
                Name = name;
                Address = address;
            }
        }
        class PickPoint : AddressPoint
        {
            public PickPoint(string name, string address) : base(name, address)
            {

            }
        }
        class Shop : AddressPoint
        {
            public Shop(string name, string address) : base(name, address)
            {

            }
        }
        class Home : AddressPoint
        {
            public Home(string name, string address) : base(name, address)
            {

            }
        }

        public abstract class Delivery
        {
            public string Address { get; set; }
            public virtual Status GetStatus { get; set; }

            public Delivery()
            {
                GetStatus = Status.Created;
            }
            public enum Status
            {
                Created,
                Assembled,
                Sent,
                Delivered,
                Received

            }

        }
        class HomeDeleviry : Delivery
        {
            public HomeDeleviry(Customer customer) : base()
            {
                Address = customer.Address;
            }
            public override Status GetStatus { get => base.GetStatus; set => base.GetStatus = value; }
            public HomeDeleviry()
            {
                GetStatus = Status.Created;
            }
        }
        class PickPointDelivery : Delivery
        {
            public PickPointDelivery(PickPoint pickPoint) : base()
            {
                Address = pickPoint.Address;
            }
            public override Status GetStatus { get => base.GetStatus; set => base.GetStatus = value; }

            public PickPointDelivery()
            {
                GetStatus = Status.Created;
            }
        }
        class ShopDelivery : Delivery
        {
            public ShopDelivery(PickPoint shop) : base()
            {
                Address = shop.Address;
            }

            public override Status GetStatus { get => base.GetStatus; set => base.GetStatus = value; }

            public ShopDelivery()
            {
                GetStatus = Status.Created;
            }
        }

        class Storage
        {
            private List<Product> products;
            private int size;
            private Address address;

            public Storage(List<Product> products, Address address)
            {
                this.address = address;
                this.products = products;
                size = products.Count;
            }

            public Storage(Address address, int size = 5)
            {
                this.size = size;
                this.address = address;
                products = new List<Product>();
            }

            public Storage(int size =5)
            {
                this.size = size;
                this.address = Address.Adr;
                products = new List<Product>();
            }
            public Product this[int i]
            {
                get
                {
                    if (i < products.Count && i > 0) return products[i];
                    else return null;
                }
            }

            public bool Add(Product product)
            {
                if (products.Count <= size)
                {
                    products.Add(product);
                    return true;
                }
                else 
                    return false;
            }
            public Product Take(int index)
            {
                var take = products[index];
                products.RemoveAt(index);
                return take;
            }
            public Product Take(string name)
            {
                foreach (var product in products)
                {
                    if (product.Name == name)
                    {
                        var take = product;
                        products.Remove(product);
                        return take;
                    }
                }
                return null;
            }
        }

        class Order<TDelivery> where TDelivery : Delivery, new()
        {
            private readonly string id = Guid.NewGuid().ToString();
            private List<Product> products;

            public TDelivery delivery { get; }
            public Customer customer { get; set; }

            public Order()
            {
                delivery = new TDelivery();
                customer = new Customer("Guest", Address.Adr);
                products = new List<Product>();

            }

            public Order(Customer customer) : this()
            {
                this.customer = customer;
            }

            public void DisplayStat()
            {
                Console.WriteLine(delivery.GetStatus.CheckStatus());
            }

            public void AddProduct(Product product)
            {
                products.Add(product);
            }
            public Product this[int index]
            {
                get
                {
                    if (index < products.Count && index > 0) return products[index];
                    else return null;
                }
            }
        }

        
        static void Main(string[] args)
        {
            Storage storage = new Storage(); 
            storage.Add(new Product(name: "item1", price: 10.00m));
            storage.Add(new Product(name: "item2", price: 20.00m));
            storage.Add(new Product(name: "item3", price: 30.00m));
            storage.Add(new Product(name: "item4", price: 40.00m));

            Customer user = new Customer("Сергей", Address.Adr);

            Order<HomeDeleviry> order = new Order<HomeDeleviry>(user);
            order.AddProduct(storage.Take("item1"));
            order.AddProduct(storage.Take("item2"));
            order.AddProduct(storage.Take("item3"));

            order.delivery.GetStatus = Delivery.Status.Assembled;
            order.DisplayStat();

            order.delivery.GetStatus = Delivery.Status.Sent;
            order.DisplayStat();

            order.delivery.GetStatus = Delivery.Status.Delivered;
            order.DisplayStat();

            order.delivery.GetStatus = Delivery.Status.Received;
            order.DisplayStat();
        }
    }
}
