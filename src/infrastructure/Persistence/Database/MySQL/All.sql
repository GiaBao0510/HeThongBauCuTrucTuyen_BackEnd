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

-- Dumping data for table baucutructuyen.ban: ~0 rows (approximately)

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

-- Dumping data for table baucutructuyen.chitietphieubau: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietthongbaocutri
CREATE TABLE IF NOT EXISTS `chitietthongbaocutri` (
  `ID_ThongBao` int(11) DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaocutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `chitietthongbaocutri_ibfk_2` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaocutri: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietthongbaoungcuvien
CREATE TABLE IF NOT EXISTS `chitietthongbaoungcuvien` (
  `ID_ThongBao` int(11) DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_2` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaoungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvanungcuvien
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvanungcuvien` (
  `ID_TrinhDo` smallint(6) DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_2` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitiettrinhdohocvanungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.cutri
CREATE TABLE IF NOT EXISTS `cutri` (
  `ID_CuTri` varchar(14) NOT NULL,
  `HoTen` varchar(50) NOT NULL,
  `GioiTinh` varchar(1) NOT NULL,
  `NgaySinh` date NOT NULL,
  `DiaChiLienLac` varchar(100) NOT NULL,
  `CCCD` varchar(12) NOT NULL,
  `SDT` varchar(10) NOT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `HinhAnh` varchar(100) DEFAULT NULL,
  `RoleID` tinyint(4) NOT NULL,
  PRIMARY KEY (`ID_CuTri`),
  UNIQUE KEY `CCCD` (`CCCD`),
  UNIQUE KEY `SDT` (`SDT`),
  UNIQUE KEY `Email` (`Email`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `cutri_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.cutri: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.danhmucungcu
CREATE TABLE IF NOT EXISTS `danhmucungcu` (
  `ID_Cap` smallint(6) NOT NULL AUTO_INCREMENT,
  `TenCapUngCu` varchar(50) DEFAULT NULL,
  `ID_DonViBauCu` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`ID_Cap`),
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  CONSTRAINT `danhmucungcu_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.danhmucungcu: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.diachitamtru
CREATE TABLE IF NOT EXISTS `diachitamtru` (
  `ID_ucv` varchar(14) DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  `ID_QH` int(11) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_QH` (`ID_QH`),
  CONSTRAINT `diachitamtru_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `diachitamtru_ibfk_2` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `diachitamtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.diachitamtru: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.diachithuongtru
CREATE TABLE IF NOT EXISTS `diachithuongtru` (
  `ID_ucv` varchar(14) DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  `ID_QH` int(11) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_QH` (`ID_QH`),
  CONSTRAINT `diachithuongtru_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `diachithuongtru_ibfk_2` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `diachithuongtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.diachithuongtru: ~0 rows (approximately)

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

-- Dumping data for table baucutructuyen.donvibaucu: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.khoa
CREATE TABLE IF NOT EXISTS `khoa` (
  `ID_Khoa` int(11) NOT NULL AUTO_INCREMENT,
  `KhoaCongKhai` text DEFAULT NULL,
  `KhoaBiMat` text DEFAULT NULL,
  PRIMARY KEY (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoa: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.kybaucu
CREATE TABLE IF NOT EXISTS `kybaucu` (
  `ngayBD` datetime NOT NULL,
  `ngayKT` datetime DEFAULT NULL,
  `TenKyBauCu` varchar(50) NOT NULL,
  `MoTa` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.kybaucu: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.lichsudangnhap
CREATE TABLE IF NOT EXISTS `lichsudangnhap` (
  `ThoiDiem` date DEFAULT NULL,
  `DiaChiIP` varchar(30) DEFAULT NULL,
  `TaiKhoan` varchar(30) DEFAULT NULL,
  KEY `TaiKhoan` (`TaiKhoan`),
  CONSTRAINT `lichsudangnhap_ibfk_1` FOREIGN KEY (`TaiKhoan`) REFERENCES `taikhoan` (`TaiKhoan`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.lichsudangnhap: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phanhoicutri
CREATE TABLE IF NOT EXISTS `phanhoicutri` (
  `Ykien` varchar(255) DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `phanhoicutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoicutri: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phanhoiungcuvien
CREATE TABLE IF NOT EXISTS `phanhoiungcuvien` (
  `Ykien` varchar(255) DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  CONSTRAINT `phanhoiungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoiungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phieubau
CREATE TABLE IF NOT EXISTS `phieubau` (
  `ID_Phieu` varchar(18) NOT NULL,
  `ngayBD` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_Phieu`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `phieubau_ibfk_1` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phieubau: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.quanhuyen
CREATE TABLE IF NOT EXISTS `quanhuyen` (
  `ID_QH` int(11) NOT NULL AUTO_INCREMENT,
  `TenQH` varchar(50) NOT NULL,
  `STT` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`ID_QH`),
  KEY `STT` (`STT`),
  CONSTRAINT `quanhuyen_ibfk_1` FOREIGN KEY (`STT`) REFERENCES `tinhthanh` (`STT`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.quanhuyen: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.taikhoan
CREATE TABLE IF NOT EXISTS `taikhoan` (
  `TaiKhoan` varchar(30) NOT NULL,
  `MatKhau` varchar(100) NOT NULL,
  `BiKhoa` varchar(1) DEFAULT NULL,
  `LyDoKhoa` varchar(100) DEFAULT NULL,
  `NgayTao` date DEFAULT NULL,
  `SuDung` tinyint(4) DEFAULT NULL,
  `RoleID` tinyint(4) NOT NULL,
  PRIMARY KEY (`TaiKhoan`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.taikhoan: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.thongbao
CREATE TABLE IF NOT EXISTS `thongbao` (
  `ID_ThongBao` int(11) NOT NULL AUTO_INCREMENT,
  `NoiDungThongBao` varchar(255) DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  PRIMARY KEY (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.thongbao: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.tinhthanh
CREATE TABLE IF NOT EXISTS `tinhthanh` (
  `STT` tinyint(4) NOT NULL,
  `TenTinhThanh` varchar(50) NOT NULL,
  PRIMARY KEY (`STT`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.tinhthanh: ~81 rows (approximately)
INSERT INTO `tinhthanh` (`STT`, `TenTinhThanh`) VALUES
	(11, 'Cao Bằng'),
	(12, 'Lạng Sơn'),
	(14, 'Quảng Ninh'),
	(15, 'Hải Phòng'),
	(16, 'Hải Phòng'),
	(17, 'Thái Bình'),
	(18, 'Nam Định'),
	(19, 'Phú Thọ'),
	(20, 'Thái Nguyên'),
	(21, 'Yên Bái'),
	(22, 'Tuyên Quang'),
	(23, 'Hà Giang'),
	(24, 'Lào Cai'),
	(25, 'Lai Châu'),
	(26, 'Sơn La'),
	(27, 'Điện Biên'),
	(28, 'Hòa Bình'),
	(29, 'Hà Nội'),
	(30, 'Hà Nội'),
	(31, 'Hà Nội'),
	(32, 'Hà Nội'),
	(33, 'Hà Nội'),
	(34, 'Hải Dương'),
	(35, 'Ninh Bình'),
	(36, 'Thanh Hóa'),
	(37, 'Nghệ An'),
	(38, 'Hà Tĩnh'),
	(39, 'Đồng Nai'),
	(40, 'Hà Nội'),
	(41, 'TP.Hồ Chí Minh'),
	(43, 'Đà Nẵng'),
	(47, 'Đắk Lắk'),
	(48, 'Đắk Nông'),
	(49, 'Lâm Đồng'),
	(50, 'TP.Hồ Chí Minh'),
	(51, 'TP.Hồ Chí Minh'),
	(52, 'TP.Hồ Chí Minh'),
	(53, 'TP.Hồ Chí Minh'),
	(54, 'TP.Hồ Chí Minh'),
	(55, 'TP.Hồ Chí Minh'),
	(56, 'TP.Hồ Chí Minh'),
	(57, 'TP.Hồ Chí Minh'),
	(58, 'TP.Hồ Chí Minh'),
	(59, 'TP.Hồ Chí Minh'),
	(60, 'Đồng Nai'),
	(61, 'Bình Dương'),
	(62, 'Long An'),
	(63, 'Tiền Giang'),
	(64, 'Vĩnh Long'),
	(65, 'Cần Thơ'),
	(66, 'Đồng Tháp'),
	(67, 'An Giang'),
	(68, 'Kiên Giang'),
	(69, 'Cà Mau'),
	(70, 'Tây Ninh'),
	(71, 'Bến Tre'),
	(72, 'Bà Rịa - Vũng Tàu'),
	(73, 'Quảng Bình'),
	(74, 'Quảng Trị'),
	(75, 'Thừa Thiên Huế'),
	(76, 'Quảng Ngãi'),
	(77, 'Bình Định'),
	(78, 'Phú Yên'),
	(79, 'Khánh Hòa'),
	(80, 'Cục Cảnh sát giao thông'),
	(81, 'Gia Lai'),
	(82, 'Kon Tum'),
	(83, 'Sóc Trăng'),
	(84, 'Trà Vinh'),
	(85, 'Ninh Thuận'),
	(86, 'Bình Thuận'),
	(88, 'Vĩnh Phúc'),
	(89, 'Hưng Yên'),
	(90, 'Hà Nam'),
	(92, 'Quảng Nam'),
	(93, 'Bình Phước'),
	(94, 'Bạc Liêu'),
	(95, 'Hậu Giang'),
	(97, 'Bắc Cạn'),
	(98, 'Bắc Giang'),
	(99, 'Bắc Ninh');

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

-- Dumping data for table baucutructuyen.trangthaibaucu: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.trinhdohocvan
CREATE TABLE IF NOT EXISTS `trinhdohocvan` (
  `ID_TrinhDo` smallint(6) NOT NULL AUTO_INCREMENT,
  `TenTrinhDoHocVan` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID_TrinhDo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.trinhdohocvan: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.ungcuvien
CREATE TABLE IF NOT EXISTS `ungcuvien` (
  `ID_ucv` varchar(14) NOT NULL,
  `HoTen` varchar(50) NOT NULL,
  `GioiTinh` varchar(1) NOT NULL,
  `NgaySinh` date NOT NULL,
  `DiaChiLienLac` varchar(100) NOT NULL,
  `CCCD` varchar(12) NOT NULL,
  `SDT` varchar(10) NOT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `HinhAnh` varchar(100) DEFAULT NULL,
  `TrangThai` varchar(10) DEFAULT NULL,
  `RoleID` tinyint(4) NOT NULL,
  PRIMARY KEY (`ID_ucv`),
  UNIQUE KEY `CCCD` (`CCCD`),
  UNIQUE KEY `SDT` (`SDT`),
  UNIQUE KEY `Email` (`Email`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `ungcuvien_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.vaitro
CREATE TABLE IF NOT EXISTS `vaitro` (
  `RoleID` tinyint(4) NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.vaitro: ~0 rows (approximately)

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
