CREATE TABLE ChiTietThongBaoUngCuVien(
	ID_ThongBao INT,
	ID_ucv VARCHAR(14),
	FOREIGN KEY(ID_ucv) REFERENCES UngCuVien(ID_ucv),
	FOREIGN KEY(ID_ThongBao) REFERENCES ThongBao(ID_ThongBao)
);