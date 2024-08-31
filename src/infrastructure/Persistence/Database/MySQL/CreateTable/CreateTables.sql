-- --------------------------------------------------------
-- Máy chủ:                      127.0.0.1
-- Server version:               10.4.32-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Phiên bản:           12.6.0.6765
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for baucutructuyen
CREATE DATABASE IF NOT EXISTS `baucutructuyen` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `baucutructuyen`;

-- Dumping structure for table baucutructuyen.ban
CREATE TABLE IF NOT EXISTS `ban` (
  `ID_Ban` smallint(6) NOT NULL AUTO_INCREMENT,
  `TenBan` varchar(50) DEFAULT NULL,
  `ID_DonViBauCu` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`ID_Ban`),
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  CONSTRAINT `ban_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.canbo
CREATE TABLE IF NOT EXISTS `canbo` (
  `ID_CanBo` varchar(14) NOT NULL,
  `NgayCongTac` datetime DEFAULT NULL,
  `GhiChu` text DEFAULT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  PRIMARY KEY (`ID_CanBo`),
  KEY `ID_user` (`ID_user`),
  CONSTRAINT `canbo_ibfk_1` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.chitietbaucu
CREATE TABLE IF NOT EXISTS `chitietbaucu` (
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_Phieu` varchar(18) DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_Phieu` (`ID_Phieu`),
  CONSTRAINT `chitietbaucu_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `chitietbaucu_ibfk_2` FOREIGN KEY (`ID_Phieu`) REFERENCES `phieubau` (`ID_Phieu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.chitietphieubau
CREATE TABLE IF NOT EXISTS `chitietphieubau` (
  `BinhChon` char(1) DEFAULT NULL,
  `ID_Phieu` varchar(18) DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  `ID_Khoa` int(11) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_Phieu` (`ID_Phieu`),
  KEY `ID_Khoa` (`ID_Khoa`),
  CONSTRAINT `chitietphieubau_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `chitietphieubau_ibfk_2` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `chitietphieubau_ibfk_3` FOREIGN KEY (`ID_Phieu`) REFERENCES `phieubau` (`ID_Phieu`),
  CONSTRAINT `chitietphieubau_ibfk_4` FOREIGN KEY (`ID_Khoa`) REFERENCES `khoa` (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.chitietthongbaocutri
CREATE TABLE IF NOT EXISTS `chitietthongbaocutri` (
  `ID_ThongBao` int(11) DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaocutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `chitietthongbaocutri_ibfk_2` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.chitietthongbaoungcuvien
CREATE TABLE IF NOT EXISTS `chitietthongbaoungcuvien` (
  `ID_ThongBao` int(11) DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_2` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvancanbo
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvancanbo` (
  `ID_TrinhDo` smallint(6) DEFAULT NULL,
  `ID_CanBo` varchar(14) DEFAULT NULL,
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_1` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_2` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvanungcuvien
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvanungcuvien` (
  `ID_TrinhDo` smallint(6) DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_2` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.chucvu
CREATE TABLE IF NOT EXISTS `chucvu` (
  `ID_ChucVu` tinyint(4) NOT NULL AUTO_INCREMENT,
  `TenchucVu` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`ID_ChucVu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.cutri
CREATE TABLE IF NOT EXISTS `cutri` (
  `ID_CuTri` varchar(14) NOT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  PRIMARY KEY (`ID_CuTri`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.danhmucungcu
CREATE TABLE IF NOT EXISTS `danhmucungcu` (
  `ID_Cap` smallint(6) NOT NULL AUTO_INCREMENT,
  `TenCapUngCu` varchar(50) DEFAULT NULL,
  `ID_DonViBauCu` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`ID_Cap`),
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  CONSTRAINT `danhmucungcu_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.dantoc
CREATE TABLE IF NOT EXISTS `dantoc` (
  `ID_DanToc` tinyint(4) NOT NULL AUTO_INCREMENT,
  `TenDanToc` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`ID_DanToc`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.diachitamtru
CREATE TABLE IF NOT EXISTS `diachitamtru` (
  `ID_QH` int(11) DEFAULT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidungtamtru` (`ID_user`),
  CONSTRAINT `diachitamtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidungtamtru` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.diachithuongtru
CREATE TABLE IF NOT EXISTS `diachithuongtru` (
  `ID_QH` int(11) DEFAULT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidung` (`ID_user`),
  CONSTRAINT `diachithuongtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidung` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.donvibaucu
CREATE TABLE IF NOT EXISTS `donvibaucu` (
  `ID_DonViBauCu` smallint(6) NOT NULL AUTO_INCREMENT,
  `TenDonViBauCu` varchar(50) DEFAULT NULL,
  `DiaChi` varchar(255) DEFAULT NULL,
  `ID_QH` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID_DonViBauCu`),
  KEY `ID_QH` (`ID_QH`),
  CONSTRAINT `donvibaucu_ibfk_1` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.hoatdong
CREATE TABLE IF NOT EXISTS `hoatdong` (
  `ID_canbo` varchar(14) DEFAULT NULL,
  `ID_ChucVu` tinyint(4) DEFAULT NULL,
  `ID_Ban` smallint(6) DEFAULT NULL,
  KEY `ID_canbo` (`ID_canbo`),
  KEY `ID_ChucVu` (`ID_ChucVu`),
  KEY `ID_Ban` (`ID_Ban`),
  CONSTRAINT `hoatdong_ibfk_1` FOREIGN KEY (`ID_canbo`) REFERENCES `canbo` (`ID_CanBo`),
  CONSTRAINT `hoatdong_ibfk_2` FOREIGN KEY (`ID_ChucVu`) REFERENCES `chucvu` (`ID_ChucVu`),
  CONSTRAINT `hoatdong_ibfk_3` FOREIGN KEY (`ID_Ban`) REFERENCES `ban` (`ID_Ban`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.ketquabaucu
CREATE TABLE IF NOT EXISTS `ketquabaucu` (
  `ID_ketQua` int(11) NOT NULL AUTO_INCREMENT,
  `SoLuotBinhChon` int(11) DEFAULT NULL,
  `ThoiDiemDangKy` datetime DEFAULT NULL,
  `TyLeBinhChon` float DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  `ID_Cap` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`ID_ketQua`),
  KEY `ID_Cap` (`ID_Cap`),
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `ketquabaucu_ibfk_1` FOREIGN KEY (`ID_Cap`) REFERENCES `danhmucungcu` (`ID_Cap`),
  CONSTRAINT `ketquabaucu_ibfk_2` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `ketquabaucu_ibfk_3` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.khoa
CREATE TABLE IF NOT EXISTS `khoa` (
  `ID_Khoa` int(11) NOT NULL AUTO_INCREMENT,
  `NgayTao` date DEFAULT NULL,
  `NgayHetHan` date DEFAULT NULL,
  PRIMARY KEY (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.khoabimat
CREATE TABLE IF NOT EXISTS `khoabimat` (
  `HamCamichanel` int(11) DEFAULT NULL,
  `GiaTriB_Phan` int(11) DEFAULT NULL,
  `ID_Khoa` int(11) DEFAULT NULL,
  KEY `ID_Khoa` (`ID_Khoa`),
  CONSTRAINT `khoabimat_ibfk_1` FOREIGN KEY (`ID_Khoa`) REFERENCES `khoa` (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.khoacongkhai
CREATE TABLE IF NOT EXISTS `khoacongkhai` (
  `Modulo` int(11) DEFAULT NULL,
  `SemiRandom_g` int(11) DEFAULT NULL,
  `ID_Khoa` int(11) DEFAULT NULL,
  KEY `ID_Khoa` (`ID_Khoa`),
  CONSTRAINT `khoacongkhai_ibfk_1` FOREIGN KEY (`ID_Khoa`) REFERENCES `khoa` (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.kybaucu
CREATE TABLE IF NOT EXISTS `kybaucu` (
  `ngayBD` datetime NOT NULL,
  `ngayKT` datetime DEFAULT NULL,
  `TenKyBauCu` varchar(50) NOT NULL,
  `MoTa` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.lichsudangnhap
CREATE TABLE IF NOT EXISTS `lichsudangnhap` (
  `ThoiDiem` date DEFAULT NULL,
  `DiaChiIP` varchar(30) DEFAULT NULL,
  `TaiKhoan` varchar(30) DEFAULT NULL,
  KEY `TaiKhoan` (`TaiKhoan`),
  CONSTRAINT `lichsudangnhap_ibfk_1` FOREIGN KEY (`TaiKhoan`) REFERENCES `taikhoan` (`TaiKhoan`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.nguoidung
CREATE TABLE IF NOT EXISTS `nguoidung` (
  `ID_user` varchar(16) NOT NULL,
  `HoTen` varchar(50) DEFAULT NULL,
  `GioiTinh` varchar(1) DEFAULT NULL,
  `NgaySinh` datetime DEFAULT NULL,
  `DiaChiLienLac` varchar(150) DEFAULT NULL,
  `CCCD` varchar(12) DEFAULT NULL,
  `Email` varchar(80) DEFAULT NULL,
  `SDT` varchar(10) DEFAULT NULL,
  `HinhAnh` varchar(100) DEFAULT NULL,
  `ID_DanToc` tinyint(4) DEFAULT NULL,
  `RoleID` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`ID_user`),
  UNIQUE KEY `CCCD` (`CCCD`),
  UNIQUE KEY `Email` (`Email`),
  UNIQUE KEY `SDT` (`SDT`),
  KEY `ID_DanToc` (`ID_DanToc`),
  KEY `fk_vaitronguoidung` (`RoleID`),
  CONSTRAINT `fk_vaitronguoidung` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`),
  CONSTRAINT `nguoidung_ibfk_1` FOREIGN KEY (`ID_DanToc`) REFERENCES `dantoc` (`ID_DanToc`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.phanhoicutri
CREATE TABLE IF NOT EXISTS `phanhoicutri` (
  `Ykien` varchar(255) DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `phanhoicutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.phanhoiungcuvien
CREATE TABLE IF NOT EXISTS `phanhoiungcuvien` (
  `Ykien` varchar(255) DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  CONSTRAINT `phanhoiungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.phieubau
CREATE TABLE IF NOT EXISTS `phieubau` (
  `ID_Phieu` varchar(18) NOT NULL,
  `GiaTriPhieuBau` int(11) DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_Phieu`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `phieubau_ibfk_1` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.quanhuyen
CREATE TABLE IF NOT EXISTS `quanhuyen` (
  `ID_QH` int(11) NOT NULL AUTO_INCREMENT,
  `TenQH` varchar(50) NOT NULL,
  `STT` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`ID_QH`),
  KEY `STT` (`STT`),
  CONSTRAINT `quanhuyen_ibfk_1` FOREIGN KEY (`STT`) REFERENCES `tinhthanh` (`STT`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.taikhoan
CREATE TABLE IF NOT EXISTS `taikhoan` (
  `TaiKhoan` varchar(30) NOT NULL,
  `MatKhau` text NOT NULL,
  `BiKhoa` varchar(1) DEFAULT NULL,
  `LyDoKhoa` varchar(100) DEFAULT NULL,
  `NgayTao` date DEFAULT NULL,
  `SuDung` tinyint(4) DEFAULT NULL,
  `RoleID` tinyint(4) NOT NULL,
  PRIMARY KEY (`TaiKhoan`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.thongbao
CREATE TABLE IF NOT EXISTS `thongbao` (
  `ID_ThongBao` int(11) NOT NULL AUTO_INCREMENT,
  `NoiDungThongBao` varchar(255) DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  PRIMARY KEY (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.tinhthanh
CREATE TABLE IF NOT EXISTS `tinhthanh` (
  `STT` tinyint(4) NOT NULL,
  `TenTinhThanh` varchar(50) NOT NULL,
  PRIMARY KEY (`STT`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.trangthaibaucu
CREATE TABLE IF NOT EXISTS `trangthaibaucu` (
  `GhiNhan` varchar(1) DEFAULT '0',
  `ID_CuTri` varchar(14) DEFAULT NULL,
  `ID_DonViBauCu` smallint(6) DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `trangthaibaucu_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`),
  CONSTRAINT `trangthaibaucu_ibfk_2` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `trangthaibaucu_ibfk_3` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.trinhdohocvan
CREATE TABLE IF NOT EXISTS `trinhdohocvan` (
  `ID_TrinhDo` smallint(6) NOT NULL AUTO_INCREMENT,
  `TenTrinhDoHocVan` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID_TrinhDo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.ungcuvien
CREATE TABLE IF NOT EXISTS `ungcuvien` (
  `ID_ucv` varchar(14) NOT NULL,
  `TrangThai` varchar(10) DEFAULT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  PRIMARY KEY (`ID_ucv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

-- Dumping structure for table baucutructuyen.vaitro
CREATE TABLE IF NOT EXISTS `vaitro` (
  `RoleID` tinyint(4) NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
