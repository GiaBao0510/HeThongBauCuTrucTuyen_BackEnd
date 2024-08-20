CREATE TABLE LichSuDangNhap(
	ThoiDiem DATE,
	DiaChiIP VARCHAR(30),
	TaiKhoan VARCHAR(30),
	FOREIGN KEY(TaiKhoan) REFERENCES TaiKhoan(TaiKhoan)
);