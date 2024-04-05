using System.Diagnostics;

static class ConstantCosts
{
    /*
    The 'decimal' floating-point type is apparently the best for financial calculations
    where floating-point errors must not occur.
    Source: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types
    */
    public static decimal DanishVAT = 0.25M;
    public static decimal DeliveryCost = 40M;
}

public class Item
{
    public string Name {get; set;}
    public decimal Price {get; set;}

    public Item(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    protected void AddPrice(decimal price)
    {
        Price += price;
    }

    public override string ToString()
    {
        return $"This item's name is {Name} and its price is {Price}.";
    }
}

public class Topping : Item
{
    public Topping(string name, decimal price) : base(name, price)
    {
    }

    public override string ToString()
    {
        return $"Topping is {Name} and its additional cost is {Price}.";
    }
}

public class Pizza : Item
{
    public int MenuNumber {get; set;}
    public Topping[] PizzaTopping {get; set;}

    public Pizza(string name, decimal price, int menuNumber) : base(name, price)
    {
        MenuNumber = menuNumber;
        PizzaTopping = Array.Empty<Topping>();
    }

    public void AddTopping(Topping newTopping)
    {
        int newArrayLength = PizzaTopping.Length + 1;
        Topping[] newToppingArray = new Topping[newArrayLength];
        PizzaTopping.CopyTo(newToppingArray, 0);
        newToppingArray[^1] = newTopping;
        PizzaTopping = newToppingArray;
        AddPrice(newTopping.Price);
    }

    public void RemoveTopping(Topping undesiredTopping)
    {
        int toppingIndexLocation = Array.IndexOf(PizzaTopping, undesiredTopping);
        if (toppingIndexLocation == -1)
        {
            throw new Exception("The undesired topping was not found on the pizza.");
        }
        int newArrayLength = PizzaTopping.Length - 1;
        Topping[] newToppingArray = new Topping[newArrayLength];
        for (int i = 0; i < newArrayLength; i++)
        {
            if (i >= toppingIndexLocation)
            {
                newToppingArray[i] = PizzaTopping[i + 1];
            }
            else
            {
                newToppingArray[i] = PizzaTopping[i];
            }
        }
        PizzaTopping = newToppingArray;
        AddPrice(-undesiredTopping.Price);
    }

    public override string ToString()
    {
        string[] pizzaInfo = {$"This pizza is called {Name}, ",
                              $"it has the menu number #{MenuNumber} ",
                              $"and its price is {Price} DKK."};
        string finalString = "";
        foreach (string info in pizzaInfo)
        {
            finalString += info;
        }
        if (PizzaTopping.Length > 0)
        {
            foreach (Topping topping in PizzaTopping)
            {
                finalString += $"\nIt has extra {topping.Name} as topping.";
            }
        }
        return finalString;
    }
}

public class Customer
{
    public string CustomerName {get;}
    public Basket CustomerBasket {get; set;}

    public Customer(string customerName)
    {
        CustomerName = customerName;
        CustomerBasket = new Basket();
    }

    /*
    The logic here is that the system doesn't necessarily know who the customer
    is before they have placed their order. In the pizzeria itself, they don't
    ask the customer for their name either. So it's only required to know the
    customer's name when the order itself has been placed.
    */
    public Customer(string customerName, Basket customerBasket)
    {
        CustomerName = customerName;
        CustomerBasket = customerBasket;
    }

    public override string ToString()
    {
        return $"Customer {CustomerName} has a basket. " + CustomerBasket.ToString();
    }
}

public class Basket
{
    public Item[] Items {get; set;}
    public decimal TotalPrice {get; set;}
    public int ItemQuantity {get; set;}

    public Basket()
    {
        Items = Array.Empty<Item>();
        TotalPrice = 0;
        ItemQuantity = 0;
    }

    public Basket(Item[] items)
    {
        Items = items;
        TotalPrice = 0;
        ItemQuantity = Items.Length;
        UpdateTotalPrice();
    }

    private void UpdateTotalPrice()
    {
        if (Items.Length == 0)
        {
            TotalPrice = 0;
        }
        else
        {
            decimal newTotalPrice = 0;
            foreach (Item item in Items)
            {
                newTotalPrice += item.Price;
            }
            TotalPrice = newTotalPrice;
        }
    }

