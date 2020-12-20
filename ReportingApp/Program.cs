using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Northwind.ReportingServices.OData.ProductReports;

namespace ReportingApp
{
    /// <summary>
    /// Programm.
    /// </summary>
    public static class Program
    {
        private const string NorthwindServiceUrl = "https://services.odata.org/V3/Northwind/Northwind.svc";
        private const string CurrentProductsReport = "current-products";
        private const string MostExpensiveProductsReport = "most-expensive-products";
        private const string PriceLessThanProductsReport = "price-less-then-products";
        private const string PriceBetweenProductsReport = "price-between-products";
        private const string PriceAboveAverageProductsReport = "price-above-average-products";
        private const string UnitsInStockDeficitProductsReport = "units-in-stock-deficit";

        /// <summary>
        /// A program entry point.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                ShowHelp();
                return;
            }

            var reportName = args[0];

            if (string.Equals(reportName, CurrentProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowCurrentProducts();
                return;
            }
            else if (string.Equals(reportName, MostExpensiveProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && int.TryParse(args[1], out int count))
                {
                    await ShowMostExpensiveProducts(count);
                    return;
                }
            }
            else if (string.Equals(reportName, PriceLessThanProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && decimal.TryParse(args[1], out decimal count))
                {
                    await ShowPriceLessThanProducts(count);
                    return;
                }
            }
            else if (string.Equals(reportName, PriceBetweenProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && decimal.TryParse(args[1], out decimal moreThan) && decimal.TryParse(args[2], out decimal lessThan))
                {
                    await ShowPriceBetweenProducts(moreThan, lessThan);
                    return;
                }
            }
            else if (string.Equals(reportName, PriceAboveAverageProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowPriceAboveAverageProducts();
                return;
            }
            else if (string.Equals(reportName, UnitsInStockDeficitProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowUnitsInStockDeficitProducts();
                return;
            }
            else
            {
                ShowHelp();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\tReportingApp.exe <report> <report-argument1> <report-argument2> ...");
            Console.WriteLine();
            Console.WriteLine("Reports:");
            Console.WriteLine($"\t{CurrentProductsReport}\t\tShows current products.");
            Console.WriteLine($"\t{MostExpensiveProductsReport}\t\tShows specified number of the most expensive products.");
            Console.WriteLine($"\t{PriceLessThanProductsReport}\t\tShows specified number of the price less than products.");
            Console.WriteLine($"\t{PriceBetweenProductsReport}\t\tShows products of the price between products.");
            Console.WriteLine($"\t{PriceAboveAverageProductsReport}\t\tShows products of the price above average products.");
            Console.WriteLine($"\t{UnitsInStockDeficitProductsReport}\t\tShows products of units in stock deficit products products.");
        }

        private static async Task ShowCurrentProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetCurrentProducts();
            PrintProductReport("current products:", report);
        }

        private static async Task ShowPriceAboveAverageProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetAboveAverageProducts();
            PrintProductReport("current products:", report);
        }

        private static async Task ShowUnitsInStockDeficitProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetUnitsInStockDeficitProducts();
            PrintProductReport("current products:", report);
        }

        private static async Task ShowPriceBetweenProducts(decimal moreThan, decimal lessThan)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetPriceBetweenProducts(moreThan, lessThan);
            PrintProductReport("current products:", report);
        }

        private static async Task ShowPriceLessThanProducts(decimal price)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.ShowPriceLessThanProducts(price);
            PrintProductReport("current products:", report);
        }

        private static async Task ShowMostExpensiveProducts(int count)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetMostExpensiveProductsReport(count);
            PrintProductReport($"{count} most expensive products:", report);
        }

        private static void PrintProductReport(string header, ProductReport<ProductPrice> productReport)
        {
            Console.WriteLine($"Report - {header}");

            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine("{0}, {1}", reportLine.Name, reportLine.Price);
            }
        }
    }
}
