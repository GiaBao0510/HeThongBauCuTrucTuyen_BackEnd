-- --------------------------------------------------------
-- Máy chủ:                      localhost
-- Server version:               8.0.39 - MySQL Community Server - GPL
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
CREATE DATABASE IF NOT EXISTS `baucutructuyen` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `baucutructuyen`;

-- Dumping structure for table baucutructuyen.ban
CREATE TABLE IF NOT EXISTS `ban` (
  `ID_Ban` smallint NOT NULL AUTO_INCREMENT,
  `TenBan` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_DonViBauCu` smallint DEFAULT NULL,
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
  `ID_CanBo` varchar(14) COLLATE utf8mb4_general_ci NOT NULL,
  `NgayCongTac` datetime DEFAULT NULL,
  `GhiChu` text COLLATE utf8mb4_general_ci,
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_CanBo`),
  KEY `ID_user` (`ID_user`),
  CONSTRAINT `canbo_ibfk_1` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.canbo: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietbaucu
CREATE TABLE IF NOT EXISTS `chitietbaucu` (
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_Phieu` varchar(18) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_Phieu` (`ID_Phieu`),
  CONSTRAINT `chitietbaucu_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `chitietbaucu_ibfk_2` FOREIGN KEY (`ID_Phieu`) REFERENCES `phieubau` (`ID_Phieu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietbaucu: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietcutri
CREATE TABLE IF NOT EXISTS `chitietcutri` (
  `ID_ChucVu` tinyint DEFAULT NULL,
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `chitietcutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietcutri: ~4 rows (approximately)
INSERT INTO `chitietcutri` (`ID_ChucVu`, `ID_CuTri`) VALUES
	(13, '20240919194845'),
	(13, '20240919200022'),
	(13, '20240930165456'),
	(13, '20240930165802');

-- Dumping structure for table baucutructuyen.chitietphieubau
CREATE TABLE IF NOT EXISTS `chitietphieubau` (
  `BinhChon` char(1) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_Phieu` varchar(18) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_Khoa` int DEFAULT NULL,
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
  `ID_ThongBao` int DEFAULT NULL,
  `ID_CanBo` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ThongBao` (`ID_ThongBao`),
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `chitietthongbaocanbo_ibfk_1` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaocanbo_ibfk_2` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaocanbo: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietthongbaocutri
CREATE TABLE IF NOT EXISTS `chitietthongbaocutri` (
  `ID_ThongBao` int DEFAULT NULL,
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaocutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `chitietthongbaocutri_ibfk_2` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaocutri: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietthongbaoungcuvien
CREATE TABLE IF NOT EXISTS `chitietthongbaoungcuvien` (
  `ID_ThongBao` int DEFAULT NULL,
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_2` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaoungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvancanbo
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvancanbo` (
  `ID_TrinhDo` smallint DEFAULT NULL,
  `ID_CanBo` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_1` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_2` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitiettrinhdohocvancanbo: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvanungcuvien
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvanungcuvien` (
  `ID_TrinhDo` smallint DEFAULT NULL,
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_2` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitiettrinhdohocvanungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitietungcuvien
CREATE TABLE IF NOT EXISTS `chitietungcuvien` (
  `ID_ChucVu` tinyint DEFAULT NULL,
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  CONSTRAINT `chitietungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chucvu
CREATE TABLE IF NOT EXISTS `chucvu` (
  `ID_ChucVu` tinyint NOT NULL AUTO_INCREMENT,
  `TenChucVu` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_ChucVu`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chucvu: ~12 rows (approximately)
INSERT INTO `chucvu` (`ID_ChucVu`, `TenChucVu`) VALUES
	(1, 'Hiệu trưởng trường đại học'),
	(2, 'Phó hiệu trưởng trường đại học'),
	(3, 'Phó hiệu trưởng trường CNTT&TT'),
	(4, 'Hiệu trưởng trường CNTT&TT'),
	(5, 'Trưởng khoa CNTT&TT'),
	(6, 'Trưởng khoa Hệ thống thông tin'),
	(7, 'Trưởng khoa Khoa học máy tính'),
	(9, 'Giảng viên mời dạy'),
	(10, 'Trưởng lớp Khoa Khoa học máy tính'),
	(11, 'Phó trưởng lớp Khoa Khoa học máy tính'),
	(12, 'Không có chức vụ'),
	(13, 'Sinh viên'),
	(14, 'Giảng viên');

-- Dumping structure for table baucutructuyen.cutri
CREATE TABLE IF NOT EXISTS `cutri` (
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci NOT NULL,
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_CuTri`),
  KEY `FK_userCuTri` (`ID_user`),
  CONSTRAINT `FK_userCuTri` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.cutri: ~7 rows (approximately)
INSERT INTO `cutri` (`ID_CuTri`, `ID_user`) VALUES
	('20240919200022', 'i420240919200017'),
	('20240919194845', 'iZ20240919194839'),
	('20240930165456', 'jp20240930165451'),
	('20240916221039', 'Pe20240916221033'),
	('20240916220819', 'sT20240916220819'),
	('20240930165802', 'uB20240930165759'),
	('20240916232132', 'Zh20240916232125');

-- Dumping structure for table baucutructuyen.danhmucungcu
CREATE TABLE IF NOT EXISTS `danhmucungcu` (
  `ID_Cap` smallint NOT NULL AUTO_INCREMENT,
  `TenCapUngCu` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_DonViBauCu` smallint DEFAULT NULL,
  PRIMARY KEY (`ID_Cap`),
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  CONSTRAINT `danhmucungcu_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.danhmucungcu: ~8 rows (approximately)
INSERT INTO `danhmucungcu` (`ID_Cap`, `TenCapUngCu`, `ID_DonViBauCu`) VALUES
	(1, 'Trưởng khoa', 3),
	(2, 'Phó trưởng khoa', 3),
	(3, 'Phó trưởng khoa', 1),
	(4, 'Hiệu trưởng', 9),
	(5, 'Phó hiệu trưởng', 9),
	(7, 'Trưởng Lớp A2 Khoa Khoa Học Máy Tính', 9),
	(8, 'Trưởng Lớp A3 Khoa Khoa Học Máy Tính', 9),
	(9, 'Trưởng Lớp A1 Khoa Khoa Học Máy Tính', 9);

-- Dumping structure for table baucutructuyen.dantoc
CREATE TABLE IF NOT EXISTS `dantoc` (
  `ID_DanToc` tinyint NOT NULL AUTO_INCREMENT,
  `TenDanToc` varchar(20) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `TenGoiKhac` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
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
  `ID_QH` int DEFAULT NULL,
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidungtamtru` (`ID_user`),
  CONSTRAINT `diachitamtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidungtamtru` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.diachitamtru: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.diachithuongtru
CREATE TABLE IF NOT EXISTS `diachithuongtru` (
  `ID_QH` int DEFAULT NULL,
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidung` (`ID_user`),
  CONSTRAINT `diachithuongtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidung` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.diachithuongtru: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.donvibaucu
CREATE TABLE IF NOT EXISTS `donvibaucu` (
  `ID_DonViBauCu` smallint NOT NULL AUTO_INCREMENT,
  `TenDonViBauCu` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `DiaChi` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_QH` int DEFAULT NULL,
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
  `ID_canbo` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_ChucVu` tinyint DEFAULT NULL,
  `ID_Ban` smallint DEFAULT NULL,
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
  `MaSo` int NOT NULL AUTO_INCREMENT,
  `TrangThaiDangKy` varchar(1) COLLATE utf8mb4_general_ci DEFAULT '0',
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`MaSo`),
  KEY `ID_user` (`ID_user`),
  CONSTRAINT `hosonguoidung_ibfk_1` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.hosonguoidung: ~12 rows (approximately)
INSERT INTO `hosonguoidung` (`MaSo`, `TrangThaiDangKy`, `ID_user`) VALUES
	(1, '0', 'sT20240916220819'),
	(2, '0', 'Pe20240916221033'),
	(3, '0', 'Zh20240916232125'),
	(6, '0', 'iZ20240919194839'),
	(7, '0', 'i420240919200017'),
	(8, '0', 'jp20240930165451'),
	(9, '0', 'uB20240930165759'),
	(10, '1', 'Ds20241003000915'),
	(11, '1', '7H20241003002115'),
	(12, '1', 'Gu20241003002922'),
	(13, '1', 'RM20241003003046'),
	(14, '1', 'Do20241003003139');

-- Dumping structure for table baucutructuyen.ketquabaucu
CREATE TABLE IF NOT EXISTS `ketquabaucu` (
  `SoLuotBinhChon` int DEFAULT NULL,
  `ThoiDiemDangKy` datetime DEFAULT NULL,
  `TyLeBinhChon` float DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_Cap` smallint DEFAULT NULL,
  KEY `ID_Cap` (`ID_Cap`),
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `ketquabaucu_ibfk_1` FOREIGN KEY (`ID_Cap`) REFERENCES `danhmucungcu` (`ID_Cap`),
  CONSTRAINT `ketquabaucu_ibfk_2` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`),
  CONSTRAINT `ketquabaucu_ibfk_3` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ketquabaucu: ~4 rows (approximately)
INSERT INTO `ketquabaucu` (`SoLuotBinhChon`, `ThoiDiemDangKy`, `TyLeBinhChon`, `ngayBD`, `ID_ucv`, `ID_Cap`) VALUES
	(0, '2024-10-03 00:09:22', 0, '2024-11-03 10:15:00', '20241003000921', 1),
	(0, '2024-10-03 00:21:22', 0, '2024-11-03 10:15:00', '20241003002121', 1),
	(0, '2024-10-03 00:29:30', 0, '2024-11-03 10:15:00', '20241003002930', 1),
	(0, '2024-10-03 00:30:51', 0, '2024-11-03 10:15:00', '20241003003050', 1),
	(0, '2024-10-03 00:31:43', 0, '2024-11-03 10:15:00', '20241003003142', 1);

-- Dumping structure for table baucutructuyen.khoa
CREATE TABLE IF NOT EXISTS `khoa` (
  `ID_Khoa` int NOT NULL AUTO_INCREMENT,
  `NgayTao` date DEFAULT NULL,
  `NgayHetHan` date DEFAULT NULL,
  PRIMARY KEY (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoa: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.khoabimat
CREATE TABLE IF NOT EXISTS `khoabimat` (
  `HamCamichanel` int DEFAULT NULL,
  `GiaTriB_Phan` int DEFAULT NULL,
  `ID_Khoa` int DEFAULT NULL,
  KEY `ID_Khoa` (`ID_Khoa`),
  CONSTRAINT `khoabimat_ibfk_1` FOREIGN KEY (`ID_Khoa`) REFERENCES `khoa` (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoabimat: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.khoacongkhai
CREATE TABLE IF NOT EXISTS `khoacongkhai` (
  `Modulo` int DEFAULT NULL,
  `SemiRandom_g` int DEFAULT NULL,
  `ID_Khoa` int DEFAULT NULL,
  KEY `ID_Khoa` (`ID_Khoa`),
  CONSTRAINT `khoacongkhai_ibfk_1` FOREIGN KEY (`ID_Khoa`) REFERENCES `khoa` (`ID_Khoa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoacongkhai: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.kybaucu
CREATE TABLE IF NOT EXISTS `kybaucu` (
  `ngayBD` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ngayKT` datetime DEFAULT NULL,
  `NgayKT_UngCu` datetime DEFAULT NULL,
  `TenKyBauCu` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
  `MoTa` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `SoLuongToiDaCuTri` int DEFAULT NULL,
  `SoLuongToiDaUngCuVien` int DEFAULT NULL,
  `SoLuotBinhChonToiDa` int DEFAULT NULL,
  PRIMARY KEY (`ngayBD`),
  CONSTRAINT `check_dates` CHECK ((`ngayKT` > `ngayBD`)),
  CONSTRAINT `Chk_kybaucu` CHECK ((`SoLuongToiDaCuTri` > `SoLuongToiDaUngCuVien`)),
  CONSTRAINT `Chk_kybaucuSoLuotBinhChon` CHECK ((`SoLuotBinhChonToiDa` < `SoLuongToiDaUngCuVien`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.kybaucu: ~6 rows (approximately)
INSERT INTO `kybaucu` (`ngayBD`, `ngayKT`, `NgayKT_UngCu`, `TenKyBauCu`, `MoTa`, `SoLuongToiDaCuTri`, `SoLuongToiDaUngCuVien`, `SoLuotBinhChonToiDa`) VALUES
	('2024-08-13 07:00:19', '2024-12-18 07:00:19', '2024-07-03 20:02:34', 'Bảo vệ luận văn', '', 30, 12, 5),
	('2024-09-19 23:54:51', '2024-11-30 12:14:00', '2024-07-03 20:02:33', 'Bao', '', 50, 10, 5),
	('2024-11-03 10:15:00', '2024-11-15 12:10:00', '2024-07-03 20:02:35', 'Bầu cử trưởng thôn', 'okok', 20, 5, 3),
	('2024-11-03 10:25:10', '2024-11-15 12:10:00', '2024-07-03 20:02:36', 'Bầu cử trưởng làng', '', 10, 7, 3),
	('2024-11-03 10:25:11', '2024-11-30 12:14:00', '2024-07-03 20:02:37', 'Bầu cử thị trưởng', '', 15, 4, 2),
	('2024-11-05 00:00:00', '2024-12-31 00:00:00', '2024-07-03 20:02:38', 'Bầu cử', '', 100, 8, 3);

-- Dumping structure for table baucutructuyen.lichsudangnhap
CREATE TABLE IF NOT EXISTS `lichsudangnhap` (
  `ThoiDiem` datetime DEFAULT NULL,
  `DiaChiIP` varchar(30) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `TaiKhoan` varchar(30) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `TaiKhoan` (`TaiKhoan`),
  CONSTRAINT `lichsudangnhap_ibfk_1` FOREIGN KEY (`TaiKhoan`) REFERENCES `taikhoan` (`TaiKhoan`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.lichsudangnhap: ~24 rows (approximately)
INSERT INTO `lichsudangnhap` (`ThoiDiem`, `DiaChiIP`, `TaiKhoan`) VALUES
	('2024-09-20 00:00:00', '0.0.0.1', 'admin2'),
	('2024-09-20 14:09:21', '0.0.0.1', 'admin2'),
	('2024-09-20 21:10:53', '0.0.0.1', 'admin2'),
	('2024-09-21 23:13:22', '0.0.0.1', 'admin2'),
	('2024-09-22 19:10:53', '0.0.0.1', 'admin2'),
	('2024-09-22 19:36:43', '0.0.0.1', 'admin2'),
	('2024-09-22 20:08:02', '0.0.0.1', 'admin2'),
	('2024-09-22 20:23:05', '0.0.0.1', 'admin2'),
	('2024-09-22 20:51:38', '0.0.0.1', 'admin2'),
	('2024-09-23 14:53:36', '0.0.0.1', 'admin2'),
	('2024-09-27 16:13:03', '0.0.0.1', 'admin2'),
	('2024-09-27 21:30:32', '0.0.0.1', 'admin2'),
	('2024-09-28 13:11:22', '0.0.0.1', 'admin2'),
	('2024-09-28 16:25:37', '0.0.0.1', 'admin2'),
	('2024-09-28 16:25:50', '0.0.0.1', 'admin2'),
	('2024-09-30 16:54:13', '127.0.0.1', 'admin2'),
	('2024-09-30 16:59:17', '127.0.0.1', '0974000752'),
	('2024-10-02 21:21:55', '0.0.0.1', 'admin2'),
	('2024-10-02 22:33:35', '127.0.0.1', 'admin2'),
	('2024-10-03 15:23:08', '127.0.0.1', 'admin2'),
	('2024-10-03 18:49:17', '127.0.0.1', 'admin2'),
	('2024-10-04 15:06:59', '127.0.0.1', 'admin2'),
	('2024-10-04 23:46:16', '127.0.0.1', 'admin2'),
	('2024-10-05 13:47:26', '127.0.0.1', 'admin2');

-- Dumping structure for table baucutructuyen.nguoidung
CREATE TABLE IF NOT EXISTS `nguoidung` (
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci NOT NULL,
  `HoTen` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `GioiTinh` varchar(1) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `NgaySinh` datetime DEFAULT NULL,
  `DiaChiLienLac` varchar(150) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `CCCD` varchar(12) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `Email` varchar(80) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `SDT` varchar(10) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `HinhAnh` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `PublicID` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_DanToc` tinyint DEFAULT NULL,
  `RoleID` tinyint DEFAULT NULL,
  PRIMARY KEY (`ID_user`),
  UNIQUE KEY `CCCD` (`CCCD`),
  UNIQUE KEY `Email` (`Email`),
  UNIQUE KEY `SDT` (`SDT`),
  KEY `ID_DanToc` (`ID_DanToc`),
  KEY `fk_vaitronguoidung` (`RoleID`),
  CONSTRAINT `fk_vaitronguoidung` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`),
  CONSTRAINT `nguoidung_ibfk_1` FOREIGN KEY (`ID_DanToc`) REFERENCES `dantoc` (`ID_DanToc`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.nguoidung: ~13 rows (approximately)
INSERT INTO `nguoidung` (`ID_user`, `HoTen`, `GioiTinh`, `NgaySinh`, `DiaChiLienLac`, `CCCD`, `Email`, `SDT`, `HinhAnh`, `PublicID`, `ID_DanToc`, `RoleID`) VALUES
	('7H20241003002115', 'NguyenVanB', '1', '2002-01-01 00:00:00', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000652', 'nguyenvanb@gmail.com', '040714008', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727889680/NguoiDung/mecgdcdk13queaimrygw.jpg', 'NguoiDung/mecgdcdk13queaimrygw', 1, 2),
	('Do20241003003139', 'NguyenVanE', '1', '2002-01-01 00:00:00', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000952', 'nguyenvane@gmail.com', '040714011', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890302/NguoiDung/svxuiuxvdgjcm9wm49v7.jpg', 'NguoiDung/svxuiuxvdgjcm9wm49v7', 1, 2),
	('Ds20241003000915', 'NguyenVanA', '1', '2002-01-01 00:00:00', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000552', 'nguyenvana@gmail.com', '040714007', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727888960/NguoiDung/zztvpcs0hopvem3gsnel.jpg', 'NguoiDung/zztvpcs0hopvem3gsnel', 1, 2),
	('Gu20241003002922', 'NguyenVanC', '1', '2002-01-01 00:00:00', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000752', 'nguyenvanc@gmail.com', '040714009', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890169/NguoiDung/cu16bwbu1tbr3jhq47yp.jpg', 'NguoiDung/cu16bwbu1tbr3jhq47yp', 1, 2),
	('i420240919200017', 'Trần Lộc Đỉnh', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', '123456789', 'tldinh2002@gmail.com', '0974000552', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726750819/NguoiDung/giajvu1tmzpixlqhxmyb.jpg', 'NguoiDung/giajvu1tmzpixlqhxmyb', 1, 5),
	('iZ20240919194839', 'Võ Hoàng Tuấn Đạt', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'vhtdat2002@gmail.com', '0974000152', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726750121/NguoiDung/b0pyecdhyid0q37zlpy2.jpg', 'NguoiDung/b0pyecdhyid0q37zlpy2', 1, 5),
	('jp20240930165451', 'Hồ Minh Trường', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'httruong2002@gmail.com', '0974000652', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727690096/NguoiDung/cpvj0gxxgzyevrbw7hgl.jpg', 'NguoiDung/cpvj0gxxgzyevrbw7hgl', 1, 5),
	('Pe20240916221033', 'Lê Hữu Đức', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000322327445', 'ducb2013070@student.ctu.edu.vn', '0974000352', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726499436/NguoiDung/nhdc4uzjfwwpwmbcmcpr.jpg', 'NguoiDung/nhdc4uzjfwwpwmbcmcpr', 1, 5),
	('RM20241003003046', 'NguyenVanD', '1', '2002-01-01 00:00:00', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000852', 'nguyenvand@gmail.com', '040714010', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890250/NguoiDung/bun34h6wktbzda5o5tug.jpg', 'NguoiDung/bun34h6wktbzda5o5tug', 1, 2),
	('rY20240918210506', 'Admin_2', '1', '2002-10-19 00:00:00', 'Q.Ninh Kiều, tp.Càn Thơ', '10000000008', 'pgbaop4@gmail.com', 'admin2', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726668312/NguoiDung/whyuizgynfpg63zrogdp.jpg', 'NguoiDung/whyuizgynfpg63zrogdp', 1, 1),
	('sT20240916220819', 'Lý Gia Nguyên', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000422327445', 'pgiabao2002@gmail.com', '0974000452', NULL, NULL, 1, 5),
	('uB20240930165759', 'Trần Vĩnh', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'baob2016947@student.ctu.edu.vn', '0974000752', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727690282/NguoiDung/r7stiwbtawuff8xcjqhn.jpg', 'NguoiDung/r7stiwbtawuff8xcjqhn', 1, 5),
	('Zh20240916232125', 'Phạm Thế HIển', '1', '2024-10-29 00:00:00', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000122327445', 'baob2016987@student.ctu.edu.vn', '0974000252', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726503689/NguoiDung/uiumpqhsuwsfri9anamz.jpg', 'NguoiDung/uiumpqhsuwsfri9anamz', 1, 5);

-- Dumping structure for table baucutructuyen.phanhoicanbo
CREATE TABLE IF NOT EXISTS `phanhoicanbo` (
  `YKien` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_CanBo` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `phanhoicanbo_ibfk_1` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoicanbo: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phanhoicutri
CREATE TABLE IF NOT EXISTS `phanhoicutri` (
  `Ykien` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `phanhoicutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoicutri: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phanhoiungcuvien
CREATE TABLE IF NOT EXISTS `phanhoiungcuvien` (
  `Ykien` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` date DEFAULT NULL,
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  CONSTRAINT `phanhoiungcuvien_ibfk_1` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoiungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phieubau
CREATE TABLE IF NOT EXISTS `phieubau` (
  `ID_Phieu` varchar(18) COLLATE utf8mb4_general_ci NOT NULL,
  `GiaTriPhieuBau` bigint DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  `ID_cap` smallint DEFAULT NULL,
  PRIMARY KEY (`ID_Phieu`),
  KEY `ngayBD` (`ngayBD`),
  KEY `FK_DMUC_Phieu` (`ID_cap`),
  CONSTRAINT `FK_DMUC_Phieu` FOREIGN KEY (`ID_cap`) REFERENCES `danhmucungcu` (`ID_Cap`),
  CONSTRAINT `phieubau_ibfk_1` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phieubau: ~87 rows (approximately)
INSERT INTO `phieubau` (`ID_Phieu`, `GiaTriPhieuBau`, `ngayBD`, `ID_cap`) VALUES
	('112024100514214409', 0, '2024-09-19 23:54:51', 8),
	('3l2024100513473999', 0, '2024-11-03 10:15:00', 3),
	('3N2024100514214400', 0, '2024-09-19 23:54:51', 8),
	('4p2024100514214416', 0, '2024-09-19 23:54:51', 8),
	('6l2024100513513366', 0, '2024-11-03 10:25:11', 7),
	('6V2024100514214418', 0, '2024-09-19 23:54:51', 8),
	('6z2024100513474002', 0, '2024-11-03 10:15:00', 3),
	('7f2024100514214404', 0, '2024-09-19 23:54:51', 8),
	('7f2024100514214422', 0, '2024-09-19 23:54:51', 8),
	('7k2024100514215176', 0, '2024-09-19 23:54:51', 8),
	('8N2024100514214402', 0, '2024-09-19 23:54:51', 8),
	('8R2024100514214416', 0, '2024-09-19 23:54:51', 8),
	('8Y2024100514214413', 0, '2024-09-19 23:54:51', 8),
	('902024100514214406', 0, '2024-09-19 23:54:51', 8),
	('A72024100514214420', 0, '2024-09-19 23:54:51', 8),
	('aB2024100514214417', 0, '2024-09-19 23:54:51', 8),
	('AT2024100514215180', 0, '2024-09-19 23:54:51', 8),
	('AU2024100513513372', 0, '2024-11-03 10:25:11', 7),
	('BL2024100513473994', 0, '2024-11-03 10:15:00', 3),
	('bn2024100514214409', 0, '2024-09-19 23:54:51', 8),
	('bq2024100513513366', 0, '2024-11-03 10:25:11', 7),
	('cB2024100513473995', 0, '2024-11-03 10:15:00', 3),
	('ch2024100514214419', 0, '2024-09-19 23:54:51', 8),
	('cj2024100513473992', 0, '2024-11-03 10:15:00', 3),
	('cS2024100513473993', 0, '2024-11-03 10:15:00', 3),
	('db2024100513473998', 0, '2024-11-03 10:15:00', 3),
	('Dv2024100514215176', 0, '2024-09-19 23:54:51', 8),
	('Eb2024100514214410', 0, '2024-09-19 23:54:51', 8),
	('eC2024100514214412', 0, '2024-09-19 23:54:51', 8),
	('EQ2024100513473998', 0, '2024-11-03 10:15:00', 3),
	('ev2024100514214419', 0, '2024-09-19 23:54:51', 8),
	('FL2024100514215176', 0, '2024-09-19 23:54:51', 8),
	('fW2024100513513371', 0, '2024-11-03 10:25:11', 7),
	('G22024100513513370', 0, '2024-11-03 10:25:11', 7),
	('G42024100513513367', 0, '2024-11-03 10:25:11', 7),
	('ga2024100514214415', 0, '2024-09-19 23:54:51', 8),
	('gq2024100513473990', 0, '2024-11-03 10:15:00', 3),
	('h22024100513513375', 0, '2024-11-03 10:25:11', 7),
	('Hd2024100513473995', 0, '2024-11-03 10:15:00', 3),
	('hj2024100513513369', 0, '2024-11-03 10:25:11', 7),
	('Ic2024100514215179', 0, '2024-09-19 23:54:51', 8),
	('jA2024100514214412', 0, '2024-09-19 23:54:51', 8),
	('ji2024100514214420', 0, '2024-09-19 23:54:51', 8),
	('jz2024100513473995', 0, '2024-11-03 10:15:00', 3),
	('KM2024100513513374', 0, '2024-11-03 10:25:11', 7),
	('LI2024100513473994', 0, '2024-11-03 10:15:00', 3),
	('lK2024100514215181', 0, '2024-09-19 23:54:51', 8),
	('lq2024100513474001', 0, '2024-11-03 10:15:00', 3),
	('lW2024100513473999', 0, '2024-11-03 10:15:00', 3),
	('lx2024100514214421', 0, '2024-09-19 23:54:51', 8),
	('m02024100514214409', 0, '2024-09-19 23:54:51', 8),
	('m62024100513513374', 0, '2024-11-03 10:25:11', 7),
	('M72024100514214421', 0, '2024-09-19 23:54:51', 8),
	('mD2024100514215175', 0, '2024-09-19 23:54:51', 8),
	('MN2024100513513366', 0, '2024-11-03 10:25:11', 7),
	('n32024100513513369', 0, '2024-11-03 10:25:11', 7),
	('n52024100514214406', 0, '2024-09-19 23:54:51', 8),
	('NK2024100514214413', 0, '2024-09-19 23:54:51', 8),
	('NT2024100513513370', 0, '2024-11-03 10:25:11', 7),
	('oF2024100514214404', 0, '2024-09-19 23:54:51', 8),
	('On2024100513473998', 0, '2024-11-03 10:15:00', 3),
	('Q32024100514214411', 0, '2024-09-19 23:54:51', 8),
	('QM2024100514214419', 0, '2024-09-19 23:54:51', 8),
	('QZ2024100513474000', 0, '2024-11-03 10:15:00', 3),
	('rc2024100514214418', 0, '2024-09-19 23:54:51', 8),
	('RG2024100513513371', 0, '2024-11-03 10:25:11', 7),
	('rp2024100514214421', 0, '2024-09-19 23:54:51', 8),
	('Rq2024100514214411', 0, '2024-09-19 23:54:51', 8),
	('sK2024100514214417', 0, '2024-09-19 23:54:51', 8),
	('Sq2024100513513371', 0, '2024-11-03 10:25:11', 7),
	('Tn2024100514214414', 0, '2024-09-19 23:54:51', 8),
	('TR2024100513473993', 0, '2024-11-03 10:15:00', 3),
	('Tw2024100514214415', 0, '2024-09-19 23:54:51', 8),
	('uK2024100513513374', 0, '2024-11-03 10:25:11', 7),
	('UX2024100514214413', 0, '2024-09-19 23:54:51', 8),
	('uz2024100514215179', 0, '2024-09-19 23:54:51', 8),
	('V72024100514215181', 0, '2024-09-19 23:54:51', 8),
	('vU2024100514215173', 0, '2024-09-19 23:54:51', 8),
	('w32024100514214410', 0, '2024-09-19 23:54:51', 8),
	('WQ2024100513473982', 0, '2024-11-03 10:15:00', 3),
	('wW2024100514214414', 0, '2024-09-19 23:54:51', 8),
	('XV2024100513474001', 0, '2024-11-03 10:15:00', 3),
	('Y32024100514214411', 0, '2024-09-19 23:54:51', 8),
	('yn2024100514214406', 0, '2024-09-19 23:54:51', 8),
	('z72024100514214417', 0, '2024-09-19 23:54:51', 8),
	('ZD2024100514215174', 0, '2024-09-19 23:54:51', 8),
	('ZX2024100513474001', 0, '2024-11-03 10:15:00', 3);

-- Dumping structure for table baucutructuyen.quanhuyen
CREATE TABLE IF NOT EXISTS `quanhuyen` (
  `ID_QH` int NOT NULL AUTO_INCREMENT,
  `TenQH` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
  `STT` tinyint DEFAULT NULL,
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

-- Dumping structure for table baucutructuyen.taikhoan
CREATE TABLE IF NOT EXISTS `taikhoan` (
  `TaiKhoan` varchar(30) COLLATE utf8mb4_general_ci NOT NULL,
  `MatKhau` text COLLATE utf8mb4_general_ci NOT NULL,
  `BiKhoa` varchar(1) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `LyDoKhoa` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `NgayTao` datetime DEFAULT NULL,
  `SuDung` tinyint DEFAULT NULL,
  `RoleID` tinyint NOT NULL,
  PRIMARY KEY (`TaiKhoan`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.taikhoan: ~12 rows (approximately)
INSERT INTO `taikhoan` (`TaiKhoan`, `MatKhau`, `BiKhoa`, `LyDoKhoa`, `NgayTao`, `SuDung`, `RoleID`) VALUES
	('040714007', '$argon2id$v=19$m=65536,t=3,p=1$LnAOv4nQnHKrtc/QfRGRRg$lCBt51MfGEerZKFngJRzvAIaWP/aYAWF6KEtd6fG100', '0', 'null', '2024-10-03 00:09:21', 1, 2),
	('040714008', '$argon2id$v=19$m=65536,t=3,p=1$uoUAWWz7IR7LR221OAcj1A$RR0TqvUb2i3upCD3b10Pz6cJIMnUbbIkvuhbwTwoHEA', '0', 'null', '2024-10-03 00:21:21', 1, 2),
	('040714009', '$argon2id$v=19$m=65536,t=3,p=1$4nF/mCkowPm5DsCWmC6Zdg$Uee1SCnFGoGIST4pskxdkbgUSAJ8X6z6YdfT7fzKS08', '0', 'null', '2024-10-03 00:29:29', 1, 2),
	('040714010', '$argon2id$v=19$m=65536,t=3,p=1$Si2i7IdKVfmaAHiDtUgtIg$z8O4S0SKmT0mPS1dYOxclydwnx2/G7YpMSDeE+u3Qew', '0', 'null', '2024-10-03 00:30:50', 1, 2),
	('040714011', '$argon2id$v=19$m=65536,t=3,p=1$YC0wZRm2qHSjwjiji6mWgw$nh6QffMQgH6n2M3qZ7SOgQKfXVdH96qRvjty5L8bD7s', '0', 'null', '2024-10-03 00:31:42', 1, 2),
	('0974000152', '$argon2id$v=19$m=65536,t=3,p=1$pCPY/6wsX8W0KGt/Sep6SA$LvZLV7SQO5GwBKHGwQ2O4SB5WE/yq32fqzxr1tM44IM', '0', 'null', '2024-09-19 19:48:45', 1, 5),
	('0974000252', '$argon2id$v=19$m=65536,t=3,p=1$lwt7t+OJt4K3oEcmupdcYQ$YIbZ5HEaY1wsvA7hP+ESq2mSK9fSfrf8OyEd/JysZlE', '0', 'null', '2024-09-16 23:21:32', 1, 5),
	('0974000352', '$argon2id$v=19$m=65536,t=3,p=1$v/bdb5wVD5iXngisZShJHA$q53DQFKYl9vxKwYaWJhj3znh8CAaSKone78S5IKtLe8', '0', 'null', '2024-09-16 22:10:39', 1, 5),
	('0974000452', '$argon2id$v=19$m=65536,t=3,p=1$JyiCqpdm/IQM1aW12lWLMQ$zHZ/ymjKBqfE8Ev76fw4ImRxOUWZby7ycGMZxoLYpaI', '0', 'null', '2024-09-16 22:08:19', 1, 5),
	('0974000552', '$argon2id$v=19$m=65536,t=3,p=1$D5boa+4ru/TDVwLJrS4R8g$LwwTCcEtvLI5kk2/MrfNYjawCPKsZmwVryXo5ClUaZY', '0', 'null', '2024-09-19 20:00:22', 1, 5),
	('0974000652', '$argon2id$v=19$m=65536,t=3,p=1$pAMFVQjpjAdilCuLf3q5yg$0IvLc0AFeB6QruC323fedUoMMdgnlFwr0rTEqx+S83Q', '0', 'null', '2024-09-30 16:54:56', 1, 5),
	('0974000752', '$argon2id$v=19$m=65536,t=3,p=1$r5dn72vYmAsibyDU54PjHw$Yh4M0C7isNbSdn0gtpyWfO5eONMwPqfn3VQbM2syPrk', '0', 'null', '2024-09-30 16:58:02', 1, 5),
	('admin', '$argon2i$v=19$m=12,t=3,p=1$bDcxdzRoaGRubWowMDAwMA$96w9DOJfMGBtu8GfeWvyfw', '0', NULL, NULL, 1, 1),
	('admin2', '$argon2id$v=19$m=65536,t=3,p=1$0tfopQz8TcG6GvpAl28d0g$jOrPw+GGqpVOv0OmOt53AnK7HbGD4sbuUFScCjR8wMA', '0', 'null', '2024-09-18 21:05:13', 1, 1);

-- Dumping structure for table baucutructuyen.thongbao
CREATE TABLE IF NOT EXISTS `thongbao` (
  `ID_ThongBao` int NOT NULL AUTO_INCREMENT,
  `NoiDungThongBao` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
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
  `STT` tinyint NOT NULL,
  `TenTinhThanh` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
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
  `GhiNhan` varchar(1) COLLATE utf8mb4_general_ci DEFAULT '0',
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_DonViBauCu` smallint DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `trangthaibaucu_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`),
  CONSTRAINT `trangthaibaucu_ibfk_2` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`),
  CONSTRAINT `trangthaibaucu_ibfk_3` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.trangthaibaucu: ~8 rows (approximately)
INSERT INTO `trangthaibaucu` (`GhiNhan`, `ID_CuTri`, `ID_DonViBauCu`, `ngayBD`) VALUES
	('0', '20240919200022', 3, '2024-09-19 23:54:51'),
	('0', '20240919194845', 3, '2024-09-19 23:54:51'),
	('0', '20240916221039', 3, '2024-09-19 23:54:51'),
	('0', '20240916220819', 3, '2024-09-19 23:54:51'),
	('0', '20240919200022', 3, '2024-11-03 10:15:00'),
	('0', '20240919194845', 3, '2024-11-03 10:15:00'),
	('0', '20240916221039', 3, '2024-11-03 10:15:00'),
	('0', '20240916220819', 3, '2024-11-03 10:15:00');

-- Dumping structure for table baucutructuyen.trinhdohocvan
CREATE TABLE IF NOT EXISTS `trinhdohocvan` (
  `ID_TrinhDo` smallint NOT NULL AUTO_INCREMENT,
  `TenTrinhDoHocVan` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
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
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci NOT NULL,
  `TrangThai` varchar(10) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_ucv`),
  KEY `FK_userUngCuVien` (`ID_user`),
  CONSTRAINT `FK_userUngCuVien` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ungcuvien: ~4 rows (approximately)
INSERT INTO `ungcuvien` (`ID_ucv`, `TrangThai`, `ID_user`) VALUES
	('20241003000921', 'acctive', 'Ds20241003000915'),
	('20241003002121', 'acctive', '7H20241003002115'),
	('20241003002930', 'acctive', 'Gu20241003002922'),
	('20241003003050', 'acctive', 'RM20241003003046'),
	('20241003003142', 'acctive', 'Do20241003003139');

-- Dumping structure for table baucutructuyen.vaitro
CREATE TABLE IF NOT EXISTS `vaitro` (
  `RoleID` tinyint NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(30) COLLATE utf8mb4_general_ci DEFAULT NULL,
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
