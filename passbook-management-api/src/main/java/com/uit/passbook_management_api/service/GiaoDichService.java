package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.request.GuiTienRequest;
import com.uit.passbook_management_api.dto.request.RutTienRequest;
import com.uit.passbook_management_api.entity.*;
import com.uit.passbook_management_api.repository.*;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigDecimal;
import java.time.temporal.ChronoUnit;

@Service
public class GiaoDichService {

    private final SoTietKiemRepository soTietKiemRepository;
    private final PhieuGuiTienRepository phieuGuiTienRepository;
    private final PhieuRutTienRepository phieuRutTienRepository;
    private final ThamSoRepository thamSoRepository;

    public GiaoDichService(SoTietKiemRepository soTietKiemRepository,
                           PhieuGuiTienRepository phieuGuiTienRepository,
                           PhieuRutTienRepository phieuRutTienRepository,
                           ThamSoRepository thamSoRepository) {
        this.soTietKiemRepository = soTietKiemRepository;
        this.phieuGuiTienRepository = phieuGuiTienRepository;
        this.phieuRutTienRepository = phieuRutTienRepository;
        this.thamSoRepository = thamSoRepository;
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

        // QĐ3: Kiểm tra quy định rút tiền
        if (stk.getLoaiTietKiem().getKyHan() > 0) {
            // SỔ CÓ KỲ HẠN
            if (request.getNgayRut().isBefore(stk.getNgayDaoHan())) {
                throw new RuntimeException("Sổ có kỳ hạn chỉ được rút khi đã quá hạn!");
            }
            if (request.getSoTienRut().compareTo(stk.getSoDu()) != 0) {
                throw new RuntimeException("Sổ có kỳ hạn bắt buộc phải rút toàn bộ số dư!");
            }
        } else {
            // SỔ KHÔNG KỲ HẠN
            long soNgayDaGui = ChronoUnit.DAYS.between(stk.getNgayMo(), request.getNgayRut());
            if (soNgayDaGui < thamSo.getSoNgayGuiToiThieu()) {
                throw new RuntimeException("Sổ không kỳ hạn phải gửi trên " + thamSo.getSoNgayGuiToiThieu() + " ngày mới được rút!");
            }
            if (request.getSoTienRut().compareTo(stk.getSoDu()) > 0) {
                throw new RuntimeException("Số tiền rút vượt quá số dư hiện tại!");
            }
        }

        // TODO: Phần tính toán Tiền Lãi có thể bổ sung sau nếu Frontend yêu cầu trả về chi tiết.
        // Ở đây mình ưu tiên luồng trừ tiền và lưu phiếu trước.

        // Tạo phiếu rút
        PhieuRutTien phieu = new PhieuRutTien();
        phieu.setSoTienRut(request.getSoTienRut());
        phieu.setNgayRut(request.getNgayRut());
        phieu.setSoTietKiem(stk);
        phieu.setKhachHang(stk.getKhachHang());
        phieuRutTienRepository.save(phieu);

        // Cập nhật số dư sổ
        stk.setSoDu(stk.getSoDu().subtract(request.getSoTienRut()));

        // Tự động đóng sổ nếu rút hết tiền
        if (stk.getSoDu().compareTo(BigDecimal.ZERO) == 0) {
            stk.setTrangThai("DA_DONG");
        }

        soTietKiemRepository.save(stk);

        return "Rút tiền thành công! Số dư còn lại: " + stk.getSoDu() + "đ";
    }
}