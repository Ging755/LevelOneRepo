using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LevelOne.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public decimal Price { get; set; }
        public bool Discount { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}