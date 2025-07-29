namespace QuickTalk.Messages.Domain.Entities;

public sealed class OperationResult<T>
{
    private readonly T? _data;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private OperationResult(T data)
    {
        Data = data;
        IsSuccess = true;
        Error = InternalError.None;
    }

    private OperationResult(InternalError error)
    {
        if (error == InternalError.None)
        {
            throw new ArgumentException("invalid error", nameof(error));
        }

        IsSuccess = false;
        Error = error;
    }

    public T Data
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("there is no value for failure");
            }

            return _data!;
        }
        private init => _data = value;
    }

    public InternalError? Error { get; }

    public static OperationResult<T> Success(T data) =>
        new(data);

    public static OperationResult<T> Failure(InternalError error) =>
        new(error);
}
