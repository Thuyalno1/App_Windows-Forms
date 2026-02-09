using System;

namespace path_2
{
    /// <summary>
    /// Class chứa tất cả các constants, error codes và messages của ứng dụng
    /// </summary>
    public static class Constants
    {
        #region Error Codes - Authentication & Authorization
        
        /// <summary>
        /// Lỗi đăng nhập sai (Unauthorized)
        /// </summary>
        public const int ERR_LOGIN_FAILED = 401;
        
        /// <summary>
        /// Lỗi không có quyền truy cập (Forbidden)
        /// </summary>
        public const int ERR_FORBIDDEN = 403;
        
        /// <summary>
        /// Lỗi phiên đăng nhập hết hạn
        /// </summary>
        public const int ERR_SESSION_EXPIRED = 440;
        
        /// <summary>
        /// Lỗi tài khoản bị khóa
        /// </summary>
        public const int ERR_ACCOUNT_LOCKED = 423;

        #endregion

        #region Error Codes - Database

        /// <summary>
        /// Lỗi kết nối database
        /// </summary>
        public const int ERR_DB_CONNECTION = 500;
        
        /// <summary>
        /// Lỗi thực thi câu lệnh SQL
        /// </summary>
        public const int ERR_DB_QUERY = 501;
        
        /// <summary>
        /// Lỗi không tìm thấy dữ liệu
        /// </summary>
        public const int ERR_DB_NOT_FOUND = 404;
        
        /// <summary>
        /// Lỗi dữ liệu đã tồn tại (Duplicate)
        /// </summary>
        public const int ERR_DB_DUPLICATE = 409;
        
        /// <summary>
        /// Lỗi vi phạm ràng buộc dữ liệu
        /// </summary>
        public const int ERR_DB_CONSTRAINT = 502;

        #endregion

        #region Error Codes - Validation

        /// <summary>
        /// Lỗi dữ liệu không hợp lệ
        /// </summary>
        public const int ERR_INVALID_INPUT = 400;
        
        /// <summary>
        /// Lỗi thiếu thông tin bắt buộc
        /// </summary>
        public const int ERR_REQUIRED_FIELD = 422;
        
        /// <summary>
        /// Lỗi định dạng email không đúng
        /// </summary>
        public const int ERR_INVALID_EMAIL = 4001;
        
        /// <summary>
        /// Lỗi mật khẩu quá yếu
        /// </summary>
        public const int ERR_WEAK_PASSWORD = 4002;
        
        /// <summary>
        /// Lỗi mật khẩu không khớp
        /// </summary>
        public const int ERR_PASSWORD_MISMATCH = 4003;

        #endregion

        #region Error Codes - General

        /// <summary>
        /// Lỗi không xác định
        /// </summary>
        public const int ERR_UNKNOWN = 999;
        
        /// <summary>
        /// Lỗi server nội bộ
        /// </summary>
        public const int ERR_INTERNAL_SERVER = 500;

        #endregion

        #region Success Codes

        /// <summary>
        /// Thành công
        /// </summary>
        public const int SUCCESS = 200;
        
        /// <summary>
        /// Tạo mới thành công
        /// </summary>
        public const int SUCCESS_CREATED = 201;
        
        /// <summary>
        /// Cập nhật thành công
        /// </summary>
        public const int SUCCESS_UPDATED = 202;
        
        /// <summary>
        /// Xóa thành công
        /// </summary>
        public const int SUCCESS_DELETED = 204;

        #endregion

        #region Messages - Authentication

        public static class AuthMessages
        {
            public const string LOGIN_SUCCESS = "Đăng nhập thành công!";
            public const string LOGIN_FAILED = "Tên đăng nhập hoặc mật khẩu không đúng!";
            public const string LOGOUT_SUCCESS = "Đăng xuất thành công!";
            public const string SESSION_EXPIRED = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!";
            public const string ACCOUNT_LOCKED = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên!";
            public const string UNAUTHORIZED = "Bạn không có quyền truy cập chức năng này!";
            public const string REGISTER_SUCCESS = "Đăng ký tài khoản thành công!";
            public const string REGISTER_FAILED = "Đăng ký tài khoản thất bại!";
        }

