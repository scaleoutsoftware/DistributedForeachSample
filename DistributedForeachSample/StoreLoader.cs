using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soss.Client;

namespace DistributedForeachSample
{
    class StoreLoader
    {
        public static NamedCache Load(IEnumerable<ShoppingCart> carts)
        {
            // Add carts to the distributed data grid.
            var cartCache = CacheFactory.GetCache("carts");
            cartCache.EnableMethodInvocationEventHandling = true;

            foreach (var cart in carts)
                cartCache.Add(cart.CustomerId, cart); // CustomerId serves as key

            return cartCache;
        }
    }
}
