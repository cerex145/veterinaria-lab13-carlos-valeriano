IF OBJECT_ID(N'dbo.Usuarios') IS NULL
CREATE TABLE dbo.Usuarios (
 Id int IDENTITY PRIMARY KEY,
 Correo nvarchar(254) NOT NULL CONSTRAINT UQ_Usuarios_Correo UNIQUE,
 ClaveHash nvarchar(100) NOT NULL,
 Rol nvarchar(30) NOT NULL CHECK (Rol IN ('Recepcionista','Veterinario')));
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo='recepcionista@veterinaria.test')
INSERT dbo.Usuarios(Correo,ClaveHash,Rol) VALUES ('recepcionista@veterinaria.test','$2b$12$x3bPAoF/zly3KpXJMk15WO52E52Lo8JUv5.uT0u7cdbIvfn8MEol2','Recepcionista');
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo='veterinario@veterinaria.test')
INSERT dbo.Usuarios(Correo,ClaveHash,Rol) VALUES ('veterinario@veterinaria.test','$2b$12$uYJuxdEz0veKLBQpgORYuelTqu5862mmucMZaQ.htq7ZffVEsHWaW','Veterinario');
