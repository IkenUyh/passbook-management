# User Flow: Lập Phiếu Gửi Tiền

Sơ đồ dạng Flowchart mô tả quy trình gửi tiền vào sổ tiết kiệm. Thao tác được thực hiện từ màn hình Tra cứu Sổ Tiết Kiệm: chọn sổ trong danh sách rồi thực hiện gửi tiền.

```mermaid
flowchart TD
    A["Bắt đầu"] --> B["Màn hình Tra cứu Sổ Tiết Kiệm"]
    B --> C["Hiển thị toàn bộ danh sách sổ tiết kiệm"]
    C --> D["Chọn sổ tiết kiệm trong danh sách"]
    D --> E["Nhấn nút Gửi tiền"]
    E --> F["Mở form Gửi tiền, hiển thị thông tin sổ"]
    F --> G["Nhập số tiền cần gửi"]
    G --> H{"Số tiền rỗng?"}
    H -- Có --> I["Hiển thị: Vui lòng nhập số tiền thực hiện giao dịch"]
    I --> G
    H -- Không --> J{"Số tiền > 0?"}
    J -- Không --> K["Hiển thị: Số tiền không hợp lệ, phải là số dương"]
    K --> G
    J -- Có --> L["Gọi API GuiTienAsync"]
    L --> M{"Thành công?"}
    M -- Không --> N["Hiển thị: Giao dịch gửi thêm tiền thất bại"]
    N --> G
    M -- Có --> O["Tải lại danh sách sổ tiết kiệm"]
    O --> P["Kết thúc"]
```
