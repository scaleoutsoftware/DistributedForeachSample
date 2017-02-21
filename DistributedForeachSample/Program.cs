/* 
 * © Copyright 2017 by ScaleOut Software, Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Soss.Client;

namespace DistributedForeachSample
{
    /// <summary>
    /// Analyze shopping carts on a single host using TPL and on multiple hosts
    /// using ScaleOut's distributed ForEach LINQ operator.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var carts = new List<ShoppingCart>();
            var myCart = new ShoppingCart() { CustomerId = "cust17023" };
            myCart.Items.Add(new ShoppingCartItem()
            {
                Name = "Acme Widget",
                Price = 199.99m,
                Quantity = 2
            });

            myCart.Items.Add(new ShoppingCartItem()
            {
                Name = "Acme Snow Globe",
                Price = 2.99m,
                Quantity = 400
            });
            carts.Add(myCart);
            // (add more carts here)

            RunParallelAnalysis(carts);
            RunDistributedAnalysis(carts);
        }

        /// <summary>
        /// Result class used during analysis.
        /// </summary>
        [Serializable]
        private class Result
        {
            // exposing members as fields instead of properties because we
            // need to perform interlocked operations on them during analysis.
            public int numMatches;
            public int numCarts;
        }


        /// <summary>
        /// Run analysis on a single system using TPL's Parallel.ForEach method.
        /// </summary>
        /// <param name="collection">Collection of shopping carts.</param>
        static void RunParallelAnalysis(IEnumerable<ShoppingCart> collection)
        {
            var productName = "Acme Snow Globe";    // product to find in the carts
            var finalResult = new Result();         // result of the computation

            Parallel.ForEach<ShoppingCart, Result>(
                collection,                   // source collection
                () => new Result(),           // thread-local result initialization
                (cart, loopState, result) =>  // body of analysis logic
                {
                    // see if the selected product is in the cart:
                    if (cart.Items.Any(item => item.Name.Equals(productName)))
                        result.numMatches++;

                    result.numCarts++;
                    return result;
                },
                (threadLocalResult) =>        // merge logic
                {
                    Interlocked.Add(ref finalResult.numMatches, threadLocalResult.numMatches);
                    Interlocked.Add(ref finalResult.numCarts, threadLocalResult.numCarts);
                });

            float resultPct = (float)finalResult.numMatches / finalResult.numCarts * 100;
            Console.WriteLine($"{resultPct:N1} percent of carts contain {productName}");
        }
    

        /// <summary>
        /// Run shopping cart analysis on ScaleOut's distributed data grid.
        /// </summary>
        /// <param name="collection">Collection of shopping carts.</param>
        static void RunDistributedAnalysis(IEnumerable<ShoppingCart> collection)
        {
            // Add carts to the distributed data grid.
            var carts = CacheFactory.GetCache("carts");
            foreach (var cart in collection)
                carts.Add(cart.CustomerId, cart); // CustomerId serves as key

            string productName = "Acme Snow Globe"; // product to find in the carts
            Result finalResult;                     // result of the computation

            finalResult = carts.QueryObjects<ShoppingCart>().ForEach(
                productName,               // parameter
                () => new Result(),        // result-initialization delegate
                (cart, pName, result) =>   // body of analysis logic
                {
                    // see if the selected product is in the cart:
                    if (cart.Items.Any(item => item.Name.Equals(pName)))
                       result.numMatches++;
                       
                    result.numCarts++;
                    return result;
                },
                (result1, result2) =>    // merge logic
                {
                    result1.numMatches += result2.numMatches;
                    result1.numCarts += result2.numCarts;
                    return result1;
                });

            float resultPct = (float)finalResult.numMatches / finalResult.numCarts * 100;
            Console.WriteLine($"{resultPct:N1} percent of carts contain {productName}");
        }
    }

    

}
