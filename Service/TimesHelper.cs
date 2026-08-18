using cron_que.Dtos;

namespace cron_que.Service;

public static class TimesHelper
{
    public static Times ParseTimes(string value) => value switch
    {
        "2_minutes" => Times.TwoMinutes,
        "5_minutes" => Times.FiveMinutes,
        "15_minutes" => Times.FifteenMinutes,
        _ => throw new ArgumentException($"Invalid times value: '{value}'.", nameof(value)),
    };

    public static int ToMinutes(this Times times) => times switch
    {
        Times.TwoMinutes => 2,
        Times.FiveMinutes => 5,
        Times.FifteenMinutes => 15,
        _ => throw new ArgumentOutOfRangeException(nameof(times)),
    };
}
