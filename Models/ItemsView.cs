using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chevaleresk.Models
{
    public class ItemsView
    {
        public int IdObject { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public double Price { get; set; }
        public string PictureGUID { get; set; }
        public int IdType { get; set; }

        public int StringToId(string type)
        {
            int valeur = default;
            if(type.Equals("arme"))
            {
                valeur = 0;
            }
            else if(type.Equals("armure"))
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
    }
}