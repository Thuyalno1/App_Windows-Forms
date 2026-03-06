# TÀI LIỆU PHÂN TÍCH HỆ THỐNG
## Hệ Thống Quản Lý Công Việc - Windows Forms Application

---

## 1. TỔNG QUAN HỆ THỐNG

### 1.1. Thông Tin Dự Án
- **Tên dự án**: path_2 (Hệ thống quản lý công việc)
- **Công nghệ**: Windows Forms Application (.NET Framework 4.7.2)
- **Ngôn ngữ**: C#
- **Database**: MySQL 
- **Kết nối Database**: ODBC Driver
- **IDE**: Visual Studio 2022

### 1.2. Mục Đích
Xây dựng ứng dụng desktop quản lý công việc với phân quyền Admin/User, cho phép:
- Quản lý nhân viên (Admin)
- Giao việc cho nhân viên (Admin)
- Theo dõi công việc được giao (User)
- Xác thực và phân quyền người dùng

### 1.3. Đặc Điểm Chính
- **Bảo mật**: Mã hóa mật khẩu bằng SHA-256 với Salt
- **Phân quyền**: Role-based Access Control (Admin/User)
- **Kiến trúc**: 3-layer Architecture (Presentation, Business Logic, Data Access)
- **Database**: Sử dụng ODBC DSN để kết nối MySQL

---

## 2. KIẾN TRÚC HỆ THỐNG

### 2.1. Công Nghệ Sử Dụng

#### Backend
- **.NET Framework**: 4.7.2
- **Platform**: x64 (Windows Desktop)
- **UI Framework**: Windows Forms

#### Database
- **RDBMS**: MySQL 
- **Driver**: ODBC (MySQL_PATH1 DSN)
- **Database Name**: `path_1`

---

## 3. PHÂN QUYỀN VÀ QUAN HỆ DỮ LIỆU

### 3.1. Use Case Diagram

```
                    ┌─────────────┐
                    │   System    │
    ┌───────────────┤   Actor     ├───────────────┐
    │               └─────────────┘               │
    │                                             │
    ▼                                             ▼
┌────────┐                                   ┌────────┐
│ Admin  │                                   │  User  │
└────┬───┘                                   └───┬────┘
     │                                           │
     ├─ Đăng nhập                                ├─ Đăng nhập
     ├─ Đăng ký                                  ├─ Đăng ký
     ├─ Quản lý nhân viên                        ├─ Xem công việc
     │   ├─ Thêm nhân viên                       └─ Đăng xuất
     │   ├─ Sửa nhân viên
     │   └─ Xóa nhân viên
     ├─ Quản lý công việc
     │   ├─ Giao việc
     │   ├─ Sửa công việc
     │   ├─ Xóa công việc
     │   └─ Xem tất cả công việc
     └─ Đăng xuất
```

---

### 3.2. Quan hệ User - Job
- 1 User tạo nhiều Job
- 1 Job chỉ có 1 người tạo

### Admin View
- Giao việc cho nhân viên
- Sửa/Xóa công việc đã tạo
- Xem TẤT CẢ công việc trong hệ thống

### User View
- Xem công việc được giao cho MÌNH

---

## 4. LUỒNG NGHIỆP VỤ CHI TIẾT

### 4.1. Flow: Đăng Nhập (DangNhap)

```
User nhập username/password
↓
ValidateInput()
↓
LoginUser()
↓
Query database: SELECT Id, Username, Email, PasswordHash, Salt, Role
↓
PasswordHelper.VerifyPassword(password, storedHash, storedSalt)
↓
[Thành công] → Lưu CurrentUser → UpdateLastLogin → Mở MainDashboard
[Thất bại] → Hiển thị lỗi
```

**Hình 1. Đăng nhập thành công**

---

### 4.2. Flow: Đăng Ký (DangKy)

```
User nhập thông tin
↓
ValidateInput()
↓
Check duplicate: SELECT COUNT(*) WHERE Username = ? OR Email = ?
↓
GenerateSalt() + HashPassword()
↓
INSERT INTO Users
↓
Thông báo thành công
```

---

### 4.3. Flow: User Xem Công Việc

```
START (MainDashboard)
↓
Click "Công Việc Của Tôi"
↓
[QuanLyCongViec Form]
↓
ConfigureForRole() → User mode
├─ Hide combobox "Chọn nhân viên"
└─ Disable Add/Edit/Delete buttons
↓
LoadJobs()
├─ Query: WHERE AssignedTo = CurrentUser.Id
└─ Display in DataGridView
    ├─ Tên công việc
    ├─ Độ khó
    ├─ Ngày tạo
    └─ Người giao
↓
User chỉ xem, không thao tác được
↓
Click "Về Dashboard"
↓
END
```

---

### 4.4. Flow: Admin Giao Việc

```
START (MainDashboard)
↓
Click "Quản Lý Công Việc"
↓
[QuanLyCongViec Form]
↓
ConfigureForRole() → Admin mode
├─ Show combobox "Chọn nhân viên"
└─ Load danh sách users vào combobox
↓
LoadJobs()
├─ Query: SELECT ALL Jobs với JOIN Users
└─ Display in DataGridView
    ├─ Id
    ├─ Tên công việc
    ├─ Độ khó
    ├─ Ngày tạo
    ├─ Người tạo
    └─ Người nhận
↓
Admin nhập thông tin công việc
├─ Tên công việc (TextBox)
├─ Độ khó: Easy/Medium/Hard (ComboBox)
└─ Chọn nhân viên nhận việc (ComboBox)
↓
Click "Thêm"
↓
ValidateInput()
├─ [Tên rỗng?] → Show "Vui lòng nhập tên công việc!" → BACK
├─ [Độ khó rỗng?] → Show "Vui lòng chọn độ khó!" → BACK
├─ [Chưa chọn nhân viên?] → Show "Vui lòng chọn nhân viên!" → BACK
└─ [OK] → Continue
↓
Chuẩn bị dữ liệu
↓
INSERT INTO Jobs (UserId, CreatedBy, AssignedTo, Name, Difficulty, CreatedAt)
VALUES (?, ?, ?, ?, ?, ?)
├─ [Lỗi DB?] → Show lỗi chi tiết → BACK
└─ [Thành công] → Continue
↓
LoadJobs() - Refresh DataGridView
↓
Clear inputs (reset form)
↓
Show "Giao việc thành công!"
↓
[Tiếp tục giao việc?]
├─ [Có] → BACK to "Admin nhập thông tin"
└─ [Không] → Click "Về Dashboard"
↓
END
```

---

## 5. KẾT LUẬN

Tài liệu này mô tả chi tiết kiến trúc và luồng nghiệp vụ của hệ thống quản lý công việc. Hệ thống được thiết kế với:
- Kiến trúc 3 lớp rõ ràng
- Bảo mật cao với mã hóa mật khẩu SHA-256
- Phân quyền Admin/User chặt chẽ
- Giao diện thân thiện với Windows Forms
- Kết nối database ổn định qua ODBC

---

**Ngày tạo**: 2026-02-10  
**Phiên bản**: 1.0
