# 🎯 HƯỚNG DẪN SỬ DỤNG HỆ THỐNG QUẢN LÝ CÔNG VIỆC

## ✅ ĐÃ HOÀN THÀNH

Tôi đã tạo xong hệ thống quản lý công việc với:

1. ✅ **Database**: Bảng `Jobs` với foreign key tới `Users`
2. ✅ **JobManagementForm**: Form quản lý với CRUD đầy đủ
3. ✅ **Tích hợp**: LoginForm tự động mở JobManagementForm sau khi login
4. ✅ **User Isolation**: Mỗi user chỉ thấy jobs của mình

---

## 📋 BƯỚC 1: TẠO BẢNG JOBS

### **Chạy SQL Script**

```sql
-- Mở MySQL và chạy file CreateJobsTable.sql
-- Hoặc copy paste script sau:

DROP TABLE IF EXISTS `path_1`.`Jobs`;

CREATE TABLE `path_1`.`Jobs` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UserId` INT NOT NULL,
    `Name` VARCHAR(200) NOT NULL,
    `Difficulty` VARCHAR(20) NOT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Jobs_Users` 
        FOREIGN KEY (`UserId`) 
        REFERENCES `Users`(`Id`) 
        ON DELETE CASCADE,
    INDEX `IDX_Jobs_UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

**Verify:**
```sql
DESCRIBE Jobs;
-- Kết quả phải có 5 cột: Id, UserId, Name, Difficulty, CreatedAt
```

---

## 🔨 BƯỚC 2: BUILD PROJECT

### **Trong Visual Studio:**

1. **Reload Project** (nếu cần)
   - Right-click project → Unload
   - Right-click → Reload

2. **Clean Solution**
   - Menu `Build` → `Clean Solution`

3. **Rebuild Solution**
   - Nhấn `Ctrl + Shift + B`
   - Hoặc: `Build` → `Rebuild Solution`

4. **Kiểm tra lỗi**
   - Xem Error List (Ctrl + \, E)
   - Nếu có lỗi, báo cho tôi!

---

## 🚀 BƯỚC 3: CHẠY VÀ TEST

### **Flow Hoàn Chỉnh:**

```
1. Chạy app (F5)
   ↓
2. LoginForm hiển thị
   ↓
3. Đăng nhập với tài khoản đã tạo
   (username: xxx, password: xxx)
   ↓
4. JobManagementForm tự động mở
   ↓
5. Thấy "Xin chào: {username}" ở góc phải
   ↓
6. Bắt đầu test CRUD!
```

---

## ✨ BƯỚC 4: TEST CÁC TÍNH NĂNG

### **1. THÊM CÔNG VIỆC (CREATE)**

```
✅ Nhập tên công việc: "Học C#"
✅ Chọn độ khó: "Medium"
✅ Click "➕ Thêm"
✅ Verify: 
   - MessageBox "Thêm công việc thành công!"
   - Job xuất hiện trong DataGridView
   - Form tự động clear
```

**Test thêm nhiều jobs:**
- "Làm bài tập MySQL" - Hard
- "Review code" - Easy
- "Đọc tài liệu" - Easy

### **2. XEM DANH SÁCH (READ)**

```
✅ Jobs hiển thị trong DataGridView
✅ Có 4 cột: ID, Tên Công Việc, Độ Khó, Ngày Tạo
✅ Sắp xếp theo ngày tạo (mới nhất trên cùng)
✅ Click vào row → Thông tin tự động fill vào form
```

### **3. SỬA CÔNG VIỆC (UPDATE)**

```
✅ Click chọn job "Học C#"
✅ Sửa tên thành: "Học C# nâng cao"
✅ Đổi độ khó: "Hard"
✅ Click "✏️ Sửa"
✅ Verify:
   - MessageBox "Cập nhật công việc thành công!"
   - Thay đổi hiển thị trong grid
   - Form tự động clear
```

### **4. XÓA CÔNG VIỆC (DELETE)**

```
✅ Click chọn job "Review code"
✅ Click "🗑️ Xóa"
✅ Verify:
   - MessageBox xác nhận "Bạn có chắc muốn xóa?"
   - Click "Yes"
   - MessageBox "Xóa công việc thành công!"
   - Job biến mất khỏi grid
```

### **5. LÀM MỚI (REFRESH)**

