namespace QuickTalk.Messages.Domain.Entities;

public class OperationResult
{
    public bool IsSuccess { get; }
    //public string ResultMessage { get; }
    public bool IsFailure => !IsSuccess;
    public InternalError? Error { get; }

    protected internal OperationResult(bool isSuccess, InternalError error)
    {
        switch (isSuccess)
        {
            case true when error != InternalError.None:
                throw new InvalidOperationException("Operation result not valid");

            case false when error == InternalError.None:
                throw new InvalidOperationException("Operation result not valid");

            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }

    public static OperationResult Success() => new(true, InternalError.None);
    public static OperationResult Failure(InternalError error) => new(false, error);
    public static OperationResult<T> Success<T>(T value) => new(value, true, InternalError.None);
    public static OperationResult<T> Failure<T>(InternalError error) => new(default, false, error);

    public static OperationResult<T> Create<T>(T? value) => value is not null ? Success(value) : Failure<T>(InternalError.NotFound("Operation result value is null"));
}

public class OperationResult<T> : OperationResult
{
    private readonly T? _value;

    protected internal OperationResult(T? value, bool isSuccess, InternalError error)
        : base(isSuccess, error) => _value = value;

    public T Value => _value! ?? throw new InvalidOperationException("Result has no value");

    public static implicit operator OperationResult<T>(T? value) => Create(value);
}
