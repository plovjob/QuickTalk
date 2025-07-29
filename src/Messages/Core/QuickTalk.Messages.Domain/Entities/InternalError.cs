namespace QuickTalk.Messages.Domain.Entities;

public sealed record InternalError(ErrorReason reason, string message)
{
    public static readonly InternalError None =
        new(ErrorReason.None, string.Empty);
    public static InternalError ValueNotAdded(string message) =>
        new(ErrorReason.NotAdded, message);
    public static InternalError NoMessages(string message) =>
        new(ErrorReason.NoMessages, message);
    //другие ошибки
}

public enum ErrorReason
{
    None,
    NotFound,
    NotAdded,
    NoMessages
}
