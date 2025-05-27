namespace tree_api.Extensions;

internal static class StringExtensions
{
    public static bool IsMigrationOption(this string str)
    {
        return str.Contains("migrate", StringComparison.OrdinalIgnoreCase) ||
            str.Contains("--migrate", StringComparison.OrdinalIgnoreCase);
    }
}
