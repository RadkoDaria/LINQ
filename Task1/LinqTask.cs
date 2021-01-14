using System;
using System.Collections.Generic;
using Task1.DoNotChange;
using System.Linq;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            var selectedCustomers = customers.Where(c => c.Orders.Select(o => o.Total).Sum() > limit);
            return selectedCustomers;
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> result
             = customers
                    .Select(cust =>
                    new ValueTuple<Customer, IEnumerable<Supplier>>(cust, suppliers.Where(supl =>
                            supl.Country == cust.Country && supl.City == cust.City)));

            return result;

        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            var selectedPairs = customers.GroupJoin(
                                                suppliers, c => (c.City, c.Country),
                                                           s => (s.City, s.Country),
                                                           (x, y) => (cust: x, sup: y));
            return selectedPairs;

        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            var selectedCustomers = customers.Where(c => c.Orders.Any(o => o.Total > limit));
            return selectedCustomers;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            IEnumerable<(Customer customer, DateTime dateOfEntry)> result =
                customers.Where(c => c.Orders.Any())
               .Select(cust =>
               new ValueTuple<Customer, DateTime>(cust, cust.Orders.Min(o => o.OrderDate)));
            return result;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            IEnumerable<(Customer customer, DateTime dateOfEntry)> result = customers
               .Where(c => c.Orders.Any())
                 .Select(cust =>
               new ValueTuple<Customer, DateTime>(cust, cust.Orders.Min(o => o.OrderDate)))
                 .OrderBy(c=>c.Item2.Year)
                 .ThenBy(c=> c.Item2.Month)
                 .ThenByDescending(c=>c.Item1.Orders.Sum(x => x.Total))
                 .ThenBy(c=>c.Item1.CompanyName);
            return result;
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            var result = customers.Where(c => c.PostalCode.Any(a => !char.IsDigit(a))
            || string.IsNullOrWhiteSpace(c.Region) || !c.Phone.StartsWith("("));
            return result;
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result

             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */
            /*  IEnumerable<Linq7CategoryGroup> result
                  = products.GroupBy(g => g.Category)
                  .Select(prod =>
                  new ValueTuple<IEnumerable<Linq7CategoryGroup>>(prod.Key, prod.GroupBy(pr => pr.UnitsInStock > 0)
                  .Select(p => new ValueTuple<IEnumerable<Linq7UnitsInStockGroup>>(p.Key, p.OrderBy(_ => _.UnitPrice)))));
              return result;
            */
            var categories = products.GroupBy(x => x.Category).Select(j => j.ToList()).ToList();
            var result = new List<Linq7CategoryGroup>();

            foreach (var item in categories)
            {

                result.Add(new Linq7CategoryGroup
                {
                    Category = item.ToList()[0].Category,
                    UnitsInStockGroup = item.GroupBy(b => b.UnitsInStock).Select(j => j.ToList()).ToList()
                .Select(f => new Linq7UnitsInStockGroup
                {
                    UnitsInStock = f[0].UnitsInStock,
                    Prices = f.Select(h => h.UnitPrice)
                })

                });

            }
            return result;
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            IEnumerable<(decimal category, IEnumerable<Product> products)> result = products.GroupBy(p => p.UnitPrice <= cheap ? "cheap" :
            p.UnitPrice >= expensive ? "expensive" : "average").Select(prod => new ValueTuple<decimal, IEnumerable<Product>>(prod.Count(), prod));
            
            return result;
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            IEnumerable<(string city, int averageIncome, int averageIntensity)> result = customers
                .GroupBy(c => c.City)
                .Select(city => new ValueTuple<string, int, int>
                (
                  city.Key,
                  (int)city.Average(x => x.Orders.Sum(o => o.Total)),
                  (int)city.Average(x => x.Orders.Length)
                ));
            return result;
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            //var result = suppliers.OrderBy(o => o.Country.Length).ThenBy(o => o.Country).Select(o=>o.ToString).Distinct();
            var res = (from sup in suppliers
                         orderby sup.Country
                         orderby sup.Country.Length
                         select sup.Country).Distinct();
            return string.Join("", res.ToArray());
        }
    }
}