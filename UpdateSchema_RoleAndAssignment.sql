-- ============================================
-- MIGRATION: THÊM PHÂN QUYỀN & GIAO VIỆC
-- Database: path_1
-- Version: 2.0
-- ============================================

-- Bước 1: Backup data hiện tại (optional - chạy trước nếu cần)
-- CREATE TABLE Jobs_backup AS SELECT * FROM Jobs;
-- CREATE TABLE Users_backup AS SELECT * FROM Users;

-- ============================================
-- STEP 1: CẬP NHẬT BẢNG USERS - THÊM ROLE
-- ============================================

-- Thêm cột Role
ALTER TABLE `path_1`.`Users`
ADD COLUMN `Role` VARCHAR(20) NOT NULL DEFAULT 'User' 
COMMENT 'Vai trò: Admin hoặc User'
AFTER `Email`;

-- Thêm index cho Role
ALTER TABLE `path_1`.`Users`
ADD INDEX `idx_role` (`Role`);

-- Set user đầu tiên làm Admin (giả sử Id=1 là admin)
-- Nếu bạn có username khác, thay đổi WHERE clause
UPDATE `path_1`.`Users` 
SET `Role` = 'Admin' 
WHERE `Id` = 1;

-- Verify
SELECT Id, Username, Email, Role FROM Users;

-- ============================================
-- STEP 2: CẬP NHẬT BẢNG JOBS - THÊM ASSIGNMENT
-- ============================================

-- Thêm cột CreatedBy (người tạo việc - Admin)
ALTER TABLE `path_1`.`Jobs`
ADD COLUMN `CreatedBy` INT NOT NULL DEFAULT 0
COMMENT 'UserId của người tạo công việc (Admin)'
AFTER `UserId`;

-- Thêm cột AssignedTo (người được giao việc)
ALTER TABLE `path_1`.`Jobs`
ADD COLUMN `AssignedTo` INT NULL
COMMENT 'UserId của người được giao việc'
AFTER `CreatedBy`;

-- Migrate data cũ: UserId -> CreatedBy
-- (Jobs cũ được coi như tự tạo cho bản thân)
UPDATE `path_1`.`Jobs` 
SET `CreatedBy` = `UserId`, 
    `AssignedTo` = `UserId`
WHERE `CreatedBy` = 0;

-- Thêm Foreign Keys
ALTER TABLE `path_1`.`Jobs`
ADD CONSTRAINT `FK_Jobs_CreatedBy` 
    FOREIGN KEY (`CreatedBy`) 
    REFERENCES `Users`(`Id`) 
    ON DELETE CASCADE,
ADD CONSTRAINT `FK_Jobs_AssignedTo` 
    FOREIGN KEY (`AssignedTo`) 
    REFERENCES `Users`(`Id`) 
    ON DELETE SET NULL;

-- Thêm indexes
ALTER TABLE `path_1`.`Jobs`
ADD INDEX `idx_created_by` (`CreatedBy`),
ADD INDEX `idx_assigned_to` (`AssignedTo`);

-- ============================================
-- STEP 3: XÓA CỘT UserId CŨ (OPTIONAL)
-- ============================================
-- Nếu muốn giữ lại cho tương thích ngược, comment dòng này
-- ALTER TABLE `path_1`.`Jobs` DROP COLUMN `UserId`;

-- ============================================
-- VERIFICATION
-- ============================================

-- Xem cấu trúc bảng Users
DESCRIBE `path_1`.`Users`;

-- Xem cấu trúc bảng Jobs
DESCRIBE `path_1`.`Jobs`;

-- Kiểm tra Admin account
SELECT Id, Username, Email, Role 
FROM `path_1`.`Users` 
WHERE Role = 'Admin';

-- Kiểm tra Jobs với thông tin người tạo và người được giao
SELECT j.Id, j.Name, 
       creator.Username AS 'Người tạo',
       assignee.Username AS 'Người nhận',
       j.Difficulty, j.CreatedAt
FROM `path_1`.`Jobs` j
LEFT JOIN `path_1`.`Users` creator ON j.CreatedBy = creator.Id
LEFT JOIN `path_1`.`Users` assignee ON j.AssignedTo = assignee.Id
ORDER BY j.CreatedAt DESC;

-- ============================================
-- TẠO DỮ LIỆU TEST (OPTIONAL)
-- ============================================

-- Tạo 2 user test nếu chưa có
INSERT INTO `path_1`.`Users` (Username, Email, PasswordHash, Salt, Role, CreatedAt)
VALUES 
    ('nhanvien1', 'nv1@test.com', 'dummy_hash', 'dummy_salt', 'User', NOW()),
    ('nhanvien2', 'nv2@test.com', 'dummy_hash', 'dummy_salt', 'User', NOW())
ON DUPLICATE KEY UPDATE Username = Username;

-- Tạo job test: Admin giao việc cho nhân viên
-- (Giả sử Admin Id=1, nhanvien1 Id=2)
-- INSERT INTO `path_1`.`Jobs` (CreatedBy, AssignedTo, Name, Difficulty, CreatedAt)
-- VALUES 
--     (1, 2, 'Làm báo cáo tháng 2', 'Medium', NOW()),
--     (1, 3, 'Review code module A', 'Hard', NOW());

SELECT 'Migration completed successfully!' AS Status;
