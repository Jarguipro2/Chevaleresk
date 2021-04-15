using System.Collections.Generic;

namespace EFA_DEMO.Models
{
    public class CartDictComparer :IEqualityComparer<Item>
    {
        public bool Equals(Item x, Item y) => x != null && y != null && x.IdObject == y.IdObject;

        public int GetHashCode(Item item) => item.IdObject;
    }
}