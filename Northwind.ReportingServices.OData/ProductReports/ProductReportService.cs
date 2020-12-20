using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using NorthwindProduct = NorthwindModel.Product;

namespace Northwind.ReportingServices.OData.ProductReports
{
    /// <summary>
    /// Represents a service that produces product-related reports.
    /// </summary>
    public class ProductReportService
    {
        private readonly NorthwindModel.NorthwindEntities entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="northwindServiceUri">An URL to Northwind OData service.</param>
        public ProductReportService(Uri northwindServiceUri)
        {
            this.entities = new NorthwindModel.NorthwindEntities(northwindServiceUri ?? throw new ArgumentNullException(nameof(northwindServiceUri)));
        }

        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetCurrentProducts()
        {
            List<NorthwindProduct> products = await this.GetAllProducts();

            var prices = from p in products
                         where !p.Discontinued
                         orderby p.ProductName
                         select new ProductPrice
                         {
                             Name = p.ProductName,
                             Price = p.UnitPrice ?? 0,
                         };

            return new ProductReport<ProductPrice>(prices);
        }

        /// <summary>
        /// Gets a product report with price less than products.
        /// </summary>
        /// <param name="price">.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> ShowPriceLessThanProducts(decimal price)
        {
            List<NorthwindProduct> products = await this.GetAllProducts();

            var prices = products.Where(p => p.UnitPrice < price)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(prices);
        }

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            List<NorthwindProduct> products = await this.GetAllProducts();

            var prices = products.
                Where(p => p.UnitPrice != null).
                OrderByDescending(p => p.UnitPrice.Value).
                Take(count).Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(prices);
        }

        /// <summary>
        /// Gets a product report between moreThan and lessthan products.
        /// </summary>
        /// <param name="moreThan">moreThan price.</param>
        /// <param name="lessThan">lessThan price.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceBetweenProducts(decimal moreThan, decimal lessThan)
        {
            List<NorthwindProduct> products = await this.GetAllProducts();

            var prices = products.
                Where(p => p.UnitPrice > moreThan && p.UnitPrice < lessThan)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(prices);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetAboveAverageProducts()
        {
            List<NorthwindProduct> products = await this.GetAllProducts();

            var prices = products.Where(p => p.UnitPrice > (products.Select(p => p.UnitPrice).Sum() / products.Count))
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(prices);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitProducts()
        {
            List<NorthwindProduct> products = await this.GetAllProducts();

            var prices = products.Where(p => p.UnitsInStock < p.UnitsOnOrder)
                .Select(p => new ProductPrice { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(prices);
        }

        private async Task<List<NorthwindProduct>> GetAllProducts()
        {
            DataServiceQueryContinuation<NorthwindProduct> token = null;

            var query = this.entities.Products;
            var products = new List<NorthwindProduct>();

            var result = await Task<IEnumerable<NorthwindProduct>>.Factory.FromAsync(query.BeginExecute(null, null), (ar) =>
            {
                return query.EndExecute(ar);
            }) as QueryOperationResponse<NorthwindProduct>;

            products.AddRange(result);

            token = result.GetContinuation();

            while (token != null)
            {
                result = await Task<IEnumerable<NorthwindProduct>>.Factory.FromAsync(this.entities.BeginExecute<NorthwindProduct>(token.NextLinkUri, null, null), (ar) =>
                {
                    return this.entities.EndExecute<NorthwindProduct>(ar);
                }) as QueryOperationResponse<NorthwindProduct>;

                products.AddRange(result);

                token = result.GetContinuation();
            }

            return products;
        }
    }
}
