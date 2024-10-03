using System.Text.Json.Serialization;

namespace FinalProjectWPF.Projects.MyLittleBusiness.Models
{
    class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        [JsonIgnore]
        public int Quantity { get; set; }

        // modify
        public Item()
        {

        }
        public Item(int itemId, string name, decimal price, int quantity = 0)
        {
            ItemId = itemId;
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }
}
