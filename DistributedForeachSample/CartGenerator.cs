/* 
 * © Copyright 2017-2019 by ScaleOut Software, Inc.
 *
 * LICENSE AND DISCLAIMER
 * ----------------------
 * This material contains sample programming source code ("Sample Code").
 * ScaleOut Software, Inc. (SSI) grants you a nonexclusive license to compile, 
 * link, run, display, reproduce, and prepare derivative works of 
 * this Sample Code.  The Sample Code has not been thoroughly
 * tested under all conditions.  SSI, therefore, does not guarantee
 * or imply its reliability, serviceability, or function. SSI
 * provides no support services for the Sample Code.
 *
 * All Sample Code contained herein is provided to you "AS IS" without
 * any warranties of any kind. THE IMPLIED WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGMENT ARE EXPRESSLY
 * DISCLAIMED.  SOME JURISDICTIONS DO NOT ALLOW THE EXCLUSION OF IMPLIED
 * WARRANTIES, SO THE ABOVE EXCLUSIONS MAY NOT APPLY TO YOU.  IN NO 
 * EVENT WILL SSI BE LIABLE TO ANY PARTY FOR ANY DIRECT, INDIRECT, 
 * SPECIAL OR OTHER CONSEQUENTIAL DAMAGES FOR ANY USE OF THE SAMPLE CODE
 * INCLUDING, WITHOUT LIMITATION, ANY LOST PROFITS, BUSINESS 
 * INTERRUPTION, LOSS OF PROGRAMS OR OTHER DATA ON YOUR INFORMATION
 * HANDLING SYSTEM OR OTHERWISE, EVEN IF WE ARE EXPRESSLY ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGES.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedForeachSample
{
    class CartGenerator : IEnumerable<ShoppingCart>
    {
        int _count;
        int _maxProductsPerCart;
        int _maxQuantityPerProduct;
        Product[] _catalog;

        public CartGenerator(int count, int maxProductsPerCart = 5, int maxQuantityPerProduct = 3)
        {
            _count = count;
            _maxProductsPerCart = maxProductsPerCart;
            _maxQuantityPerProduct = maxQuantityPerProduct;

            _catalog = new Product[] {
                new Product("Anvil", 324.99m),
                new Product("Bird Seed", 3.99m),
                new Product("Iron Bird Seed", 6.99m),
                new Product("Tornado Seeds", 149.99m),
                new Product("Female Road-Runner Costume", 86.49m),
                new Product("Cactus Costume", 49.99m),
                new Product("Quick Drying Cement", 43.00m),
                new Product("Dehydrated Boulders", 87.22m),
                new Product("Giant Rubber Band", 1.99m),
                new Product("Bumble Bees", 26.99m),
                new Product("Bed Springs", 3.99m),
                new Product("Spring-Powered Shoes", 74.99m),
                new Product("Roller Skis", 99.99m),
                new Product("Jet-Propelled Skis", 34.99m),
                new Product("Jet-Propelled Pogo-Stick", 34.99m),
                new Product("Jet-Propelled Unicycle", 24.99m),
                new Product("Instant Icicle Maker", 99.99m),
                new Product("Boomerang", 12.99m),
                new Product("Super Speed Vitamins", 24.99m),
                new Product("Giant Fly Paper", 3.99m),
                new Product("Giant Mouse Trap", 24.99m),
                new Product("Instant Road", 99.99m),
                new Product("Rocket Sled Kit", 149.99m),
                new Product("Acme Snow Globe", 2.99m) };
        }

        public IEnumerator<ShoppingCart> GetEnumerator()
        {
            // hard-coding seed for deterministic cart population.
            Random rand = new Random(123); 

            for (int i = 0; i < _count; i++)
            {
                ShoppingCart c = new ShoppingCart() { CustomerId = i.ToString() };
                int itemcount = rand.Next(1, _maxProductsPerCart + 1);
                var productIds = new List<int>(_maxProductsPerCart);

                for (int j = 0; j < itemcount; j++)
                {
                    int productIndex = rand.Next(_catalog.Length);
                    if (productIds.Contains(productIndex))
                        continue; // don't allow duplicates.

                    var prod = _catalog[productIndex];
                    c.Items.Add(new ShoppingCartItem()
                    {
                        Name = prod.Name,
                        Price = prod.Price,
                        Quantity = rand.Next(1, _maxQuantityPerProduct + 1)
                    });
                }
                yield return c;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Product
    {
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
        public string Name { get; }

        public decimal Price { get; }
    }

}
