IF DB_ID(N'VeterinariaLab13') IS NULL CREATE DATABASE VeterinariaLab13;
GO
USE VeterinariaLab13;
GO
IF OBJECT_ID(N'dbo.Propietarios') IS NULL
CREATE TABLE dbo.Propietarios (
 Id int IDENTITY PRIMARY KEY, Nombre nvarchar(100) NOT NULL,
 Dni varchar(8) NOT NULL CONSTRAINT UQ_Propietarios_Dni UNIQUE,
 Telefono nvarchar(20) NOT NULL);
IF OBJECT_ID(N'dbo.Mascotas') IS NULL
CREATE TABLE dbo.Mascotas (
 Id int IDENTITY PRIMARY KEY, Nombre nvarchar(80) NOT NULL,
 Especie nvarchar(40) NOT NULL, Edad int NOT NULL CONSTRAINT CK_Mascotas_Edad CHECK (Edad BETWEEN 0 AND 40),
 PropietarioId int NOT NULL CONSTRAINT FK_Mascotas_Propietarios REFERENCES dbo.Propietarios(Id));
IF NOT EXISTS (SELECT 1 FROM dbo.Propietarios)
BEGIN
 INSERT dbo.Propietarios(Nombre,Dni,Telefono) VALUES (N'Ana Torres','12345678','987654321'),(N'Luis Rojas','87654321','912345678');
 INSERT dbo.Mascotas(Nombre,Especie,Edad,PropietarioId) VALUES (N'Luna',N'Perro',3,1),(N'Milo',N'Gato',2,1),(N'Toby',N'Perro',5,2);
END;
GO
