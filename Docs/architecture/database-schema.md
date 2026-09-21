# Tài Liệu Thiết Kế Cơ Sở Dữ Liệu (Database Schema)

## 1. Sơ đồ thực thể quan hệ (ERD Overview)
Hệ thống quản lý thuê phòng sử dụng cơ sở dữ liệu quan hệ (Relational Database) để lưu trữ và quản lý thông tin phòng, hồ sơ khách hàng, phiếu đặt phòng cũng như hóa đơn thanh toán dịch vụ.

## 2. Chi tiết các bảng (Tables)

### Bảng: `Phong` (Rooms) - Phục vụ QUANLY-1
- `id` (INT, Primary Key, Auto Increment)
- `so_phong` (VARCHAR(20), Unique, Not Null) - Mã số phòng (Ví dụ: P.101, P.202)
- `loai_phong` (VARCHAR(50)) - Loại phòng (Phòng đơn, phòng đôi, VIP)
- `gia_phong` (DECIMAL(10,2)) - Đơn giá tính theo ngày/giờ
- `trang_thai` (VARCHAR(30)) - Trạng thái phòng: `'Trong'` (Trống), `'DangThuê'` (Đang thuê), `'DangDonDap'` (Đang dọn dẹp)

### Bảng: `KhachHang` (Customers) - Phục vụ QUANLY-2
- `id` (INT, Primary Key, Auto Increment)
- `ho_ten` (VARCHAR(100), Not Null) - Họ và tên khách hàng
- `cccd` (VARCHAR(20), Unique, Not Null) - Số căn cước công dân / Hộ chiếu
- `so_dien_thoai` (VARCHAR(15), Not Null) - Số điện thoại liên lạc

### Bảng: `DatPhong` (Bookings) - Phục vụ QUANLY-2
- `id` (INT, Primary Key, Auto Increment)
- `phong_id` (INT, Foreign Key -> Phong.id)
- `khach_hang_id` (INT, Foreign Key -> KhachHang.id)
- `ngay_checkin` (DATETIME, Not Null) - Thời điểm nhận phòng thực tế
- `ngay_checkout` (DATETIME) - Thời điểm trả phòng dự kiến / thực tế
- `trang_thai_dat` (VARCHAR(30)) - `'DangDat'` (Đã đặt trước), `'DangO'` (Đang lưu trú), `'DaTraPhong'` (Đã hoàn tất)

### Bảng: `HoaDon` (Invoices) - Phục vụ QUANLY-3
- `id` (INT, Primary Key, Auto Increment)
- `dat_phong_id` (INT, Foreign Key -> DatPhong.id)
- `tong_tien_phong` (DECIMAL(10,2))
- `chi_phi_dich_vu` (DECIMAL(10,2)) - Tiền điện, nước, dịch vụ phát sinh
- `tong_thanh_toan` (DECIMAL(10,2)) - Tổng số tiền khách cần thanh toán
- `ngay_tao_hoa_don` (DATETIME)
