namespace QuickTalk.Identity.ComponentTests.Mock.Services;

public class FakeTimeProvider : TimeProvider
{
    private readonly DateTime defaultDateTime = new DateTime(2025, 11, 25, 13, 0, 0, kind: DateTimeKind.Utc).ToUniversalTime();

    public DateTime CurrentTime { get; private set; }

    public FakeTimeProvider()
    {
        CurrentTime = defaultDateTime;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return new DateTimeOffset(CurrentTime.ToUniversalTime(), TimeSpan.Zero);
    }

    public void AdvanceTime(TimeSpan timeToAdd) => CurrentTime = CurrentTime.Add(timeToAdd);

    public void SetToDefault() => CurrentTime = defaultDateTime;
}
