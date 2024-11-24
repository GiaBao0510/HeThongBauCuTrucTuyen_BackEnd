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
CREATE DATABASE IF NOT EXISTS `baucutructuyen` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `baucutructuyen`;

-- Dumping structure for table baucutructuyen.ban
CREATE TABLE IF NOT EXISTS `ban` (
  `ID_Ban` smallint NOT NULL AUTO_INCREMENT,
  `TenBan` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_DonViBauCu` smallint DEFAULT NULL,
  PRIMARY KEY (`ID_Ban`),
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  KEY `idx_ban_ID_Ban` (`ID_Ban`),
  CONSTRAINT `ban_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ban: ~5 rows (approximately)
INSERT INTO `ban` (`ID_Ban`, `TenBan`, `ID_DonViBauCu`) VALUES
	(1, 'Ban bầu cử', 1),
	(2, 'Ban Kiểm phiếu', 1),
	(3, 'Ban thanh tra', 1),
	(6, 'test', 1);

-- Dumping structure for table baucutructuyen.canbo
CREATE TABLE IF NOT EXISTS `canbo` (
  `ID_CanBo` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `NgayCongTac` datetime DEFAULT NULL,
  `GhiChu` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `ID_user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_CanBo`),
  KEY `ID_user` (`ID_user`),
  KEY `idx_canbo_ID_user` (`ID_user`),
  CONSTRAINT `canbo_ibfk_ID_user` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.canbo: ~5 rows (approximately)
INSERT INTO `canbo` (`ID_CanBo`, `NgayCongTac`, `GhiChu`, `ID_user`) VALUES
	('20241025151435', '1999-01-01 00:00:00', 'canbo8', 'Pv20241025151431'),
	('20241025152418', '1999-01-01 00:00:00', 'canbo7', '7L20241025152412'),
	('20241025153453', '1999-01-01 00:00:00', 'canbo6', 'in20241025153445'),
	('20241025153720', '1999-01-01 00:00:00', 'canbo5', 'Dp20241025153713'),
	('20241025153942', '1999-01-01 00:00:00', 'canbo4', 'rw20241025153937');

-- Dumping structure for table baucutructuyen.chitietbaucu
CREATE TABLE IF NOT EXISTS `chitietbaucu` (
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_Phieu` varchar(18) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_CuTri` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `XacThuc` tinyint DEFAULT '-1',
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_Phieu` (`ID_Phieu`),
  CONSTRAINT `chitietbaucu_ibfk_2` FOREIGN KEY (`ID_Phieu`) REFERENCES `phieubau` (`ID_Phieu`),
  CONSTRAINT `chitietbaucu_ibfk_ID_CuTri` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietbaucu: ~6 rows (approximately)
INSERT INTO `chitietbaucu` (`ThoiDiem`, `ID_Phieu`, `ID_CuTri`, `XacThuc`) VALUES
	('2024-10-27 20:32:29', 'HL2024102720322871', '20240919194845', -1),
	('2024-10-27 20:35:55', 'gg2024102720355539', '20240930165802', -1),
	('2024-10-27 20:37:53', 'ok2024102720375308', '20240919200022', -1),
	('2024-10-27 20:39:43', 'C42024102720394280', '20240916232132', -1),
	('2024-10-27 20:41:23', 'gD2024102720412253', '20240916221039', -1),
	('2024-11-11 10:16:53', 'w62024111110165247', '20240930165456', -1);

-- Dumping structure for table baucutructuyen.chitietcongboketqua
CREATE TABLE IF NOT EXISTS `chitietcongboketqua` (
  `ThoiDiemCongBo` datetime DEFAULT NULL,
  `ID_CanBo` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_ucv` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  KEY `chitietcongboketqua_ibfk_ID_CanBo` (`ID_CanBo`),
  KEY `chitietcongboketqua_ibfk_ID_ucv` (`ID_ucv`),
  KEY `chitietcongboketqua_ibfk_ngayBD` (`ngayBD`),
  CONSTRAINT `chitietcongboketqua_ibfk_ID_CanBo` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`) ON DELETE CASCADE,
  CONSTRAINT `chitietcongboketqua_ibfk_ID_ucv` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`) ON DELETE CASCADE,
  CONSTRAINT `chitietcongboketqua_ibfk_ngayBD` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table baucutructuyen.chitietcongboketqua: ~1 rows (approximately)
INSERT INTO `chitietcongboketqua` (`ThoiDiemCongBo`, `ID_CanBo`, `ID_ucv`, `ngayBD`) VALUES
	('2024-11-11 19:39:19', '20241025151435', '20241017143445', '2024-10-22 12:12:12');

-- Dumping structure for table baucutructuyen.chitietcutri
CREATE TABLE IF NOT EXISTS `chitietcutri` (
  `ID_ChucVu` tinyint DEFAULT NULL,
  `ID_CuTri` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `chitietcutri_ibfk_ID_CuTri` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietcutri: ~4 rows (approximately)
INSERT INTO `chitietcutri` (`ID_ChucVu`, `ID_CuTri`) VALUES
	(13, '20241109134558'),
	(13, '20241109142305'),
	(13, '20241109221936'),
	(13, '20241109234117');

-- Dumping structure for table baucutructuyen.chitietthongbaocanbo
CREATE TABLE IF NOT EXISTS `chitietthongbaocanbo` (
  `ID_ThongBao` int DEFAULT NULL,
  `ID_CanBo` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ThongBao` (`ID_ThongBao`),
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `chitietthongbaocanbo_ibfk_ID_CanBo` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`) ON DELETE CASCADE,
  CONSTRAINT `chitietthongbaocanbo_ibfk_ID_ThongBao` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaocanbo: ~41 rows (approximately)
INSERT INTO `chitietthongbaocanbo` (`ID_ThongBao`, `ID_CanBo`) VALUES
	(59, '20241025151435'),
	(59, '20241025152418'),
	(59, '20241025153453'),
	(59, '20241025153720'),
	(59, '20241025153942'),
	(80, '20241025151435'),
	(81, '20241025152418'),
	(82, '20241025153453'),
	(83, '20241025153720'),
	(84, '20241025153942'),
	(105, '20241025151435'),
	(106, '20241025152418'),
	(107, '20241025153453'),
	(108, '20241025153720'),
	(109, '20241025153942'),
	(130, '20241025151435'),
	(131, '20241025152418'),
	(132, '20241025153453'),
	(133, '20241025153720'),
	(134, '20241025153942'),
	(155, '20241025151435'),
	(156, '20241025152418'),
	(157, '20241025153453'),
	(158, '20241025153720'),
	(159, '20241025153942'),
	(180, '20241025151435'),
	(181, '20241025152418'),
	(182, '20241025153453'),
	(183, '20241025153720'),
	(184, '20241025153942'),
	(205, '20241025151435'),
	(206, '20241025152418'),
	(207, '20241025153453'),
	(208, '20241025153720'),
	(209, '20241025153942'),
	(210, '20241025151435'),
	(210, '20241025152418'),
	(210, '20241025153453'),
	(210, '20241025153720'),
	(210, '20241025153942'),
	(211, '20241025151435'),
	(211, '20241025152418'),
	(211, '20241025153453'),
	(211, '20241025153720'),
	(211, '20241025153942');

-- Dumping structure for table baucutructuyen.chitietthongbaocutri
CREATE TABLE IF NOT EXISTS `chitietthongbaocutri` (
  `ID_ThongBao` int DEFAULT NULL,
  `ID_CuTri` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaocutri_ibfk_ID_CuTri` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`) ON DELETE CASCADE,
  CONSTRAINT `chitietthongbaocutri_ibfk_ID_ThongBao` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaocutri: ~97 rows (approximately)
INSERT INTO `chitietthongbaocutri` (`ID_ThongBao`, `ID_CuTri`) VALUES
	(59, '20240930165802'),
	(59, '20240919200022'),
	(59, '20240916232132'),
	(59, '20240916220819'),
	(59, '20240916221039'),
	(59, '20240930165456'),
	(59, '20240919194845'),
	(60, '20240919200022'),
	(61, '20240919194845'),
	(62, '20240916221039'),
	(63, '20240916220819'),
	(64, '20240930165802'),
	(65, '20240919200022'),
	(66, '20240919194845'),
	(67, '20240916221039'),
	(68, '20240930165802'),
	(69, '20240919200022'),
	(70, '20240919194845'),
	(71, '20240916221039'),
	(72, '20240930165802'),
	(85, '20240919200022'),
	(86, '20240919194845'),
	(87, '20240916221039'),
	(88, '20240916220819'),
	(89, '20240930165802'),
	(90, '20240919200022'),
	(91, '20240919194845'),
	(92, '20240916221039'),
	(93, '20240930165802'),
	(94, '20240919200022'),
	(95, '20240919194845'),
	(96, '20240916221039'),
	(97, '20240930165802'),
	(110, '20240919200022'),
	(111, '20240919194845'),
	(112, '20240916221039'),
	(113, '20240916220819'),
	(114, '20240930165802'),
	(115, '20240919200022'),
	(116, '20240919194845'),
	(117, '20240916221039'),
	(118, '20240930165802'),
	(119, '20240919200022'),
	(120, '20240919194845'),
	(121, '20240916221039'),
	(122, '20240930165802'),
	(135, '20240919200022'),
	(136, '20240919194845'),
	(137, '20240916221039'),
	(138, '20240916220819'),
	(139, '20240930165802'),
	(140, '20240919200022'),
	(141, '20240919194845'),
	(142, '20240916221039'),
	(143, '20240930165802'),
	(144, '20240919200022'),
	(145, '20240919194845'),
	(146, '20240916221039'),
	(147, '20240930165802'),
	(160, '20240919200022'),
	(161, '20240919194845'),
	(162, '20240916221039'),
	(163, '20240916220819'),
	(164, '20240930165802'),
	(165, '20240919200022'),
	(166, '20240919194845'),
	(167, '20240916221039'),
	(168, '20240930165802'),
	(169, '20240919200022'),
	(170, '20240919194845'),
	(171, '20240916221039'),
	(172, '20240930165802'),
	(185, '20240919200022'),
	(186, '20240919194845'),
	(187, '20240916221039'),
	(188, '20240916220819'),
	(189, '20240930165802'),
	(190, '20240919200022'),
	(191, '20240919194845'),
	(192, '20240916221039'),
	(193, '20240930165802'),
	(194, '20240919200022'),
	(195, '20240919194845'),
	(196, '20240916221039'),
	(197, '20240930165802'),
	(210, '20240930165802'),
	(210, '20240919200022'),
	(210, '20240916232132'),
	(210, '20240916220819'),
	(210, '20240916221039'),
	(210, '20240930165456'),
	(210, '20240919194845'),
	(211, '20240930165802'),
	(211, '20240919200022'),
	(211, '20240916232132'),
	(211, '20240916220819'),
	(211, '20240916221039'),
	(211, '20240930165456'),
	(211, '20240919194845');

-- Dumping structure for table baucutructuyen.chitietthongbaoungcuvien
CREATE TABLE IF NOT EXISTS `chitietthongbaoungcuvien` (
  `ID_ThongBao` int DEFAULT NULL,
  `ID_ucv` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_ThongBao` (`ID_ThongBao`),
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_ID_ThongBao` FOREIGN KEY (`ID_ThongBao`) REFERENCES `thongbao` (`ID_ThongBao`) ON DELETE CASCADE,
  CONSTRAINT `chitietthongbaoungcuvien_ibfk_ID_ucv` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietthongbaoungcuvien: ~55 rows (approximately)
INSERT INTO `chitietthongbaoungcuvien` (`ID_ThongBao`, `ID_ucv`) VALUES
	(59, '20241017143132'),
	(59, '20241017143237'),
	(59, '20241017143328'),
	(59, '20241017143409'),
	(59, '20241017143445'),
	(73, '20241003000921'),
	(74, '20241003002121'),
	(75, '20241003002930'),
	(76, '20241003003050'),
	(77, '20241003003142'),
	(78, '20241012212410'),
	(79, '20241012212527'),
	(98, '20241003000921'),
	(99, '20241003002121'),
	(100, '20241003002930'),
	(101, '20241003003050'),
	(102, '20241003003142'),
	(103, '20241012212410'),
	(104, '20241012212527'),
	(123, '20241003000921'),
	(124, '20241003002121'),
	(125, '20241003002930'),
	(126, '20241003003050'),
	(127, '20241003003142'),
	(128, '20241012212410'),
	(129, '20241012212527'),
	(148, '20241003000921'),
	(149, '20241003002121'),
	(150, '20241003002930'),
	(151, '20241003003050'),
	(152, '20241003003142'),
	(153, '20241012212410'),
	(154, '20241012212527'),
	(173, '20241003000921'),
	(174, '20241003002121'),
	(175, '20241003002930'),
	(176, '20241003003050'),
	(177, '20241003003142'),
	(178, '20241012212410'),
	(179, '20241012212527'),
	(198, '20241003000921'),
	(199, '20241003002121'),
	(200, '20241003002930'),
	(201, '20241003003050'),
	(202, '20241003003142'),
	(203, '20241012212410'),
	(204, '20241012212527'),
	(210, '20241017143132'),
	(210, '20241017143237'),
	(210, '20241017143328'),
	(210, '20241017143409'),
	(210, '20241017143445'),
	(211, '20241017143132'),
	(211, '20241017143237'),
	(211, '20241017143328'),
	(211, '20241017143409'),
	(211, '20241017143445');

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvancanbo
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvancanbo` (
  `ID_TrinhDo` smallint DEFAULT NULL,
  `ID_CanBo` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_1` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvancanbo_ibfk_ID_CanBo` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitiettrinhdohocvancanbo: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.chitiettrinhdohocvanungcuvien
CREATE TABLE IF NOT EXISTS `chitiettrinhdohocvanungcuvien` (
  `ID_TrinhDo` smallint DEFAULT NULL,
  `ID_ucv` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ID_TrinhDo` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_2` FOREIGN KEY (`ID_TrinhDo`) REFERENCES `trinhdohocvan` (`ID_TrinhDo`),
  CONSTRAINT `chitiettrinhdohocvanungcuvien_ibfk_ID_ucv` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitiettrinhdohocvanungcuvien: ~13 rows (approximately)
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
	(8, '20241017143445'),
	(12, '20241114150058');

-- Dumping structure for table baucutructuyen.chitietungcuvien
CREATE TABLE IF NOT EXISTS `chitietungcuvien` (
  `ID_ChucVu` tinyint DEFAULT NULL,
  `ID_ucv` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  CONSTRAINT `chitietungcuvien_ibfk_ID_ucv` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chitietungcuvien: ~0 rows (approximately)
INSERT INTO `chitietungcuvien` (`ID_ChucVu`, `ID_ucv`) VALUES
	(0, '20241114150058');

-- Dumping structure for table baucutructuyen.chucvu
CREATE TABLE IF NOT EXISTS `chucvu` (
  `ID_ChucVu` tinyint NOT NULL AUTO_INCREMENT,
  `TenChucVu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_ChucVu`),
  KEY `idx_chucvu_ID_ChucVu` (`ID_ChucVu`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.chucvu: ~13 rows (approximately)
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
	(14, 'Giảng viên'),
	(15, 'Kiểm soát viên'),
	(16, 'test');

-- Dumping structure for table baucutructuyen.cutri
CREATE TABLE IF NOT EXISTS `cutri` (
  `ID_CuTri` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ID_user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_CuTri`),
  KEY `FK_userCuTri` (`ID_user`),
  CONSTRAINT `FK_userCuTri_ID_user` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.cutri: ~11 rows (approximately)
INSERT INTO `cutri` (`ID_CuTri`, `ID_user`) VALUES
	('20241109134558', 'bp20241109134555'),
	('20240919200022', 'i420240919200017'),
	('20240919194845', 'iZ20240919194839'),
	('20241109142305', 'J820241109142300'),
	('20240930165456', 'jp20240930165451'),
	('20240916221039', 'Pe20240916221033'),
	('20241109234117', 'pf20241109234113'),
	('20241109221936', 'ps20241109221931'),
	('20240916220819', 'sT20240916220819'),
	('20240930165802', 'uB20240930165759'),
	('20240916232132', 'Zh20240916232125');

-- Dumping structure for table baucutructuyen.danhmucungcu
CREATE TABLE IF NOT EXISTS `danhmucungcu` (
  `ID_Cap` smallint NOT NULL AUTO_INCREMENT,
  `TenCapUngCu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_DonViBauCu` smallint DEFAULT NULL,
  PRIMARY KEY (`ID_Cap`),
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  CONSTRAINT `danhmucungcu_ibfk_1` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.danhmucungcu: ~11 rows (approximately)
INSERT INTO `danhmucungcu` (`ID_Cap`, `TenCapUngCu`, `ID_DonViBauCu`) VALUES
	(1, 'Trưởng khoa', 3),
	(2, 'Phó trưởng khoa', 3),
	(3, 'Phó trưởng khoa', 1),
	(4, 'Hiệu trưởng', 9),
	(5, 'Phó hiệu trưởng', 9),
	(7, 'Trưởng Lớp A2 Khoa Khoa Học Máy Tính', 9),
	(8, 'Trưởng Lớp A3 Khoa Khoa Học Máy Tính', 9),
	(9, 'Trưởng Lớp A1 Khoa Khoa Học Máy Tính', 3),
	(10, 'Phó bảo vệ', 9),
	(11, 'Thủ Quỹ', 9),
	(12, 'Cấp temp', 3);

-- Dumping structure for table baucutructuyen.dantoc
CREATE TABLE IF NOT EXISTS `dantoc` (
  `ID_DanToc` tinyint NOT NULL AUTO_INCREMENT,
  `TenDanToc` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `TenGoiKhac` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
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
  `ID_user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidungtamtru` (`ID_user`),
  CONSTRAINT `diachitamtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidungtamtru_ID_user` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.diachitamtru: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.diachithuongtru
CREATE TABLE IF NOT EXISTS `diachithuongtru` (
  `ID_QH` int DEFAULT NULL,
  `ID_user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_QH` (`ID_QH`),
  KEY `fk_nguoidung` (`ID_user`),
  CONSTRAINT `diachithuongtru_ibfk_3` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`),
  CONSTRAINT `fk_nguoidung_ID_user` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.diachithuongtru: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.donvibaucu
CREATE TABLE IF NOT EXISTS `donvibaucu` (
  `ID_DonViBauCu` smallint NOT NULL AUTO_INCREMENT,
  `TenDonViBauCu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `DiaChi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_QH` int DEFAULT NULL,
  PRIMARY KEY (`ID_DonViBauCu`),
  KEY `ID_QH` (`ID_QH`),
  CONSTRAINT `donvibaucu_ibfk_1` FOREIGN KEY (`ID_QH`) REFERENCES `quanhuyen` (`ID_QH`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.donvibaucu: ~6 rows (approximately)
INSERT INTO `donvibaucu` (`ID_DonViBauCu`, `TenDonViBauCu`, `DiaChi`, `ID_QH`) VALUES
	(1, 'Khoa công nghệ thông tin', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1),
	(3, 'Khoa khoa học máy tính', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1),
	(6, 'Khoa Hệ thống thông tin', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1),
	(9, 'Trường CNTT&TT', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1),
	(10, 'Trường ĐH Cần Thơ', 'Phường An Khánh, Q.Ninh Kiều, tp.Cần Thơ', 1);

-- Dumping structure for table baucutructuyen.hoatdong
CREATE TABLE IF NOT EXISTS `hoatdong` (
  `ID_canbo` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_ChucVu` tinyint DEFAULT '16',
  `ID_Ban` smallint DEFAULT '6',
  `ngayBD` datetime DEFAULT NULL,
  KEY `ID_canbo` (`ID_canbo`),
  KEY `ID_ChucVu` (`ID_ChucVu`),
  KEY `ID_Ban` (`ID_Ban`),
  KEY `ngayBD` (`ngayBD`),
  KEY `idx_hoatdong_ngayBD` (`ngayBD`),
  KEY `idx_hoatdong_ID_canbo` (`ID_canbo`),
  KEY `idx_hoatdong_ID_ChucVu` (`ID_ChucVu`),
  KEY `idx_hoatdong_ID_Ban` (`ID_Ban`),
  CONSTRAINT `hoatdong_ibfk_2` FOREIGN KEY (`ID_ChucVu`) REFERENCES `chucvu` (`ID_ChucVu`),
  CONSTRAINT `hoatdong_ibfk_3` FOREIGN KEY (`ID_Ban`) REFERENCES `ban` (`ID_Ban`),
  CONSTRAINT `hoatdong_ibfk_ID_CanBo` FOREIGN KEY (`ID_canbo`) REFERENCES `canbo` (`ID_CanBo`) ON DELETE CASCADE,
  CONSTRAINT `hoatdong_ibfk_ngayBD` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.hoatdong: ~13 rows (approximately)
INSERT INTO `hoatdong` (`ID_canbo`, `ID_ChucVu`, `ID_Ban`, `ngayBD`) VALUES
	('20241025151435', 14, 2, '2024-10-22 12:12:12'),
	('20241025152418', 14, 2, '2024-10-22 12:12:12'),
	('20241025153453', 14, 6, '2024-10-22 12:12:12'),
	('20241025153720', 14, 2, '2024-10-22 12:12:12'),
	('20241025153942', 14, 2, '2024-10-22 12:12:12'),
	('20241025151435', 14, 2, '2024-11-03 10:25:11'),
	('20241025152418', 14, 2, '2024-11-03 10:25:11'),
	('20241025153453', 14, 6, '2024-11-03 10:25:11'),
	('20241025153720', 14, 2, '2024-11-03 10:25:11'),
	('20241025153942', 14, 2, '2024-11-03 10:25:11'),
	('20241025153453', 16, 6, '2024-11-28 11:11:11'),
	('20241025151435', 16, 6, '2024-11-28 11:11:11'),
	('20241025153942', 16, 6, '2024-11-28 11:11:11');

-- Dumping structure for table baucutructuyen.hosonguoidung
CREATE TABLE IF NOT EXISTS `hosonguoidung` (
  `MaSo` int NOT NULL AUTO_INCREMENT,
  `TrangThaiDangKy` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '0',
  `ID_user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`MaSo`),
  KEY `ID_user` (`ID_user`),
  CONSTRAINT `hosonguoidung_ibfk_ID_user` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.hosonguoidung: ~31 rows (approximately)
INSERT INTO `hosonguoidung` (`MaSo`, `TrangThaiDangKy`, `ID_user`) VALUES
	(1, '1', 'sT20240916220819'),
	(2, '1', 'Pe20240916221033'),
	(3, '1', 'Zh20240916232125'),
	(6, '1', 'iZ20240919194839'),
	(7, '1', 'i420240919200017'),
	(8, '1', 'jp20240930165451'),
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
	(22, '1', 'TC20241017143443'),
	(23, '1', 'Pv20241025151431'),
	(24, '1', '7L20241025152412'),
	(25, '1', 'in20241025153445'),
	(26, '1', 'Dp20241025153713'),
	(27, '1', 'rw20241025153937'),
	(28, '0', 'bp20241109134555'),
	(29, '0', 'J820241109142300'),
	(30, '0', 'ps20241109221931'),
	(31, '0', 'pf20241109234113'),
	(39, '1', '2K20241114150052');

-- Dumping structure for table baucutructuyen.ketquabaucu
CREATE TABLE IF NOT EXISTS `ketquabaucu` (
  `SoLuotBinhChon` int DEFAULT NULL,
  `ThoiDiemDangKy` datetime DEFAULT NULL,
  `TyLeBinhChon` float DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  `ID_ucv` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_Cap` smallint DEFAULT NULL,
  KEY `ID_Cap` (`ID_Cap`),
  KEY `ID_ucv` (`ID_ucv`),
  KEY `ngayBD` (`ngayBD`),
  KEY `idx_ketquabaucu_ngayBD` (`ngayBD`),
  CONSTRAINT `ketquabaucu_ibfk_1` FOREIGN KEY (`ID_Cap`) REFERENCES `danhmucungcu` (`ID_Cap`),
  CONSTRAINT `ketquabaucu_ibfk_ID_ucv` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`) ON DELETE CASCADE,
  CONSTRAINT `ketquabaucu_ibfk_ngayBD` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ketquabaucu: ~16 rows (approximately)
INSERT INTO `ketquabaucu` (`SoLuotBinhChon`, `ThoiDiemDangKy`, `TyLeBinhChon`, `ngayBD`, `ID_ucv`, `ID_Cap`) VALUES
	(0, '2024-10-03 00:09:22', 0, '2024-11-03 10:15:00', '20241003000921', 9),
	(0, '2024-10-03 00:21:22', 0, '2024-11-03 10:15:00', '20241003002121', 9),
	(0, '2024-10-03 00:29:30', 0, '2024-11-03 10:15:00', '20241003002930', 9),
	(0, '2024-10-03 00:30:51', 0, '2024-11-03 10:15:00', '20241003003050', 9),
	(0, '2024-10-03 00:31:43', 0, '2024-11-03 10:15:00', '20241003003142', 9),
	(0, '2024-10-12 21:24:10', 0, '2024-11-03 10:25:10', '20241012212410', 1),
	(0, '2024-10-12 21:25:28', 0, '2024-11-03 10:25:10', '20241012212527', 1),
	(4, '2024-10-17 14:31:32', 17.3913, '2024-10-22 12:12:12', '20241017143132', 3),
	(4, '2024-10-17 14:32:37', 17.3913, '2024-10-22 12:12:12', '20241017143237', 3),
	(5, '2024-10-17 14:33:28', 21.7391, '2024-10-22 12:12:12', '20241017143328', 3),
	(5, '2024-10-17 14:34:10', 21.7391, '2024-10-22 12:12:12', '20241017143409', 3),
	(5, '2024-10-17 14:34:45', 21.7391, '2024-10-22 12:12:12', '20241017143445', 3),
	(0, '2024-11-14 15:00:59', 0, '2024-11-28 11:11:11', '20241114150058', 12),
	(0, '2024-11-15 16:52:27', 0, '2024-11-28 11:11:11', '20241003003142', 11),
	(0, '2024-11-15 16:52:28', 0, '2024-11-28 11:11:11', '20241003000921', 11),
	(0, '2024-11-15 16:52:28', 0, '2024-11-28 11:11:11', '20241017143409', 11);

-- Dumping structure for table baucutructuyen.khoa
CREATE TABLE IF NOT EXISTS `khoa` (
  `ID_Khoa` int NOT NULL AUTO_INCREMENT,
  `NgayTao` datetime DEFAULT NULL,
  `N` bigint DEFAULT NULL,
  `G` bigint DEFAULT NULL,
  `path_PK` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_Khoa`),
  KEY `ngayBD` (`ngayBD`),
  CONSTRAINT `khoa_ibfk_ngayBD` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.khoa: ~15 rows (approximately)
INSERT INTO `khoa` (`ID_Khoa`, `NgayTao`, `N`, `G`, `path_PK`, `ngayBD`) VALUES
	(1, '2024-10-15 00:00:00', 299939, 81168015365, 'F:\\PrivateKey\\2024-11-15_19-11-03.txt', '2024-10-15 19:11:03'),
	(2, '2024-10-15 00:00:00', 455129, 152725822457, 'F:\\PrivateKey\\2024-14-15_19-14-42.txt', '2024-10-15 19:14:42'),
	(3, '2024-10-17 14:11:48', 209999, 19252207454, 'F:\\PrivateKey\\2024-11-17_14-11-47.txt', '2024-10-19 12:15:55'),
	(4, '2024-10-18 19:55:07', 455467, 167862253795, 'F:\\PrivateKey\\2024-55-18_19-55-07.txt', '2024-10-21 12:15:55'),
	(5, '2024-10-18 19:59:56', 377579, 4817600286, 'F:\\PrivateKey\\2024-59-18_19-59-56.txt', '2024-10-23 12:15:45'),
	(6, '2024-10-18 20:01:20', 210451, 37863109925, 'F:\\PrivateKey\\2024-01-18_20-01-20.txt', '2024-10-23 12:17:35'),
	(7, '2024-10-18 20:04:44', 254017, 21990754504, 'F:\\PrivateKey\\2024-04-18_20-04-43.txt', '2024-10-23 11:12:35'),
	(10, '2024-10-18 21:02:22', 321337, 81703324318, 'F:\\PrivateKey\\2024-02-18_21-02-22.txt', '2024-10-22 12:12:12'),
	(11, '2024-10-31 11:34:23', 436789, 64416581397, 'F:\\PrivateKey\\2024-34-31_11-34-23.txt', '2024-10-22 12:10:10'),
	(12, '2024-10-31 11:35:32', 204863, 21042017494, 'F:\\PrivateKey\\2024-35-31_11-35-31.txt', '2024-10-22 12:50:10'),
	(14, '2024-10-31 19:18:02', 227923, 23486733020, 'F:\\PrivateKey\\2024-18-31_19-18-01.txt', '2024-10-30 12:50:10'),
	(16, '2024-10-31 19:26:00', 547693, 297973395241, 'F:\\PrivateKey\\2024-26-31_19-26-00.txt', '2024-10-31 12:50:00'),
	(17, '2024-10-31 21:45:04', 319369, 68851783052, 'F:\\PrivateKey\\2024-45-31_21-45-04.txt', '2024-10-31 21:43:59'),
	(18, '2024-10-31 21:48:15', 367063, 133865138971, 'F:\\PrivateKey\\2024-48-31_21-48-14.txt', '2024-10-31 21:46:50'),
	(19, '2024-11-01 18:06:40', 217801, 21443713795, 'F:\\PrivateKey\\2024-06-01_18-06-39.txt', '2024-10-31 12:51:00'),
	(22, '2024-11-13 21:05:53', 373831, 59174556355, 'F:\\PrivateKey\\2024-05-13_21-05-53.txt', '2024-11-28 11:11:11');

-- Dumping structure for table baucutructuyen.kybaucu
CREATE TABLE IF NOT EXISTS `kybaucu` (
  `ngayBD` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ngayKT` datetime DEFAULT NULL,
  `NgayKT_UngCu` datetime DEFAULT NULL,
  `TenKyBauCu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `MoTa` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `SoLuongToiDaCuTri` int DEFAULT NULL,
  `SoLuongToiDaUngCuVien` int DEFAULT NULL,
  `SoLuotBinhChonToiDa` int DEFAULT NULL,
  `CongBo` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '0',
  `ID_Cap` smallint DEFAULT NULL,
  PRIMARY KEY (`ngayBD`),
  KEY `fk_ViTriTaiKyBauCu` (`ID_Cap`),
  CONSTRAINT `fk_ViTriTaiKyBauCu` FOREIGN KEY (`ID_Cap`) REFERENCES `danhmucungcu` (`ID_Cap`),
  CONSTRAINT `check_dates` CHECK ((`ngayKT` > `ngayBD`)),
  CONSTRAINT `Chk_kybaucu` CHECK ((`SoLuongToiDaCuTri` > `SoLuongToiDaUngCuVien`)),
  CONSTRAINT `Chk_kybaucuSoLuotBinhChon` CHECK ((`SoLuotBinhChonToiDa` < `SoLuongToiDaUngCuVien`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.kybaucu: ~22 rows (approximately)
INSERT INTO `kybaucu` (`ngayBD`, `ngayKT`, `NgayKT_UngCu`, `TenKyBauCu`, `MoTa`, `SoLuongToiDaCuTri`, `SoLuongToiDaUngCuVien`, `SoLuotBinhChonToiDa`, `CongBo`, `ID_Cap`) VALUES
	('2024-08-13 07:00:19', '2024-12-18 07:00:19', '2024-07-03 20:02:34', 'Bầu cử trưởng lớp KHMT A1', '', 30, 12, 5, '0', 5),
	('2024-09-19 23:54:51', '2024-11-30 12:14:00', '2024-07-03 20:02:33', 'Bầu cử trưởng lớp KHMT A2', '', 50, 10, 5, '0', 2),
	('2024-10-15 19:05:00', '2024-12-16 12:15:00', '2024-10-18 00:00:00', 'Bầu cử trưởng lớp KHMT A3', 'bảo vệ luận văn', 20, 5, 3, '0', 2),
	('2024-10-15 19:11:03', '2024-12-16 12:15:00', '2024-10-18 00:00:00', 'Bầu cử trưởng lớp HTTT A1', 'bảo vệ luận văn', 20, 5, 3, '0', 9),
	('2024-10-15 19:14:42', '2024-12-15 12:15:55', '2024-10-18 00:00:00', 'Bầu cử trưởng lớp HTTT A2', 'Dealine2', 20, 6, 3, '0', 7),
	('2024-10-19 12:15:55', '2024-11-15 12:15:55', '2024-10-18 00:00:00', 'Bầu cử trưởng lớp HTTT A3', 'Dealine3', 20, 5, 3, '0', 9),
	('2024-10-21 12:15:55', '2024-11-16 12:15:55', '0001-01-01 00:00:00', 'Bầu cử trưởng lớp MMT A1', 'Dealine4', 20, 5, 3, '0', 3),
	('2024-10-22 12:10:10', '2024-11-09 10:10:10', '2024-11-05 11:34:23', 'Bầu cử trưởng lớp MMT A2', 'Diễn ra tại phòng bảo vệ', 10, 5, 4, '0', 10),
	('2024-10-22 12:12:12', '2024-12-12 12:12:12', '0001-01-01 00:00:00', 'Bầu cử trưởng lớp MMT A3', 'Dealine12', 12, 5, 4, '1', 3),
	('2024-10-22 12:50:10', '2024-11-11 11:11:11', '2024-11-05 11:35:32', 'Bầu cử trưởng lớp CNTT&TT A1', 'Diễn ra tại phòng bảo vệ', 10, 5, 4, '0', 10),
	('2024-10-23 11:12:35', '2024-11-17 12:15:55', '0001-01-01 00:00:00', 'Bầu cử trưởng lớp CNTT&TT A2', 'Dealine5', 20, 5, 3, '0', 7),
	('2024-10-23 12:15:45', '2024-11-17 12:15:55', '0001-01-01 00:00:00', 'Bầu cử trưởng lớp CNTT&TT A3', 'Dealine5', 20, 5, 3, '0', 5),
	('2024-10-23 12:17:35', '2024-11-17 12:15:55', '0001-01-01 00:00:00', 'Bầu cử trưởng lớp TTDPT A1', 'Dealine5', 20, 5, 3, '0', 2),
	('2024-10-30 12:50:10', '2024-11-13 11:13:13', '2024-11-05 19:18:01', 'Bầu cử trưởng lớp TTDPT A2', 'Diễn ra tại phòng bảo vệ', 10, 5, 4, '0', 10),
	('2024-10-31 12:50:00', '2024-11-09 09:09:09', '2024-11-05 19:26:00', 'Bầu cử trưởng lớp TTDPT A3', 'Diễn ra tại phòng 204', 10, 5, 4, '0', 9),
	('2024-10-31 12:51:00', '2024-11-09 09:09:09', '2024-11-06 18:06:39', 'Bầu cử trưởng lớp KTPM A1', 'Test có thể xóa (Xóa)', 10, 5, 4, '0', 9),
	('2024-10-31 21:43:59', '2024-11-09 21:44:08', '2024-11-05 21:45:04', 'Bầu cử phó trưởng lớp KHMT A1', 'Diễn ra tại phòng 217', 15, 4, 3, '0', 8),
	('2024-10-31 21:46:50', '2024-11-10 21:47:03', '2024-11-05 21:48:15', 'Bầu cử thủ quỹ', 'Diễn ra tại phòng 101', 10, 3, 2, '0', 10),
	('2024-11-03 10:15:00', '2024-11-15 12:10:00', '2024-07-03 20:02:35', 'Bầu cử trưởng lớp KTPM A2', 'okok', 20, 5, 3, '0', 9),
	('2024-11-03 10:25:10', '2024-11-15 12:10:00', '2024-10-25 20:02:36', 'Bầu cử trưởng lớp KTPM A3', '', 10, 7, 3, '0', 1),
	('2024-11-03 10:25:11', '2024-11-30 12:14:00', '2024-07-03 20:02:37', 'Bầu cử trưởng lớp KTPM A4', '', 15, 4, 2, '0', 9),
	('2024-11-05 00:00:00', '2024-12-31 00:00:00', '2024-07-03 20:02:38', 'Bầu cử', '', 100, 8, 3, '0', 5),
	('2024-11-28 11:11:11', '2024-12-05 09:09:09', '2024-11-18 21:05:53', 'Bầu cử thủ quỹ lớp CNTT a3 ', 'Diễn ra tại 207/DI', 10, 5, 4, '0', 11);

-- Dumping structure for table baucutructuyen.lichsudangnhap
CREATE TABLE IF NOT EXISTS `lichsudangnhap` (
  `ThoiDiem` datetime DEFAULT NULL,
  `DiaChiIP` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `TaiKhoan` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  UNIQUE KEY `UNK_IDTaiKhoan` (`TaiKhoan`),
  KEY `TaiKhoan` (`TaiKhoan`),
  CONSTRAINT `lichsudangnhap_ibfk_TaiKhoan` FOREIGN KEY (`TaiKhoan`) REFERENCES `taikhoan` (`TaiKhoan`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.lichsudangnhap: ~13 rows (approximately)
INSERT INTO `lichsudangnhap` (`ThoiDiem`, `DiaChiIP`, `TaiKhoan`) VALUES
	('2024-11-21 21:42:27', '127.0.0.1', 'admin2'),
	('2024-10-25 22:04:56', '192.168.2.113', '098910005'),
	('2024-11-11 20:28:48', '192.168.1.205', '098950005'),
	('2024-10-27 20:39:26', '192.168.2.113', '0974000252'),
	('2024-10-26 21:50:20', '192.168.2.113', '0974000452'),
	('2024-11-11 12:09:45', '192.168.1.107', '0974000652'),
	('2024-10-29 18:24:47', '192.168.2.113', '0974000352'),
	('2024-11-11 11:35:13', '192.168.1.107', '0974000162'),
	('2024-10-27 20:35:02', '192.168.2.113', '0974000752'),
	('2024-11-10 15:06:29', '192.168.1.92', '0974000552'),
	('2024-11-17 21:04:11', '192.168.112.1', '040714015'),
	('2024-10-29 18:33:44', '192.168.2.113', '040714016'),
	('2024-10-29 19:16:02', '192.168.2.113', '040714018');

-- Dumping structure for table baucutructuyen.nguoidung
CREATE TABLE IF NOT EXISTS `nguoidung` (
  `ID_user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `HoTen` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `GioiTinh` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `NgaySinh` date DEFAULT NULL,
  `DiaChiLienLac` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `CCCD` varchar(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `Email` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `SDT` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `HinhAnh` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT 'null',
  `PublicID` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT 'null',
  `ID_DanToc` tinyint DEFAULT NULL,
  `RoleID` tinyint DEFAULT NULL,
  PRIMARY KEY (`ID_user`),
  UNIQUE KEY `CCCD` (`CCCD`),
  UNIQUE KEY `Email` (`Email`),
  UNIQUE KEY `SDT` (`SDT`),
  KEY `ID_DanToc` (`ID_DanToc`),
  KEY `fk_vaitronguoidung` (`RoleID`),
  KEY `idx_nguoidung_ID_user` (`ID_user`),
  CONSTRAINT `fk_vaitronguoidung` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`),
  CONSTRAINT `nguoidung_ibfk_1` FOREIGN KEY (`ID_DanToc`) REFERENCES `dantoc` (`ID_DanToc`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.nguoidung: ~30 rows (approximately)
INSERT INTO `nguoidung` (`ID_user`, `HoTen`, `GioiTinh`, `NgaySinh`, `DiaChiLienLac`, `CCCD`, `Email`, `SDT`, `HinhAnh`, `PublicID`, `ID_DanToc`, `RoleID`) VALUES
	('2K20241114150052', 'Phạm Huy Hoàng 5555', '1', '2024-10-21', 'Quận ninh kiều ,Thành phố cần thơ', NULL, 'phamhuyh0ang115@gmail.com', '0947888952', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1731571258/NguoiDung/idb21pn0ufbhrylxderm.jpg', 'NguoiDung/idb21pn0ufbhrylxderm', 1, 2),
	('5520241012212400', 'NguyenVanF', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001053', 'nguyenvanf@gmail.com', '040714013', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1728743049/NguoiDung/pdcpemhuohcom13ph71z.jpg', 'NguoiDung/pdcpemhuohcom13ph71z', 1, 2),
	('7H20241003002115', 'NguyenVanB', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000652', 'nguyenvanb@gmail.com', '040714008', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727889680/NguoiDung/mecgdcdk13queaimrygw.jpg', 'NguoiDung/mecgdcdk13queaimrygw', 1, 2),
	('7L20241025152412', 'PhamVanG', '1', '1999-01-01', 'q.Ninh Kiều, tp.Cần Thơ', NULL, 'phamvang@ctu.edu.vn', '098940005', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729844658/NguoiDung/jjqxglfizr2ou0cs0kru.jpg', 'NguoiDung/jjqxglfizr2ou0cs0kru', 1, 8),
	('bp20241109134555', 'Trần Vĩnh Tiến', '0', '2003-06-12', 'tp Bạc Liêu', NULL, 'tranvinhtien@gmail.com', '0974008888', 'null', 'null', 14, 8),
	('Do20241003003139', 'NguyenVanE', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000952', 'nguyenvane@gmail.com', '040714011', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890302/NguoiDung/svxuiuxvdgjcm9wm49v7.jpg', 'NguoiDung/svxuiuxvdgjcm9wm49v7', 1, 2),
	('Dp20241025153713', 'PhamVanE', '1', '1999-01-01', 'q.Ninh Kiều, tp.Cần Thơ', NULL, 'phamvane@ctu.edu.vn', '098920005', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729845440/NguoiDung/axuxnvasmnaurocimz2j.jpg', 'NguoiDung/axuxnvasmnaurocimz2j', 1, 8),
	('Ds20241003000915', 'NguyenVanA', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000552', 'nguyenvana@gmail.com', '040714007', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727888960/NguoiDung/zztvpcs0hopvem3gsnel.jpg', 'NguoiDung/zztvpcs0hopvem3gsnel', 1, 2),
	('Dz20241017143406', 'TranVanD', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001058', 'liyor6okey@aleitar.com', '040714018', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150449/NguoiDung/g95k0de68o0nizgdhntn.jpg', 'NguoiDung/g95k0de68o0nizgdhntn', 1, 2),
	('Gu20241003002922', 'NguyenVanC', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000752', 'nguyenvanc@gmail.com', '040714009', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890169/NguoiDung/cu16bwbu1tbr3jhq47yp.jpg', 'NguoiDung/cu16bwbu1tbr3jhq47yp', 1, 2),
	('i420240919200017', 'Tiểu Xuân Tử', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '123456789', 'pgiabao2002@gmail.com', '0974000552', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726750819/NguoiDung/giajvu1tmzpixlqhxmyb.jpg', 'NguoiDung/giajvu1tmzpixlqhxmyb', 1, 5),
	('in20241025153445', 'PhamVanF', '1', '1999-01-01', 'q.Ninh Kiều, tp.Cần Thơ', NULL, 'phamvanf@ctu.edu.vn', '098930005', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729845293/NguoiDung/a6wiydsmwefoojijtp05.jpg', 'NguoiDung/a6wiydsmwefoojijtp05', 1, 8),
	('iZ20240919194839', 'Võ Hoàng Tuấn Đạt G', '1', '2002-10-29', ' tp.Cà Mau', NULL, 'halaopis479@cironex.com', '0974000162', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726750121/NguoiDung/b0pyecdhyid0q37zlpy2.jpg', 'NguoiDung/b0pyecdhyid0q37zlpy2', 1, 5),
	('J820241109142300', 'Trần Vĩnh2', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'hanagaming5@gmail.com', '0974000754', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1731136983/NguoiDung/pg2btxfazvhkkih12oqj.jpg', 'NguoiDung/pg2btxfazvhkkih12oqj', 1, 5),
	('jp20240930165451', 'Hồ Minh Trường', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'kepon8893@gianes.com', '0974000652', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727690096/NguoiDung/cpvj0gxxgzyevrbw7hgl.jpg', 'NguoiDung/cpvj0gxxgzyevrbw7hgl', 1, 5),
	('ME20241017143125', 'TranVanA', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001055', 'pudrenurdu@gufum.com', '040714015', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150291/NguoiDung/mzo3aew6ixn1asas3bno.jpg', 'NguoiDung/mzo3aew6ixn1asas3bno', 1, 2),
	('Pe20240916221033', 'Lê Hữu Đức', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000322327445', 'wobaxa3469@acroins.com', '0974000352', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726499436/NguoiDung/nhdc4uzjfwwpwmbcmcpr.jpg', 'NguoiDung/nhdc4uzjfwwpwmbcmcpr', 1, 5),
	('pf20241109234113', 'Châu Phi Hùng', '1', '2024-11-01', 'Quận ninh kiều ,Thành phố cần thơ', NULL, 'chauphihung@gmail.com', '0947517952', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1731170474/NguoiDung/zx2we5wi8ybw7bgseecg.jpg', 'NguoiDung/zx2we5wi8ybw7bgseecg', 1, 5),
	('ps20241109221931', 'Châu Tiểu Long', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'chauTieuLong@gmail.com', '0974000755', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1731165573/NguoiDung/pxsbq5qerrjhuh40v4a3.jpg', 'NguoiDung/pxsbq5qerrjhuh40v4a3', 1, 5),
	('Pv20241025151431', 'PhamVanH', '1', '1999-01-01', 'q.Ninh Kiều, tp.Cần Thơ', NULL, 'yinikil924@edectus.com', '098950005', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729844076/NguoiDung/qfyf9uwti3adcqptxg6d.jpg', 'NguoiDung/qfyf9uwti3adcqptxg6d', 1, 8),
	('RM20241003003046', 'NguyenVanD', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170000852', 'nguyenvand@gmail.com', '040714010', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727890250/NguoiDung/bun34h6wktbzda5o5tug.jpg', 'NguoiDung/bun34h6wktbzda5o5tug', 1, 2),
	('rw20241025153937', 'PhamVanD', '1', '1999-01-01', 'q.Ninh Kiều, tp.Cần Thơ', NULL, 'rad12162@inohm.com', '098910005', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729845582/NguoiDung/hvqbyz73wrultd40uy9c.jpg', 'NguoiDung/hvqbyz73wrultd40uy9c', 1, 8),
	('rY20240918210506', 'Đỗ Thánh', '1', '2002-10-19', 'Q.Ninh Kiều, tp.Càn Thơ', '10000000008', 'pgbaop4@gmail.com', 'admin2', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726668312/NguoiDung/whyuizgynfpg63zrogdp.jpg', 'NguoiDung/whyuizgynfpg63zrogdp', 1, 1),
	('sT20240916220819', 'Lý Gia Nguyên', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000422327445', 'daxato3045@nestvia.com', '0974000452', 'null', 'null', 1, 5),
	('sX20241017143325', 'TranVanC', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001057', 'tranvanc@gmail.com', '040714017', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150408/NguoiDung/wp07ucvdpzo3yo1lf3mr.jpg', 'NguoiDung/wp07ucvdpzo3yo1lf3mr', 1, 2),
	('TC20241017143443', 'TranVanE', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001059', 'tranvane@gmail.com', '040714019', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150485/NguoiDung/t2pq9paw4kjszl7gs7zm.jpg', 'NguoiDung/t2pq9paw4kjszl7gs7zm', 1, 2),
	('uB20240930165759', 'Đường Bá Hổ', '1', '2024-10-17', '3/2, q.Ninh Kiều, tp.Cần Thơ', NULL, 'baob2016947@student.ctu.edu.vn', '0974000752', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1727690282/NguoiDung/r7stiwbtawuff8xcjqhn.jpg', 'NguoiDung/r7stiwbtawuff8xcjqhn', 1, 5),
	('ui20241017143233', 'TranVanB', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001056', 'liyor63542@aleitar.com', '040714016', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1729150356/NguoiDung/hgxqmkv6kjubrodqvubt.jpg', 'NguoiDung/hgxqmkv6kjubrodqvubt', 1, 2),
	('uO20241012212509', 'NguyenVanT', '1', '2002-01-01', 'hẻm 69,3/2, q.Ninh Kiều, tp.Cần Thơ', '0170001054', 'nguyenvant@gmail.com', '040714014', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1728743128/NguoiDung/odehghnembqoqctknl0d.jpg', 'NguoiDung/odehghnembqoqctknl0d', 1, 2),
	('Zh20240916232125', 'Phạm Thế HIển', '1', '2024-10-29', '3/2, q.Ninh Kiều, tp.Cần Thơ', '000122327445', 'riwopaokey@regishub.com', '0974000252', 'http://res.cloudinary.com/dkajnklq6/image/upload/v1726503689/NguoiDung/uiumpqhsuwsfri9anamz.jpg', 'NguoiDung/uiumpqhsuwsfri9anamz', 1, 5);

-- Dumping structure for table baucutructuyen.phanhoicanbo
CREATE TABLE IF NOT EXISTS `phanhoicanbo` (
  `YKien` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_CanBo` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CanBo` (`ID_CanBo`),
  CONSTRAINT `phanhoicanbo_ibfk_ID_CanBo` FOREIGN KEY (`ID_CanBo`) REFERENCES `canbo` (`ID_CanBo`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoicanbo: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phanhoicutri
CREATE TABLE IF NOT EXISTS `phanhoicutri` (
  `Ykien` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_CuTri` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_CuTri` (`ID_CuTri`),
  CONSTRAINT `phanhoicutri_ibfk_ID_CuTri` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoicutri: ~2 rows (approximately)
INSERT INTO `phanhoicutri` (`Ykien`, `ThoiDiem`, `ID_CuTri`) VALUES
	('test12345678', '2024-10-11 00:00:00', '20240930165802'),
	('cần cải thiện giao diện', '2024-10-12 00:00:00', '20240930165802');

-- Dumping structure for table baucutructuyen.phanhoiungcuvien
CREATE TABLE IF NOT EXISTS `phanhoiungcuvien` (
  `Ykien` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  `ID_ucv` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  KEY `ID_ucv` (`ID_ucv`),
  CONSTRAINT `phanhoiungcuvien_ibfk_ID_ucv` FOREIGN KEY (`ID_ucv`) REFERENCES `ungcuvien` (`ID_ucv`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phanhoiungcuvien: ~0 rows (approximately)

-- Dumping structure for table baucutructuyen.phieubau
CREATE TABLE IF NOT EXISTS `phieubau` (
  `ID_Phieu` varchar(18) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `GiaTriPhieuBau` bigint DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  `ID_cap` smallint DEFAULT NULL,
  PRIMARY KEY (`ID_Phieu`),
  KEY `ngayBD` (`ngayBD`),
  KEY `FK_DMUC_Phieu` (`ID_cap`),
  CONSTRAINT `FK_DMUC_Phieu` FOREIGN KEY (`ID_cap`) REFERENCES `danhmucungcu` (`ID_Cap`),
  CONSTRAINT `phieubau_ibfk_ngayBD` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.phieubau: ~6 rows (approximately)
INSERT INTO `phieubau` (`ID_Phieu`, `GiaTriPhieuBau`, `ngayBD`, `ID_cap`) VALUES
	('a52024102720424695', 1069714063, '2024-10-22 12:12:12', 3),
	('C42024102720394280', 81748566241, '2024-10-22 12:12:12', 3),
	('gD2024102720412253', 75715792743, '2024-10-22 12:12:12', 3),
	('gg2024102720355539', 40278313698, '2024-10-22 12:12:12', 3),
	('HL2024102720322871', 79519378900, '2024-10-22 12:12:12', 3),
	('ok2024102720375308', 85427687397, '2024-10-22 12:12:12', 3),
	('w62024111110165247', 84791985507, '2024-10-22 12:12:12', 3);

-- Dumping structure for table baucutructuyen.quanhuyen
CREATE TABLE IF NOT EXISTS `quanhuyen` (
  `ID_QH` int NOT NULL AUTO_INCREMENT,
  `TenQH` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `STT` tinyint DEFAULT NULL,
  PRIMARY KEY (`ID_QH`),
  KEY `STT` (`STT`),
  CONSTRAINT `quanhuyen_ibfk_1` FOREIGN KEY (`STT`) REFERENCES `tinhthanh` (`STT`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
  `TaiKhoan` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `MatKhau` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT '0',
  `BiKhoa` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `LyDoKhoa` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `NgayTao` datetime DEFAULT NULL,
  `SuDung` tinyint DEFAULT NULL,
  `RoleID` tinyint NOT NULL,
  PRIMARY KEY (`TaiKhoan`),
  KEY `RoleID` (`RoleID`),
  CONSTRAINT `fk_taikhoan_SDT` FOREIGN KEY (`TaiKhoan`) REFERENCES `nguoidung` (`SDT`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `taikhoan_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `vaitro` (`RoleID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.taikhoan: ~31 rows (approximately)
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
	('0947517952', '$argon2id$v=19$m=65536,t=3,p=1$aLTFHuHEDZg5xnRGtue+Ww$5nJmA42xKVNa04zO8tgNzi6wYMQrF+HPZmt6FRvux+c', '0', 'null', '2024-11-09 23:41:17', 1, 5),
	('0947888952', '$argon2id$v=19$m=65536,t=3,p=1$5ArWgnDSiuTZaanSb1lJ0Q$1AoC6J/+rgtbC3HMkiZqMHasVUY3L+Y0wNttv1E8ZKQ', '0', 'null', '2024-11-14 15:00:58', 1, 2),
	('0974000162', '$argon2id$v=19$m=65536,t=3,p=1$9X213jZA5D3IQ3fTHUD/Kg$jkdrrtKPM0kQN9t5ssvcwDDRHIzwJL8Qb02BM9HRwR8', '0', 'null', '2024-09-19 19:48:45', 1, 5),
	('0974000252', '$argon2id$v=19$m=65536,t=3,p=1$XRd1Nk4gP6vLTZoGP5fIaQ$MMZV6UTKcjvX/DqrZWID66KfWux81bCFjMo2VrH9Ffc', '0', 'null', '2024-09-16 23:21:32', 1, 5),
	('0974000352', '$argon2id$v=19$m=65536,t=3,p=1$EyaIakq2nLN3FumoP9cPTQ$xLF09ge+h6hTd8YXqBGlVDlyKCTYmg80QY6Xz5ppzVM', '0', 'null', '2024-09-16 22:10:39', 1, 5),
	('0974000452', '$argon2id$v=19$m=65536,t=3,p=1$xKk6fb1jvXh0Ys6eZDG/Gg$EsHDttfk8Lp9V3sLEVqSiu1eHmkjpRGxaUuFHpHHQYs', '0', 'null', '2024-09-16 22:08:19', 1, 5),
	('0974000552', '$argon2id$v=19$m=65536,t=3,p=1$5dnHAQUKnugZ7P/RXY10sg$aIhTrCG440ign1ZtoRQJU5pHvMkFRlGmgOf3cIkToBA', '0', 'null', '2024-09-19 20:00:22', 1, 5),
	('0974000652', '$argon2id$v=19$m=65536,t=3,p=1$GJltnBx0XqLkHK7YhKWNzg$7TqoG31MwUe694TBhZytH6kyG7OkQw54wd+TLvskDy0', '0', 'null', '2024-09-30 16:54:56', 1, 5),
	('0974000752', '$argon2id$v=19$m=65536,t=3,p=1$ZBZQmzk6awvKlkjRMA+WSA$mBoieyV1+gSj6A3oc5HSTZCv8PNaa0yI0iY4oxa9qzY', '0', 'null', '2024-09-30 16:58:02', 1, 5),
	('0974000754', '$argon2id$v=19$m=65536,t=3,p=1$d7FT6U0tLnBcrb/jph2mQQ$Eew7oU70LZMXWseAH5PSxd9jTRLTO9bEzFStz8/pD2U', '0', 'null', '2024-11-09 14:23:04', 1, 5),
	('0974000755', '$argon2id$v=19$m=65536,t=3,p=1$GubhtuQZvj+v1Z3HFNunYQ$tYxK6vuhkqrkKlh5aWUvLQ1lky6a4RccfvcCcza8dqw', '0', 'null', '2024-11-09 22:19:35', 1, 5),
	('0974008888', '$argon2id$v=19$m=65536,t=3,p=1$4OC/RiPyafhivUcKqxIifA$GPqn2J9Gx+ujZW/nGjmYKLIXcNDOWngwaOob8D68qgw', '0', 'null', '2024-11-09 13:45:58', 1, 5),
	('098910005', '$argon2id$v=19$m=65536,t=3,p=1$DNJr/NGXxMiBukWReNhogQ$2n5vOuC1AfcJfdecTlMJocUXkhudiRjgde7QxPqkce8', '0', 'null', '2024-10-25 15:39:42', 1, 8),
	('098920005', '$argon2id$v=19$m=65536,t=3,p=1$7HSj4A9ow4uGmISgjOKW7A$d3VLg1qXO1+5Q124e5NPvnfLb8SxjfIH0BpOd26ZRCg', '0', 'null', '2024-10-25 15:37:20', 1, 8),
	('098930005', '$argon2id$v=19$m=65536,t=3,p=1$C8nUcEt1BDjal7+J7PByYQ$TvjFt5FysArjRi7YoSEVvxEaHtLRw5mrS347CkOzt64', '0', 'null', '2024-10-25 15:34:53', 1, 8),
	('098940005', '$argon2id$v=19$m=65536,t=3,p=1$5lxnCi5mBeNLwC9xA9tRMQ$aPm7ItUGkPn0Vp94hNOnMAjE3S5yLPzlW83ZxUYV2m0', '0', 'null', '2024-10-25 15:24:18', 1, 8),
	('098950005', '$argon2id$v=19$m=65536,t=3,p=1$bOwWIzPq+mbIWrNSsXcqmA$2MLSj/9zB0xeeugJIkfBYGs2FOf4MaA/bLkTeswqJK8', '0', 'null', '2024-10-25 15:14:35', 1, 8),
	('admin2', '$argon2id$v=19$m=65536,t=3,p=1$0tfopQz8TcG6GvpAl28d0g$jOrPw+GGqpVOv0OmOt53AnK7HbGD4sbuUFScCjR8wMA', '0', 'null', '2024-09-18 21:05:13', 1, 1);

-- Dumping structure for table baucutructuyen.thongbao
CREATE TABLE IF NOT EXISTS `thongbao` (
  `ID_ThongBao` int NOT NULL AUTO_INCREMENT,
  `NoiDungThongBao` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ThoiDiem` datetime DEFAULT NULL,
  PRIMARY KEY (`ID_ThongBao`)
) ENGINE=InnoDB AUTO_INCREMENT=212 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.thongbao: ~154 rows (approximately)
INSERT INTO `thongbao` (`ID_ThongBao`, `NoiDungThongBao`, `ThoiDiem`) VALUES
	(1, 'Ngày bầu cử', '2024-12-18 10:15:44'),
	(2, 'Ngay đắt cử', '2024-12-18 11:30:44'),
	(3, 'Chuẩn bị cuộc bầu cử', '2024-10-02 09:21:34'),
	(59, 'Thông báo kết quả bầu cử tại kỳ: 2024-10-22 12:12:12', '2024-10-28 15:21:28'),
	(60, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:57'),
	(61, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:57'),
	(62, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:57'),
	(63, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:57'),
	(64, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:58'),
	(65, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:01:58'),
	(66, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:01:58'),
	(67, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:01:58'),
	(68, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:01:58'),
	(69, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:58'),
	(70, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:58'),
	(71, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:58'),
	(72, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:58'),
	(73, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:58'),
	(74, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:59'),
	(75, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:59'),
	(76, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:59'),
	(77, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:01:59'),
	(78, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 14:01:59'),
	(79, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 14:01:59'),
	(80, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:59'),
	(81, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:59'),
	(82, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:59'),
	(83, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:59'),
	(84, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:01:59'),
	(85, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:45'),
	(86, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(87, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(88, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(89, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(90, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:44:46'),
	(91, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:44:46'),
	(92, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:44:46'),
	(93, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 14:44:46'),
	(94, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:46'),
	(95, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:46'),
	(96, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:46'),
	(97, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:46'),
	(98, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(99, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(100, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(101, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(102, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 14:44:46'),
	(103, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 14:44:46'),
	(104, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 14:44:46'),
	(105, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:46'),
	(106, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:47'),
	(107, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:47'),
	(108, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:47'),
	(109, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 14:44:47'),
	(110, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:40'),
	(111, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:40'),
	(112, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:40'),
	(113, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:40'),
	(114, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:40'),
	(115, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 18:34:40'),
	(116, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 18:34:40'),
	(117, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 18:34:40'),
	(118, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 18:34:41'),
	(119, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(120, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(121, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(122, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(123, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:41'),
	(124, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:41'),
	(125, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:41'),
	(126, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:41'),
	(127, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 18:34:41'),
	(128, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 18:34:41'),
	(129, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 18:34:41'),
	(130, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(131, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(132, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(133, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(134, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 18:34:41'),
	(135, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(136, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(137, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(138, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(139, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(140, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 21:33:27'),
	(141, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 21:33:27'),
	(142, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 21:33:27'),
	(143, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 21:33:27'),
	(144, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:27'),
	(145, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:27'),
	(146, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:27'),
	(147, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:27'),
	(148, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(149, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(150, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(151, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(152, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 21:33:27'),
	(153, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 21:33:27'),
	(154, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 21:33:28'),
	(155, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:28'),
	(156, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:28'),
	(157, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:28'),
	(158, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:28'),
	(159, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 21:33:28'),
	(160, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:07'),
	(161, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:07'),
	(162, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:07'),
	(163, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:07'),
	(164, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:07'),
	(165, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:33:07'),
	(166, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:33:07'),
	(167, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:33:07'),
	(168, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:33:07'),
	(169, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:07'),
	(170, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:07'),
	(171, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:07'),
	(172, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:07'),
	(173, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:08'),
	(174, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:08'),
	(175, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:08'),
	(176, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:08'),
	(177, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:33:08'),
	(178, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 23:33:08'),
	(179, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 23:33:08'),
	(180, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:08'),
	(181, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:08'),
	(182, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:08'),
	(183, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:08'),
	(184, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:33:08'),
	(185, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(186, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(187, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(188, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(189, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(190, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:34:35'),
	(191, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:34:35'),
	(192, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:34:35'),
	(193, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:10 SA.', '2024-11-01 23:34:35'),
	(194, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:35'),
	(195, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:35'),
	(196, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:35'),
	(197, 'Ngày bầu cử sắp tới của bạn tham dự là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:35'),
	(198, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(199, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(200, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(201, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(202, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:15:00 SA.', '2024-11-01 23:34:35'),
	(203, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 23:34:35'),
	(204, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:10 SA.', '2024-11-01 23:34:35'),
	(205, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:35'),
	(206, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:36'),
	(207, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:36'),
	(208, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:36'),
	(209, 'Ngày bầu cử sắp tới của bạn là 03/11/2024 10:25:11 SA.', '2024-11-01 23:34:36'),
	(210, 'Thông báo kết quả bầu cử tại kỳ: 2024-10-22 12:12:12', '2024-11-11 12:07:21'),
	(211, 'Thông báo kết quả bầu cử tại kỳ: 2024-10-22 12:12:12', '2024-11-11 19:39:20');

-- Dumping structure for table baucutructuyen.tinhthanh
CREATE TABLE IF NOT EXISTS `tinhthanh` (
  `STT` tinyint NOT NULL,
  `TenTinhThanh` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`STT`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.tinhthanh: ~83 rows (approximately)
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
  `GhiNhan` varchar(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT '0',
  `ID_CuTri` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `ID_DonViBauCu` smallint DEFAULT NULL,
  `ngayBD` datetime DEFAULT NULL,
  KEY `ID_DonViBauCu` (`ID_DonViBauCu`),
  KEY `index_TrangThaiBauCu_ngayBD` (`ngayBD`),
  KEY `index_TrangThaiBauCu_ID_CuTri` (`ID_CuTri`),
  KEY `idx_ttbaucu_CuTri_ngayBD` (`ID_CuTri`,`ngayBD`),
  KEY `idx_trangthaibaucu_ngayBD` (`ngayBD`),
  CONSTRAINT `fk_trangThaiBauCu_ID_CuTri` FOREIGN KEY (`ID_CuTri`) REFERENCES `cutri` (`ID_CuTri`) ON DELETE CASCADE,
  CONSTRAINT `fk_trangThaiBauCu_ID_DonViBauCu` FOREIGN KEY (`ID_DonViBauCu`) REFERENCES `donvibaucu` (`ID_DonViBauCu`) ON DELETE CASCADE,
  CONSTRAINT `fk_trangThaiBauCu_ngayBD` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`) ON DELETE CASCADE,
  CONSTRAINT `trangthaibaucu_ibfk_3` FOREIGN KEY (`ngayBD`) REFERENCES `kybaucu` (`ngayBD`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.trangthaibaucu: ~39 rows (approximately)
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
	('1', '20240919200022', 1, '2024-10-22 12:12:12'),
	('1', '20240916232132', 1, '2024-10-22 12:12:12'),
	('0', '20240916220819', 1, '2024-10-22 12:12:12'),
	('1', '20240916221039', 1, '2024-10-22 12:12:12'),
	('1', '20240930165456', 1, '2024-10-22 12:12:12'),
	('1', '20240919194845', 1, '2024-10-22 12:12:12'),
	('0', '20241109134558', 9, '2024-08-13 07:00:19'),
	('0', '20240919200022', 9, '2024-08-13 07:00:19'),
	('0', '20240919194845', 9, '2024-08-13 07:00:19'),
	('0', '20240916232132', 9, '2024-08-13 07:00:19'),
	('0', '20240930165802', 9, '2024-08-13 07:00:19'),
	('0', '20241109142305', 9, '2024-08-13 07:00:19'),
	('0', '20240930165456', 9, '2024-08-13 07:00:19'),
	('0', '20241109234117', 3, '2024-09-19 23:54:51'),
	('0', '20240930165456', 3, '2024-09-19 23:54:51'),
	('0', '20241109142305', 3, '2024-09-19 23:54:51');

-- Dumping structure for table baucutructuyen.trinhdohocvan
CREATE TABLE IF NOT EXISTS `trinhdohocvan` (
  `ID_TrinhDo` smallint NOT NULL AUTO_INCREMENT,
  `TenTrinhDoHocVan` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_TrinhDo`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.trinhdohocvan: ~10 rows (approximately)
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
  `ID_ucv` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `TrangThai` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT 'active',
  `ID_user` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `GioiThieu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`ID_ucv`),
  KEY `FK_userUngCuVien` (`ID_user`),
  CONSTRAINT `FK_userUngCuVien_ID_user` FOREIGN KEY (`ID_user`) REFERENCES `nguoidung` (`ID_user`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.ungcuvien: ~13 rows (approximately)
INSERT INTO `ungcuvien` (`ID_ucv`, `TrangThai`, `ID_user`, `GioiThieu`) VALUES
	('20241003000921', 'active', 'Ds20241003000915', 'KOKOKOKOKOKOK'),
	('20241003002121', 'active', '7H20241003002115', 'OKOKOKOK'),
	('20241003002930', 'active', 'Gu20241003002922', 'OKOKOKOK'),
	('20241003003050', 'active', 'RM20241003003046', 'OKOKOK'),
	('20241003003142', 'active', 'Do20241003003139', 'OKOK'),
	('20241012212410', 'active', '5520241012212400', 'rất ok.'),
	('20241012212527', 'active', 'uO20241012212509', 'rất ok.'),
	('20241017143132', 'active', 'ME20241017143125', 'rất ok.'),
	('20241017143237', 'active', 'ui20241017143233', 'rất ok.'),
	('20241017143328', 'active', 'sX20241017143325', 'rất ok.'),
	('20241017143409', 'active', 'Dz20241017143406', 'rất ok.'),
	('20241017143445', 'active', 'TC20241017143443', 'rất ok.'),
	('20241114150058', 'acctive', '2K20241114150052', 'OKOKOKOK');

-- Dumping structure for table baucutructuyen.vaitro
CREATE TABLE IF NOT EXISTS `vaitro` (
  `RoleID` tinyint NOT NULL AUTO_INCREMENT,
  `TenVaiTro` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table baucutructuyen.vaitro: ~7 rows (approximately)
INSERT INTO `vaitro` (`RoleID`, `TenVaiTro`) VALUES
	(1, 'Admin'),
	(2, 'Ứng cử viên'),
	(3, 'Ban Tổ chức'),
	(4, 'Ban kiểm phiếu'),
	(5, 'Cử tri'),
	(8, 'Cán bộ'),
	(11, 'Test 123456888');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
