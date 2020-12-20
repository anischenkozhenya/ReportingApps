using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.ReportingServices.OData.ProductReports
{
    /// <summary>
    /// Represents a product report line.
    /// </summary>
    public class ProductPrice
    {
        /// <summary>
        /// Gets or sets a product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a product price.
        /// </summary>
        public decimal Price { get; set; }
    }
}
