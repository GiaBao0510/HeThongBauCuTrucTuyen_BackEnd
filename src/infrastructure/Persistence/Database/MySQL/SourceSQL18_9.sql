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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ban: ~4 rows (approximately)
INSERT INTO `ban` (`ID_Ban`, `TenBan`, `ID_DonViBauCu`) VALUES
	(1, 'Ban bầu cử', 1),
	(2, 'Ban Kiểm phiếu', 1),
	(3, 'Ban thanh tra', 1),
	(6, 'test', 1);

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

-- Dumping data for table baucutructuyen.canbo: ~0 rows (approximately)

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

-- Dumping data for table baucutructuyen.chitietbaucu: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietcutri
CREATE TABLE IF NOT EXISTS `chitietcutri` (
  `ID_ChucVu` tinyint(4) DEFAULT NULL,
  `ID_CuTri` varchar(14) DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `chitietcutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietcutri: ~0 rows (approximately)

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

-- Dumping structure for table baucutructuyen.chitietthongbaocanbo
CREATE TABLE IF NOT EXISTS `chitietthongbaocanbo` (
  `ID_ThongBao` int(11) DEFAULT NULL,
  `ID_CanBo` varchar(14) DEFAULT NULL,
  KEY `ID_ThongBao` (`ID_ThongBao`),
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `chitietthongbaocanbo_ibfk_1` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaocanbo_ibfk_2` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaocanbo: ~0 rows (approximately)

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

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvancanbo
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvancanbo` (
  `ID_TrinhDo` smallint(6) DEFAULT NULL,
  `ID_CanBo` varchar(14) DEFAULT NULL,
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_1` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_2` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitiettrinhdohocvancanbo: ~0 rows (approximately)

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

-- Dumping structure for table baucutructuyen.chitietungcuvien
CREATE TABLE IF NOT EXISTS `chitietungcuvien` (
  `ID_ChucVu` tinyint(4) DEFAULT NULL,
  `ID_ucv` varchar(14) DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  CONSTRAINT `chitietungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chucvu
CREATE TABLE IF NOT EXISTS `chucvu` (
  `ID_ChucVu` tinyint(4) NOT NULL AUTO_INCREMENT,
  `TenChucVu` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID_ChucVu`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chucvu: ~7 rows (approximately)
INSERT INTO `chucvu` (`ID_ChucVu`, `TenChucVu`) VALUES
	(1, 'Hiệu trưởng trường đại học'),
	(2, 'Phó hiệu trưởng trường đại học'),
	(3, 'Phó hiệu trưởng trường CNTT&TT'),
	(4, 'Hiệu trưởng trường CNTT&TT'),
	(5, 'Trưởng khoa CNTT&TT'),
	(6, 'Trưởng khoa Hệ thống thông tin'),
	(7, 'Trưởng khoa Khoa học máy tính');

-- Dumping structure for table baucutructuyen.cutri
CREATE TABLE IF NOT EXISTS `cutri` (
  `ID_CuTri` varchar(14) NOT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  PRIMARY KEY (`ID_CuTri`),
  KEY `FK_userCuTri` (`ID_user`),
  CONSTRAINT `FK_userCuTri` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.cutri: ~2 rows (approximately)
INSERT INTO `cutri` (`ID_CuTri`, `ID_user`) VALUES
	('20240916221039', 'Pe20240916221033'),
	('20240916220819', 'sT20240916220819'),
	('20240916232132', 'Zh20240916232125');

-- Dumping structure for table baucutructuyen.danhmucungcu
CREATE TABLE IF NOT EXISTS `danhmucungcu` (
  `ID_Cap` smallint(6) NOT NULL AUTO_INCREMENT,
  `TenCapUngCu` varchar(50) DEFAULT NULL,
  `ID_DonViBauCu` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`ID_Cap`),
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  CONSTRAINT `danhmucungcu_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.danhmucungcu: ~5 rows (approximately)
INSERT INTO `danhmucungcu` (`ID_Cap`, `TenCapUngCu`, `ID_DonViBauCu`) VALUES
	(1, 'Trưởng khoa', 3),
	(2, 'Phó trưởng khoa', 3),
	(3, 'Phó trưởng khoa', 1),
	(4, 'Hiệu trưởng', 9),
	(5, 'Phó hiệu trưởng', 9);

-- Dumping structure for table baucutructuyen.dantoc
CREATE TABLE IF NOT EXISTS `dantoc` (
  `ID_DanToc` tinyint(4) NOT NULL AUTO_INCREMENT,
  `TenDanToc` varchar(20) DEFAULT NULL,
  `TenGoiKhac` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ID_DanToc`)
) ENGINE=InnoDB AUTO_INCREMENT=56 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.dantoc: ~54 rows (approximately)
INSERT INTO `dantoc` (`ID_DanToc`, `TenDanToc`, `TenGoiKhac`) VALUES
	(1, 'Kinh', 'Việt'),
	(2, 'Tày', 'Thổ, Ngạn, Phén, Thù Lao, Pa Dí, Tày Khao'),
	(3, 'Thái', 'Tày Đăm, Tày Mười, Tày Thanh, Mán Thanh, Hàng Bông, Tày Mường, Pa Thay, Thổ Đà Bắc'),
	(4, 'Hoa', 'Hán, Triều Châu, Phúc Kiến, Quảng Đông, Hải Nam, Hạ, Xạ Phạng'),
	(5, 'Khơ-me', 'Cur, Cul, Cu, Thổ, Việt gốc Miên, Krôm'),
	(6, 'Mường', 'Mol, Mual, Mọi, Mọi Bi, Ao Tá, Ậu Tá'),
	(7, 'Nùng', 'Xuồng, Giang, Nùng An, Phàn Sinh, Nùng Cháo, Nùng Lòi, Quý Rim, Khèn Lài'),
	(8, 'HMông', 'Mèo, Hoa, Mèo Xanh, Mèo Đỏ, Mèo Đen, Ná Mẻo, Mán Trắng'),
	(9, 'Dao', 'Mán, Động, Trại, Xá, Dìu, Miên, Kiềm, Miền, Quần Trắng, Dao Đỏ, Quần Chẹt, Lô Giang, Dao Tiền, Thanh'),
	(10, 'Gia-rai', 'Giơ-rai, Tơ-buăn, Chơ-rai, Hơ-bau, Hđrung, Chor'),
	(11, 'Ngái', 'Xín, Lê, Đản, Khách Gia'),
	(12, 'Ê-đê', 'Ra-đê, Đê, Kpạ, A-đham, Krung, Ktul, Đliê Ruê, Blô, Epan, Mđhur, Bih'),
	(13, 'Ba na', 'Giơ-lar. Tơ-lô, Giơ-lâng, Y-lăng, Rơ-ngao, Krem, Roh, ConKđe, A-la Công, Kpăng Công, Bơ-nâm'),
	(14, 'Xơ-Đăng', 'Xơ-teng, Hđang, Tơ-đra, Mơ-nâm, Ha-lăng, Ca-dong, Kmrâng, ConLan, Bri-la, Tang'),
	(15, 'Sán Chay', 'Cao Lan, Sán Chỉ, Mán Cao Lan, Hờn Bạn, Sơn Tử'),
	(16, 'Cơ-ho', 'Xrê, Nốp, Tu-lốp, Cơ-don, Chil, Lat, Lach, Trinh'),
	(17, 'Chăm', 'Chàm, Chiêm Thành, Hroi'),
	(18, 'Sán Dìu', 'Sán Dẻo, Trại, Trại Đất, Mán, Quần Cộc'),
	(19, 'Hrê', 'Chăm Rê, Chom, Krẹ Luỹ'),
	(20, 'Mnông', 'Pnông, Nông, Pré, Bu-đâng, ĐiPri, Biat, Gar, Rơ-lam, Chil'),
	(21, 'Ra-glai', 'Ra-clây, Rai, Noang, La-oang'),
	(22, 'Xtiêng', 'Xa-điêng'),
	(23, 'Bru-Vân Kiều', 'Bru, Vân Kiều, Măng Coong, Tri Khùa'),
	(24, 'Thổ', 'Kẹo, Mọn, Cuối, Họ, Đan Lai, Ly Hà, Tày Pọng, Con Kha, Xá Lá Vàng'),
	(25, 'Giáy', 'Nhắng, Dẩng, Pầu Thìn Nu Nà, Cùi Chu, Xa'),
	(26, 'Cơ-tu', 'Ca-tu, Cao, Hạ, Phương, Ca-tang'),
	(27, 'Gié Triêng', 'Đgiéh, Tareb, Giang Rẫy Pin, Triêng, Treng, Ta-riêng, Ve, Veh, La-ve, Ca-tang'),
	(28, 'Mạ', 'Châu Mạ, Mạ Ngăn, Mạ Xóp, Mạ Tô, Mạ Krung'),
	(29, 'Khơ-mú', 'Xá Cẩu, Mứn Xen, Pu Thênh, Tềnh, Tày Hay'),
	(30, 'Co', 'Cor, Col, Cùa, Trầu'),
	(31, 'Tà-ôi', 'Tôi-ôi, Pa-co, Pa-hi, Ba-hi'),
	(32, 'Chơ-ro', 'Dơ-ro, Châu-ro'),
	(33, 'Kháng', 'Xá Khao, Xá Súa, Xá Dón, Xá Dẩng, Xá Hốc, Xá Ái, Xá Bung, Quảng Lâm'),
	(34, 'Xinh-mun', 'Puộc, Pụa'),
	(35, 'Hà Nhì', 'U Ni, Xá U Ni'),
	(36, 'Chu ru', 'Chơ-ru, Chu'),
	(37, 'Lào    ', 'Là Bốc, Lào Nọi'),
	(38, 'La Chí', 'Cù Tê, La Quả'),
	(39, 'La Ha', 'Xá Khao, Khlá Phlạo'),
	(40, 'Phù Lá', 'Bồ Khô Pạ, Mu Di Pạ Xá, Phó, Phổ, Va Xơ'),
	(41, 'La Hủ', 'Lao, Pu Đang, Khù Xung, Cò Xung, Khả Quy'),
	(42, '4Lự    ', 'Lừ, Nhuồn, Duôn'),
	(43, 'Lô Lô', 'Mun Di'),
	(44, 'Chứt', 'Sách, Máy, Rục, Mã-liêng, A-rem, Tu vang, Pa-leng, Xơ-Lang, Tơ-hung, Chà-củi, Tắc-củi, U-mo, Xá Lá V'),
	(45, 'Mảng', 'Mảng Ư, Xá Lá Vàng'),
	(46, 'Pà Thẻn        Pà Hư', 'undefined'),
	(47, 'Co Lao', NULL),
	(48, 'Cống', 'Xắm Khống, Mấng Nhé, Xá Xeng'),
	(49, 'Bố Y', 'Chủng Chá, Trọng Gia, Tu Di, Tu Din'),
	(50, 'Si La', 'Cù Dề Xừ, Khả pẻ'),
	(51, 'Pu Péo', 'Ka Pèo, Pen Ti Lô Lô'),
	(52, 'Brâu', 'Brao'),
	(53, 'Ơ Đu', 'Tày Hạt'),
	(54, 'Rơ măm', NULL);

-- Dumping structure for table baucutructuyen.diachitamtru
CREATE TABLE IF NOT EXISTS `diachitamtru` (
  `ID_QH` int(11) DEFAULT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidungtamtru` (`ID_user`),
  CONSTRAINT `diachitamtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidungtamtru` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.diachitamtru: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.diachithuongtru
CREATE TABLE IF NOT EXISTS `diachithuongtru` (
  `ID_QH` int(11) DEFAULT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidung` (`ID_user`),
  CONSTRAINT `diachithuongtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidung` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
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
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.donvibaucu: ~4 rows (approximately)
INSERT INTO `donvibaucu` (`ID_DonViBauCu`, `TenDonViBauCu`, `DiaChi`, `ID_QH`) VALUES
	(1, 'Khoa công nghệ thông tin', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1),
	(3, 'Khoa khoa học máy tính', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1),
	(6, 'Khoa Hệ thống thông tin', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1),
	(9, 'Trường CNTT&TT', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1);

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

-- Dumping data for table baucutructuyen.hoatdong: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.hosonguoidung
CREATE TABLE IF NOT EXISTS `hosonguoidung` (
  `MaSo` int(11) NOT NULL AUTO_INCREMENT,
  `TrangThaiDangKy` varchar(1) DEFAULT '0',
  `ID_user` varchar(16) DEFAULT NULL,
  PRIMARY KEY (`MaSo`),
  KEY `ID_user` (`ID_user`),
  CONSTRAINT `hosonguoidung_ibfk_1` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.hosonguoidung: ~2 rows (approximately)
INSERT INTO `hosonguoidung` (`MaSo`, `TrangThaiDangKy`, `ID_user`) VALUES
	(1, '0', 'sT20240916220819'),
	(2, '0', 'Pe20240916221033'),
	(3, '0', 'Zh20240916232125');

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

-- Dumping data for table baucutructuyen.ketquabaucu: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.khoa
CREATE TABLE IF NOT EXISTS `khoa` (
  `ID_Khoa` int(11) NOT NULL AUTO_INCREMENT,
  `NgayTao` date DEFAULT NULL,
  `NgayHetHan` date DEFAULT NULL,
  PRIMARY KEY (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoa: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.khoabimat
CREATE TABLE IF NOT EXISTS `khoabimat` (
  `HamCamichanel` int(11) DEFAULT NULL,
  `GiaTriB_Phan` int(11) DEFAULT NULL,
  `ID_Khoa` int(11) DEFAULT NULL,
  KEY `ID_Khoa` (`ID_Khoa`),
  CONSTRAINT `khoabimat_ibfk_1` FOREIGN KEY (`ID_Khoa`) REFERENCES `khoa` (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoabimat: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.khoacongkhai
CREATE TABLE IF NOT EXISTS `khoacongkhai` (
  `Modulo` int(11) DEFAULT NULL,
  `SemiRandom_g` int(11) DEFAULT NULL,
  `ID_Khoa` int(11) DEFAULT NULL,
  KEY `ID_Khoa` (`ID_Khoa`),
  CONSTRAINT `khoacongkhai_ibfk_1` FOREIGN KEY (`ID_Khoa`) REFERENCES `khoa` (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoacongkhai: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.kybaucu
CREATE TABLE IF NOT EXISTS `kybaucu` (
  `ngayBD` datetime NOT NULL,
  `ngayKT` datetime DEFAULT NULL,
  `TenKyBauCu` varchar(50) NOT NULL,
  `MoTa` varchar(255) DEFAULT NULL,
  `SoLuongToiDaCuTri` int(11) DEFAULT NULL,
  `SoLuongToiDaUngCuVien` int(11) DEFAULT NULL,
  `SoLuotBinhChonToiDa` int(11) DEFAULT NULL,
  PRIMARY KEY (`ngayBD`),
  CONSTRAINT `check_dates` CHECK (`ngayKT` > `ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.kybaucu: ~5 rows (approximately)
INSERT INTO `kybaucu` (`ngayBD`, `ngayKT`, `TenKyBauCu`, `MoTa`, `SoLuongToiDaCuTri`, `SoLuongToiDaUngCuVien`, `SoLuotBinhChonToiDa`) VALUES
	('2024-08-13 07:00:19', '2024-12-18 07:00:19', 'Bảo vệ luận văn', '', NULL, NULL, NULL),
	('2024-11-03 10:15:00', '2024-11-15 12:10:00', 'Bầu cử trưởng thôn', 'okok', NULL, NULL, NULL),
	('2024-11-03 10:25:10', '2024-11-15 12:10:00', 'Bầu cử trưởng làng', '', NULL, NULL, NULL),
	('2024-11-03 10:25:11', '2024-11-30 12:14:00', 'Bầu cử thị trưởng', '', NULL, NULL, NULL),
	('2024-11-05 00:00:00', '2024-12-31 00:00:00', 'Bầu cử', '', NULL, NULL, NULL);

-- Dumping structure for table baucutructuyen.lichsudangnhap
CREATE TABLE IF NOT EXISTS `lichsudangnhap` (
  `ThoiDiem` date DEFAULT NULL,
  `DiaChiIP` varchar(30) DEFAULT NULL,
  `TaiKhoan` varchar(30) DEFAULT NULL,
  KEY `TaiKhoan` (`TaiKhoan`),
  CONSTRAINT `lichsudangnhap_ibfk_1` FOREIGN KEY (`TaiKhoan`) REFERENCES `taikhoan` (`TaiKhoan`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.lichsudangnhap: ~0 rows (approximately)

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
  `HinhAnh` varchar(255) DEFAULT NULL,
  `PublicID` varchar(50) DEFAULT NULL,
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

-- Dumping data for table baucutructuyen.nguoidung: ~3 rows (approximately)
INSERT INTO `nguoidung` (`ID_user`, `HoTen`, `GioiTinh`, `NgaySinh`, `DiaChiLienLac`, `CCCD`, `Email`, `SDT`, `HinhAnh`, `PublicID`, `ID_DanToc`, `RoleID`) VALUES
	('Pe20240916221033', 'Lê Hữu Đức', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000322327445', 'ducb2013070@student.ctu.edu.vn', '0974000352', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726499436/NguoiDung/nhdc4uzjfwwpwmbcmcpr.jpg', 'NguoiDung/nhdc4uzjfwwpwmbcmcpr', 1, 5),
	('sT20240916220819', 'Lý Gia Nguyên', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000422327445', 'pgiabao2002@gmail.com', '0974000452', NULL, NULL, 1, 5),
	('Zh20240916232125', 'Phạm Thế HIển', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000122327445', 'baob2016947@student.ctu.edu.vn', '0974000252', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726503689/NguoiDung/uiumpqhsuwsfri9anamz.jpg', 'NguoiDung/uiumpqhsuwsfri9anamz', 1, 5);

-- Dumping structure for table baucutructuyen.phanhoicanbo
CREATE TABLE IF NOT EXISTS `phanhoicanbo` (
  `YKien` varchar(255) DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_CanBo` varchar(14) DEFAULT NULL,
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `phanhoicanbo_ibfk_1` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoicanbo: ~0 rows (approximately)

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
  `GiaTriPhieuBau` int(11) DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_Phieu`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `phieubau_ibfk_1` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phieubau: ~14 rows (approximately)
INSERT INTO `phieubau` (`ID_Phieu`, `GiaTriPhieuBau`, `ngayBD`) VALUES
	('1Q2024090316561822', 0, '2024-08-13 07:00:19'),
	('8v2024090316561829', 0, '2024-08-13 07:00:19'),
	('B72024090316370195', 0, '2024-08-13 07:00:19'),
	('bd2024090316561841', 0, '2024-08-13 07:00:19'),
	('cP2024090316561847', 0, '2024-08-13 07:00:19'),
	('DM2024090316561797', 0, '2024-08-13 07:00:19'),
	('h22024090316561802', 0, '2024-08-13 07:00:19'),
	('Hi2024090316561815', 0, '2024-08-13 07:00:19'),
	('KQVu20240903154415', 0, '2024-11-03 10:25:10'),
	('Nu2024090315521417', 0, '2024-11-03 10:25:10'),
	('sA2024090316561826', 0, '2024-08-13 07:00:19'),
	('Tq2024090316561818', 0, '2024-08-13 07:00:19'),
	('Ua2024090316561837', 0, '2024-08-13 07:00:19'),
	('Xr2024090316444625', 0, '2024-08-13 07:00:19');

-- Dumping structure for table baucutructuyen.quanhuyen
CREATE TABLE IF NOT EXISTS `quanhuyen` (
  `ID_QH` int(11) NOT NULL AUTO_INCREMENT,
  `TenQH` varchar(50) NOT NULL,
  `STT` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`ID_QH`),
  KEY `STT` (`STT`),
  CONSTRAINT `quanhuyen_ibfk_1` FOREIGN KEY (`STT`) REFERENCES `tinhthanh` (`STT`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.quanhuyen: ~26 rows (approximately)
INSERT INTO `quanhuyen` (`ID_QH`, `TenQH`, `STT`) VALUES
	(1, 'Quận Ninh Kiều', 65),
	(2, 'Quận Bình Thủy', 65),
	(3, 'Quận Cái Răng', 65),
	(4, 'Quận Ô Môn', 65),
	(5, 'Quận Thốt Nốt', 65),
	(6, 'Huyện Cờ Đỏ', 65),
	(7, 'Huyện Phong Điền', 65),
	(8, 'Huyện Thới Lai', 65),
	(9, 'Huyện Vĩnh Thạnh', 65),
	(10, 'Huyện Cái Nước', 69),
	(11, 'Huyện Đầm Dơi', 69),
	(12, 'Huyện Năm Căn', 69),
	(13, 'Huyện Ngọc Hiển', 69),
	(14, 'Huyện Phú Tân', 69),
	(15, 'Huyện Thới Bình', 69),
	(16, 'Huyện Trần Văn Thời', 69),
	(17, 'Huyện U Minh', 69),
	(18, 'Thành Phố Cà Mau', 69),
	(19, 'Huyện Vĩnh Lợi', 94),
	(20, 'Huyện Hông Dân', 94),
	(21, 'Huyện Hòa Bình', 94),
	(22, 'Huyện Phước Long', 94),
	(23, 'Thị xã Giá Rai', 94),
	(24, 'Huyện Đông Hải', 94),
	(25, 'Thành Phố Bạc Liêu', 94),
	(28, 'Test5555', 69);

-- Dumping structure for table baucutructuyen.refreshtoken
CREATE TABLE IF NOT EXISTS `refreshtoken` (
  `IDrfToken` int(11) NOT NULL AUTO_INCREMENT,
  `token` text DEFAULT NULL,
  `JwtId` varchar(255) DEFAULT NULL,
  `IsUsed` bit(1) DEFAULT NULL,
  `IsRevoked` bit(1) DEFAULT NULL,
  `IssuedAt` datetime DEFAULT NULL,
  `ExpiredAt` datetime DEFAULT NULL,
  `taikhoan` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`IDrfToken`),
  UNIQUE KEY `taikhoan` (`taikhoan`),
  CONSTRAINT `refreshtoken_ibfk_1` FOREIGN KEY (`taikhoan`) REFERENCES `taikhoan` (`TaiKhoan`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.refreshtoken: ~0 rows (approximately)
INSERT INTO `refreshtoken` (`IDrfToken`, `token`, `JwtId`, `IsUsed`, `IsRevoked`, `IssuedAt`, `ExpiredAt`, `taikhoan`) VALUES
	(1, 'YedpECSveCZQDCcnNA7PO3ocdX8xg1VE4h1eFP+JEdM=', 'bbe2b636-19d6-4153-8e19-060bb6a3778a', b'0', b'0', '2024-09-18 02:19:11', '2024-09-18 12:19:11', 'admin');

-- Dumping structure for table baucutructuyen.taikhoan
CREATE TABLE IF NOT EXISTS `taikhoan` (
  `TaiKhoan` varchar(30) NOT NULL,
  `MatKhau` text NOT NULL,
  `BiKhoa` varchar(1) DEFAULT NULL,
  `LyDoKhoa` varchar(100) DEFAULT NULL,
  `NgayTao` datetime DEFAULT NULL,
  `SuDung` tinyint(4) DEFAULT NULL,
  `RoleID` tinyint(4) NOT NULL,
  PRIMARY KEY (`TaiKhoan`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.taikhoan: ~3 rows (approximately)
INSERT INTO `taikhoan` (`TaiKhoan`, `MatKhau`, `BiKhoa`, `LyDoKhoa`, `NgayTao`, `SuDung`, `RoleID`) VALUES
	('0974000252', '88888888', '0', 'null', '2024-09-16 23:21:32', 1, 5),
	('0974000352', '88888888', '0', 'null', '2024-09-16 22:10:39', 1, 5),
	('0974000452', '88888888', '0', 'null', '2024-09-16 22:08:19', 1, 5),
	('admin', '$argon2d$v=19$m=12,t=3,p=1$OWhmdmhzZ2Zld24wMDAwMA$5b+goGZlUR9dYxmBHMxjyQ', '0', NULL, NULL, 1, 1);

-- Dumping structure for table baucutructuyen.thongbao
CREATE TABLE IF NOT EXISTS `thongbao` (
  `ID_ThongBao` int(11) NOT NULL AUTO_INCREMENT,
  `NoiDungThongBao` varchar(255) DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_ThongBao`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.thongbao: ~3 rows (approximately)
INSERT INTO `thongbao` (`ID_ThongBao`, `NoiDungThongBao`, `ThoiDiem`) VALUES
	(1, 'Ngày bầu cử', '2024-12-18 10:15:44'),
	(2, 'Ngay đắt cử', '2024-12-18 11:30:44'),
	(3, 'Chuẩn bị cuộc bầu cử', '2024-10-02 09:21:34');

-- Dumping structure for table baucutructuyen.tinhthanh
CREATE TABLE IF NOT EXISTS `tinhthanh` (
  `STT` tinyint(4) NOT NULL,
  `TenTinhThanh` varchar(50) NOT NULL,
  PRIMARY KEY (`STT`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.tinhthanh: ~83 rows (approximately)
INSERT INTO `tinhthanh` (`STT`, `TenTinhThanh`) VALUES
	(0, 'Test'),
	(1, 'Test2'),
	(2, 'Test2'),
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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.trinhdohocvan: ~8 rows (approximately)
INSERT INTO `trinhdohocvan` (`ID_TrinhDo`, `TenTrinhDoHocVan`) VALUES
	(1, 'Tiểu học'),
	(2, ' Trung học cơ sở'),
	(3, ' Trung học phổ thông'),
	(4, 'Cao đẳng'),
	(5, 'Đại học'),
	(6, 'Tiến sĩ'),
	(7, 'Trung cấp chuyên nghiệp'),
	(8, 'Nghiên cứu sinh');

-- Dumping structure for table baucutructuyen.ungcuvien
CREATE TABLE IF NOT EXISTS `ungcuvien` (
  `ID_ucv` varchar(14) NOT NULL,
  `TrangThai` varchar(10) DEFAULT NULL,
  `ID_user` varchar(16) DEFAULT NULL,
  PRIMARY KEY (`ID_ucv`),
  KEY `FK_userUngCuVien` (`ID_user`),
  CONSTRAINT `FK_userUngCuVien` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.vaitro
CREATE TABLE IF NOT EXISTS `vaitro` (
  `RoleID` tinyint(4) NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.vaitro: ~6 rows (approximately)
INSERT INTO `vaitro` (`RoleID`, `TenVaiTro`) VALUES
	(1, 'Admin'),
	(2, 'Ứng cử viên'),
	(3, 'Ban Tổ chức'),
	(4, 'Ban kiểm phiếu'),
	(5, 'Cử tri'),
	(8, 'Cán bộ');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
