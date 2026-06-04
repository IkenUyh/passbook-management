package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.GuiTienRequest;
import com.uit.passbook_management_api.dto.request.RutTienRequest;
import com.uit.passbook_management_api.entity.*;
import com.uit.passbook_management_api.repository.*;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.temporal.ChronoUnit;

@Service
public class GiaoDichService {

    private final SoTietKiemRepository soTietKiemRepository;
    private final PhieuGuiTienRepository phieuGuiTienRepository;
    private final PhieuRutTienRepository phieuRutTienRepository;
    private final ThamSoRepository thamSoRepository;
    private final AuditLogService auditLogService;

    public GiaoDichService(SoTietKiemRepository soTietKiemRepository,
                           PhieuGuiTienRepository phieuGuiTienRepository,
                           PhieuRutTienRepository phieuRutTienRepository,
                           ThamSoRepository thamSoRepository,
                           AuditLogService auditLogService) {
        this.soTietKiemRepository = soTietKiemRepository;
        this.phieuGuiTienRepository = phieuGuiTienRepository;
        this.phieuRutTienRepository = phieuRutTienRepository;
        this.thamSoRepository = thamSoRepository;
        this.auditLogService = auditLogService;
    }

    // ==================== FR2: GỬI TIỀN ====================
    @Transactional
    public String lapPhieuGuiTien(GuiTienRequest request) {
        SoTietKiem stk = soTietKiemRepository.findById(request.getMaSoTietKiem())
                .orElseThrow(() -> new RuntimeException("Không tìm thấy mã sổ tiết kiệm!"));

        if (stk.getTrangThai().equals("DA_DONG")) {
            throw new RuntimeException("Sổ này đã đóng, không thể giao dịch!");
        }

        ThamSo thamSo = thamSoRepository.findById(1).orElseThrow();

        // QĐ2: Số tiền gửi thêm phải >= quy định
        if (request.getSoTienGui().compareTo(thamSo.getTienGuiThemToiThieu()) < 0) {
            throw new RuntimeException("Số tiền gửi thêm tối thiểu là " + thamSo.getTienGuiThemToiThieu() + "đ");
        }

        // QĐ2: Chỉ nhận gửi thêm khi đến kỳ hạn (Đối với sổ có kỳ hạn)
        if (stk.getLoaiTietKiem().getKyHan() > 0) {
            if (request.getNgayGui().isBefore(stk.getNgayDaoHan())) {
                throw new RuntimeException("Sổ chưa đến ngày đáo hạn, không thể gửi thêm!");
            }
            // Nếu gửi thêm thành công, gia hạn sổ thêm 1 chu kỳ nữa
            stk.setNgayDaoHan(request.getNgayGui().plusMonths(stk.getLoaiTietKiem().getKyHan()));
        }

        // Tạo phiếu gửi
        PhieuGuiTien phieu = new PhieuGuiTien();
        phieu.setSoTienGui(request.getSoTienGui());
        phieu.setNgayGui(request.getNgayGui());
        phieu.setSoTietKiem(stk);
        phieu.setKhachHang(stk.getKhachHang());
        phieuGuiTienRepository.save(phieu);

        // Cập nhật số dư sổ
        stk.setSoDu(stk.getSoDu().add(request.getSoTienGui()));
        soTietKiemRepository.save(stk);

        // --- GHI LOG HỆ THỐNG ---
        auditLogService.ghiLog("GỬI TIỀN", "Mã sổ: " + stk.getId() + " | Số tiền gửi: " + request.getSoTienGui() + "đ");

        return "Gửi tiền thành công! Đã cộng " + request.getSoTienGui() + "đ vào sổ.";
    }

