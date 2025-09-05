namespace Web.Models.Account
{
    public class UserProfileViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Settings
        public bool DarkMode { get; set; }
    }
}
