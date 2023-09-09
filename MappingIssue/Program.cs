using AutoMapper;
using System.Net;
using System.Runtime.Serialization;

namespace MappingIssue
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new MapperConfiguration(c =>
            {
                //c.CreateMap<Order, OrderToReturnDto>();
                c.CreateMap<Order, OrderToReturnDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.BuyerEmail, opt => opt.MapFrom(src => src.BuyerEmail))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.ShipToAddress, opt => opt.MapFrom(src => src.ShipToAddress))
                    .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod.ShortName))
                    .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                    .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                    .ForMember(dest => dest.ShippingPrice, opt => opt.MapFrom(src => src.DeliveryMethod.Price))
                    .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.GetTotal()))
                    .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderState))
                    .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

                //c.CreateMap<OrderItem, OrderItemDto>();
                c.CreateMap<OrderItem, OrderItemDto>()
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ItemOrdered.ProductItemId))
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ItemOrdered.ProductName))
                    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ItemOrdered.ImageUrl));
            });

            config.AssertConfigurationIsValid();

            var mapper = config.CreateMapper();

            var source = new Order
            {
                Id = 20,
                BuyerEmail = "bob@test.com",
                CreatedAt = DateTime.UtcNow,
                ShipToAddress = new Address("Bob", "Bobbity", "Long Street 123", "071620", "New York"),
                DeliveryMethod = new DeliveryMethod(),
                Subtotal = 150m,
                OrderState = OrderState.PaymentReceived,
                Invoice = "Invoice nr 1",
                PaymentIntentId = "Payment Intent Id: 123-456",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = 1,
                        Quantity = 1,
                        ItemOrdered = new ProductItemOrdered
                        {
                            ProductItemId = 30,
                            ProductName = "Name of Product",
                            ImageUrl = "Url to the Image"
                        },
                        Price = 150m
                    }
                }
            };
            source.Parent = source;
            try
            {
                var dest = mapper.Map<OrderItemDto>(source);//.Dump();
            }
            catch (Exception ex)
            {
                ex.ToString();//.Dump();
            }
        }

        public class Order
        {
            public Order()
            {
            }

            public Order(List<OrderItem> orderItems, string buyerEmail, Address shipToAddress,
                DeliveryMethod deliveryMethod, decimal subtotal, string invoice)
            {
                OrderItems = orderItems;
                BuyerEmail = buyerEmail;
                ShipToAddress = shipToAddress;
                DeliveryMethod = deliveryMethod;
                Subtotal = subtotal;
            }

            public int Id { get; set; }
            public string BuyerEmail { get; set; }
            public DateTime CreatedAt { get; set; }
            public Address ShipToAddress { get; set; }
            public DeliveryMethod DeliveryMethod { get; set; }
            public decimal Subtotal { get; set; }
            public OrderState OrderState { get; set; } = OrderState.Pending;
            public string Invoice { get; set; }
            public string PaymentIntentId { get; set; }
            public Order Parent { get; set; }

            public List<OrderItem> OrderItems { get; set; } = new();

            public decimal GetTotal()
            {
                return Subtotal + DeliveryMethod.Price;
            }
        }

        public class Address
        {
            public Address()
            {
            }

            public Address(string firstName, string lastName,
                string street, string postalCode, string city)
            {
                FirstName = firstName;
                LastName = lastName;
                Street = street;
                PostalCode = postalCode;
                City = city;
            }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Street { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }
        }

        public class DeliveryMethod
        {
            public int Id { get; set; }
            public string ShortName { get; set; }
            public string DeliveryTime { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
        }

        public enum OrderState
        {
            [EnumMember(Value = "Pending")]
            Pending = 1,

            [EnumMember(Value = "PaymentReceived")]
            PaymentReceived,

            [EnumMember(Value = "PaymentFailed")]
            PaymentFailed,

            [EnumMember(Value = "Shipped")]
            Shipped,

            [EnumMember(Value = "Completed")]
            Completed,

            [EnumMember(Value = "Cancelled")]
            Cancelled,

            [EnumMember(Value = "Expired")]
            Expired
        }

        public class OrderItem
        {
            public OrderItem()
            {
            }

            public OrderItem(ProductItemOrdered itemOrdered, decimal price, int quantity)
            {
                ItemOrdered = itemOrdered;
                Price = price;
                Quantity = quantity;
            }

            public int Id { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public ProductItemOrdered ItemOrdered { get; set; }

        }

        public class ProductItemOrdered
        {
            public ProductItemOrdered()
            {
            }

            public ProductItemOrdered(int productItemId, string productName, string imageUrl)
            {
                ProductItemId = productItemId;
                ProductName = productName;
                ImageUrl = imageUrl;
            }

            public int ProductItemId { get; set; }
            public string ProductName { get; set; }
            public string ImageUrl { get; set; }
        }
        
        public class OrderToReturnDto
        {
            public int Id { get; init; }
            public string BuyerEmail { get; init; }
            public DateTime CreatedAt { get; init; }
            public Address ShipToAddress { get; init; }
            public string DeliveryMethod { get; init; }
            public List<OrderItemDto> OrderItems { get; set; } = new();
            public decimal Subtotal { get; init; }
            public decimal ShippingPrice { get; init; }
            public decimal Total { get; init; }
            public string Invoice { get; init; }
            public string Status { get; init; }
            public OrderToReturnDto Parent { get; set; }
        }
        public class OrderItemDto
        {
            public int ProductId { get; init; }
            public decimal Price { get; init; }
            public string ProductName { get; init; }
            public string ImageUrl { get; init; }
            public int Quantity { get; init; }
        }
    }
}