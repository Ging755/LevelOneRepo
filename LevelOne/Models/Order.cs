using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LevelOne.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string RecipientName { get; set; }
        public string RecipientSurname { get; set; }
        public string RecipientAdress { get; set; }
        public string RecipientZipCode { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public string OrderReciveTime { get; set; }
        public bool OrderSent { get; set; }
        public IEnumerable<Item> OrderedItems { get; set; }
    }
}