# HƯỚNG DẪN TEST HỆ THỐNG PHÂN QUYỀN

## 📋 CHUẨN BỊ

### Bước 1: Chạy SQL Migration

1. Mở MySQL Workbench
2. Connect vào database `path_1`
3. Mở file [`UpdateSchema_RoleAndAssignment.sql`](file:///d:/VSII/Learn_VSII/path_2/UpdateSchema_RoleAndAssignment.sql)
4. Chạy toàn bộ script

**Kết quả mong đợi:**
```sql
-- Kiểm tra bảng Users có cột Role
DESCRIBE path_1.Users;
-- Phải thấy: | Role | varchar(20) | NO | | User | |

-- Kiểm tra table Jobs có cột CreatedBy và AssignedTo
DESCRIBE path_1.Jobs;
-- Phải thấy: | CreatedBy | int | YES | MUL | NULL | |
--           | AssignedTo | int | YES | MUL | NULL | |

-- Xem admin account
SELECT Id, Username, Role FROM path_1.Users WHERE Role = 'Admin';
```

### Bước 2: Build Project

1. Mở Visual Studio
2. **Clean Solution**: Menu Build → Clean Solution
3. **Rebuild**: Nhấn `Ctrl + Shift + B`
4. Kiểm tra **Error List** (Ctrl + \, E) - Phải 0 errors

---

## ✅ TEST 1: ADMIN - ĐĂNG NHẬP

### Thực hiện:
1. Chạy app (F5)
2. Login với account đầu tiên (đã được set Role='Admin' trong SQL)
   ```
   Username: admin (hoặc username của user Id=1)
   Password: [password bạn đã tạo]
   ```

### Kết quả mong đợi:
✅ MessageBox hiển thị: "Đăng nhập thành công! Chào mừng admin (email@...) Vai trò: Admin"
✅ Mở **MainDashboardForm**
✅ Thấy 2 buttons:
   - 👥 Quản Lý Nhân Viên (màu xanh lá)
   - 📋 Quản Lý Công Việc (màu xanh dương)
✅ Label hiển thị: "Xin chào: admin" và "Vai trò: Admin"

---

## ✅ TEST 2: ADMIN - TẠO NHÂN VIÊN

### Thực hiện:
1. Tại Dashboard, click **"Quản Lý Nhân Viên"**
2. Thấy **UserManagementForm** mở ra

**Tạo nhân viên 1:**
```
Username:   nhanvien1
Email:      nv1@test.com  
Password:   123456
Role:       User
```
3. Click **"Thêm"**

**Tạo nhân viên 2:**
```
Username:   nhanvien2
Email:      nv2@test.com
Password:   123456
Role:       User
```
4. Click **"Thêm"**

### Kết quả mong đợi:
✅ MessageBox "Thêm nhân viên thành công!"
✅ DataGridView hiển thị 2 nhân viên vừa tạo
✅ Roles đều là "User"

---

## ✅ TEST 3: ADMIN - GIAO VIỆC CHO NHÂN VIÊN

### Thực hiện:
1. Click **"Quay lại"** → về Dashboard
2. Click **"Quản Lý Công Việc"**
3. Thấy **JobManagementForm** với ComboBox "Giao cho"

**Giao việc 1 cho nhanvien1:**
```
Tên công việc:  Làm báo cáo tháng 2
Độ khó:         Medium
Giao cho:       nhanvien1 (chọn từ dropdown)
```
4. Click **"Thêm"**

**Giao việc 2 cho nhanvien2:**
```
Tên công việc:  Review code module Auth
Độ khó:         Hard
Giao cho:       nhanvien2
```
5. Click **"Thêm"**

**Giao việc 3 cho nhanvien1:**
```
Tên công việc:  Fix bug login form
Độ khó:         Easy
Giao cho:       nhanvien1
```
6. Click **"Thêm"**

### Kết quả mong đợi:
✅ DataGridView hiển thị 3 công việc
✅ Các cột hiển thị:
   - ID
   - Tên Công Việc
   - Độ Khó
   - Ngày Tạo
   - **Người tạo**: admin
   - **Người nhận**: nhanvien1 / nhanvien2

---

## ✅ TEST 4: USER - ĐĂNG NHẬP VÀ XEM CÔNG VIỆC

### Thực hiện:
1. Click **"Quay lại"** → Dashboard
2. Click **"Đăng xuất"**
3. Confirm "Có"

**Đăng nhập lại với nhanvien1:**
```
Username: nhanvien1
Password: 123456
```

### Kết quả mong đợi Dashboard (User):
✅ Dashboard chỉ hiển thị 1 button: "Công Việc Của Tôi"
✅ **KHÔNG** thấy button "Quản Lý Nhân Viên"
✅ Label: "Xin chào: nhanvien1" và "Vai trò: User"

### Thực hiện tiếp:
4. Click **"Công Việc Của Tôi"**

### Kết quả mong đợi JobManagementForm (User):
✅ DataGridView chỉ hiển thị **2 công việc** được giao cho nhanvien1:
   1. Làm báo cáo tháng 2
   2. Fix bug login form
✅ **KHÔNG** thấy job "Review code module Auth" (của nhanvien2)
✅ Các cột hiển thị:
   - ID, Tên Công Việc, Độ Khó, Ngày Tạo
   - **Người giao**: admin
✅ ComboBox "Giao cho" **KHÔNG hiển thị** (Visible = false)
✅ Buttons Thêm/Sửa/Xóa **BỊ DISABLED** (Enabled = false)

---

## ✅ TEST 5: USER - KHÔNG VÀO ĐƯỢC QUẢN LÝ NHÂN VIÊN

### Thực hiện:
1. Logout nhanvien1
2. Login nhanvien2 (password: 123456)
3. Dashboard chỉ thấy "Công Việc Của Tôi"
4. Click vào

### Kết quả mong đợi:
✅ Chỉ thấy 1 công việc: "Review code module Auth"
✅ Không thể thêm/sửa/xóa

---

## ✅ TEST 6: ADMIN - SỬA VÀ XÓA CÔNG VIỆC

### Thực hiện:
1. Logout nhanvien2
2. Login admin
3. Dashboard → "Quản Lý Công Việc"
4. Click chọn job "Fix bug login form"
5. Form tự động điền thông tin
6. Sửa:
   ```
   Tên: Fix bug login form - URGENT
   Độ khó: Hard
   Giao cho: nhanvien2 (đổi người)
   ```
7. Click **"Sửa"**

### Kết quả mong đợi:
✅ MessageBox "Cập nhật công việc thành công!"
✅ DataGridView refresh - job đã đổi thông tin
✅ "Người nhận" bây giờ là nhanvien2

### Thực hiện tiếp - Xóa:
8. Click chọn job vừa sửa
9. Click **"Xóa"**
10. Confirm "Yes"

### Kết quả mong đợi:
✅ MessageBox "Xóa công việc thành công!"
✅ Job biến mất khỏi danh sách

---

## ✅ TEST 7: ADMIN - XÓA NHÂN VIÊN

### Thực hiện:
1. Quay Dashboard → "Quản Lý Nhân Viên"
2. Click chọn nhanvien2
3. Click **"Xóa"**
4. Confirm "Yes"

### Kết quả mong đợi:
✅ MessageBox "Xóa nhân viên thành công!"
✅ nhanvien2 biến mất
✅ **Tất cả jobs được giao cho nhanvien2 cũng bị xóa** (CASCADE delete)

---

## ✅ TEST 8: KIỂM TRA DATABASE

```sql
-- Xem tất cả users và roles
SELECT Id, Username, Email, Role, CreatedAt 
FROM path_1.Users 
ORDER BY Id;

-- Xem jobs với info người tạo và người nhận
SELECT j.Id, j.Name, j.Difficulty,
       creator.Username AS 'Admin tạo',
       IFNULL(assignee.Username, '(Đã xóa)') AS 'Nhân viên nhận',
       j.CreatedAt
FROM path_1.Jobs j
LEFT JOIN path_1.Users creator ON j.CreatedBy = creator.Id
LEFT JOIN path_1.Users assignee ON j.AssignedTo = assignee.Id
ORDER BY j.CreatedAt DESC;
```

---

## 🎯 CHECKLIST TỔNGứng

| Test Case | Kỳ vọng | Trạng thái |
|-----------|---------|------------|
| SQL Migration thành công | ✅ Cột Role, CreatedBy, AssignedTo được tạo | ⬜ |
| Admin login → Dashboard | ✅ Thấy 2 buttons | ⬜ |
| Admin tạo User | ✅ User xuất hiện trong list | ⬜ |
| Admin giao việc | ✅ Job có "Người nhận" |  |
| User login → Dashboard | ✅ Chỉ 1 button | ⬜ |
| User xem jobs | ✅ Chỉ thấy job của mình | ⬜ |
| User không Add/Edit/Delete | ✅ Buttons disabled | ⬜ |
| Admin sửa job | ✅ Cập nhật thành công | ⬜ |
| Admin xóa job | ✅ Xóa thành công | ⬜ |
| Admin xóa User | ✅ User + jobs bị xóa | ⬜ |

---

## 🐛 TROUBLESHOOTING

### Lỗi: "Column 'Role' not found"
➡️ Chưa chạy SQL migration → Chạy lại UpdateSchema_RoleAndAssignment.sql

### Lỗi: "MainDashboardForm không tồn tại"
➡️ Rebuild project (Ctrl + Shift + B)

### Lỗi Build: "InitializeComponent không có lblAssignTo"
➡️ Delete bin/ và obj/ folders → Rebuild

### Admin không thấy ComboBox "Giao cho"
➡️ Kiểm tra `CurrentUser.Role` có đúng là "Admin" không (thêm breakpoint)

### User vẫn thấy tất cả jobs
➡️ Kiểm tra query trong `LoadJobs()` - Phải có WHERE clause cho User

---

**Sau khi test xong, báo kết quả cho tôi nhé!** 🚀
