using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.src.infrastructure.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options){}

        //Thêm DbSet cho các thực thể khác, Mỗi DbSet là đại diện cho một tập các thực thể
        #region DbSet
        public DbSet<Roles> Roles{set; get;}    //Đại diện cho tập thực thể Roles, có thể thêm sửa xóa trực tiếp trên đây
        public DbSet<Account> Accounts{set;get;}
        public DbSet<Board> Boards{set;get;}
        public DbSet<Candidate> Candidates{set;get;}
        public DbSet<CandidateNoticeDetails> CandidateNoticeDetails{set;get;}
        public DbSet<Constituency> Constituencys{set;get;}
        public DbSet<District> Districts{set;get;}
        public DbSet<EducationLevel> EducationLevels{set;get;}
        public DbSet<EducationLevelDetails> EducationLevelDetails{set;get;}
        public DbSet<BallotDetails> BallotDetails{set;get;}
        public DbSet<ElectionDetails> ElectionDetails{set;get;}
        public DbSet<ElectionResults> ElectionResults{set;get;}  
        public DbSet<Elections> Elections{set;get;}
        public DbSet<Ethnicity> Ethnicities{set;get;}
        public DbSet<Lock> Locks{set;get;}
        public DbSet<ElectionStatus> ElectionStatus{set;get;} 
        public DbSet<ListOfPositions> ListOfPositions{set;get;} 
        public DbSet<LoginHistory> LoginHistories{set;get;} 
        public DbSet<Notifications> Notifications{set;get;} 
        public DbSet<PermanentAddress> PermanentAddress{set;get;} 
        public DbSet<Province> Provinces{set;get;}
        public DbSet<PrivateKey> PrivateKey{set;get;}
        public DbSet<PublicKey> PublicKey{set;get;}
        public DbSet<ResponseCadre> ResponseCadres{set;get;}
        public DbSet<Position> Positions{set;get;}  
        public DbSet<ResponseCandidate> ResponseCandidates{set;get;}
        public DbSet<ResponseVoter> ResponseVoters{set;get;}
        public DbSet<TemporaryAddress> TemporaryAddress{set;get;}
        public DbSet<Vote> Votes{set;get;}
        public DbSet<VoterNoticeDetails> VoterNoticeDetails{set;get;}
        public DbSet<Voter> Vouters{set;get;}
        public DbSet<Users> User{set;get;}
        public DbSet<Cadre> Cadres{set;get;}
        public DbSet<WorkPlace> WorkPlaces{set;get;}
        public DbSet<CadreEducationLevelDetail> CadreEducationLevelDetails{set;get;}
        public DbSet<CadreNoiticeDetail> CadreNoiticeDetail{set;get;}
        public DbSet<VoterDetails> VoterDetails{get;set;}
        public DbSet<CandidateDetails> CandidateDetails{get;set;}
        public DbSet<Profiles> Profiles{get;set;}
        public DbSet<RefreshToken> RefreshTokens{get;set;}
        #endregion

        /*
            Sử dụng các API thích hợp để định nghĩa ra mối quan hệ giữa các bảng
        Cần nạp chồng OnModelCreating của lớp DbContext.
        */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Các Fluent API. Để định nghĩa mối quan hệ giữa các entity cho nó rõ ràng hơn
            
            modelBuilder.Entity<Roles>(entity => {
                entity.ToTable("VaiTro");
                entity.HasKey(e => e.RoleID);
                entity.Property(e => e.TenVaiTro).IsRequired().HasMaxLength(30);
            });
            modelBuilder.Entity<Account>(entity => {
                entity.ToTable("TaiKhoan");
                entity.HasKey(e => e.TaiKhoan);
                entity.Property(e => e.MatKhau).IsRequired();
                entity.Property(e => e.BiKhoa).HasDefaultValue("0");
                entity.Property(e => e.LyDoKhoa).HasMaxLength(100);
                entity.Property(e=> e.NgayTao).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e=>e.SuDung).HasDefaultValue(1);
                entity.HasOne(e => e.role)
                    .WithMany(e => e.account)
                    .HasForeignKey(e => e.RoleID)
                    .HasConstraintName("FK_RoleAcc");                
            });
            modelBuilder.Entity<BallotDetails>(E => {
                E.ToTable("ChiTietPhieuBau");
                E.HasOne(e => e.vote)
                    .WithMany(e => e.ballotDetails)
                    .HasForeignKey(e => e.ID_Phieu)
                    .HasConstraintName("FK_CTPB_phieu");
                E.HasOne(e => e.candidate)
                    .WithMany(e => e.ballotDetails)
                    .HasForeignKey(e => e.ID_ucv)
                    .HasConstraintName("FK_CTPB_ungcuvien");
                E.HasOne(e => e.voter)
                    .WithMany(e => e.ballotDetails)
                    .HasForeignKey(e => e.ID_CuTri)
                    .HasConstraintName("FK_CTPB_cutri");
                E.HasOne(e => e._lock)
                    .WithMany(e => e.ballotDetails)
                    .HasForeignKey(e => e.ID_Khoa)
                    .HasConstraintName("FK_CTPB_khoa");
            });
            modelBuilder.Entity<Board>(E=>{
                E.ToTable("Ban");
                E.HasKey(e => e.ID_Ban);
                E.Property(e => e.TenBan).IsRequired().HasMaxLength(50);
                E.HasOne( e => e.constituency)
                    .WithMany(e => e.board)
                    .HasForeignKey(e => e.ID_DonViBauCu)
                    .HasConstraintName("FK_BanThuocDVBC");
            });
            modelBuilder.Entity<Cadre>(E=>{
                E.ToTable("CanBo");
                E.HasKey(e => e.ID_canbo);
                E.Property(e => e.NgayCongTac).IsRequired();
                E.Property(e => e.GhiChu).HasMaxLength(255);
                E.HasOne( e => e.user)
                    .WithMany(e => e.cadre)
                    .HasForeignKey(e => e.ID_user)
                    .HasConstraintName("FK_nguoiDungCanBo");
            });
            modelBuilder.Entity<CadreEducationLevelDetail>(E => {
                E.ToTable("ChiTietTrinhDoHocVanCanBo");
                E.HasOne(e => e.cadre)
                    .WithMany(e => e.cadreEducationLevelDetail)
                    .HasForeignKey(e => e.ID_canbo)
                    .HasConstraintName("FK_CTHV_canbo");
                E.HasOne(e => e.educationLevel)
                    .WithMany(e => e.cadreEducationLevelDetail)
                    .HasForeignKey(e => e.ID_TrinhDo)
                    .HasConstraintName("FK_TDHV_canbo");
            });
            modelBuilder.Entity<CadreNoiticeDetail>(E => {
                E.ToTable("chitietthongbaocanbo");
                E.HasOne(e => e.notifications)
                    .WithMany(e => e.cadreNoiticeDetail)
                    .HasForeignKey(e => e.ID_ThongBao)
                    .HasConstraintName("FK_thongbao_canbo");
                E.HasOne(e => e.Cadre)
                    .WithMany(e => e.cadreNoiticeDetail)
                    .HasForeignKey(e => e.ID_canbo)
                    .HasConstraintName("FK_CTTB_canbo");
            });
            modelBuilder.Entity<Candidate>(E=>{
                E.ToTable("UngCuVien");
                E.HasKey(e => e.ID_ucv);
                E.Property(e => e.TrangThai).HasDefaultValue("Activity");
                E.HasOne( e => e.user)
                    .WithMany(e => e.candidates)
                    .HasForeignKey(e => e.ID_user)
                    .HasConstraintName("FK_nguoiDungUngCuVien");
            });
            modelBuilder.Entity<EducationLevelDetails>(E => {
                E.ToTable("ChiTietTrinhDoHocVanUngCuVien");
                E.HasOne(e => e.educationLevel)
                    .WithMany(e => e.educationLevelDetails)
                    .HasForeignKey(e => e.ID_TrinhDo)
                    .HasConstraintName("FK_CTHV_ungcuvien");
                E.HasOne(e => e.candidate)
                    .WithMany(e => e.educationLevelDetails)
                    .HasForeignKey(e => e.ID_ucv)
                    .HasConstraintName("FK_TDHV_ungcuvien");
            });
            modelBuilder.Entity<CandidateNoticeDetails>(E => {
                E.ToTable("ChiTietThongBaoUngCuVien");
                E.HasOne(e => e.notifications)
                    .WithMany(e => e.candidateNoticeDetails)
                    .HasForeignKey(e => e.ID_ThongBao)
                    .HasConstraintName("FK_thongbao_ungcuvien");
                E.HasOne(e => e.candidate)
                    .WithMany(e => e.candidateNoticeDetails)
                    .HasForeignKey(e => e.ID_ucv)
                    .HasConstraintName("FK_CTTB_ungcuvien");
            });
            modelBuilder.Entity<Constituency>(E=>{
                E.ToTable("DonViBauCu");
                E.HasKey(e => e.ID_DonViBauCu);
                E.Property(e => e.TenDonViBauCu).IsRequired();
                E.Property(e => e.DiaChi).IsRequired().HasMaxLength(255);
                E.HasOne( e => e.district)
                    .WithMany(e => e.constituency)
                    .HasForeignKey(e => e.ID_QH)
                    .HasConstraintName("FK_quanhuyen_donvi");
            }); 
            modelBuilder.Entity<District>(E=>{
                E.ToTable("QuanHuyen");
                E.HasKey(e => e.ID_QH);
                E.Property(e => e.TenQH).IsRequired().HasMaxLength(50);
                E.HasOne( e => e.province)
                    .WithMany(e => e.district)
                    .HasForeignKey(e => e.STT)
                    .HasConstraintName("FK_quanhuyen_tinhthanh");
            });
            modelBuilder.Entity<EducationLevel>(E=>{
                E.ToTable("TrinhDoHocVan");
                E.HasKey(e => e.ID_TrinhDo);
                E.Property(e => e.TenTrinhDoHocVan).IsRequired().HasMaxLength(50);
            });
            modelBuilder.Entity<ElectionDetails>(E=>{
                E.ToTable("ChiTietBauCu");
                E.Property(e => e.ThoiDiemBau).IsRequired();
                E.HasOne( e => e.vote)
                    .WithMany(e => e.electionDetails)
                    .HasForeignKey(e => e.ID_Phieu)
                    .HasConstraintName("FK_CTBC_PhieuBau");
            });
            modelBuilder.Entity<ElectionResults>(E=>{
                E.ToTable("ChiTietBauCu");
                E.HasKey(e => e.ID_ketQua);
                E.Property(e => e.SoLuotBinhChon).HasDefaultValue(0);
                E.Property(e => e.TyLeBinhChon).IsRequired();
                E.Property(e => e.ThoiDiemDangKy).IsRequired();
                E.HasOne( e => e.candidate)
                    .WithMany(e => e.electionResults)
                    .HasForeignKey(e => e.ID_ucv)
                    .HasConstraintName("FK_CTKQBC_ungcuvien");
                E.HasOne( e => e.listOfPositions)
                    .WithMany(e => e.electionResults)
                    .HasForeignKey(e => e.ID_Cap)
                    .HasConstraintName("FK_CTKQBC_danhmucungcu");
                E.HasOne( e => e.elections)
                    .WithMany(e => e.electionResults)
                    .HasForeignKey(e => e.ngayBD)
                    .HasConstraintName("FK_CTKQBC_kybaucu");
            });
            modelBuilder.Entity<Elections>(E=>{
                E.ToTable("KyBauCu");
                E.HasKey(e => e.ngayBD);
                E.Property(e => e.ngayKT);
                E.Property(e => e.TenKyBauCu).IsRequired().HasMaxLength(50);
                E.Property(e => e.TenKyBauCu).HasMaxLength(255);
            });
            modelBuilder.Entity<ElectionStatus>(E=>{
                E.ToTable("TrangThaiBauCu");
                E.Property(e => e.GhiNhan).HasDefaultValue("0");
                E.HasOne( e => e.voter)
                    .WithMany(e => e.electionStatus)
                    .HasForeignKey(e => e.ID_CuTri)
                    .HasConstraintName("FK_TTBC_cutri");
                E.HasOne( e => e.constituency)
                    .WithMany(e => e.electionStatus)
                    .HasForeignKey(e => e.ID_DonViBauCu)
                    .HasConstraintName("FK_TTBC_donvibaucu");
                E.HasOne( e => e.elections)
                    .WithMany(e => e.electionStatus)
                    .HasForeignKey(e => e.ngayBD)
                    .HasConstraintName("FK_TTBC_kybaucu");
            });
            modelBuilder.Entity<Ethnicity>(E=>{
                E.ToTable("DanToc");
                E.HasKey(e => e.ID_DanToc);
                E.Property(e => e.TenDanToc).IsRequired().HasMaxLength(20);
                E.Property(e => e.TenGoiKhac).HasMaxLength(100);
            });
            modelBuilder.Entity<ListOfPositions>(E=>{
                E.ToTable("DanhMucUngCu");
                E.HasKey(e => e.ID_Cap);
                E.Property(e => e.TenCapUngCu).IsRequired().HasMaxLength(50);
                E.HasOne( e => e.consistency)
                    .WithMany(e => e.listofpositions)
                    .HasForeignKey(e => e.ID_DonViBauCu)
                    .HasConstraintName("FK_danhmucungcu_dvbc");
            });
            modelBuilder.Entity<Lock>(E=>{
                E.ToTable("Khoa");
                E.HasKey(e => e.ID_Khoa);
                E.Property(e => e.NgayTao).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                E.Property(e => e.NgayHetHan).IsRequired();
            });
            modelBuilder.Entity<LoginHistory>(E=>{
                E.ToTable("LichSuDangNhap");
                E.Property(e => e.ThoiDiem).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                 E.Property(e => e.DiaChiIP).IsRequired().HasMaxLength(30);
                E.HasOne( e => e._account)
                    .WithMany(e => e.loginHistories)
                    .HasForeignKey(e => e.TaiKhoan)
                    .HasConstraintName("FK_lichsudangnhaptaikhoan");
            });
            modelBuilder.Entity<Notifications>(E=>{
                E.ToTable("ThongBao");
                E.HasKey(e => e.ID_ThongBao);
                E.Property(e => e.NoiDungThongBao).IsRequired().HasMaxLength(255);
                E.Property(e => e.ThoiDiem).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            modelBuilder.Entity<PermanentAddress>(E=>{
                E.ToTable("DiaChiThuongTru");
                E.HasOne( e => e.user)
                    .WithMany(e => e.permanentAddress)
                    .HasForeignKey(e => e.ID_user)
                    .HasConstraintName("FK_DCTT_nguoidung");
                E.HasOne( e => e.district)
                    .WithMany(e => e.permanentAddress)
                    .HasForeignKey(e => e.ID_QH)
                    .HasConstraintName("FK_DCTT_quanhuyen");
            });
            modelBuilder.Entity<Position>(E=>{
                E.ToTable("ChucVu");
                E.HasKey(e => e.ID_ChucVu);
                E.Property(e => e.TenChucVu).IsRequired().HasMaxLength(50);
            });
            modelBuilder.Entity<PrivateKey>(E=>{
                E.ToTable("KhoaBiMat");
                E.Property(e => e.HamCamichanel).HasDefaultValue(0);
                E.Property(e => e.GiaTriB_Phan).HasDefaultValue(0);
                E.HasOne( e => e._lock)
                    .WithMany(e => e.privateKey)
                    .HasForeignKey(e => e.ID_Khoa)
                    .HasConstraintName("FK_khoa_privatek");
            });
            modelBuilder.Entity<Province>(E=>{
                E.ToTable("TinhThanh");
                E.HasKey(e => e.STT);
                E.Property(e => e.TenTinhThanh).IsRequired().HasMaxLength(50);
            });
            modelBuilder.Entity<PublicKey>(E=>{
                E.ToTable("KhoaCongKhai");
                E.Property(e => e.Modulo).HasDefaultValue(0);
                E.Property(e => e.SemiRandom_g).HasDefaultValue(0);
                E.HasOne( e => e._lock)
                    .WithMany(e => e.publicKey)
                    .HasForeignKey(e => e.ID_Khoa)
                    .HasConstraintName("FK_khoa_publick");
            });
            modelBuilder.Entity<ResponseCadre>(E=>{
                E.ToTable("PhanHoiCanBo");
                E.Property(e => e.ThoiDiem).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                 E.Property(e => e.YKien).IsRequired().HasMaxLength(255);
                E.HasOne( e => e.Cadre)
                    .WithMany(e => e.responseCadre)
                    .HasForeignKey(e => e.ID_canbo)
                    .HasConstraintName("FK_phanhoi_canbo");
            });
            modelBuilder.Entity<ResponseCandidate>(E=>{
                E.ToTable("PhanHoiUngCuVien");
                E.Property(e => e.ThoiDiem).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                 E.Property(e => e.YKien).IsRequired().HasMaxLength(255);
                E.HasOne( e => e.candidate)
                    .WithMany(e => e.responseCandidate)
                    .HasForeignKey(e => e.ID_ucv)
                    .HasConstraintName("FK_phanhoi_ungcuvien");
            });
            modelBuilder.Entity<ResponseVoter>(E=>{
                E.ToTable("PhanHoiUngCuVien");
                E.Property(e => e.ThoiDiem).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                 E.Property(e => e.YKien).IsRequired().HasMaxLength(255);
                E.HasOne( e => e.voter)
                    .WithMany(e => e.responseVoter)
                    .HasForeignKey(e => e.ID_CuTri)
                    .HasConstraintName("FK_phanhoi_cutri");
            });
            modelBuilder.Entity<TemporaryAddress>(E=>{
                E.ToTable("DiaChiTamTru");
                E.HasOne( e => e.user)
                    .WithMany(e => e.temporaryAddress)
                    .HasForeignKey(e => e.ID_user)
                    .HasConstraintName("FK_DCTamTru_nguoidung");
                E.HasOne( e => e.district)
                    .WithMany(e => e.temporaryAddress)
                    .HasForeignKey(e => e.ID_QH)
                    .HasConstraintName("FK_DCTamTru_quanhuyen");
            });
            modelBuilder.Entity<Users>(E=>{
                E.ToTable("NguoiDung");
                E.HasKey(e => e.ID_user); // Sửa thành ID_user
                E.Property(e => e.HoTen).IsRequired().HasMaxLength(50);
                E.Property(e => e.GioiTinh).IsRequired().HasDefaultValue("1");
                E.Property(e => e.NgaySinh).IsRequired();
                E.Property(e => e.DiaChiLienLac).IsRequired().HasMaxLength(150);
                E.Property(e => e.CCCD).IsRequired().HasMaxLength(12);
                E.Property(e => e.SDT).IsRequired().HasMaxLength(10);
                E.Property(e => e.Email).IsRequired().HasMaxLength(80);
                E.Property(e => e.HinhAnh).HasMaxLength(255);
                E.Property(e => e.PublicID).HasMaxLength(50);
                E.HasOne( e => e.ethnicity)
                    .WithMany(e => e.users)
                    .HasForeignKey(e => e.ID_DanToc)
                    .HasConstraintName("FK_dantoc_nguoidung");
                E.HasOne( e => e.roles)
                    .WithMany(e => e.users)
                    .HasForeignKey(e => e.RoleID)
                    .HasConstraintName("FK_vaitrocua_nguoidung");
            });
            modelBuilder.Entity<Vote>(E=>{
                E.ToTable("PhieuBau");
                E.HasKey(e => e.ID_Phieu);
                E.Property(e => e.GiaTriPhieuBau).IsRequired().HasDefaultValue(0);
                E.HasOne( e => e.elections)
                    .WithMany(e => e.vote)
                    .HasForeignKey(e => e.ngayBD)
                    .HasConstraintName("FK_thoidiemphathanhphieu");
            });
            modelBuilder.Entity<Voter>(E=>{
                E.ToTable("CuTri");
                E.HasKey(e => e.ID_CuTri);
                E.HasOne( e => e.user)
                    .WithMany(e => e.voter)
                    .HasForeignKey(e => e.ID_user)
                    .HasConstraintName("FK_nguoiDungCuTri");
            });
            modelBuilder.Entity<VoterNoticeDetails>(E => {
                E.ToTable("ChiTietThongBaoCuTri");
                E.HasOne(e => e.notifications)
                    .WithMany(e => e.voterNoticeDetails)
                    .HasForeignKey(e => e.ID_ThongBao)
                    .HasConstraintName("FK_thongbao_cutri");
                E.HasOne(e => e.voter)
                    .WithMany(e => e.voterNoticeDetails)
                    .HasForeignKey(e => e.ID_CuTri)
                    .HasConstraintName("FK_CTTB_cutri");
            });
            modelBuilder.Entity<WorkPlace>(E => {
                E.ToTable("Congtac");
                E.HasOne(e => e.position)
                    .WithMany(e => e.workPlace)
                    .HasForeignKey(e => e.ID_ChucVu)
                    .HasConstraintName("FK_damnhan_chucvu");
                E.HasOne(e => e.board)
                    .WithMany(e => e.workPlace)
                    .HasForeignKey(e => e.ID_Ban)
                    .HasConstraintName("FK_HoatDongTaiBan");
                E.HasOne(e => e.cadre)
                    .WithMany(e => e.workPlace)
                    .HasForeignKey(e => e.ID_canbo)
                    .HasConstraintName("FK_CanBo_Congtac");
            });
            modelBuilder.Entity<VoterDetails>(E=>{
                E.ToTable("ChiTietCuTri");
                E.HasOne( e => e.voter)
                    .WithMany(e => e.voterDetails)
                    .HasForeignKey(e => e.ID_CuTri)
                    .HasConstraintName("FK_ChiTiet_Cutri");
                E.HasOne( e => e.position)
                    .WithMany(e => e.voterDetails)
                    .HasForeignKey(e => e.ID_ChucVu)
                    .HasConstraintName("FK_ChiTiet_ChucVuCuTri");
            });
            modelBuilder.Entity<CandidateDetails>(E=>{
                E.ToTable("ChiTietUngCuVien");
                E.HasOne( e => e.candidate)
                    .WithMany(e => e.candidateDetails)
                    .HasForeignKey(e => e.ID_ucv)
                    .HasConstraintName("FK_ChiTiet_UngCuVien");
                E.HasOne( e => e.position)
                    .WithMany(e => e.candidateDetails)
                    .HasForeignKey(e => e.ID_ChucVu)
                    .HasConstraintName("FK_ChiTiet_ChucVuUngCuVien");
            });
            modelBuilder.Entity<Profiles>(E=>{
                E.ToTable("HoSoNguoiDung");
                E.HasKey(e => e.MaSo);
                E.Property(e => e.TrangThaiDangKy).HasDefaultValue("0");
                E.HasOne( e => e.users)
                    .WithMany(e => e.profile)
                    .HasForeignKey(e => e.ID_user)
                    .HasConstraintName("FK_hoSo_NguoiDung");
            });
        }
    }
}