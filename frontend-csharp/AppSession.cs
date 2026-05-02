using System;
using System.Collections.Generic;
using System.Text;

namespace frontend_csharp
{
    public static class AppSession
    {
        public static string CurrentToken { get; set; }
        public static string LoggedInUsername { get; set; }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(CurrentToken);

        public static void Logout()
        {
            CurrentToken = null;
            LoggedInUsername = null;
        }
    }
}