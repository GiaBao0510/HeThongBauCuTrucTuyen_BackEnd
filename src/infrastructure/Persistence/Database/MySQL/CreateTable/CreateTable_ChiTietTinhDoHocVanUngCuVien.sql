CREATE TABLE ChiTietTrinhDoHocVanUngCuVien(
	ID_TrinhDo SMALLINT, 
	ID_ucv VARCHAR(14),
	FOREIGN KEY(ID_ucv) REFERENCES UngCuVien(ID_ucv),
	FOREIGN KEY(ID_TrinhDo) REFERENCES TrinhDoHocVan(ID_TrinhDo)
);