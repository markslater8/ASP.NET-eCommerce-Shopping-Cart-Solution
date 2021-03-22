﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Smartstore.Core.Checkout.Orders;
using Smartstore.Core.Checkout.Orders.Reporting;
using Smartstore.Core.Data;

namespace Smartstore
{
    public static partial class OrderItemQueryExtensions
    {
        /// <summary>
        /// Applies a standard filter for order or customer identifier.
        /// </summary>
        /// <param name="query">Order item query.</param>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="customerId">Customer identifier.</param>
        /// <returns>Order item query.</returns>
        public static IQueryable<OrderItem> ApplyStandardFilter(this IQueryable<OrderItem> query, int? orderId = null, int? customerId = null)
        {
            Guard.NotNull(query, nameof(query));

            if (orderId.HasValue)
            {
                query = query.Where(x => x.OrderId == orderId);
            }

            if (customerId.HasValue)
            {
                var db = query.GetDbContext<SmartDbContext>();

                query =
                    from oi in query
                    join o in db.Orders.AsNoTracking() on oi.OrderId equals o.Id
                    where o.CustomerId == customerId.Value
                    select oi;
            }

            return query;
        }

        /// <summary>
        /// Filters order items by criteria met by assigned orders.
        /// </summary>
        /// <param name="storeId"><see cref="Order.StoreId"/></param>
        /// <param name="fromUtc">Earliest <see cref="Order.CreatedOnUtc"/></param>
        /// <param name="toUtc">Latest <see cref="Order.CreatedOnUtc"/></param>
        /// <param name="orderStatusIds">IN <see cref="Order.OrderStatusId"/></param>
        /// <param name="paymentStatusIds">IN IN <see cref="Order.PaymentStatusId"/></param>
        /// <param name="shippingStatusIds">IN <see cref="Order.ShippingStatusId"/></param>
        /// <param name="billingCountryId">Order.BillingAddress.CountryId</param>
        public static IQueryable<OrderItem> ApplyOrderFilter(this IQueryable<OrderItem> query,
            int storeId = 0,
            DateTime? fromUtc = null,
            DateTime? toUtc = null,
            int[] orderStatusIds = null,
            int[] paymentStatusIds = null,
            int[] shippingStatusIds = null,
            int? billingCountryId = null)
        {
            Guard.NotNull(query, nameof(query));

            var db = query.GetDbContext<SmartDbContext>();

            query =
                from oi in query
                join o in db.Orders on oi.OrderId equals o.Id
                where (storeId == 0 || o.StoreId == storeId) &&
                    (!fromUtc.HasValue || o.CreatedOnUtc >= fromUtc.Value) &&
                    (!toUtc.HasValue || o.CreatedOnUtc <= toUtc.Value) &&
                    (orderStatusIds == null || orderStatusIds.Contains(o.OrderStatusId)) &&
                    (paymentStatusIds == null || paymentStatusIds.Contains(o.PaymentStatusId)) &&
                    (shippingStatusIds == null || shippingStatusIds.Contains(o.ShippingStatusId)) &&
                    (!billingCountryId.HasValue || o.BillingAddress.CountryId == billingCountryId)
                select oi;

            return query;
        }

        /// <summary>
        /// Filters order items by criteria met by assigned line items.
        /// </summary>
        /// <param name="includeHidden">Include unpublished products also</param>
        public static IQueryable<OrderItem> ApplyProductFilter(this IQueryable<OrderItem> query,
            int[] productIds = null,
            bool includeHidden = false)
        {
            // TODO: (ms) Add more params to OrderItemQueryExtensions.ApplyProductFilter()
            Guard.NotNull(query, nameof(query));

            var db = query.GetDbContext<SmartDbContext>();

            query =
                from oi in query
                join p in db.Products on oi.ProductId equals p.Id
                where
                    !p.IsSystemProduct
                    && (includeHidden || p.Published)
                    && (!productIds.IsNullOrEmpty() && productIds.Contains(p.Id))
                select oi;

            return query;
        }

        public static IQueryable<BestsellersReportLine> SelectAsBestSellersReportLine(this IQueryable<OrderItem> query, ReportSorting sorting = ReportSorting.ByQuantityDesc)
        {
            Guard.NotNull(query, nameof(query));

            // Group by product ID.
            var selector = query
                .GroupBy(x => x.ProductId)
                .Select(x => new BestsellersReportLine
                {
                    ProductId = x.Key,
                    TotalAmount = x.Sum(x => x.PriceExclTax),
                    TotalQuantity = x.Sum(x => x.Quantity)
                });

            selector = sorting switch
            {
                ReportSorting.ByAmountAsc => selector.OrderBy(x => x.TotalAmount),
                ReportSorting.ByAmountDesc => selector.OrderByDescending(x => x.TotalAmount),
                ReportSorting.ByQuantityAsc => selector.OrderBy(x => x.TotalQuantity).ThenByDescending(x => x.TotalAmount),
                _ => selector.OrderByDescending(x => x.TotalQuantity).ThenByDescending(x => x.TotalAmount),
            };

            return selector;
        }
    }
}
