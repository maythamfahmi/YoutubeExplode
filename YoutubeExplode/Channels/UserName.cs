using System;
using System.Text.RegularExpressions;

namespace YoutubeExplode.Channels
{
    [Equals(DoNotAddEqualityOperators = true)]
    public readonly partial struct UserName
    {
        public string Value { get; }

        public UserName(string nameOrUrl) =>
            Value = TryNormalize(nameOrUrl) ??
                    throw new ArgumentException($"Invalid YouTube username or URL: '{nameOrUrl}'.");

        /// <inheritdoc />
        public override string ToString() => Value;
    }

    public partial struct UserName
    {
        public static implicit operator UserName(string nameOrUrl) => new UserName(nameOrUrl);

        public static implicit operator string(UserName id) => id.ToString();
    }

    public partial struct UserName
    {
        private static bool IsValid(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Usernames can't be longer than 20 characters
            if (name.Length > 20)
                return false;

            return !Regex.IsMatch(name, @"[^0-9a-zA-Z]");
        }

        private static string? TryNormalize(string? nameOrUrl)
        {
            if (string.IsNullOrWhiteSpace(nameOrUrl))
                return null;

            // Name
            // TheTyrrr
            if (IsValid(nameOrUrl))
                return nameOrUrl;

            // URL
            // https://www.youtube.com/user/TheTyrrr
            var regularMatch = Regex.Match(nameOrUrl, @"youtube\..+?/user/(.*?)(?:\?|&|/|$)").Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(regularMatch) && IsValid(regularMatch))
            {
                return regularMatch;
            }

            // Invalid input
            return null;
        }
    }
}