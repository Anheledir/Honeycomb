using NetArchTest.Rules;
using System.Runtime.CompilerServices;

namespace BaseBotService.Tests;
internal static class TestHelpers
{
    internal static string GetFailingTypes(this TestResult result, [CallerMemberName] string? testName = null)
    {
        if (result == null || result.FailingTypes == null) return string.Empty;
        var failingTypes = string.Join(", ", result.FailingTypes.Select(t => t.FullName));
        return $"{testName ?? "unknown"}:{Environment.NewLine}{failingTypes}";
    }
}
