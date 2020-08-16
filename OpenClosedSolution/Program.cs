using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenClosedSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderItem objItem1 = new OrderItem
            {
                Identifier = "Each",
                Quantity = 1
            };
            OrderItem objItem2 = new OrderItem
            {
                Identifier = "Weight",
                Quantity = 2
            };
            OrderItem objItem3 = new OrderItem
            {
                Identifier = "Spec",
                Quantity = 4
            };
            //Using this interface so that it decides which class to go for calculating total
            IPriceCalculator priceCalculator = new DefaultPriceCalculator();
            ShoppingCart objCart = new ShoppingCart(priceCalculator);
            objCart.Add(objItem1);
            objCart.Add(objItem2);
            objCart.Add(objItem3);
            //Now ShoppingCart is no longer responsible for actual price calculation
            decimal k = objCart.TotalAmount();
        }
    }

    public class OrderItem
    {
        public string Identifier { get; set; }
        public int Quantity { get; set; }
    }

    public interface IStrategy
    {
        bool IsMatch(OrderItem item);
        decimal CalculateTotal(OrderItem item);
    }

    public class PricePerUnitStrategy : IStrategy
    {
        public bool IsMatch(OrderItem item)
        {
            return item.Identifier.StartsWith("Each");
        }
        public decimal CalculateTotal(OrderItem item)
        {
            return item.Quantity * 4m;
        }
    }

    public class PricePerKilogramStrategy : IStrategy
    {
        public bool IsMatch(OrderItem item)
        {
            return item.Identifier.StartsWith("Weight");
        }
        public decimal CalculateTotal(OrderItem item)
        {
            return item.Quantity * 3m / 1000;
        }
    }

    public class SpecialPriceStrategy : IStrategy
    {
        public bool IsMatch(OrderItem item)
        {
            return item.Identifier.StartsWith("Spec");
        }
        public decimal CalculateTotal(OrderItem item)
        {
            decimal total = 0m;
            total += item.Quantity * .3m;
            int setsOfFour = item.Quantity / 4;
            total -= setsOfFour * .15m;
            return total;
        }
    }

    /*
     In case if you need to add one more strategy then, you can just add it to DefaultPriceCalculator class
     */
    public class BuyThreeGetOneFree : IStrategy
    {
        public bool IsMatch(OrderItem item)
        {
            return item.Identifier.StartsWith("Buy3OneFree");
        }

        public decimal CalculateTotal(OrderItem item)
        {
            decimal total = 0m;
            total += item.Quantity * 1m;
            int setsOfThree = item.Quantity / 3;
            total -= setsOfThree * 1m;
            return total;
        }
    }

    //Introducing Interface to abstract the method
    public interface IPriceCalculator
    {
        decimal CalculatePrice(OrderItem item);
    }

    /*You can say that the class is open for extension but closed for modification : OCP : Open Closed Principle
      If you Inherit from IStrategy, you need to give implementation to Both the method, which  is not required,instead create separate Interface for new Method */
    public class DefaultPriceCalculator : IPriceCalculator
    {
        private readonly List<IStrategy> _strategies;
        //Add all the classes to '_strategies' which contains both the method of all three classes 
        public DefaultPriceCalculator()
        {
            _strategies = new List<IStrategy>();
            _strategies.Add(new PricePerKilogramStrategy());
            _strategies.Add(new PricePerUnitStrategy());
            _strategies.Add(new SpecialPriceStrategy());
        }
        //Include both logic here in one line : i.e to check for Match and also for calculating the total
        public decimal CalculatePrice(OrderItem item)
        {
            return _strategies.First(r => r.IsMatch(item)).CalculateTotal(item);
        }
    }

    public class ShoppingCart
    {
        private readonly List<OrderItem> _orderItems;
        private readonly IPriceCalculator _priceCalculator;
        //When ShoppingCart class gets instantiated, Get all class logic through IPriceCalculator
        public ShoppingCart(IPriceCalculator priceCalculator)
        {
            _priceCalculator = priceCalculator;   //Setting the interface
            _orderItems = new List<OrderItem>();
        }
        public IEnumerable<OrderItem> OrderItems
        {
            get { return _orderItems; }
        }
        public void Add(OrderItem item)
        {
            _orderItems.Add(item);
        }
        public decimal TotalAmount()
        {
            decimal total = 0m;
            foreach (var orderItem in OrderItems)
            {
                // From IPriceCalculator interface invoke a method
                total += _priceCalculator.CalculatePrice(orderItem);
            }
            return total;
        }
    }


    /* Look at the possibility of further refactoring
    
     *public class DefaultPriceCalculator : IPriceCalculator
        {
            private readonly IEnumerable<IPriceStrategy> _priceStrategies;
 
    public DefaultPriceCalculator(IEnumerable<IPriceStrategy> priceStrategies)
    {
        _priceStrategies = priceStrategies;
    }
 
    public decimal CalculatePrice(OrderItem item)
    {
        return _priceStrategies.First(r => r.IsMatch(item)).CalculatePrice(item);
    }
} 
     
     */
}
