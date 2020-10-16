using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Product> Products { get; set; }
    }
}