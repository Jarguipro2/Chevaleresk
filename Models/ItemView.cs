using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EFA_DEMO.Models
{
    public class ItemView
    {
        public int IdObject { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public double Price { get; set; }
        public string PictureGUID { get; set; }
        public int IdType { get; set; }
        public virtual Items_Type Items_Type { get; set; }

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
        public static ItemView AddItem(this DBEntities2 DB, ItemView itemView)
        {
            Item item = itemView.ToItem();
            item = DB.Items.Add(item);
            DB.SaveChanges();
            return item.ToItemView();
        }
        public static bool UpdateItem(this DBEntities2 DB, ItemView itemView)
        {
            Item itemToUpdate = itemView.ToItem();
            DB.Entry(itemToUpdate).State = EntityState.Modified;
            DB.SaveChanges();
            return true;
        }
        public static bool RemoveItem(this DBEntities2 DB, ItemView itemView)
        {
            Item itemToDelete = itemView.ToItem();
            DB.Items.Remove(itemToDelete);
            DB.SaveChanges();
            return true;
        }
    }
}