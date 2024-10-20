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

-- Dumping data for table baucutructuyen.chitietbaucu: ~2 rows (approximately)
INSERT INTO `chitietbaucu` (`ThoiDiem`, `ID_Phieu`, `ID_CuTri`) VALUES
	('2024-10-18 21:37:47', 'Nl2024101821374643', '20240930165802'),
	('2024-10-18 22:16:43', 'WL2024101822164303', '20240919200022');

-- Dumping structure for table baucutructuyen.chitietcutri
CREATE TABLE IF NOT EXISTS `chitietcutri` (
  `ID_ChucVu` tinyint DEFAULT NULL,
  `ID_CuTri` varchar(14) COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `chitietcutri_ibfk_1` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietcutri: ~0 rows (approximately)

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

-- Dumping data for table baucutructuyen.chitiettrinhdohocvanungcuvien: ~12 rows (approximately)
INSERT INTO `chitiettrinhdohocvanungcuvien` (`ID_TrinhDo`, `ID_ucv`) VALUES
	(11, '20241003000921'),
	(11, '20241003002121'),
	(11, '20241003002930'),
	(11, '20241003003050'),
	(11, '20241003003142'),
	(8, '20241012212410'),
	(8, '20241012212527'),
	(8, '20241017143132'),
	(8, '20241017143237'),
	(8, '20241017143328'),
	(8, '20241017143409'),
	(8, '20241017143445');

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

-- Dumping data for table baucutructuyen.danhmucungcu: ~7 rows (approximately)
INSERT INTO `danhmucungcu` (`ID_Cap`, `TenCapUngCu`, `ID_DonViBauCu`) VALUES
	(1, 'Trưởng khoa', 3),
	(2, 'Phó trưởng khoa', 3),
	(3, 'Phó trưởng khoa', 1),
	(4, 'Hiệu trưởng', 9),
	(5, 'Phó hiệu trưởng', 9),
	(7, 'Trưởng Lớp A2 Khoa Khoa Học Máy Tính', 9),
	(8, 'Trưởng Lớp A3 Khoa Khoa Học Máy Tính', 9),
	(9, 'Trưởng Lớp A1 Khoa Khoa Học Máy Tính', 3);

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
  `ngayBD` datetime DEFAULT NULL,
  KEY `ID_canbo` (`ID_canbo`),
  KEY `ID_ChucVu` (`ID_ChucVu`),
  KEY `ID_Ban` (`ID_Ban`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `hoatdong_ibfk_1` FOREIGN KEY (`ID_canbo`) REFERENCES `canbo` (`ID_CanBo`),
  CONSTRAINT `hoatdong_ibfk_2` FOREIGN KEY (`ID_ChucVu`) REFERENCES `chucvu` (`ID_ChucVu`),
  CONSTRAINT `hoatdong_ibfk_3` FOREIGN KEY (`ID_Ban`) REFERENCES `ban` (`ID_Ban`),
  CONSTRAINT `hoatdong_ibfk_4` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
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
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.hosonguoidung: ~20 rows (approximately)
INSERT INTO `hosonguoidung` (`MaSo`, `TrangThaiDangKy`, `ID_user`) VALUES
	(1, '0', 'sT20240916220819'),
	(2, '0', 'Pe20240916221033'),
	(3, '0', 'Zh20240916232125'),
	(6, '1', 'iZ20240919194839'),
	(7, '1', 'i420240919200017'),
	(8, '0', 'jp20240930165451'),
	(9, '1', 'uB20240930165759'),
	(10, '1', 'Ds20241003000915'),
	(11, '1', '7H20241003002115'),
	(12, '1', 'Gu20241003002922'),
	(13, '1', 'RM20241003003046'),
	(14, '1', 'Do20241003003139'),
	(15, '1', '5520241012212400'),
	(16, '1', 'uO20241012212509'),
	(17, '1', 'rY20240918210506'),
	(18, '1', 'ME20241017143125'),
	(19, '1', 'ui20241017143233'),
	(20, '1', 'sX20241017143325'),
	(21, '1', 'Dz20241017143406'),
	(22, '1', 'TC20241017143443');

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

-- Dumping data for table baucutructuyen.ketquabaucu: ~12 rows (approximately)
INSERT INTO `ketquabaucu` (`SoLuotBinhChon`, `ThoiDiemDangKy`, `TyLeBinhChon`, `ngayBD`, `ID_ucv`, `ID_Cap`) VALUES
	(0, '2024-10-03 00:09:22', 0, '2024-11-03 10:15:00', '20241003000921', 9),
	(0, '2024-10-03 00:21:22', 0, '2024-11-03 10:15:00', '20241003002121', 9),
	(0, '2024-10-03 00:29:30', 0, '2024-11-03 10:15:00', '20241003002930', 9),
	(0, '2024-10-03 00:30:51', 0, '2024-11-03 10:15:00', '20241003003050', 9),
	(0, '2024-10-03 00:31:43', 0, '2024-11-03 10:15:00', '20241003003142', 9),
	(0, '2024-10-12 21:24:10', 0, '2024-11-03 10:25:10', '20241012212410', 1),
	(0, '2024-10-12 21:25:28', 0, '2024-11-03 10:25:10', '20241012212527', 1),
	(0, '2024-10-17 14:31:32', 0, '2024-10-22 12:12:12', '20241017143132', 3),
	(0, '2024-10-17 14:32:37', 0, '2024-10-22 12:12:12', '20241017143237', 3),
	(0, '2024-10-17 14:33:28', 0, '2024-10-22 12:12:12', '20241017143328', 3),
	(0, '2024-10-17 14:34:10', 0, '2024-10-22 12:12:12', '20241017143409', 3),
	(0, '2024-10-17 14:34:45', 0, '2024-10-22 12:12:12', '20241017143445', 3);

-- Dumping structure for table baucutructuyen.khoa
CREATE TABLE IF NOT EXISTS `khoa` (
  `ID_Khoa` int NOT NULL AUTO_INCREMENT,
  `NgayTao` datetime DEFAULT NULL,
  `N` bigint DEFAULT NULL,
  `G` bigint DEFAULT NULL,
  `path_PK` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_Khoa`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `khoa_ibfk_1` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoa: ~8 rows (approximately)
INSERT INTO `khoa` (`ID_Khoa`, `NgayTao`, `N`, `G`, `path_PK`, `ngayBD`) VALUES
	(1, '2024-10-15 00:00:00', 299939, 81168015365, 'F:\\PrivateKey\\2024-11-15_19-11-03.txt', '2024-10-15 19:11:03'),
	(2, '2024-10-15 00:00:00', 455129, 152725822457, 'F:\\PrivateKey\\2024-14-15_19-14-42.txt', '2024-10-15 19:14:42'),
	(3, '2024-10-17 14:11:48', 209999, 19252207454, 'F:\\PrivateKey\\2024-11-17_14-11-47.txt', '2024-10-19 12:15:55'),
	(4, '2024-10-18 19:55:07', 455467, 167862253795, 'F:\\PrivateKey\\2024-55-18_19-55-07.txt', '2024-10-21 12:15:55'),
	(5, '2024-10-18 19:59:56', 377579, 4817600286, 'F:\\PrivateKey\\2024-59-18_19-59-56.txt', '2024-10-23 12:15:45'),
	(6, '2024-10-18 20:01:20', 210451, 37863109925, 'F:\\PrivateKey\\2024-01-18_20-01-20.txt', '2024-10-23 12:17:35'),
	(7, '2024-10-18 20:04:44', 254017, 21990754504, 'F:\\PrivateKey\\2024-04-18_20-04-43.txt', '2024-10-23 11:12:35'),
	(10, '2024-10-18 21:02:22', 321337, 81703324318, 'F:\\PrivateKey\\2024-02-18_21-02-22.txt', '2024-10-22 12:12:12');

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
  `ID_Cap` smallint DEFAULT NULL,
  PRIMARY KEY (`ngayBD`),
  KEY `fk_ViTriTaiKyBauCu` (`ID_Cap`),
  CONSTRAINT `fk_ViTriTaiKyBauCu` FOREIGN KEY (`ID_Cap`) REFERENCES `danhmucungcu` (`ID_Cap`),
  CONSTRAINT `check_dates` CHECK ((`ngayKT` > `ngayBD`)),
  CONSTRAINT `Chk_kybaucu` CHECK ((`SoLuongToiDaCuTri` > `SoLuongToiDaUngCuVien`)),
  CONSTRAINT `Chk_kybaucuSoLuotBinhChon` CHECK ((`SoLuotBinhChonToiDa` < `SoLuongToiDaUngCuVien`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.kybaucu: ~15 rows (approximately)
INSERT INTO `kybaucu` (`ngayBD`, `ngayKT`, `NgayKT_UngCu`, `TenKyBauCu`, `MoTa`, `SoLuongToiDaCuTri`, `SoLuongToiDaUngCuVien`, `SoLuotBinhChonToiDa`, `ID_Cap`) VALUES
	('2024-08-13 07:00:19', '2024-12-18 07:00:19', '2024-07-03 20:02:34', 'Bảo vệ luận văn', '', 30, 12, 5, 5),
	('2024-09-19 23:54:51', '2024-11-30 12:14:00', '2024-07-03 20:02:33', 'Bao', '', 50, 10, 5, 2),
	('2024-10-15 19:05:00', '2024-12-16 12:15:00', '2024-10-18 00:00:00', 'Deadline', 'bảo vệ luận văn', 20, 5, 3, 2),
	('2024-10-15 19:11:03', '2024-12-16 12:15:00', '2024-10-18 00:00:00', 'Deadline', 'bảo vệ luận văn', 20, 5, 3, 9),
	('2024-10-15 19:14:42', '2024-12-15 12:15:55', '2024-10-18 00:00:00', 'Bầu cử trưởng xóm', 'Dealine2', 20, 6, 3, 7),
	('2024-10-19 12:15:55', '2024-11-15 12:15:55', '2024-10-18 00:00:00', 'Bầu cử phó trưởng xóm', 'Dealine3', 20, 5, 3, 9),
	('2024-10-21 12:15:55', '2024-11-16 12:15:55', '0001-01-01 00:00:00', 'Bầu cử phó trưởng làng', 'Dealine4', 20, 5, 3, 3),
	('2024-10-22 12:12:12', '2024-12-12 12:12:12', '0001-01-01 00:00:00', 'Bầu cử phó trưởng làng 12', 'Dealine12', 12, 5, 3, 3),
	('2024-10-23 11:12:35', '2024-11-17 12:15:55', '0001-01-01 00:00:00', 'Bầu cử phó trưởng làng5', 'Dealine5', 20, 5, 3, 7),
	('2024-10-23 12:15:45', '2024-11-17 12:15:55', '0001-01-01 00:00:00', 'Bầu cử phó trưởng làng3', 'Dealine5', 20, 5, 3, 5),
	('2024-10-23 12:17:35', '2024-11-17 12:15:55', '0001-01-01 00:00:00', 'Bầu cử phó trưởng làng4', 'Dealine5', 20, 5, 3, 2),
	('2024-11-03 10:15:00', '2024-11-15 12:10:00', '2024-07-03 20:02:35', 'Bầu cử trưởng thôn', 'okok', 20, 5, 3, 9),
	('2024-11-03 10:25:10', '2024-11-15 12:10:00', '2024-10-25 20:02:36', 'Bầu cử trưởng làng', '', 10, 7, 3, 1),
	('2024-11-03 10:25:11', '2024-11-30 12:14:00', '2024-07-03 20:02:37', 'Bầu cử thị trưởng', '', 15, 4, 2, 9),
	('2024-11-05 00:00:00', '2024-12-31 00:00:00', '2024-07-03 20:02:38', 'Bầu cử', '', 100, 8, 3, 5);

-- Dumping structure for table baucutructuyen.lichsudangnhap
CREATE TABLE IF NOT EXISTS `lichsudangnhap` (
  `ThoiDiem` datetime DEFAULT NULL,
  `DiaChiIP` varchar(30) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `TaiKhoan` varchar(30) COLLATE utf8mb4_general_ci DEFAULT NULL,
  UNIQUE KEY `UNK_IDTaiKhoan` (`TaiKhoan`),
  KEY `TaiKhoan` (`TaiKhoan`),
  CONSTRAINT `lichsudangnhap_ibfk_1` FOREIGN KEY (`TaiKhoan`) REFERENCES `taikhoan` (`TaiKhoan`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.lichsudangnhap: ~1 rows (approximately)
INSERT INTO `lichsudangnhap` (`ThoiDiem`, `DiaChiIP`, `TaiKhoan`) VALUES
	('2024-10-19 23:20:46', '0.0.0.1', 'admin2');

-- Dumping structure for table baucutructuyen.nguoidung
CREATE TABLE IF NOT EXISTS `nguoidung` (
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci NOT NULL,
  `HoTen` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `GioiTinh` varchar(1) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `NgaySinh` date DEFAULT NULL,
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

-- Dumping data for table baucutructuyen.nguoidung: ~20 rows (approximately)
INSERT INTO `nguoidung` (`ID_user`, `HoTen`, `GioiTinh`, `NgaySinh`, `DiaChiLienLac`, `CCCD`, `Email`, `SDT`, `HinhAnh`, `PublicID`, `ID_DanToc`, `RoleID`) VALUES
	('5520241012212400', 'NguyenVanF', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001053', 'nguyenvanf@gmail.com', '040714013', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1728743049/NguoiDung/pdcpemhuohcom13ph71z.jpg', 'NguoiDung/pdcpemhuohcom13ph71z', 1, 2),
	('7H20241003002115', 'NguyenVanB', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000652', 'nguyenvanb@gmail.com', '040714008', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727889680/NguoiDung/mecgdcdk13queaimrygw.jpg', 'NguoiDung/mecgdcdk13queaimrygw', 1, 2),
	('Do20241003003139', 'NguyenVanE', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000952', 'nguyenvane@gmail.com', '040714011', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890302/NguoiDung/svxuiuxvdgjcm9wm49v7.jpg', 'NguoiDung/svxuiuxvdgjcm9wm49v7', 1, 2),
	('Ds20241003000915', 'NguyenVanA', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000552', 'nguyenvana@gmail.com', '040714007', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727888960/NguoiDung/zztvpcs0hopvem3gsnel.jpg', 'NguoiDung/zztvpcs0hopvem3gsnel', 1, 2),
	('Dz20241017143406', 'TranVanD', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001058', 'tranvand@gmail.com', '040714018', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150449/NguoiDung/g95k0de68o0nizgdhntn.jpg', 'NguoiDung/g95k0de68o0nizgdhntn', 1, 2),
	('Gu20241003002922', 'NguyenVanC', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000752', 'nguyenvanc@gmail.com', '040714009', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890169/NguoiDung/cu16bwbu1tbr3jhq47yp.jpg', 'NguoiDung/cu16bwbu1tbr3jhq47yp', 1, 2),
	('i420240919200017', 'Trần Lộc Đỉnh', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '123456789', 'pgiabao2002@gmail.com', '0974000552', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726750819/NguoiDung/giajvu1tmzpixlqhxmyb.jpg', 'NguoiDung/giajvu1tmzpixlqhxmyb', 1, 5),
	('iZ20240919194839', 'Võ Hoàng Tuấn Đạt', '1', '2002-10-29', ' tp.Cà Mau', NULL, 'vhtdat2002@gmail.com', '0974000162', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726750121/NguoiDung/b0pyecdhyid0q37zlpy2.jpg', 'NguoiDung/b0pyecdhyid0q37zlpy2', 1, 5),
	('jp20240930165451', 'Hồ Minh Trường', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'httruong2002@gmail.com', '0974000652', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727690096/NguoiDung/cpvj0gxxgzyevrbw7hgl.jpg', 'NguoiDung/cpvj0gxxgzyevrbw7hgl', 1, 5),
	('ME20241017143125', 'TranVanA', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001055', 'dicako9489@adosnan.com', '040714015', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150291/NguoiDung/mzo3aew6ixn1asas3bno.jpg', 'NguoiDung/mzo3aew6ixn1asas3bno', 1, 2),
	('Pe20240916221033', 'Lê Hữu Đức', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000322327445', 'ducb2013070@student.ctu.edu.vn', '0974000352', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726499436/NguoiDung/nhdc4uzjfwwpwmbcmcpr.jpg', 'NguoiDung/nhdc4uzjfwwpwmbcmcpr', 1, 5),
	('RM20241003003046', 'NguyenVanD', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000852', 'nguyenvand@gmail.com', '040714010', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890250/NguoiDung/bun34h6wktbzda5o5tug.jpg', 'NguoiDung/bun34h6wktbzda5o5tug', 1, 2),
	('rY20240918210506', 'Đỗ Thánh', '1', '2002-10-19', 'Q.Ninh Kiều, tp.Càn Thơ', '10000000008', 'pgbaop4@gmail.com', 'admin2', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726668312/NguoiDung/whyuizgynfpg63zrogdp.jpg', 'NguoiDung/whyuizgynfpg63zrogdp', 1, 1),
	('sT20240916220819', 'Lý Gia Nguyên', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000422327445', 'pgiabao2003@gmail.com', '0974000452', NULL, NULL, 1, 5),
	('sX20241017143325', 'TranVanC', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001057', 'tranvanc@gmail.com', '040714017', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150408/NguoiDung/wp07ucvdpzo3yo1lf3mr.jpg', 'NguoiDung/wp07ucvdpzo3yo1lf3mr', 1, 2),
	('TC20241017143443', 'TranVanE', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001059', 'tranvane@gmail.com', '040714019', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150485/NguoiDung/t2pq9paw4kjszl7gs7zm.jpg', 'NguoiDung/t2pq9paw4kjszl7gs7zm', 1, 2),
	('uB20240930165759', 'Đường Bá Hổ', '1', '2024-10-17', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'baob2016947@student.ctu.edu.vn', '0974000752', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727690282/NguoiDung/r7stiwbtawuff8xcjqhn.jpg', 'NguoiDung/r7stiwbtawuff8xcjqhn', 1, 5),
	('ui20241017143233', 'TranVanB', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001056', 'tranvanb@gmail.com', '040714016', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150356/NguoiDung/hgxqmkv6kjubrodqvubt.jpg', 'NguoiDung/hgxqmkv6kjubrodqvubt', 1, 2),
	('uO20241012212509', 'NguyenVanT', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001054', 'nguyenvant@gmail.com', '040714014', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1728743128/NguoiDung/odehghnembqoqctknl0d.jpg', 'NguoiDung/odehghnembqoqctknl0d', 1, 2),
	('Zh20240916232125', 'Phạm Thế HIển', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000122327445', 'baob2016987@student.ctu.edu.vn', '0974000252', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726503689/NguoiDung/uiumpqhsuwsfri9anamz.jpg', 'NguoiDung/uiumpqhsuwsfri9anamz', 1, 5);

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

-- Dumping data for table baucutructuyen.phanhoicutri: ~2 rows (approximately)
INSERT INTO `phanhoicutri` (`Ykien`, `ThoiDiem`, `ID_CuTri`) VALUES
	('test12345678', '2024-10-11', '20240930165802'),
	('cần cải thiện giao diện', '2024-10-12', '20240930165802');

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

-- Dumping data for table baucutructuyen.phieubau: ~2 rows (approximately)
INSERT INTO `phieubau` (`ID_Phieu`, `GiaTriPhieuBau`, `ngayBD`, `ID_cap`) VALUES
	('Nl2024101821374643', 68536306727, '2024-10-22 12:12:12', 3),
	('WL2024101822164303', 99094252714, '2024-10-22 12:12:12', 3);

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
  `MatKhau` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
  `BiKhoa` varchar(1) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `LyDoKhoa` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `NgayTao` datetime DEFAULT NULL,
  `SuDung` tinyint DEFAULT NULL,
  `RoleID` tinyint NOT NULL,
  PRIMARY KEY (`TaiKhoan`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.taikhoan: ~21 rows (approximately)
INSERT INTO `taikhoan` (`TaiKhoan`, `MatKhau`, `BiKhoa`, `LyDoKhoa`, `NgayTao`, `SuDung`, `RoleID`) VALUES
	('040714007', '$argon2id$v=19$m=65536,t=3,p=1$LnAOv4nQnHKrtc/QfRGRRg$lCBt51MfGEerZKFngJRzvAIaWP/aYAWF6KEtd6fG100', '0', 'null', '2024-10-03 00:09:21', 1, 2),
	('040714008', '$argon2id$v=19$m=65536,t=3,p=1$uoUAWWz7IR7LR221OAcj1A$RR0TqvUb2i3upCD3b10Pz6cJIMnUbbIkvuhbwTwoHEA', '0', 'null', '2024-10-03 00:21:21', 1, 2),
	('040714009', '$argon2id$v=19$m=65536,t=3,p=1$4nF/mCkowPm5DsCWmC6Zdg$Uee1SCnFGoGIST4pskxdkbgUSAJ8X6z6YdfT7fzKS08', '0', 'null', '2024-10-03 00:29:29', 1, 2),
	('040714010', '$argon2id$v=19$m=65536,t=3,p=1$Si2i7IdKVfmaAHiDtUgtIg$z8O4S0SKmT0mPS1dYOxclydwnx2/G7YpMSDeE+u3Qew', '0', 'null', '2024-10-03 00:30:50', 1, 2),
	('040714011', '$argon2id$v=19$m=65536,t=3,p=1$YC0wZRm2qHSjwjiji6mWgw$nh6QffMQgH6n2M3qZ7SOgQKfXVdH96qRvjty5L8bD7s', '0', 'null', '2024-10-03 00:31:42', 1, 2),
	('040714013', '$argon2id$v=19$m=65536,t=3,p=1$RoHxcCIfDQfnk5wsz8S2Ww$emEcivNYHwZOMhVB5Ri/MA+hrtYDy0PM68pVfFyf/0w', '0', 'null', '2024-10-12 21:24:10', 1, 2),
	('040714014', '$argon2id$v=19$m=65536,t=3,p=1$zMbnYZdfTLdq0NYYeRg4tQ$XwjCkqjG8f7kpeR52FU3iLLlidW1NcA/c+5TFzJMr5k', '0', 'null', '2024-10-12 21:25:27', 1, 2),
	('040714015', '$argon2id$v=19$m=65536,t=3,p=1$tYW/Ee7BRSwYMrTNY4m36g$gndb5trhYYMX2KG0mLjBwKLfvFBsjupzOFDQ5OtD7KE', '0', 'null', '2024-10-17 14:31:32', 1, 2),
	('040714016', '$argon2id$v=19$m=65536,t=3,p=1$zZqYQpE6oWgB7hV7TI7XyQ$nxOn9xFIJBhl3B/Eku25gI3cghkJgkLRFqF3lZTQ61M', '0', 'null', '2024-10-17 14:32:37', 1, 2),
	('040714017', '$argon2id$v=19$m=65536,t=3,p=1$gVsCxtsLCcqNrPVJNqXqzw$ru/M4N5yAD5UL2yLCiTWKTBVg5aoAV8LpO5ZpVVnOSQ', '0', 'null', '2024-10-17 14:33:28', 1, 2),
	('040714018', '$argon2id$v=19$m=65536,t=3,p=1$8Aq2oMjGD1QJeMat3afktg$TAbXzX3OsfOPfv1sdjOVm7uXwITpwZVGBt3FxcRBmP0', '0', 'null', '2024-10-17 14:34:09', 1, 2),
	('040714019', '$argon2id$v=19$m=65536,t=3,p=1$PKWiBfsRm46qi0pGYyN8TQ$Z/SV8+vUKjijFzlTEXu3rfBYOE2zsCZw/4R+Zr9xgfM', '0', 'null', '2024-10-17 14:34:45', 1, 2),
	('0974000162', '$argon2id$v=19$m=65536,t=3,p=1$9X213jZA5D3IQ3fTHUD/Kg$jkdrrtKPM0kQN9t5ssvcwDDRHIzwJL8Qb02BM9HRwR8', '0', 'null', '2024-09-19 19:48:45', 1, 5),
	('0974000252', '$argon2id$v=19$m=65536,t=3,p=1$lwt7t+OJt4K3oEcmupdcYQ$YIbZ5HEaY1wsvA7hP+ESq2mSK9fSfrf8OyEd/JysZlE', '0', 'null', '2024-09-16 23:21:32', 1, 5),
	('0974000352', '$argon2id$v=19$m=65536,t=3,p=1$v/bdb5wVD5iXngisZShJHA$q53DQFKYl9vxKwYaWJhj3znh8CAaSKone78S5IKtLe8', '0', 'null', '2024-09-16 22:10:39', 1, 5),
	('0974000452', '$argon2id$v=19$m=65536,t=3,p=1$JyiCqpdm/IQM1aW12lWLMQ$zHZ/ymjKBqfE8Ev76fw4ImRxOUWZby7ycGMZxoLYpaI', '0', 'null', '2024-09-16 22:08:19', 1, 5),
	('0974000552', '$argon2id$v=19$m=65536,t=3,p=1$M/h5VDFWeAU49YtonSu1Eg$au9/sUwul2z1aDAK12C5YhoAe56DRctx9/nqsxStTww', '0', 'null', '2024-09-19 20:00:22', 1, 5),
	('0974000652', '$argon2id$v=19$m=65536,t=3,p=1$pAMFVQjpjAdilCuLf3q5yg$0IvLc0AFeB6QruC323fedUoMMdgnlFwr0rTEqx+S83Q', '0', 'null', '2024-09-30 16:54:56', 1, 5),
	('0974000752', '$argon2id$v=19$m=65536,t=3,p=1$Zjcn2mFPwOIq51Vf2IiXnQ$nQxiOUzB7mbIyPOQSCAGxF7KHtxGzhljcfTd+rJ62yg', '0', 'null', '2024-09-30 16:58:02', 1, 5),
	('admin', '$argon2i$v=19$m=12,t=3,p=1$bDcxdzRoaGRubWowMDAwMA$96w9DOJfMGBtu8GfeWvyfw', '0', NULL, NULL, 1, 1),
	('admin2', '$argon2id$v=19$m=65536,t=3,p=1$0tfopQz8TcG6GvpAl28d0g$jOrPw+GGqpVOv0OmOt53AnK7HbGD4sbuUFScCjR8wMA', '0', 'null', '2024-09-18 21:05:13', 1, 1);

-- Dumping structure for table baucutructuyen.thongbao
CREATE TABLE IF NOT EXISTS `thongbao` (
  `ID_ThongBao` int NOT NULL AUTO_INCREMENT,
  `NoiDungThongBao` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_ThongBao`)
) ENGINE=InnoDB AUTO_INCREMENT=58 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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

-- Dumping data for table baucutructuyen.trangthaibaucu: ~24 rows (approximately)
INSERT INTO `trangthaibaucu` (`GhiNhan`, `ID_CuTri`, `ID_DonViBauCu`, `ngayBD`) VALUES
	('0', '20240919200022', 3, '2024-09-19 23:54:51'),
	('0', '20240919194845', 3, '2024-09-19 23:54:51'),
	('0', '20240916221039', 3, '2024-09-19 23:54:51'),
	('0', '20240916220819', 3, '2024-09-19 23:54:51'),
	('0', '20240919200022', 3, '2024-11-03 10:15:00'),
	('0', '20240919194845', 3, '2024-11-03 10:15:00'),
	('0', '20240916221039', 3, '2024-11-03 10:15:00'),
	('0', '20240916220819', 3, '2024-11-03 10:15:00'),
	('0', '20240930165802', 3, '2024-11-03 10:15:00'),
	('0', '20240919200022', 3, '2024-11-03 10:25:10'),
	('0', '20240919194845', 3, '2024-11-03 10:25:10'),
	('0', '20240916221039', 3, '2024-11-03 10:25:10'),
	('0', '20240930165802', 3, '2024-11-03 10:25:10'),
	('0', '20240919200022', 3, '2024-11-03 10:25:11'),
	('0', '20240919194845', 3, '2024-11-03 10:25:11'),
	('0', '20240916221039', 3, '2024-11-03 10:25:11'),
	('0', '20240930165802', 3, '2024-11-03 10:25:11'),
	('0', '20240919200022', 3, '2024-10-15 19:11:03'),
	('0', '20240919194845', 3, '2024-10-15 19:11:03'),
	('0', '20240916221039', 3, '2024-10-15 19:11:03'),
	('0', '20240930165802', 3, '2024-10-15 19:11:03'),
	('0', '20240930165802', 1, '2024-10-19 12:15:55'),
	('1', '20240930165802', 1, '2024-10-22 12:12:12'),
	('1', '20240919200022', 1, '2024-10-22 12:12:12');

-- Dumping structure for table baucutructuyen.trinhdohocvan
CREATE TABLE IF NOT EXISTS `trinhdohocvan` (
  `ID_TrinhDo` smallint NOT NULL AUTO_INCREMENT,
  `TenTrinhDoHocVan` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_TrinhDo`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.trinhdohocvan: ~8 rows (approximately)
INSERT INTO `trinhdohocvan` (`ID_TrinhDo`, `TenTrinhDoHocVan`) VALUES
	(1, 'Tiểu học'),
	(2, ' Trung học cơ sở'),
	(3, ' Trung học phổ thông'),
	(4, 'Cao đẳng'),
	(5, 'Đại học'),
	(6, 'Tiến sĩ'),
	(7, 'Trung cấp chuyên nghiệp'),
	(8, 'Nghiên cứu sinh'),
	(11, 'Thạc sĩ'),
	(12, 'Khác');

-- Dumping structure for table baucutructuyen.ungcuvien
CREATE TABLE IF NOT EXISTS `ungcuvien` (
  `ID_ucv` varchar(14) COLLATE utf8mb4_general_ci NOT NULL,
  `TrangThai` varchar(10) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_user` varchar(16) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `GioiThieu` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_ucv`),
  KEY `FK_userUngCuVien` (`ID_user`),
  CONSTRAINT `FK_userUngCuVien` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ungcuvien: ~12 rows (approximately)
INSERT INTO `ungcuvien` (`ID_ucv`, `TrangThai`, `ID_user`, `GioiThieu`) VALUES
	('20241003000921', 'acctive', 'Ds20241003000915', 'KOKOKOKOKOKOK'),
	('20241003002121', 'acctive', '7H20241003002115', 'OKOKOKOK'),
	('20241003002930', 'acctive', 'Gu20241003002922', 'OKOKOKOK'),
	('20241003003050', 'acctive', 'RM20241003003046', 'OKOKOK'),
	('20241003003142', 'acctive', 'Do20241003003139', 'OKOK'),
	('20241012212410', 'acctive', '5520241012212400', 'rất ok.'),
	('20241012212527', 'acctive', 'uO20241012212509', 'rất ok.'),
	('20241017143132', 'acctive', 'ME20241017143125', 'rất ok.'),
	('20241017143237', 'acctive', 'ui20241017143233', 'rất ok.'),
	('20241017143328', 'acctive', 'sX20241017143325', 'rất ok.'),
	('20241017143409', 'acctive', 'Dz20241017143406', 'rất ok.'),
	('20241017143445', 'acctive', 'TC20241017143443', 'rất ok.');

-- Dumping structure for table baucutructuyen.vaitro
CREATE TABLE IF NOT EXISTS `vaitro` (
  `RoleID` tinyint NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(30) COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.vaitro: ~6 rows (approximately)
INSERT INTO `vaitro` (`RoleID`, `TenVaiTro`) VALUES
	(1, 'Admin'),
	(2, 'Ứng cử viên'),
	(3, 'Ban Tổ chức'),
	(4, 'Ban kiểm phiếu'),
	(5, 'Cử tri'),
	(8, 'Cán bộ'),
	(9, 'Test');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
