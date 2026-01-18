namespace QuickTalk.Identity.Domain.Entities;

public class Result
{
    protected Result(bool isSuccess, Error? error = null)
    {
        if (isSuccess && error != Error.None ||
           !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; protected set; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}

public class Result<T> : Result where T : class?
{
    protected Result(bool isSuccess, T? data, Error? error)
        : base(isSuccess, error)
    {
        Data = data;
    }

    public T? Data { get; }

    public static Result<T> Success(T data) => new(true, data, Error.None);

    public static new Result<T> Failure(Error error) => new(false, null, error);
}

public record Error(string ErrorName, string Description, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, 200);
}

public static class UserErrors
{
    public static readonly Error EmailAlreadyRegistered = new(
        "Users.EmailAlreadyRegistered", "User with this email already exists", 409);

    public static readonly Error InvalidLogin = new(
        "Users.InvalidAccountDetails", "Error while trying to log in. Invalid login or password", 401);

    public static readonly Error InvalidJwtToken = new(
        "JwtToken.InvalidToken", "JWT Access token is not valid", 400);

    public static readonly Error InvalidJwtClaims = new(
        "JwtToken.InvalidClaims", "One or more JWT invalid", 400);

    public static readonly Error InvalidRefreshToken = new(
        "RefreshToken.InvalidToken", "refresh token invalid or expired", 400);

    public static Error UserNotFound(string email) => new(
        "Users.NotFoundByEmail", $"User with this email: {email} was not found", 404);

    public static Error UserNotFound(Guid id) => new(
        "Users.NotFoundById", $"User with this id: {id} was not found", 404);

    public static Error RefreshTokenNotFound(Guid userId) => new(
        "RefreshToken.NotFound", $"Operation failed with an error. The token for the user with id:{userId} was not found.", 404);

    public static Error RefreshTokenAlreadyExists(Guid userId) => new(
        "RefreshToken.AlreadyExists", $"Operation failed with an error. The token for the user with id:{userId} already exists.", 404);

    public static Error InternalError(string internalMessage) => new(
        "InternalError", $"Operation failed with an error: {internalMessage}", 500);
}
