//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFA_DEMO.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Inventory
    {
        public int IdPlayer { get; set; }
        public int IdObject { get; set; }
        public int Quantity { get; set; }

        public IEnumerable<Item> Items { get; set; }

        public virtual Item Item { get; set; }
        public User User { get; set; }
    }
}
