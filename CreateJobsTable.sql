-- ============================================
-- SCRIPT TẠO BẢNG JOBS - HỆ THỐNG QUẢN LÝ CÔNG VIỆC
-- Database: path_1
-- ============================================

-- Xóa bảng cũ nếu tồn tại (cẩn thận - sẽ mất dữ liệu!)
DROP TABLE IF EXISTS `path_1`.`Jobs`;

-- Tạo bảng Jobs
CREATE TABLE `path_1`.`Jobs` (
    `Id` INT NOT NULL AUTO_INCREMENT COMMENT 'ID tự động tăng',
    `UserId` INT NOT NULL COMMENT 'ID của user tạo job (FK tới Users.Id)',
    `Name` VARCHAR(200) NOT NULL COMMENT 'Tên công việc',
    `Difficulty` VARCHAR(20) NOT NULL COMMENT 'Mức độ khó: Easy, Medium, Hard',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo',
    
    PRIMARY KEY (`Id`),
    
    -- Foreign key tới bảng Users
    CONSTRAINT `FK_Jobs_Users` 
        FOREIGN KEY (`UserId`) 
        REFERENCES `Users`(`Id`) 
        ON DELETE CASCADE,  -- Xóa user thì xóa luôn jobs của user đó
    
    -- Index để tăng tốc query theo UserId
    INDEX `IDX_Jobs_UserId` (`UserId`),
    INDEX `IDX_Jobs_Difficulty` (`Difficulty`)
    
) ENGINE=InnoDB 
  DEFAULT CHARSET=utf8mb4 
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Bảng lưu công việc của users';

-- Kiểm tra bảng đã tạo thành công
SELECT 'Bảng Jobs đã được tạo thành công!' AS Status;

-- Xem cấu trúc bảng
DESCRIBE `path_1`.`Jobs`;

-- Query test: Xem jobs của user cụ thể
-- SELECT * FROM Jobs WHERE UserId = 1;
