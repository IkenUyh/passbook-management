package com.uit.passbook_management_api.service;

import com.uit.passbook_management_api.dto.response.BaoCaoMoDongNgayDTO;
import com.uit.passbook_management_api.dto.response.BaoCaoNgayDTO;
import com.uit.passbook_management_api.dto.response.BaoCaoThangDTO;
import com.uit.passbook_management_api.entity.*;
import com.uit.passbook_management_api.repository.*;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.util.List;

@Service
public class BaoCaoService {

    private final LoaiTietKiemRepository loaiTietKiemRepository;
    private final BaoCaoNgayRepository baoCaoNgayRepository;
    private final BaoCaoThangRepository baoCaoThangRepository;

    public BaoCaoService(LoaiTietKiemRepository loaiTietKiemRepository,
                         BaoCaoNgayRepository baoCaoNgayRepository,
                         BaoCaoThangRepository baoCaoThangRepository) {
        this.loaiTietKiemRepository = loaiTietKiemRepository;
        this.baoCaoNgayRepository = baoCaoNgayRepository;
        this.baoCaoThangRepository = baoCaoThangRepository;
    }

    @Transactional
    public List<BaoCaoNgayDTO> chotBaoCaoNgay(LocalDate ngay) {
        // 1. Tính toán
        List<BaoCaoNgayDTO> dsBaoCao = loaiTietKiemRepository.lapBaoCaoNgay(ngay);

        // 2. Lưu vào CSDL vật lý (Bảng bao_cao_ngay)
        for (BaoCaoNgayDTO dto : dsBaoCao) {
            BaoCaoNgayId id = new BaoCaoNgayId();
            id.setNgay(ngay);
            id.setMaLoaiTk(dto.getMaLoaiTk());

            BaoCaoNgay bc = new BaoCaoNgay();
            bc.setId(id);
            bc.setTongThu(dto.getTongThu());
            bc.setTongChi(dto.getTongChi());
            bc.setChenhLech(dto.getChenhLech());

            baoCaoNgayRepository.save(bc); // Save() trong JPA sẽ tự động tạo mới hoặc update nếu đã có
        }

        return dsBaoCao;
    }

    @Transactional
    public List<BaoCaoThangDTO> chotBaoCaoThang(int thang, int nam) {
        List<BaoCaoThangDTO> dsBaoCao = loaiTietKiemRepository.lapBaoCaoThang(thang, nam);
        String thangNam = String.format("%04d-%02d", nam, thang); // Chuyển thành YYYY-MM

        for (BaoCaoThangDTO dto : dsBaoCao) {
            BaoCaoThangId id = new BaoCaoThangId();
            id.setThangNam(thangNam);
            id.setMaLoaiTk(dto.getMaLoaiTk());

            BaoCaoThang bc = new BaoCaoThang();
            bc.setId(id);
            bc.setSoSoMo(dto.getSoSoMo());
            bc.setSoSoDong(dto.getSoSoDong());
            bc.setChenhLech(dto.getChenhLech());

            baoCaoThangRepository.save(bc);
        }

        return dsBaoCao;
    }

        @Transactional
        public List<BaoCaoMoDongNgayDTO> xemBaoCaoMoDongNgay(LocalDate ngay) {
        // Gọi repository lấy dữ liệu thống kê số lượng sổ đóng/mở trong ngày
        return loaiTietKiemRepository.lapBaoCaoMoDongNgay(ngay);
    }
}