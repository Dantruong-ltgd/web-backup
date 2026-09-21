# Tài Liệu Thiết Kế Cơ Sở Dữ Liệu (Database Schema)

## 1. Sơ đồ thực thể quan hệ (ERD Overview)
Hệ thống sử dụng cơ sở dữ liệu quan hệ với các bảng chính phục vụ quản lý khách sạn.

## 2. Chi tiết các bảng (Tables)

### Bảng: `Phong` (Rooms)
- `id` (INT, Primary Key)
- `so_phong` (VARCHAR, Unique)
- `loai_phong` (VARCHAR)
- `gia_phong` (DECIMAL)
- `trang_thai` (VARCHAR: 'Trong', 'DangThuê', 'DangDonDap')

### Bảng: `KhachHang` (Customers)
- `id` (INT, Primary Key)
- `ho_ten` (VARCHAR)
- `cccd` (VARCHAR, Unique)
- `so_dien_thoai` (VARCHAR)

### Bảng: `DatPhong` (Bookings)
- `id` (INT, Primary Key)
- `phong_id` (INT, Foreign Key -> Phong.id)
- `khach_hang_id` (INT, Foreign Key -> KhachHang.id)
- `ngay_checkin` (DATETIME)
- `ngay_checkout` (DATETIME)
- `trang_thai` (VARCHAR)