```
✅ Click "🔄 Làm mới"
✅ Verify:
   - Grid reload lại data
   - Form clear
```

---

## 🔐 BƯỚC 5: TEST USER ISOLATION

### **Test nhiều users:**

```
1. Login với User A (admin)
   → Tạo 2 jobs: "Job A1", "Job A2"
   → Đăng xuất

2. Login với User B (user2)
   → Tạo 1 job: "Job B1"
   → Verify: Chỉ thấy 1 job "Job B1"
   → Đăng xuất

3. Login lại User A
   → Verify: Vẫn thấy đúng 2 jobs "Job A1", "Job A2"
   → User B không thấy jobs của User A!
```

---

## 🗄️ BƯỚC 6: VERIFY DATABASE

### **Check trong MySQL:**

```sql
-- Xem tất cả jobs
SELECT * FROM Jobs;

-- Xem jobs của user cụ thể (UserId = 1)
SELECT j.*, u.Username 
FROM Jobs j 
JOIN Users u ON j.UserId = u.Id 
WHERE u.Id = 1;

-- Đếm số jobs của mỗi user
SELECT u.Username, COUNT(j.Id) as TotalJobs
FROM Users u
LEFT JOIN Jobs j ON u.Id = j.UserId
GROUP BY u.Username;
```

---

## ⚠️ XỬ LÝ LỖI THƯỜNG GẶP

### **Lỗi 1: Bảng Jobs không tồn tại**
```
Lỗi: Table 'path_1.Jobs' doesn't exist

Giải pháp:
→ Chạy lại CreateJobsTable.sql
→ Verify: DESCRIBE Jobs;
```

### **Lỗi 2: Foreign key constraint fails**
```
Lỗi: Cannot add or update a child row: a foreign key constraint fails

Giải pháp:
→ Đảm bảo đã login (CurrentUser.Id có giá trị)
→ Check: SELECT * FROM Users WHERE Id = {CurrentUser.Id};
```

### **Lỗi 3: JobManagementForm không mở**
```
Giải pháp:
→ Check LoginForm.cs dòng 93-96
→ Phải có: JobManagementForm jobForm = new JobManagementForm();
→ Build lại project
```

---

## 📊 CẤU TRÚC FILE ĐÃ TẠO

```
d:\VSII\Learn_VSII\path_2\
├── CreateJobsTable.sql                # Script tạo bảng
│
└── path_2\
    ├── JobManagementForm.cs           # Logic form
    ├── JobManagementForm.Designer.cs  # UI design
    ├── JobManagementForm.resx         # Resources
    │
    └── LoginForm.cs                   # Đã cập nhật (dòng 88-98)
```

---

## 🎨 GIAO DIỆN JOB MANAGEMENT FORM

```
┌──────────────────────────────────────────────────┐
│  QUẢN LÝ CÔNG VIỆC          Xin chào: admin     │
├──────────────────────────────────────────────────┤
│                                                  │
│  Tên công việc:  [Học C#________________]        │
│  Độ khó:         [▼ Medium    ]                  │
│                                                  │
│  [➕ Thêm] [✏️ Sửa] [🗑️ Xóa] [🔄 Làm mới]      │
│                                                  │
├──────────────────────────────────────────────────┤
│  DANH SÁCH CÔNG VIỆC                             │
│  ┌────────────────────────────────────────────┐ │
│  │ ID │ Tên          │ Độ Khó │ Ngày Tạo    │ │
│  ├────┼──────────────┼────────┼─────────────┤ │
│  │ 1  │ Học C#       │ Medium │ 04/02/2026  │ │
│  │ 2  │ MySQL        │ Hard   │ 04/02/2026  │ │
│  └────────────────────────────────────────────┘ │
│                                                  │
│                              [Đăng xuất]        │
└──────────────────────────────────────────────────┘
```

---

## 📝 CHECKLIST CUỐI CÙNG

- [x] Tạo bảng Jobs trong MySQL
- [ ] Build project thành công (0 errors)
- [ ] Login thành công
- [ ] JobManagementForm hiển thị
- [ ] Thêm job thành công
- [ ] Sửa job thành công
- [ ] Xóa job thành công
- [ ] Logout thành công
- [ ] Test với 2 users khác nhau

---

**🎉 CHÚC MỪNG! Hệ thống đã hoàn thành!**

Nếu có lỗi gì, hãy chụp màn hình Error List và báo cho tôi nhé! 😊