    public void AddItem(Item item)
    {
        Item[] newItems = new Item[Items.Length + 1];
        Items.CopyTo(newItems, 0);
        newItems[^1] = item;
        Items = newItems;
        ItemQuantity = newItems.Length;
        UpdateTotalPrice();
    }

    public void RemoveItem(Item item)
    {
        int itemIndexLocation = Array.IndexOf(Items, item);
        if (itemIndexLocation == -1)
        {
            // TODO: Make new type of exception?
            throw new Exception("The undesired item was not found in the basket.");
        }
        else
        {
            int newArrayLength = Items.Length - 1;
            Item[] newItemArray = new Item[newArrayLength];
            for (int i = 0; i < newArrayLength; i++)
            {
                if (i >= itemIndexLocation)
                {
                    newItemArray[i] = Items[i + 1];
                }
                else
                {
                    newItemArray[i] = Items[i];
                }
            }
            Items = newItemArray;
            ItemQuantity = newItemArray.Length;
            UpdateTotalPrice();
        }
    }

    public override string ToString()
    {
        if (ItemQuantity == 0)
        {
            return "This basket contains no items.";
        }
        else
        {
            string finalString = $"This basket contains {ItemQuantity} item:";
            if (ItemQuantity > 1)
            {
                finalString = $"This basket contains {ItemQuantity} items:";
            }
            foreach (Item item in Items)
            {
                finalString += "\n" + item.Name + $" - {item.Price} DKK.";
            }
            return finalString;
        }
    }
}

public class Order
{
    public int OrderNumber {get;}
    public Customer Customer {get;}
    public DateTime TimeOrderPlaced {get;}
    public bool IsOrderCompleted {get; set;}
    public decimal TotalPrice {get; set;}

    public Order(Customer customer, int orderNumber)
    {
        OrderNumber = orderNumber;
        Customer = customer;
        TimeOrderPlaced = DateTime.Now;
        IsOrderCompleted = false;
        TotalPrice = CalculateTotalPrice();
    }

    public int GetMinutesSinceOrderPlaced()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeDifference = currentTime - TimeOrderPlaced;
        return timeDifference.Minutes;
    }

    private decimal CalculateTaxOfItems(decimal price)
    {
        return price * ConstantCosts.DanishVAT;
    }

    public decimal CalculateTotalPrice()
    {
        decimal newTotalPrice = Customer.CustomerBasket.TotalPrice;
        newTotalPrice += ConstantCosts.DeliveryCost;
        return newTotalPrice;
    }

    public override string ToString()
    {
        string finalString = $"\nCustomer name: {Customer.CustomerName}";
        finalString += $"\nOrder was placed {GetMinutesSinceOrderPlaced()} minutes ago:";
        foreach (Item item in Customer.CustomerBasket.Items)
        {
            if (item.GetType() == typeof(Pizza))
            {
                Pizza currentItem = (Pizza) item;
                finalString += $"\n#{currentItem.MenuNumber} {currentItem.Name} - {currentItem.Price} DKK.";
            }
            else
            {
                finalString += $"\n{item.Name} - {item.Price} DKK.";
            }
        }
        finalString += $"\n\nSubtotal: {TotalPrice} DKK.";
        finalString += $"\nVAT 25%: {CalculateTaxOfItems(Customer.CustomerBasket.TotalPrice)} DKK.";
        finalString += $"\nDelivery: {ConstantCosts.DeliveryCost} DKK.";
        if (IsOrderCompleted)
        {
            finalString += $"\n\nOrder status: Completed";
        }
        else
        {
            finalString += $"\n\nOrder status: In Progress";
        }
        return finalString;
    }
}

public class MenuCatalog
{
    // Right now I'll just stick to Pizza objects, but for the real deal it would have to be List<Item> instead.
    public List<Pizza> Pizzas {get; set;}
    public string[] MenuChoices {get; set;}

    public MenuCatalog(string[] pizzaNames, decimal[] pizzaPrices, string[] menuChoices)
    {
        var newPizzas = new List<Pizza>();
        for (int i = 0; i < pizzaNames.Length; i++)
        {
            var pizzaToBeAdded = new Pizza(pizzaNames[i], pizzaPrices[i], i + 1);
            newPizzas.Add(pizzaToBeAdded);
        }
        Pizzas = newPizzas;
        MenuChoices = menuChoices;
    }

