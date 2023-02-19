using System;
using DeliveryOrderService.Data.Entities;
using Newtonsoft.Json;

namespace DeliveryOrderService.Data.Entities;

public class OrderDelivery: BaseEntity
{ 
    public string Customer { get; set; }
    public Product[] Products { get; set; }
    public Address Address { get; set; }
    public decimal TotalPrice { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    } 
}

public class Product
{
    public string Name { get; set; }
    public string Price { get; set; }
    public string Units { get; set; }
}

public class Address 
{
    public string Street { get; private set; }

    public string City { get; private set; }

    public string State { get; private set; }

    public string Country { get; private set; }

    public string ZipCode { get; private set; }

    private Address() { }

    public Address(string street, string city, string state, string country, string zipcode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipcode;
    }
}
