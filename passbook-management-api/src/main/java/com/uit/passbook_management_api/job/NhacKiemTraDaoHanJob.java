package com.uit.passbook_management_api.job;

import com.uit.passbook_management_api.entity.SoTietKiem;
import com.uit.passbook_management_api.repository.SoTietKiemRepository;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import java.time.LocalDate;
import java.util.List;

@Component
public class NhacKiemTraDaoHanJob {

    private final SoTietKiemRepository soTietKiemRepository;

    public NhacKiemTraDaoHanJob(SoTietKiemRepository soTietKiemRepository) {
        this.soTietKiemRepository = soTietKiemRepository;
    }

    // Annotation @Scheduled giúp hàm này tự động chạy ngầm.
    // "0 * * * * ?" nghĩa là chạy mỗi đầu phút (để anh dễ test).
    // Khi nộp bài cho thầy, anh sửa thành "0 0 0 * * ?" (Chạy vào 00:00:00 mỗi ngày).
    @Scheduled(cron = "0 * * * * ?")
    public void chayNgamQuetSoDaoHan() {
        LocalDate homNay = LocalDate.now();
        LocalDate baNgayToi = homNay.plusDays(3); // Cấu hình báo trước 3 ngày

        List<SoTietKiem> danhSachSapHan = soTietKiemRepository.timSoSapDaoHan(homNay, baNgayToi);

        System.out.println("==================================================");
        System.out.println("[CRON JOB] Đang quét các sổ tiết kiệm sắp đáo hạn...");

        if (danhSachSapHan.isEmpty()) {
            System.out.println("[CRON JOB] Hôm nay không có sổ nào sắp đáo hạn.");
        } else {
            System.out.println("[CRON JOB] TÌM THẤY " + danhSachSapHan.size() + " SỔ SẮP ĐẾN HẠN!");
            for (SoTietKiem stk : danhSachSapHan) {
                System.out.println("  -> Mã sổ: " + stk.getId() + " | Khách hàng: " + stk.getKhachHang().getTen() +
                        " | Ngày đáo hạn: " + stk.getNgayDaoHan());
            }
        }
        System.out.println("==================================================");
    }
}