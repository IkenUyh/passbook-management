# User Flow: Mở Sổ Tiết Kiệm

Sơ đồ dạng Flowchart mô tả quy trình mở sổ tiết kiệm mới cho khách hàng. Chức năng Mở Sổ nằm trong màn hình Quản lý Khách hàng: chọn khách hàng trước, rồi mở sổ cho khách hàng đó.

```mermaid
flowchart TD
    A["Bắt đầu"] --> B["Màn hình Quản lý Khách Hàng"]
    B --> C["Chọn khách hàng trong danh sách"]
    C --> D["Nhấn nút Mở Sổ Tiết Kiệm"]
    D --> E["Mở form Mở Sổ, tự động load danh sách Loại Tiết Kiệm"]
    E --> F["Chọn Loại Tiết Kiệm từ dropdown"]
    F --> G["Nhập số tiền gửi ban đầu"]
    G --> H{"Số tiền rỗng?"}
    H -- Có --> I["Hiển thị: Vui lòng nhập số tiền gửi ban đầu"]
    I --> G
    H -- Không --> J{"Số tiền > 0?"}
    J -- Không --> K["Hiển thị: Số tiền phải là số dương lớn hơn 0"]
    K --> G
    J -- Có --> L["Gọi API MoSoTietKiemAsync"]
    L --> M{"Thành công?"}
    M -- Không --> N["Hiển thị: Mở sổ thất bại, kiểm tra quy định số tiền gửi tối thiểu"]
    N --> G
    M -- Có --> O["Tải lại danh sách khách hàng & sổ tiết kiệm"]
    O --> P["Kết thúc"]
```
