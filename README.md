# OpenClosedPrinciple-Solution


Instead of writing the multiple if else statement for calculation, we write a different class as a strategy and apply different calculations.

Now we can say that the class is open for any new extension but closed for modification.

In 'DefaultPriceCalculator' class we add all hte strategy and inject it as a dependency whenever its required.

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