    // ==================== FR3: RÚT TIỀN ====================
    @Transactional
    public String lapPhieuRutTien(RutTienRequest request) {
        SoTietKiem stk = soTietKiemRepository.findById(request.getMaSoTietKiem())
                .orElseThrow(() -> new RuntimeException("Không tìm thấy mã sổ tiết kiệm!"));

        if (stk.getTrangThai().equals("DA_DONG")) {
            throw new RuntimeException("Sổ này đã đóng!");
        }

        ThamSo thamSo = thamSoRepository.findById(1).orElseThrow();

        // 1. Tính số ngày đã gửi
        long soNgayDaGui = ChronoUnit.DAYS.between(stk.getNgayMo(), request.getNgayRut());
        BigDecimal tienLai = BigDecimal.ZERO;

        // 2. TÍNH LÃI SUẤT & KIỂM TRA ĐIỀU KIỆN KỲ HẠN
        if (stk.getLoaiTietKiem().getKyHan() > 0) {
            // ---> SỔ CÓ KỲ HẠN
            if (request.getNgayRut().isBefore(stk.getNgayDaoHan())) {
                throw new RuntimeException("Sổ có kỳ hạn chỉ được rút khi đã quá hạn!");
            }
            if (request.getSoTienRut().compareTo(stk.getSoDu()) != 0) {
                throw new RuntimeException("Sổ có kỳ hạn bắt buộc phải rút toàn bộ số dư gốc!");
            }
            BigDecimal laiSuat = stk.getLoaiTietKiem().getLaiSuat().divide(BigDecimal.valueOf(100), 4, RoundingMode.HALF_UP);
            BigDecimal thoiGian = BigDecimal.valueOf(stk.getLoaiTietKiem().getKyHan()).divide(BigDecimal.valueOf(12), 4, RoundingMode.HALF_UP);
            tienLai = stk.getSoDu().multiply(laiSuat).multiply(thoiGian);
        } else {
            // ---> SỔ KHÔNG KỲ HẠN
            if (soNgayDaGui < thamSo.getSoNgayGuiToiThieu()) {
                throw new RuntimeException("Sổ không kỳ hạn phải gửi từ " + thamSo.getSoNgayGuiToiThieu() + " ngày trở lên mới được rút!");
            }
            BigDecimal laiSuat = stk.getLoaiTietKiem().getLaiSuat().divide(BigDecimal.valueOf(100), 4, RoundingMode.HALF_UP);
            BigDecimal thoiGian = BigDecimal.valueOf(soNgayDaGui).divide(BigDecimal.valueOf(365), 4, RoundingMode.HALF_UP);
            tienLai = stk.getSoDu().multiply(laiSuat).multiply(thoiGian);
        }

        // 3. CỘNG LÃI VÀO SỐ DƯ GỐC TRƯỚC KHI CHO RÚT
        stk.setSoDu(stk.getSoDu().add(tienLai));

        // 4. KIỂM TRA SỐ TIỀN RÚT CÓ HỢP LỆ KHÔNG (Chỉ áp dụng cho sổ KHÔNG kỳ hạn)
        if (stk.getLoaiTietKiem().getKyHan() == 0) {
            if (request.getSoTienRut().compareTo(stk.getSoDu()) > 0) {
                throw new RuntimeException("Số tiền rút vượt quá số dư hiện tại (Gốc + Lãi là: " + stk.getSoDu() + "đ)!");
            }
        }

        // 5. TẠO PHIẾU RÚT
        PhieuRutTien phieu = new PhieuRutTien();
        phieu.setSoTienRut(request.getSoTienRut());
        phieu.setNgayRut(request.getNgayRut());
        phieu.setSoTietKiem(stk);
        phieu.setKhachHang(stk.getKhachHang());
        phieuRutTienRepository.save(phieu);

        // 6. TRỪ TIỀN TRONG SỔ
        stk.setSoDu(stk.getSoDu().subtract(request.getSoTienRut()));

        // 7. TỰ ĐỘNG ĐÓNG SỔ NẾU RÚT HẾT TIỀN VỀ 0
        // Dùng compareTo để so sánh an toàn với BigDecimal
        if (stk.getSoDu().compareTo(BigDecimal.ZERO) == 0) {
            stk.setTrangThai("DA_DONG");
        }

        soTietKiemRepository.save(stk);

        // --- GHI LOG HỆ THỐNG ---
        auditLogService.ghiLog("RÚT TIỀN", "Mã sổ: " + stk.getId() + " | Số tiền rút: " + request.getSoTienRut() + "đ");

        return "Rút tiền thành công! Số dư còn lại: " + stk.getSoDu() + "đ";
    }
}