
USE [master];
GO

-- 1. Crear base de datos
CREATE DATABASE [db_facturacion];
GO

USE [db_facturacion];
GO

-- 2. Tablas

-- 2.1 Tabla de Artículos
CREATE TABLE dbo.T_Articulos (
    Codigo             INT           IDENTITY(1,1) NOT NULL,
    Nombre             VARCHAR(50)   NOT NULL,
    PrecioUnitario     DECIMAL(18,2) NOT NULL,
    Activo             BIT           NOT NULL CONSTRAINT DF_Articulos_Activo DEFAULT(1),
    CONSTRAINT PK_T_Articulos PRIMARY KEY CLUSTERED (Codigo)
);
GO

-- 2.2 Tabla de Formas de Pago (lookup)
CREATE TABLE dbo.T_Formas_Pago (
    IdFormaPago   INT           IDENTITY(1,1) NOT NULL,
    Descripcion   VARCHAR(50)   NOT NULL,
    Activo        BIT           NOT NULL CONSTRAINT DF_FormasPago_Activo DEFAULT(1),
    CONSTRAINT PK_T_Formas_Pago PRIMARY KEY CLUSTERED (IdFormaPago)
);
GO

-- 2.3 Tabla de Facturas (maestro)
CREATE TABLE dbo.T_Facturas (
    NumeroFactura   INT           IDENTITY(1,1) NOT NULL,
    Fecha           DATETIME      NOT NULL,
    FormaPago       INT           NOT NULL,
    Cliente         VARCHAR(50)   NOT NULL,
    Activo          BIT           NOT NULL CONSTRAINT DF_Facturas_Activo DEFAULT(1),
    CONSTRAINT PK_T_Facturas PRIMARY KEY CLUSTERED (NumeroFactura),
    CONSTRAINT FK_Facturas_FormasPago FOREIGN KEY (FormaPago)
        REFERENCES dbo.T_Formas_Pago (IdFormaPago)
);
GO

-- 2.4 Tabla de Detalles (detalle)
CREATE TABLE dbo.T_Detalles_Factura (
    IdDetalle       INT           IDENTITY(1,1) NOT NULL,
    NumeroFactura   INT           NOT NULL,
    CodigoArticulo  INT           NOT NULL,
    Cantidad        INT           NOT NULL,
    Precio          DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_T_Detalles_Factura PRIMARY KEY CLUSTERED (IdDetalle),
    CONSTRAINT FK_Detalles_Factura_Facturas FOREIGN KEY (NumeroFactura)
        REFERENCES dbo.T_Facturas (NumeroFactura)
        ON DELETE CASCADE,
    CONSTRAINT FK_Detalles_Factura_Articulos FOREIGN KEY (CodigoArticulo)
        REFERENCES dbo.T_Articulos (Codigo)
);
GO

-- 3. Datos de prueba
INSERT INTO dbo.T_Articulos (Nombre, PrecioUnitario) VALUES 
  ('Leche', 50.00),
  ('Pan',   20.00);
GO

INSERT INTO dbo.T_Formas_Pago (Descripcion) VALUES 
  ('Efectivo'),
  ('Tarjeta'),
  ('Transferencia');
GO

-- 4. Índices de apoyo
CREATE NONCLUSTERED INDEX IX_Detalles_NumeroFactura 
    ON dbo.T_Detalles_Factura(NumeroFactura);
GO

CREATE NONCLUSTERED INDEX IX_Detalles_CodigoArticulo 
    ON dbo.T_Detalles_Factura(CodigoArticulo);
GO

-- 5. Procedimientos almacenados

-- 5.1 Artículos
CREATE PROCEDURE dbo.SP_RECUPERAR_ARTICULOS
AS
BEGIN
    SELECT Codigo, Nombre, PrecioUnitario, Activo
      FROM dbo.T_Articulos
     WHERE Activo = 1
     ORDER BY Nombre;
END;
GO

CREATE PROCEDURE dbo.SP_RECUPERAR_ARTICULO_POR_ID
    @Codigo INT
AS
BEGIN
    SELECT Codigo, Nombre, PrecioUnitario, Activo
      FROM dbo.T_Articulos
     WHERE Codigo = @Codigo
       AND Activo = 1;
END;
GO

-- 5.2 Formas de Pago
CREATE PROCEDURE dbo.SP_RECUPERAR_FORMAS_PAGO
AS
BEGIN
    SELECT IdFormaPago, Descripcion, Activo
      FROM dbo.T_Formas_Pago
     WHERE Activo = 1
     ORDER BY Descripcion;
END;
GO

CREATE PROCEDURE dbo.SP_RECUPERAR_FORMA_PAGO_POR_ID
    @IdFormaPago INT
AS
BEGIN
    SELECT IdFormaPago, Descripcion, Activo
      FROM dbo.T_Formas_Pago
     WHERE IdFormaPago = @IdFormaPago
       AND Activo = 1;
END;
GO

-- 5.3 Facturas (maestro)
CREATE PROCEDURE dbo.SP_RECUPERAR_FACTURAS
AS
BEGIN
    SELECT NumeroFactura, Fecha, FormaPago, Cliente
      FROM dbo.T_Facturas
     WHERE Activo = 1
     ORDER BY NumeroFactura;
END;
GO

CREATE PROCEDURE dbo.SP_RECUPERAR_FACTURA_POR_ID
    @NumeroFactura INT
