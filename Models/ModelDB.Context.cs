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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBEntities2 : DbContext
    {
        public DBEntities2()
            : base("name=DBEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Items_Reviews> Items_Reviews { get; set; }
        public virtual DbSet<Items_Type> Items_Type { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<User_Inventory> User_Inventory { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
