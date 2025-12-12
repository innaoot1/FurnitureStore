using System;

namespace FurnitureStore
{
    public static class CurrentUser
    {
        public static int UserId { get; set; }
        public static string UserLogin { get; set; }
        public static int UserRole { get; set; }
        public static string UserFIO { get; set; }
    }
}