namespace PartialResponseFormatter
{
    public static class EnumerableExtentions
    {
        public static bool IsEmpty<T>(this T[] arr) => arr.Length == 0;
    }
}