[Serializable]
public class PizzaNotFoundException : Exception
{
    public PizzaNotFoundException() : base() { }
    public PizzaNotFoundException(string message) : base(message) { }
    public PizzaNotFoundException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class PizzaNotRemovedException : Exception
{
    public PizzaNotRemovedException() : base() { }
    public PizzaNotRemovedException(string message) : base(message) { }
    public PizzaNotRemovedException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class UserInputIsNullException : Exception
{
    public UserInputIsNullException() : base() { }
    public UserInputIsNullException(string message) : base(message) { }
    public UserInputIsNullException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class ParsingFailureException : Exception
{
    public ParsingFailureException() : base() { }
    public ParsingFailureException(string message) : base(message) { }
    public ParsingFailureException(string message, Exception inner) : base(message, inner) { }
}

[Serializable]
public class UserChoiceOutOfRangeException : Exception
{
    public UserChoiceOutOfRangeException() : base() { }
    public UserChoiceOutOfRangeException(string message) : base(message) { }
    public UserChoiceOutOfRangeException(string message, Exception inner) : base(message, inner) { }
}