    public MenuCatalog(List<string> pizzaNames, List<decimal> pizzaPrices, List<string> menuChoices)
    {
        var newPizzas = new List<Pizza>();
        for (int i = 0; i < pizzaNames.Count; i++)
        {
            var pizzaToBeAdded = new Pizza(pizzaNames[i], pizzaPrices[i], i + 1);
            newPizzas.Add(pizzaToBeAdded);
        }
        Pizzas = newPizzas;
        MenuChoices = menuChoices.ToArray();
    }

    // Attempt at implementing a constructor in a different way,
    // but probably not good in terms of memory usage if I had to guess.

    // public MenuCatalog(List<string> pizzaNames, List<decimal> pizzaPrices)
    // {
    //     string[] arr1 = pizzaNames.ToArray();
    //     decimal[] arr2 = pizzaPrices.ToArray();
    //     var temp = new MenuCatalog(arr1, arr2);
    //     Pizzas = temp.Pizzas;
    // }

    public void PrintMenu()
    {
        Console.WriteLine(BuildMenuString());
    }

    private string BuildMenuString()
    {
        string pizzaMenu = "";
        foreach (Pizza pizza in Pizzas)
        {
            pizzaMenu += $"{pizza.MenuNumber}. {pizza.Name} - {pizza.Price},-\n";
        }
        return pizzaMenu;
    }

    public Pizza SearchPizza(int pizzaNumber)
    {
        foreach (Pizza pizza in Pizzas)
        {
            if (pizza.MenuNumber == pizzaNumber)
            {
                return pizza;
            }
        }
        throw new PizzaNotFoundException($"Pizza with nr. {pizzaNumber} was not found.");
    }

    public Pizza SearchPizza(string pizzaName)
    {
        foreach (Pizza pizza in Pizzas)
        {
            if (pizza.Name == pizzaName || pizza.Name.ToLower() == pizzaName || pizza.Name.ToUpper() == pizzaName)
            {
                return pizza;
            }
        }
        throw new PizzaNotFoundException($"Pizza with the name {pizzaName} was not found.");
    }

    // TODO: Kind of pointless unless this method takes over args like name, price, etc.
    public void AddPizza(Pizza newPizza)
    {
        Pizzas.Add(newPizza);
    }

    public void DeletePizza(Pizza undesiredPizza)
    {
        bool isPizzaRemoved = Pizzas.Remove(undesiredPizza);
        if (!isPizzaRemoved)
        {
            throw new PizzaNotRemovedException($"Pizza {undesiredPizza.Name} #{undesiredPizza.MenuNumber} could not be removed. Is the pizza object in the list?");
        }
    }

