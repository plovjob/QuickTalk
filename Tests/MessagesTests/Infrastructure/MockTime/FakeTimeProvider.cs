namespace QuickTalk.Messages.ComponentTests.Infrastructure.MockTime;

/// <summary>
/// Mock server time. For each time request a fixed value is returned, which is incremented by one for repeated operations.
/// </summary>
/// <param name="timeConfigurationObject">Default time value for server</param>
public class FakeTimeProvider(TimeConfigurationObject timeConfigurationObject) : TimeProvider
{
    private DateTime _currentTime = timeConfigurationObject.CurrentTime;

    public List<DateTimeOffset> messagesSentTimeHistory = new();

    public override DateTimeOffset GetUtcNow()
    {
        var currentFakeTime = new DateTimeOffset(_currentTime.ToUniversalTime(), TimeSpan.Zero);
        messagesSentTimeHistory.Add(currentFakeTime);
        AdvanceTime(timeConfigurationObject.TimeToAdd);
        return currentFakeTime;
    }

    /// <summary>
    /// Increases the time after each use of <see cref="GetUtcNow"/>.
    /// </summary>
    /// <param name="timeToAdd">Time interval for next request.</param>
    private void AdvanceTime(TimeSpan timeToAdd) => _currentTime = _currentTime.Add(timeToAdd);

    public void SetToDefault()
    {
        _currentTime = timeConfigurationObject.CurrentTime;
        messagesSentTimeHistory = new();
    }
}

/// <summary>
/// Default configuration for fixed server time.
/// </summary>
/// <param name="CurrentTime">Fixed server time.</param>
/// <param name="TimeToAdd">Time interval for next request.</param>
public record TimeConfigurationObject(DateTime CurrentTime, TimeSpan TimeToAdd);
