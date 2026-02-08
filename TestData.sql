-- ============================================
-- SCRIPT THÊM DỮ LIỆU DEMO CHO TESTING
-- ============================================

-- Tài khoản demo 1
-- Username: admin
-- Password: admin123
-- Salt: demo_salt_12345
-- Hash: đã được tính sẵn với SHA256

INSERT INTO `path_1`.`Users` 
    (`Username`, `Email`, `PasswordHash`, `Salt`, `CreatedAt`, `LastLogin`)
VALUES
    ('demo_user', 
     'demo@example.com', 
     'HASH_PLACEHOLDER', -- Sẽ được generate khi đăng ký qua form
     'SALT_PLACEHOLDER', 
     NOW(), 
     NULL);

-- Xem dữ liệu đã thêm
SELECT * FROM `path_1`.`Users`;

-- ============================================
-- LƯU Ý: Để test, hãy dùng RegisterForm để đăng ký
-- Vì password cần được hash đúng cách bởi PasswordHelper.cs
-- ============================================

-- Xóa tất cả users (nếu muốn reset)
-- DELETE FROM `path_1`.`Users`;

-- Xóa 1 user cụ thể
-- DELETE FROM `path_1`.`Users` WHERE Username = 'demo_user';

-- Reset Auto Increment
-- ALTER TABLE `path_1`.`Users` AUTO_INCREMENT = 1;