    public void DeletePizza(string pizzaName)
    {
        try
        {
            Pizza pizzaToBeRemoved = SearchPizza(pizzaName);
            DeletePizza(pizzaToBeRemoved);
        }
        catch (PizzaNotFoundException e)
        {
            Console.WriteLine(e.Message); // Simple handling for now.
        }
        catch (PizzaNotRemovedException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void DeletePizza(int pizzaNumber)
    {
        try
        {
            Pizza pizzaObjToBeDeleted = SearchPizza(pizzaNumber);
            DeletePizza(pizzaObjToBeDeleted);
        }
        catch (PizzaNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (PizzaNotRemovedException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void UpdatePizza(Pizza pizzaToBeUpdated, Pizza newPizza)
    {
        try
        {
            int pizzaLocation = Pizzas.FindIndex(obj => obj == pizzaToBeUpdated);
            if (pizzaLocation == -1)
            {
                throw new Exception("Pizza object could not be found or does not match.");
            }
            Pizzas[pizzaLocation] = newPizza;
        }
        catch (PizzaNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void UpdatePizza(string pizzaToBeUpdatedName, Pizza newPizza)
    {
        Pizza pizzaElementInList = SearchPizza(pizzaToBeUpdatedName);
        UpdatePizza(pizzaElementInList, newPizza);
    }
    
    public void UpdatePizza(int pizzaToBeUpdated, Pizza newPizza)
    {
        Pizza pizzaElementInList = SearchPizza(pizzaToBeUpdated);
        UpdatePizza(pizzaElementInList, newPizza);
    }

    private string CreateMenuOfChoice(string[] choices)
    {
        string menu = "";
        for (int i = 0; i < choices.Length; i++)
        {
            menu += $"{i + 1} " + choices[i] + "\n";
        }
        return menu;
    }

    private void CheckUserInput(string? userInput)
    {
        if (userInput != null)
        {}
        else
        {
            throw new UserInputIsNullException($"Value: {userInput}");
        }
    }

    private int ParseUserInputToInt(string userInput)
    {
        CheckUserInput(userInput);
        if (int.TryParse(userInput, out int userChoice)) // Might have to switch to Parse() instead if unexpected errors happen.
        {
            return userChoice;
        }
        else
        {
            throw new ParsingFailureException("User input parsing failed.");
        }
    }

    private decimal ParseUserInputToDecimal(string userInput)
    {
        CheckUserInput(userInput);
        if (decimal.TryParse(userInput, out decimal userChoice)) // Might have to switch to Parse() instead if unexpected errors happen.
        {
            return userChoice;
        }
        else
        {
            throw new ParsingFailureException("User input parsing failed. Was the input a number?");
        }
    }

    public int MenuChoice()
    {
        string menu = CreateMenuOfChoice(MenuChoices);
        Console.WriteLine(menu);
        var initialRead = Console.ReadLine();
        try
        {
            int userInput = ParseUserInputToInt(initialRead);
            if (userInput >= 1 && userInput <= MenuChoices.Length)
            {
                return userInput;
            }
            else
            {
                throw new UserChoiceOutOfRangeException($"User input value has to be between 1 and {MenuChoices.Length}.");
            }
        }
        catch (UserInputIsNullException e)
        {
            Console.WriteLine(e.Message);
            MenuChoice();
        }
        catch (ParsingFailureException e)
        {
            Console.WriteLine(e.Message + "\nTry a different input:\n");
            MenuChoice();
        }
        return -1;
    }

    public void InitiateMenuChoice(int userChoice)
    {
        switch (userChoice)
        {
            case 1:
                Console.WriteLine("Please, enter the pizza's name:\n");
                var userInputName = Console.ReadLine();
                CheckUserInput(userInputName);
                Console.WriteLine("Please, enter the pizza's price:\n");
                decimal userInputPrice = ParseUserInputToDecimal(Console.ReadLine());
                Console.WriteLine("Please, enter the pizza's menu number:\n");
                int userInputNumber = ParseUserInputToInt(Console.ReadLine());
                var userPizza = new Pizza(userInputName, userInputPrice, userInputNumber);
                AddPizza(userPizza);
                Console.WriteLine("Pizza added successfully!");
                break;

            // More cases to be covered.
        }
    }

    public int PickPizza()
    {
        PrintMenu();
        Console.WriteLine("Please, select an item by number:\n");
        var initialRead = Console.ReadLine();
        if (initialRead != null)
        {
            if (int.TryParse(initialRead, out int userInput))
            {
                if (userInput >= 1 && userInput <= Pizzas.Count)
                {
                    return userInput;
                }
                else
                {
                    throw new Exception($"User input has to be between 1 and {Pizzas.Count}.");
                }
            }
            else
            {
                throw new Exception("User input parsing failed.");
            }
        }
        else
        {
            throw new Exception("User input is null");
        }
    }

    public override string ToString()
    {
        string objectInfo = "This menu catalog has the following items listed:\n\n";
        objectInfo += BuildMenuString();
        return objectInfo;
    }
}

public class Store
{
    public string Name {get; set;}
    public MenuCatalog Menu {get; set;}

    public Store(string name, MenuCatalog menu)
    {
        Name = name;
        Menu = menu;
    }

    public void Start()
    {
        // Testing AddPizza() method.
        string kleePizzaName = "Klee Pizza :)";
        decimal kleePizzaPrice = 500;
        int kleePizzaNumber = Menu.Pizzas.Count + 1;
        var kleePizza = new Pizza(kleePizzaName, kleePizzaPrice, kleePizzaNumber);
        Menu.AddPizza(kleePizza);
        Debug.Assert(Menu.Pizzas.Last() == kleePizza);
        Debug.Assert(Menu.Pizzas.Last().Name == kleePizzaName);
        Debug.Assert(Menu.Pizzas.Last().Price == kleePizzaPrice);
        Debug.Assert(Menu.Pizzas.Last().MenuNumber == kleePizzaNumber);

        // Testing MenuChoice() method.
        int testResult = Menu.MenuChoice();
        Debug.Assert(testResult >= 1 && testResult <= 5);

        // Testing PickPizza() and SearchPizza() methods.
        // int userPickedNumber = Menu.PickPizza();
        // Debug.Assert(userPickedNumber >= 1 && userPickedNumber <= Menu.Pizzas.Count);
        // Pizza pickedPizza = Menu.SearchPizza(userPickedNumber);
        // Debug.Assert(pickedPizza == Menu.Pizzas[pickedPizza.MenuNumber - 1]);

        // Testing DeletePizza() method.
        var noGoodPizza = new Pizza("Old stinky pizza", 10, Menu.Pizzas.Count + 1);
        Menu.AddPizza(noGoodPizza);
        Menu.DeletePizza(noGoodPizza); // Uncomment to see assertion fail.
        bool isPizzaPresent = false;
        foreach (Pizza pizza in Menu.Pizzas)
        {
            if (pizza == noGoodPizza)
            {
                isPizzaPresent = true;
                break;
            }
        }
        Debug.Assert(!isPizzaPresent);

        // Testing UpdatePizza() method.
        decimal newKleePizzaPrice = 250;
        var newKleePizza = new Pizza(kleePizzaName, newKleePizzaPrice, kleePizzaNumber);
        Menu.UpdatePizza(kleePizza, newKleePizza);

        bool isNewPizzaPresent = false;
        foreach (Pizza pizza in Menu.Pizzas)
        {
            if (pizza == newKleePizza)
            {
                isNewPizzaPresent = true;
                break;
            }
        }
        Debug.Assert(isNewPizzaPresent);

        bool isOldPizzaRemoved = true;
        foreach (Pizza pizza in Menu.Pizzas)
        {
            if (pizza == kleePizza)
            {
                isOldPizzaRemoved = false;
                break;
            }
        }
        Debug.Assert(isOldPizzaRemoved);
    }
}

class Program
{
    static void Main(string[] args)
    {
        string[] pizzaNames = {"Margherita", "Vesuvio", "Capricciosa", "Calzone",
                               "Quattro Stagioni", "Marinara", "Vegetariana", "Italiana",
                               "Gorgonzola", "Contadina","Napoli","Vichinga",
                               "Calzone Speciale", "Esotica", "Tonno", "Sardegna",
                               "Romana", "Sole", "Big Mamma", "La salami",
                               "Rocco", "Marco", "KoKKode", "Antonello",
                               "Pasqualino", "Felix", "Bambino"};
        int[] pizzaPrices   = {80, 92, 98, 98,
                               98, 97, 98, 93,
                               97, 92, 95, 98,
                               98, 98, 97, 97,
                               98, 98, 99, 98,
                               99, 99, 99, 99,
                               98, 95, 65};
        var decimalPizzaPrices = new decimal[pizzaPrices.Length];
        for (int i = 0; i < pizzaPrices.Length; i++)
        {
            decimalPizzaPrices[i] = pizzaPrices[i];
        }
        // Also for testing the different overloads.
        var pizzaNamesList = pizzaNames.ToList<string>();
        var pizzaPricesList = decimalPizzaPrices.ToList<decimal>();
        string[] menuChoices = {"Add new to pizza menu", "Delete pizza",
                                "Update pizza", "Search pizza",
                                "Display pizza menu"};
        var storeMenu1 = new MenuCatalog(pizzaNames, decimalPizzaPrices, menuChoices);
        // var storeMenu2 = new MenuCatalog(pizzaNamesList, pizzaPricesList, menuChoices);
        var BigMamma1 = new Store("Big Mamma", storeMenu1);
        // var BigMamma2 = new Store("Big Mamma", storeMenu2);
        BigMamma1.Start();
        // BigMamma2.Start();
    }
}