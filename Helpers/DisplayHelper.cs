using System.Globalization;

namespace LibrarySystem.Helpers
{
    // Small formatting helpers shared by the Razor views.
    public static class DisplayHelper
    {
        private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

        public static string Money(decimal value) => value.ToString("C2", UsCulture);

        public static string Date(DateTime value) => value.ToString("dd MMM yyyy", UsCulture);

        public static string Date(DateTime? value) => value.HasValue ? Date(value.Value) : "—";

        public static string Initials(string fullName)
        {
            string[] parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0][..1].ToUpper();
            return (parts[0][..1] + parts[^1][..1]).ToUpper();
        }

        public static string ImageUrl(string? imagePath) =>
            string.IsNullOrEmpty(imagePath) ? "/images/No_Image.png" : imagePath.Replace('\\', '/');
    }
}
