# Phần việc D, giai đoạn 1 (TDevElectronics)

Gộp vào project có sẵn của nhóm. Chưa có CSDL cho phần D: dữ liệu mẫu (Data/SampleAdminData.cs) + Session.

## Đối chiếu cây thư mục (phần D)
| Mục trong cây | Trạng thái |
|---|---|
| Controllers/AccountController.cs: Index, Orders, CancelRequest, Settings | Có (Login, Register, Logout ở AccountController.Auth.cs) |
| Controllers/QuotesController.cs: Rfq, Index, Detail | Có (+ POST Rfq kiểm tra RfqViewModel) |
| Areas/Admin/Controllers: Products, Categories, Orders, Quotes, Settings, Dashboard | Có |
| Areas/Admin/Views: Products (4), Categories (3+fields), Orders (2), Quotes (2), Settings, Shared/_Pagination, Dashboard | Có |
| Views/Account: Index, Orders, Settings; Views/Quotes: Rfq, Index (+Detail) | Có |
| Models/Entities: Order, OrderItem, CancelRequest, Quote, QuoteItem, SiteSetting | Có (chưa có DbSet, giai đoạn 2) |
| Models/Enums: OrderStatus, QuoteStatus | Có |
| Models/ViewModels: RfqViewModel, DashboardViewModel (+ Product/Category form, CancelRequest) | Có |
| Services: IOrderService, IQuoteService, ICurrencyService, IExportService + 4 lớp cài đặt | Có (đã đăng ký trong Program.cs) |
| wwwroot: auth-portal.css, chatbot.css, page-edit.css, avatar.js, chatbot.js, page-edit.js, admin-pages.js, export.js, export-font.js, uploads/ | Đã bổ sung từ template |

## Bổ sung lần 2 (đối chiếu từng dòng của cây thư mục, mọi file đều đã có trừ InviteCode.cs)
- Controllers/CartController.cs, CheckoutController.cs: ĐÃ CÓ sẵn trong project (Index/Add/Update/Remove/Sync và Index/PlaceOrder/Success), không sửa.
- Users: thêm action Edit (GET/POST), Delete, Invites + views Users/Edit.cshtml, Users/Invites.cshtml; Users/Index có nút Sửa, Xóa, link mã mời.
- Models/Enums/UserRole.cs; Entities Category, Product, ProductImage, PriceTier (lớp dữ liệu, chưa DbSet).
- Services: ICatalogService/CatalogService, ICartService/CartService (bọc SampleData và Session); IAuthService chuyển vào Services/Interfaces/.
- Data/Configurations/*Configuration.cs cho 10 entity (chưa áp dụng, bật ở giai đoạn 2); Migrations chuyển vào Data/Migrations/ (namespace giữ nguyên).
- wwwroot/lib/ (rỗng: bootstrap, jquery chưa dùng vì giao diện hiện tại không cần).
- InviteCode.cs: không tạo, vì trong code mã mời là Models/Entities/AdminInvite.cs. Nếu muốn đúng tên cây thì đổi tên AdminInvite thành InviteCode (phải sửa DbContext, UsersController, AuthService, Migrations).

## Chưa thuộc D (không làm)
(đã bổ sung ở mục trên)

## Giai đoạn 2
Xóa SampleAdminData + SessionState, thêm DbSet<Order, OrderItem, CancelRequest, Quote, QuoteItem, SiteSetting> vào AppDbContext rồi Add-Migration; OrderService, QuoteService đọc/ghi CSDL; ExportService thêm xlsx/pdf (ClosedXML, QuestPDF).

## Sửa lỗi trang Settings nháy liên tục (lần 3)
Nguyên nhân: server dùng Cookie Auth, còn app.js (từ template) tự đọc phiên từ localStorage `tdev-sess`.
Đã đăng nhập ở server nhưng app.js coi là khách nên chuyển /Account -> /Account/Login, server lại chuyển
/Account/Login -> /Account (vòng lặp vô tận, trang nháy).
- Views/Shared/_SessionBridge.cshtml (mới): ghi phiên của server vào localStorage trước khi app.js chạy. Nạp trong `_Layout` và `_AdminLayout`.
- wwwroot/js/app.js: nút "Sign out" trước đây bị link /Account/Login nuốt mất (không gọi logout); nay gọi server `/Account/Logout` (khách) hoặc form `Auth/Logout` (admin).
- AccountController.Logout: sau khi đăng xuất chuyển về trang đăng nhập (như template).
- Tên và công ty sửa ở Settings được giữ trong trình duyệt (giai đoạn 2 lưu CSDL).
