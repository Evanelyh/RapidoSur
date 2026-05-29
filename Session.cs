namespace RapidoSurWinForms
{
    public static class Session
    {
        public static bool IsLoggedIn { get; set; } = false;
        public static string UserType { get; set; } = string.Empty; 
        public static int UserId { get; set; } = 0;
        public static string UserName { get; set; } = string.Empty;
        public static string UserRole { get; set; } = string.Empty; 
        public static string UserLicence { get; set; } = string.Empty;

        public static void Clear()
        {
            IsLoggedIn = false;
            UserType = string.Empty;
            UserId = 0;
            UserName = string.Empty;
            UserRole = string.Empty;
            UserLicence = string.Empty;
        }
    }
}
