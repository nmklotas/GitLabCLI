using System;

namespace GitlabCmd.Console.Utilities
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoringCase(this string value, string other) => 
            string.Equals(value, other, StringComparison.OrdinalIgnoreCase);

        public static bool IsNotEmpty(this string value) => 
            !string.IsNullOrEmpty(value);

        public static bool IsEmpty(this string value) => 
            string.IsNullOrEmpty(value);

        public static string NormalizeSpaces(this string value) => 
            (value ?? "").Trim();

        public static string SafeToString(this object value) => 
            (value ?? "").ToString();

        public static string Expand(this char value, int length)
        {
            string result = "";

            for (int i = 0; i < length; i++)
                result += value;

            return result;
        }
    }
}