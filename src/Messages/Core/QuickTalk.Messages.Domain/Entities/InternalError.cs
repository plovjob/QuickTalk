namespace QuickTalk.Messages.Domain.Entities;

public sealed record InternalError(MessageErrors error, string message)
{
    public static readonly InternalError None =
        new(MessageErrors.None, string.Empty);
    public static InternalError ValueNotAdded(string message) =>
        new(MessageErrors.MessageNotSaved, message);
    public static InternalError CollectionDoesNotExists(string message) =>
        new(MessageErrors.NullCollection, message);
    public static InternalError NotFound(string message) =>
       new(MessageErrors.NullValue, message);
    public static InternalError DuplicateMessage(string message) =>
        new(MessageErrors.MessageNotSaved, message);
    public static InternalError UserAlreadyExists(string message) =>
        new(MessageErrors.UserAlreadyExists, message);
    public static InternalError UserDoesNotExists(string message) =>
        new(MessageErrors.UserDoesNotExists, message);
}

public enum MessageErrors
{
    None,
    MessageNotFound,
    MessageNotSaved,
    NullCollection,
    NullValue,
    UserAlreadyExists,
    UserDoesNotExists
}
