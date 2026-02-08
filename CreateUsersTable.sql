-- ============================================
-- SCRIPT TẠO BẢNG USERS CHO HỆ THỐNG ĐĂNG KÝ/ĐĂNG NHẬP
-- Database: path_1
-- ============================================

-- Xóa bảng cũ nếu tồn tại (cẩn thận - sẽ mất dữ liệu!)
DROP TABLE IF EXISTS `path_1`.`Users`;

-- Tạo bảng Users với charset UTF8MB4
CREATE TABLE `path_1`.`Users` (
    `Id` INT NOT NULL AUTO_INCREMENT COMMENT 'ID tự động tăng',
    `Username` VARCHAR(100) NOT NULL COMMENT 'Tên đăng nhập (unique)',
    `Email` VARCHAR(150) NOT NULL COMMENT 'Email (unique)',
    `PasswordHash` VARCHAR(255) NOT NULL COMMENT 'Mật khẩu đã hash (SHA256)',
    `Salt` VARCHAR(255) NOT NULL COMMENT 'Salt để hash password',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo tài khoản',
    `LastLogin` DATETIME NULL COMMENT 'Lần đăng nhập cuối',
    
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UQ_Users_Username` (`Username`),
    UNIQUE KEY `UQ_Users_Email` (`Email`),
    
    INDEX `IDX_Username` (`Username`),
    INDEX `IDX_Email` (`Email`)
) ENGINE=InnoDB 
  DEFAULT CHARSET=utf8mb4 
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Bảng lưu thông tin user';

-- Kiểm tra bảng đã tạo thành công
SELECT 'Bảng Users đã được tạo thành công!' AS Status;

-- Xem cấu trúc bảng
DESCRIBE `path_1`.`Users`;
