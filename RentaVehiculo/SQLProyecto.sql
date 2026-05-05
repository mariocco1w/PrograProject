CREATE DATABASE RentaVehiculos;
GO

USE RentaVehiculos;
GO

CREATE TABLE Usuario(
IdUsuario INT PRIMARY KEY IDENTITY (1,1),
Username NVARCHAR(50) NOT NULL,
Password NVARCHAR(100) NOT NULL,
Rol NVARCHAR(20), CHECK (Rol IN ('Admin','Cliente'))
);

CREATE TABLE Vehiculo(
Placa NVARCHAR(20) PRIMARY KEY,
Marca NVARCHAR(50) NOT NULL,
PrecioPorDia DECIMAL(10,2) NOT NULL,
RutaImagen NVARCHAR(MAX),
Disponible BIT NOT NULL DEFAULT 1
);