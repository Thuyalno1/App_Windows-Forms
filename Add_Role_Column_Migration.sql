-- ============================================
-- MIGRATION: Thêm cột Role vào bảng Users
-- Database: path_1
-- ============================================

USE `path_1`;

-- Kiểm tra xem cột Role đã tồn tại chưa
SELECT COUNT(*) AS RoleColumnExists
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'path_1'
  AND TABLE_NAME = 'Users'
  AND COLUMN_NAME = 'Role';

-- Thêm cột Role nếu chưa tồn tại
ALTER TABLE `Users` 
ADD COLUMN IF NOT EXISTS `Role` VARCHAR(50) NOT NULL DEFAULT 'User' 
COMMENT 'Vai trò: Admin, Manager, User'
AFTER `Salt`;

-- Thêm index cho cột Role
ALTER TABLE `Users`
ADD INDEX IF NOT EXISTS `IDX_Role` (`Role`);

-- Cập nhật Role cho các user hiện có (nếu có)
-- User đầu tiên (Id = 1) sẽ là Admin
UPDATE `Users` 
SET `Role` = 'Admin' 
WHERE `Id` = 1 AND `Role` = 'User';

-- Kiểm tra kết quả
SELECT 'Đã thêm cột Role thành công!' AS Status;

-- Xem lại cấu trúc bảng
DESCRIBE `Users`;

-- Xem dữ liệu
SELECT Id, Username, Email, Role, CreatedAt FROM `Users`;
