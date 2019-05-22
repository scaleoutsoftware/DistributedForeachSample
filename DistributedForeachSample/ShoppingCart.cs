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
using System.Collections.Generic;
using System.Linq;
using Soss.Client;

namespace DistributedForeachSample
{
    /// <summary>
    /// Trivial shopping cart implementation.
    /// </summary>
    [Serializable]
    [SossIndex]
    public class ShoppingCart
    {
        public string CustomerId { get; set; }
        public IList<ShoppingCartItem> Items { get; } = new List<ShoppingCartItem>();

        [SossIndex(HashIndexPriority.HighPriorityHashable)]
        public decimal TotalValue
        {
            get { return Items.Sum((item) => item.Quantity * item.Price); }
        }
        public decimal ItemCount
        {
            get { return Items.Sum((item) => item.Quantity); }
        }
    }

    [Serializable]
    public class ShoppingCartItem
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public override string ToString()
        {
            return $"{Name}: ${Price:N2}, Count: {Quantity}";
        }
    }

}
