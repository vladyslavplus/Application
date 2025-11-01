namespace Evently.Application.DTOs.Event
{
    public class ParticipantDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string Initials => GetInitials(FullName);
        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "??";

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "??";

            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            return string.Join("", parts.Take(2).Select(p => char.ToUpper(p[0])));
        }
    }
}