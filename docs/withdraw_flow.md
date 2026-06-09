# User Flow: Lập Phiếu Rút Tiền

Sơ đồ dạng Flowchart mô tả quy trình rút tiền từ sổ tiết kiệm. Thao tác được thực hiện từ màn hình Tra cứu Sổ Tiết Kiệm: chọn sổ trong danh sách rồi thực hiện rút tiền.

```mermaid
flowchart TD
    A["Bắt đầu"] --> B["Màn hình Tra cứu Sổ Tiết Kiệm"]
    B --> C["Hiển thị toàn bộ danh sách sổ tiết kiệm"]
    C --> D["Chọn sổ tiết kiệm trong danh sách"]
    D --> E["Nhấn nút Rút tiền"]
    E --> F["Mở form Rút tiền, hiển thị thông tin sổ & số dư"]
    F --> G["Nhập số tiền cần rút"]
    G --> H{"Số tiền rỗng?"}
    H -- Có --> I["Hiển thị: Vui lòng nhập số tiền thực hiện giao dịch"]
    I --> G
    H -- Không --> J{"Số tiền > 0?"}
    J -- Không --> K["Hiển thị: Số tiền không hợp lệ, phải là số dương"]
    K --> G
    J -- Có --> L{"Số tiền rút > Số dư?"}
    L -- Có --> M["Hiển thị: Số tiền rút vượt quá số dư tài khoản"]
    M --> G
    L -- Không --> N["Gọi API RutTienAsync"]
    N --> O{"Thành công?"}
    O -- Không --> P["Hiển thị: Giao dịch rút tiền/tất toán thất bại"]
    P --> G
    O -- Có --> Q["Tải lại danh sách sổ tiết kiệm"]
    Q --> R["Kết thúc"]
```
