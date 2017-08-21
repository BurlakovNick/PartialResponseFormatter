namespace PartialResponseFormatter
{
    public static class EnumerableExtentions
    {
        public static bool IsEmpty<T>(this T[] arr) => arr.Length == 0;
        public static bool IsNullOrEmpty<T>(this T[] arr) => arr == null || arr.Length == 0;
    }
}