AS
BEGIN
    SELECT NumeroFactura, Fecha, FormaPago, Cliente
      FROM dbo.T_Facturas
     WHERE NumeroFactura = @NumeroFactura
       AND Activo = 1;
END;
GO

CREATE PROCEDURE dbo.SP_INSERTAR_FACTURA
    @Fecha DATETIME,
    @FormaPago INT,
    @Cliente VARCHAR(50),
    @NumeroFactura INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.T_Facturas (Fecha, FormaPago, Cliente)
    VALUES (@Fecha, @FormaPago, @Cliente);

    SET @NumeroFactura = SCOPE_IDENTITY();
END;
GO

CREATE PROCEDURE dbo.SP_BAJA_LOGICA_FACTURA
    @NumeroFactura INT
AS
BEGIN
    UPDATE dbo.T_Facturas
       SET Activo = 0
     WHERE NumeroFactura = @NumeroFactura;
END;
GO

-- 5.4 Detalles (detalle)
CREATE PROCEDURE dbo.SP_RECUPERAR_DETALLES_POR_FACTURA
    @NumeroFactura INT
AS
BEGIN
    SELECT df.IdDetalle,
           df.NumeroFactura,
           df.CodigoArticulo,
           a.Nombre,
           df.Cantidad,
           df.Precio
      FROM dbo.T_Detalles_Factura AS df
      JOIN dbo.T_Articulos           AS a
        ON a.Codigo = df.CodigoArticulo
     WHERE df.NumeroFactura = @NumeroFactura;
END;
GO

CREATE PROCEDURE dbo.SP_INSERTAR_DETALLE_FACTURA
    @NumeroFactura INT,
    @CodigoArticulo INT,
    @Cantidad INT,
    @Precio DECIMAL(18,2)
AS
BEGIN
    INSERT INTO dbo.T_Detalles_Factura
        (NumeroFactura, CodigoArticulo, Cantidad, Precio)
    VALUES
        (@NumeroFactura, @CodigoArticulo, @Cantidad, @Precio);
END;
GO

-- 6. Ajustes finales
USE [master];
GO
ALTER DATABASE [db_facturacion] SET READ_WRITE;
GO

SELECT * FROM T_Articulos;

SELECT * 
  FROM T_Articulos
 WHERE Codigo = 10;


 INSERT INTO T_Articulos (Codigo, Nombre, PrecioUnitario)
VALUES (10, 'Artículo Prueba', 150.00);




--------------------------------------------------------------------------------------------

IF OBJECT_ID('dbo.SP_INSERTAR_ARTICULO', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_INSERTAR_ARTICULO;
GO

CREATE PROCEDURE dbo.SP_INSERTAR_ARTICULO
    @Nombre         NVARCHAR(100),
    @PrecioUnitario DECIMAL(18,2),
    @NuevoCodigo    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.T_Articulos (Nombre, PrecioUnitario)
    VALUES (@Nombre, @PrecioUnitario);

    -- Captura el identity recién generado
    SET @NuevoCodigo = SCOPE_IDENTITY();
END
GO

------------------------------------------------------------------------------------------------

CREATE PROCEDURE dbo.SP_INSERTAR_ARTICULO
    @Nombre        NVARCHAR(100),
    @PrecioUnitario DECIMAL(18,2),
    @NuevoCodigo   INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  INSERT INTO dbo.T_Articulos (Nombre, PrecioUnitario)
  VALUES (@Nombre, @PrecioUnitario);
  SET @NuevoCodigo = SCOPE_IDENTITY();
END

------------------------------------------------------------------------------------------

USE [db_facturacion];
GO

-- Si ya existe, lo borra
IF OBJECT_ID('dbo.SP_INSERTAR_ARTICULO', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_INSERTAR_ARTICULO;
GO

-- Ahora creás de nuevo
CREATE PROCEDURE dbo.SP_INSERTAR_ARTICULO
    @Nombre         NVARCHAR(100),
    @PrecioUnitario DECIMAL(18,2),
    @NuevoCodigo    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.T_Articulos (Nombre, PrecioUnitario)
    VALUES (@Nombre, @PrecioUnitario);

    SET @NuevoCodigo = SCOPE_IDENTITY();
END;
GO


------------------------------------------------------------------------------------------------------
USE [db_facturacion];
GO

ALTER PROCEDURE dbo.SP_RECUPERAR_DETALLES_POR_FACTURA
    @NumeroFactura INT
AS
BEGIN
    SELECT df.IdDetalle, 
           df.NumeroFactura, 
           df.CodigoArticulo, 
           a.Nombre, 
           a.PrecioUnitario,   
           df.Cantidad, 
           df.Precio
      FROM dbo.T_Detalles_Factura AS df
      JOIN dbo.T_Articulos           AS a
        ON a.Codigo = df.CodigoArticulo
     WHERE df.NumeroFactura = @NumeroFactura;
END;

--------------------------------------------------------------------------------------------
USE [db_facturacion];
GO


ALTER PROCEDURE dbo.SP_RECUPERAR_FACTURAS
AS
BEGIN
    SELECT f.NumeroFactura,
           f.Fecha,
           f.FormaPago,
           fp.Descripcion AS FormaPagoNombre,
           f.Cliente
      FROM dbo.T_Facturas f
      JOIN dbo.T_Formas_Pago fp
        ON f.FormaPago = fp.IdFormaPago
     WHERE f.Activo = 1
     ORDER BY f.NumeroFactura;
END;