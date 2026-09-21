# Tài Liệu API Endpoints

## 1. Quản lý Phòng
- **GET /api/rooms**
  - *Mô tả:* Lấy danh sách tất cả các phòng.
  - *Query Params:* `status` (tùy chọn lọc theo trạng thái).
- **POST /api/rooms**
  - *Mô tả:* Thêm mới một phòng vào hệ thống (Dành cho Admin).

## 2. Quản lý Đặt phòng & Khách hàng
- **POST /api/bookings**
  - *Mô tả:* Tạo phiếu đặt phòng hoặc check-in cho khách mới.
- **GET /api/customers/{id}**
  - *Mô tả:* Lấy thông tin chi tiết hồ sơ của khách hàng theo ID.