        #endregion

        #region Messages - Database

        public static class DbMessages
        {
            public const string CONNECTION_SUCCESS = "Kết nối database thành công!";
            public const string CONNECTION_FAILED = "Không thể kết nối đến database!";
            public const string QUERY_ERROR = "Lỗi khi thực thi câu lệnh SQL!";
            public const string DATA_NOT_FOUND = "Không tìm thấy dữ liệu!";
            public const string DATA_DUPLICATE = "Dữ liệu đã tồn tại trong hệ thống!";
            public const string CONSTRAINT_VIOLATION = "Vi phạm ràng buộc dữ liệu!";
            public const string INSERT_SUCCESS = "Thêm dữ liệu thành công!";
            public const string UPDATE_SUCCESS = "Cập nhật dữ liệu thành công!";
            public const string DELETE_SUCCESS = "Xóa dữ liệu thành công!";
            public const string INSERT_FAILED = "Thêm dữ liệu thất bại!";
            public const string UPDATE_FAILED = "Cập nhật dữ liệu thất bại!";
            public const string DELETE_FAILED = "Xóa dữ liệu thất bại!";
        }

        #endregion

        #region Messages - Validation

        public static class ValidationMessages
        {
            public const string REQUIRED_FIELD = "Vui lòng điền đầy đủ thông tin bắt buộc!";
            public const string INVALID_EMAIL = "Email không đúng định dạng!";
            public const string INVALID_PHONE = "Số điện thoại không đúng định dạng!";
            public const string PASSWORD_TOO_SHORT = "Mật khẩu phải có ít nhất 6 ký tự!";
            public const string PASSWORD_MISMATCH = "Mật khẩu xác nhận không khớp!";
            public const string INVALID_DATE = "Ngày tháng không hợp lệ!";
            public const string INVALID_NUMBER = "Vui lòng nhập số hợp lệ!";
            
            public static string RequiredField(string fieldName) => $"{fieldName} là bắt buộc!";
            public static string InvalidFormat(string fieldName) => $"{fieldName} không đúng định dạng!";
            public static string MinLength(string fieldName, int length) => $"{fieldName} phải có ít nhất {length} ký tự!";
            public static string MaxLength(string fieldName, int length) => $"{fieldName} không được quá {length} ký tự!";
        }

        #endregion

        #region Messages - General

        public static class GeneralMessages
        {
            public const string CONFIRM_DELETE = "Bạn có chắc chắn muốn xóa?";
            public const string CONFIRM_UPDATE = "Bạn có chắc chắn muốn cập nhật?";
            public const string CONFIRM_EXIT = "Bạn có chắc chắn muốn thoát?";
            public const string OPERATION_SUCCESS = "Thao tác thành công!";
            public const string OPERATION_FAILED = "Thao tác thất bại!";
            public const string PLEASE_WAIT = "Vui lòng đợi...";
            public const string LOADING = "Đang tải dữ liệu...";
            public const string NO_DATA = "Không có dữ liệu để hiển thị!";
            public const string SAVE_SUCCESS = "Lưu thành công!";
            public const string SAVE_FAILED = "Lưu thất bại!";
        }

        #endregion

        #region Database Settings

        public static class DatabaseSettings
        {
            /// <summary>
            /// Tên DSN cho ODBC connection
            /// </summary>
            public const string DSN_NAME = "db_customer";
            
            /// <summary>
            /// Connection string
            /// </summary>
            public const string CONNECTION_STRING = "DSN=" + DSN_NAME + ";";
        }

        #endregion

        #region Application Settings

        public static class AppSettings
        {
            public const string APP_NAME = "Quản Lý Công Việc";
            public const string APP_VERSION = "1.0.0";
            public const int SESSION_TIMEOUT_MINUTES = 30;
            public const int MAX_LOGIN_ATTEMPTS = 5;
        }

        #endregion
    }
}
