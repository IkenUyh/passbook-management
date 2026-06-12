package com.uit.passbook_management_api.job;

import com.uit.passbook_management_api.entity.SoTietKiem;
import com.uit.passbook_management_api.service.SoTietKiemService;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import java.util.List;

@Component
public class NhacKiemTraDaoHanJob {

    private final SoTietKiemService soTietKiemService;

    // Thay đổi Repository thành Service ở đây
    public NhacKiemTraDaoHanJob(SoTietKiemService soTietKiemService) {
        this.soTietKiemService = soTietKiemService;
    }

    // Cấu hình chạy mỗi phút để bạn dễ test log dưới local console
    @Scheduled(cron = "0 * * * * ?")
    public void chayNgamQuetSoDaoHan() {
        int soNgayBaoTruoc = 3; // Hoặc lấy từ cấu hình tham số hệ thống nếu muốn linh hoạt

        // 1. Quét ghi nhận log thông tin các sổ sắp tới hạn (Hỗ trợ theo dõi hệ thống)
        List<SoTietKiem> danhSachSapHan = soTietKiemService.layDanhSachSapDaoHan(soNgayBaoTruoc);

        System.out.println("==================================================");
        System.out.println("[CRON JOB] Đang quét kiểm tra các sổ tiết kiệm sắp đáo hạn...");

        if (danhSachSapHan.isEmpty()) {
            System.out.println("[CRON JOB] Hiện tại không có sổ nào sắp đáo hạn trong " + soNgayBaoTruoc + " ngày tới.");
        } else {
            System.out.println("[CRON JOB] PHÁT HIỆN " + danhSachSapHan.size() + " SỔ SẮP ĐẾN HẠN PHỤC VỤ FR8!");
            for (SoTietKiem stk : danhSachSapHan) {
                System.out.println("  -> Mã sổ: " + stk.getId() + " | Khách hàng: " + stk.getKhachHang().getTen() +
                        " | Ngày đáo hạn: " + stk.getNgayDaoHan());
            }
        }

        // 2. Tự động xử lý nghiệp vụ ngầm cho các sổ đến hạn TRONG NGÀY HÔM NAY
        soTietKiemService.xuLySotietKiemDenHanHomNay();

        System.out.println("==================================================");
    }
}