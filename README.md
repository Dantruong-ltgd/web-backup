<div align="center">

# HỆ THỐNG QUẢN LÝ THUÊ PHÒNG
### (Room Rental Management System - Final Assignment)

</div>

---

## 👥 Thành viên nhóm phát triển
* **Ngô Dân Trường** (Project Manager & Lead Developer)
* **Phạm Duy Tâm** (Developer & QA)
* **Lê Quốc Cường** (Database Administrator & Developer)

---

## 📋 Giới thiệu dự án
Dự án **Hệ Thống Quản Lý Thuê Phòng** được xây dựng nhằm số hóa và tối ưu hóa quy trình vận hành cho các nhà trọ, căn hộ cho thuê. Hệ thống giải quyết các bài toán cốt lõi bao gồm: quản lý danh sách phòng, lưu trữ thông tin khách hàng, xử lý đặt phòng/check-in, tự động hóa tính toán hóa đơn chi phí (phòng, điện, nước, dịch vụ phát sinh) và thống kê báo cáo doanh thu theo chu kỳ.

---

## ⚙️ Quy trình phát triển (Agile/Scrum Framework)
Dự án được thực hiện trong 2 Sprint (từ 31/08/2026 đến 14/09/2026)[cite: 4] tuân thủ mô hình quản lý Agile:
* **Sprint 1 (31/08/2026 – 07/09/2026):** Khởi tạo nền tảng dự án, cấu trúc kho lưu trữ Git trên nhánh `update`, hoàn thành Epic `QUANLY-1` (Quản lý phòng) và Epic `QUANLY-2` (Khách hàng & Đặt phòng)[cite: 4].
* **Sprint 2 (08/09/2026 – 14/09/2026):** Phát triển Epic `QUANLY-3` (Tính tiền, Hóa đơn dịch vụ và Thống kê báo cáo), thực hiện kiểm thử toàn hệ thống (E2E Testing) và nghiệm thu tổng kết[cite: 4].

---

## 📂 Cấu trúc thư mục tài liệu dự án (`docs/`)
Toàn bộ hồ sơ quản lý và kỹ thuật của hệ thống được lưu trữ đồng bộ tại nhánh `update`:

```text
├── docs/
│   ├── agile/
│   │   ├── sprint-1/
│   │   │   ├── meeting-1-planning.md
│   │   │   └── meeting-2-review-retro.md
│   │   └── sprint-2/
│   │       ├── meeting-3-planning.md
│   │       ├── meeting-4-mid-sprint.md
│   │       └── meeting-5-review-retro.md
│   │   └── Bien-ban-tong-hop-du-an.md
│   ├── architecture/
│   │   ├── database-schema.md
│   │   └── api-docs.md
└── .github/
    ├── ISSUE_TEMPLATE/
    └── PULL_REQUEST_TEMPLATE/
