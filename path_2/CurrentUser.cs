using System;

namespace path_2
{
    /// <summary>
    /// Static class lưu thông tin user hiện tại đã đăng nhập
    /// </summary>
    public static class CurrentUser
    {
        public static int Id { get; set; }
        public static string Username { get; set; }
        public static string Email { get; set; }
        public static DateTime? LastLogin { get; set; }
        
        /// <summary>
        /// Vai trò của user: "Admin" hoặc "User"
        /// </summary>
        public static string Role { get; set; }

        /// <summary>
        /// Kiểm tra xem user hiện tại đã đăng nhập chưa
        /// </summary>
        public static bool IsLoggedIn
        {
            get { return Id > 0 && !string.IsNullOrEmpty(Username); }
        }

        /// <summary>
        /// Kiểm tra xem user hiện tại có phải là Admin không
        /// </summary>
        public static bool IsAdmin
        {
            get { return Role == "Admin"; }
        }

        /// <summary>
        /// Clear toàn bộ thông tin user (dùng khi logout)
        /// </summary>
        public static void Clear()
        {
            Id = 0;
            Username = string.Empty;
            Email = string.Empty;
            LastLogin = null;
            Role = string.Empty;
        }
    }
}
