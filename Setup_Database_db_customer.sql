-- ============================================
-- SCRIPT SETUP DATABASE CHO ỨNG DỤNG PATH_2
-- Database: db_customer
-- ============================================

-- Tạo database nếu chưa tồn tại
CREATE DATABASE IF NOT EXISTS `path_1` 
    DEFAULT CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

USE `path_1`;

-- ============================================
-- TẠO BẢNG USERS
-- ============================================

-- Xóa bảng cũ nếu tồn tại (cẩn thận - sẽ mất dữ liệu!)
DROP TABLE IF EXISTS `Users`;

-- Tạo bảng Users
CREATE TABLE `Users` (
    `Id` INT NOT NULL AUTO_INCREMENT COMMENT 'ID tự động tăng',
    `Username` VARCHAR(100) NOT NULL COMMENT 'Tên đăng nhập (unique)',
    `Email` VARCHAR(150) NOT NULL COMMENT 'Email (unique)',
    `PasswordHash` VARCHAR(255) NOT NULL COMMENT 'Mật khẩu đã hash (SHA256)',
    `Salt` VARCHAR(255) NOT NULL COMMENT 'Salt để hash password',
    `Role` VARCHAR(50) NOT NULL DEFAULT 'User' COMMENT 'Vai trò: Admin, Manager, User',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo tài khoản',
    `LastLogin` DATETIME NULL COMMENT 'Lần đăng nhập cuối',
    
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UQ_Users_Username` (`Username`),
    UNIQUE KEY `UQ_Users_Email` (`Email`),
    
    INDEX `IDX_Username` (`Username`),
    INDEX `IDX_Email` (`Email`),
    INDEX `IDX_Role` (`Role`)
) ENGINE=InnoDB 
  DEFAULT CHARSET=utf8mb4 
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Bảng lưu thông tin user';

-- ============================================
-- TẠO BẢNG JOBS (Công việc)
-- ============================================

DROP TABLE IF EXISTS `Jobs`;

CREATE TABLE `Jobs` (
    `Id` INT NOT NULL AUTO_INCREMENT COMMENT 'ID công việc',
    `Title` VARCHAR(200) NOT NULL COMMENT 'Tiêu đề công việc',
    `Description` TEXT NULL COMMENT 'Mô tả chi tiết',
    `AssignedTo` INT NULL COMMENT 'User được gán (FK to Users.Id)',
    `Status` VARCHAR(50) NOT NULL DEFAULT 'Pending' COMMENT 'Trạng thái: Pending, InProgress, Completed',
    `Priority` VARCHAR(20) NOT NULL DEFAULT 'Medium' COMMENT 'Ưu tiên: Low, Medium, High',
    `DueDate` DATE NULL COMMENT 'Hạn hoàn thành',
    `CreatedBy` INT NOT NULL COMMENT 'Người tạo (FK to Users.Id)',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo',
    `UpdatedAt` DATETIME NULL ON UPDATE CURRENT_TIMESTAMP COMMENT 'Ngày cập nhật',
    
    PRIMARY KEY (`Id`),
    
    FOREIGN KEY (`AssignedTo`) REFERENCES `Users`(`Id`) 
        ON DELETE SET NULL 
        ON UPDATE CASCADE,
    FOREIGN KEY (`CreatedBy`) REFERENCES `Users`(`Id`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE,
        
    INDEX `IDX_AssignedTo` (`AssignedTo`),
    INDEX `IDX_Status` (`Status`),
    INDEX `IDX_CreatedBy` (`CreatedBy`)
) ENGINE=InnoDB 
  DEFAULT CHARSET=utf8mb4 
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Bảng quản lý công việc';

-- ============================================
-- TẠO TÀI KHOẢN ADMIN MẶC ĐỊNH
-- ============================================

-- Tạo admin account
-- Username: admin
-- Password: admin123
-- Salt: fixed_salt_for_admin_account_only
-- PasswordHash: SHA256(admin123 + salt)
INSERT INTO `Users` (`Username`, `Email`, `PasswordHash`, `Salt`, `Role`, `CreatedAt`)
VALUES (
    'admin',
    'admin@example.com',
    'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855',
    'fixed_salt_for_admin_account_only',
    'Admin',
    NOW()
);

-- ============================================
-- KIỂM TRA KẾT QUẢ
-- ============================================

SELECT 'Database setup completed successfully!' AS Status;

-- Xem cấu trúc các bảng
SHOW TABLES;

SELECT * FROM Users;
