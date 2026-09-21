# Tài Liệu API Endpoints (Hệ Thống Quản Lý Thuê Phòng)

## 1. Module Quản lý Phòng (`QUANLY-1`)
- **GET /api/rooms**
  - *Mô tả:* Lấy danh sách toàn bộ các phòng trong hệ thống.
  - *Query Parameters (Tùy chọn):* `status` (Lọc theo trạng thái như `Trong`, `DangThuê`, `DangDonDap`).
- **POST /api/rooms**
  - *Mô tả:* Thêm mới một phòng vào hệ thống (Dành cho tài khoản Quản trị viên).
- **PUT /api/rooms/{id}**
  - *Mô tả:* Cập nhật thông tin chi tiết hoặc trạng thái của phòng.
- **DELETE /api/rooms/{id}**
  - *Mô tả:* Xóa phòng khỏi hệ thống (Yêu cầu xác nhận).

## 2. Module Quản lý Khách hàng & Đặt phòng (`QUANLY-2`)
- **POST /api/bookings**
  - *Mô tả:* Tạo phiếu đặt phòng mới hoặc thực hiện thủ tục check-in trực tiếp cho khách.
- **GET /api/customers/{id}**
  - *Mô tả:* Truy xuất thông tin hồ sơ chi tiết và lịch sử thuê phòng của khách hàng thông qua ID hoặc số CCCD.

## 3. Module Tính tiền & Thống kê (`QUANLY-3`)
- **POST /api/invoices/calculate**
  - *Mô tả:* Tính toán tự động chi phí tiền phòng dựa trên thời gian thực tế kết hợp các dịch vụ phát sinh.
- **GET /api/reports/revenue**
  - *Mô tả:* Trích xuất báo cáo tổng hợp doanh thu theo tuần, tháng phục vụ công tác quản lý.
