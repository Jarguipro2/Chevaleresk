using System.Data.Entity;

namespace EFA_DEMO.Models
{
    public class ItemView
    {
        public int IdObject { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public double Price { get; set; }
        public string PictureGUID { get; set; }
        public int? IdType { get; set; }
        public virtual Items_Type Items_Type { get; set; }

        static public string FoundType(int id)
        {
            string valeur = default;
            if (id == 0)
            {
                valeur = "arme";
            }
            else if (id == 1)
            {
                valeur = "armure";
            }
            else if (id == 2)
            {
                valeur = "potion";
            }
            else if (id == 3)
            {
                valeur = "ressource";
            }
            return valeur;
        }

        public int StringToId(string type)
        {
            int valeur = default;
            if (type.Equals("arme"))
            {
                valeur = 0;
            }
            else if (type.Equals("armure"))
            {
                valeur = 1;
            }
            else if (type.Equals("potion"))
            {
                valeur = 2;
            }
            else if (type.Equals("ressource"))
            {
                valeur = 3;
            }
            return valeur;
        }
        public Item ToItem()
        {
            return new Item()
            {
                IdObject = this.IdObject,
                Name = this.Name,
                StockQuantity = this.StockQuantity,
                Price = this.Price,
                PictureGUID = this.PictureGUID,
                IdType = this.IdType,
                Items_Type = this.Items_Type
            };
        }
    }
    public static class MyExtension
    {
        public static ItemView ToItemView(this Item item)
        {
            return new ItemView()
            {
                IdObject = item.IdObject,
                Name = item.Name,
                StockQuantity = item.StockQuantity,
                Price = item.Price,
                PictureGUID = item.PictureGUID,
                IdType = item.IdType,
                Items_Type = item.Items_Type                
            };
        }
    }
}