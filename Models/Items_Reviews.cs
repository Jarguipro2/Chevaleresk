
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

    public partial class Items_Reviews
    {

        public int IdReview { get; set; }

        public int IdObject { get; set; }

        public int IdPlayer { get; set; }

        public int Star { get; set; }

        public string Review { get; set; }

        public virtual Item Item { get; set; }

        public virtual User User { get; set; }

    }

}
