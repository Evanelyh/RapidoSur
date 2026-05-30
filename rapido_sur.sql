CREATE DATABASE IF NOT EXISTS rapidosur_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_spanish_ci;
 
USE rapidosur_db;

CREATE TABLE IF NOT EXISTS CLIENTE (
    id_cliente      INT             NOT NULL AUTO_INCREMENT,
    nombre          VARCHAR(60)     NOT NULL,
    apellido        VARCHAR(60)     NOT NULL,
    telefono        VARCHAR(15)     NULL,
    correo          VARCHAR(100)    NULL,
    direccion       VARCHAR(200)    NULL,
    fecha_registro  DATE            NOT NULL DEFAULT (CURRENT_DATE),
    CONSTRAINT PK_CLIENTE PRIMARY KEY (id_cliente)
);

CREATE TABLE IF NOT EXISTS OPERADOR (
    id_operador     INT             NOT NULL AUTO_INCREMENT,
    nombre          VARCHAR(60)     NOT NULL,
    apellido        VARCHAR(60)     NOT NULL,
    usuario         VARCHAR(50)     NOT NULL,
    clave           VARCHAR(255)    NOT NULL,
    rol             VARCHAR(30)     NOT NULL DEFAULT 'Operador',
    correo          VARCHAR(100)    NULL,
    CONSTRAINT PK_OPERADOR  PRIMARY KEY (id_operador),
    CONSTRAINT UQ_USUARIO   UNIQUE      (usuario),
    CONSTRAINT CK_ROL CHECK (rol IN ('Operador', 'Administrador'))
);

CREATE TABLE IF NOT EXISTS VEHICULO (
    id_vehiculo         INT             NOT NULL AUTO_INCREMENT,
    placa               VARCHAR(15)     NOT NULL,
    tipo                VARCHAR(50)     NOT NULL,
    capacidad_carga_kg  DECIMAL(8,2)    NOT NULL,
    estado_operativo    VARCHAR(30)     NOT NULL DEFAULT 'Disponible',
    marca               VARCHAR(50)     NULL,
    modelo              VARCHAR(50)     NULL,
    anio                INT             NULL,
    CONSTRAINT PK_VEHICULO          PRIMARY KEY (id_vehiculo),
    CONSTRAINT UQ_PLACA             UNIQUE      (placa),
    CONSTRAINT CK_ESTADO_VEHICULO   CHECK (estado_operativo IN ('Disponible', 'En Ruta', 'En Mantenimiento')),
    CONSTRAINT CK_CAPACIDAD         CHECK (capacidad_carga_kg > 0)
);

CREATE TABLE IF NOT EXISTS CONDUCTOR (
    id_conductor    INT             NOT NULL AUTO_INCREMENT,
    nombre          VARCHAR(60)     NOT NULL,
    apellido        VARCHAR(60)     NOT NULL,
    num_licencia    VARCHAR(20)     NOT NULL,
    tipo_licencia   VARCHAR(10)     NULL,
    telefono        VARCHAR(15)     NULL,
    disponible      BOOLEAN         NOT NULL DEFAULT TRUE,
    fecha_ingreso   DATE            NULL,
    CONSTRAINT PK_CONDUCTOR     PRIMARY KEY (id_conductor),
    CONSTRAINT UQ_LICENCIA      UNIQUE      (num_licencia)
);

CREATE TABLE IF NOT EXISTS PEDIDO (
    id_pedido           INT             NOT NULL AUTO_INCREMENT,
    id_cliente          INT             NOT NULL,
    fecha_solicitud     DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    direccion_entrega   VARCHAR(200)    NOT NULL,
    tipo_carga          VARCHAR(80)     NULL,
    peso_kg             DECIMAL(8,2)    NULL,
    prioridad           VARCHAR(20)     NOT NULL DEFAULT 'Media',
    estado              VARCHAR(30)     NOT NULL DEFAULT 'Pendiente',
    observaciones       TEXT            NULL,
    CONSTRAINT PK_PEDIDO        PRIMARY KEY (id_pedido),
    CONSTRAINT FK_PEDIDO_CLI    FOREIGN KEY (id_cliente)
        REFERENCES CLIENTE(id_cliente)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT CK_PRIORIDAD     CHECK (prioridad IN ('Alta', 'Media', 'Baja')),
    CONSTRAINT CK_ESTADO_PEDIDO CHECK (estado IN ('Pendiente', 'Asignado', 'En Ruta', 'Entregado', 'Con Incidencia', 'Reprogramado', 'Cancelado'))
);

CREATE TABLE IF NOT EXISTS ENVIO (
    id_envio                INT             NOT NULL AUTO_INCREMENT,
    id_pedido               INT             NOT NULL,
    id_vehiculo             INT             NOT NULL,
    id_conductor            INT             NOT NULL,
    id_operador             INT             NOT NULL,
    fecha_asignacion        DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_entrega_estimada  DATE            NULL,
    fecha_entrega_real      DATE            NULL,
    ruta_descripcion        TEXT            NULL,
    estado_envio            VARCHAR(30)     NOT NULL DEFAULT 'Asignado',
    CONSTRAINT PK_ENVIO         PRIMARY KEY (id_envio),
    CONSTRAINT FK_ENVIO_PED     FOREIGN KEY (id_pedido)
        REFERENCES PEDIDO(id_pedido)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT FK_ENVIO_VEH     FOREIGN KEY (id_vehiculo)
        REFERENCES VEHICULO(id_vehiculo)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT FK_ENVIO_CON     FOREIGN KEY (id_conductor)
        REFERENCES CONDUCTOR(id_conductor)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT FK_ENVIO_OPE     FOREIGN KEY (id_operador)
        REFERENCES OPERADOR(id_operador)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT CK_ESTADO_ENVIO  CHECK (estado_envio IN ('Asignado', 'En Ruta', 'Entregado', 'Con Incidencia', 'Reprogramado'))
);


CREATE TABLE IF NOT EXISTS HISTORIAL_ESTADO (
    id_historial        INT             NOT NULL AUTO_INCREMENT,
    id_envio            INT             NOT NULL,
    estado              VARCHAR(30)     NOT NULL,
    fecha_actualizacion DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    descripcion         TEXT            NULL,
    registrado_por      VARCHAR(60)     NULL,
    CONSTRAINT PK_HISTORIAL     PRIMARY KEY (id_historial),
    CONSTRAINT FK_HIST_ENVIO    FOREIGN KEY (id_envio)
        REFERENCES ENVIO(id_envio)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);
 

CREATE INDEX IDX_PEDIDO_CLIENTE   ON PEDIDO          (id_cliente);
CREATE INDEX IDX_PEDIDO_ESTADO    ON PEDIDO          (estado);
CREATE INDEX IDX_ENVIO_PEDIDO     ON ENVIO           (id_pedido);
CREATE INDEX IDX_ENVIO_CONDUCTOR  ON ENVIO           (id_conductor);
CREATE INDEX IDX_ENVIO_VEHICULO   ON ENVIO           (id_vehiculo);
CREATE INDEX IDX_HIST_ENVIO       ON HISTORIAL_ESTADO(id_envio);
CREATE INDEX IDX_HIST_FECHA       ON HISTORIAL_ESTADO(fecha_actualizacion);

INSERT INTO CLIENTE (nombre, apellido, telefono, correo, direccion, fecha_registro) VALUES
('Carlos',    'Martínez',  '7823-1100', 'cmartinez@gmail.com',    'Colonia Escalón, San Salvador',              '2026-01-10'),
('Laura',     'Pérez',     '6712-2200', 'lperez@hotmail.com',     'Residencial Santa Elena, Antiguo Cuscatlán', '2026-01-15'),
('Roberto',   'Gómez',     '7934-3300', 'rgomez@yahoo.com',       'Barrio San Jacinto, San Salvador',           '2026-01-20'),
('Ana',       'López',     '6645-4400', 'alopez@gmail.com',       'Col. Jardines de Guadalupe, San Salvador',   '2026-01-25'),
('Mario',     'Hernández', '7756-5500', 'mhernandez@outlook.com', 'Urbanización Madre Selva, Soyapango',        '2026-02-01'),
('Sofía',     'Ramos',     '7867-6600', 'sramos@gmail.com',       'Residencial Las Palmas, San Miguel',         '2026-02-05'),
('Jorge',     'Torres',    '6978-7700', 'jtorres@gmail.com',      'Col. La Rábida, Santa Ana',                  '2026-02-10'),
('Carmen',    'Flores',    '7089-8800', 'cflores@yahoo.com',      'Barrio El Centro, Usulután',                 '2026-02-14'),
('Esteban',   'Cruz',      '7190-9900', 'ecruz@gmail.com',        'Col. Quiñónez, San Salvador',                '2026-02-18'),
('Patricia',  'Vásquez',   '7201-0011', 'pvasquez@gmail.com',     'Residencial Altavista, Antiguo Cuscatlán',   '2026-02-22');

INSERT INTO OPERADOR (nombre, apellido, usuario, clave, rol, correo) VALUES
('Andrea', 'Solano',    'asolano',    MD5('asolano123'),    'Operador',       'asolano@rapidosur.com'),
('Luis',   'Contreras', 'lcontreras', MD5('lcontreras123'), 'Administrador',  'lcontreras@rapidosur.com'),
('Diana',  'Morales',   'dmorales',   MD5('dmorales123'),   'Operador',       'dmorales@rapidosur.com'),
('René',   'Escobar',   'rescobar',   MD5('rescobar123'),   'Administrador',  'rescobar@rapidosur.com');

INSERT INTO VEHICULO (placa, tipo, capacidad_carga_kg, estado_operativo, marca, modelo, anio) VALUES
('P-123-ABC', 'Camión',    5000.00, 'Disponible',       'Toyota',   'Dyna',      2019),
('P-456-DEF', 'Furgoneta', 2000.00, 'Disponible',       'Hyundai',  'H100',      2020),
('P-789-GHI', 'Camión',    8000.00, 'En Ruta',          'Isuzu',    'NQR',       2018),
('P-321-JKL', 'Furgoneta', 1500.00, 'Disponible',       'Kia',      'Bongo',     2021),
('P-654-MNO', 'Camioneta', 1000.00, 'Disponible',       'Nissan',   'Frontier',  2022),
('P-987-PQR', 'Camión',    6000.00, 'En Mantenimiento', 'Mercedes', 'Atego',     2017),
('P-111-STU', 'Furgoneta', 2500.00, 'En Ruta',          'Ford',     'Transit',   2020),
('P-222-VWX', 'Camioneta', 1200.00, 'Disponible',       'Toyota',   'Hilux',     2023);

INSERT INTO CONDUCTOR (nombre, apellido, num_licencia, tipo_licencia, telefono, disponible, fecha_ingreso) VALUES
('Miguel', 'Argueta', 'LIC-001-SV', 'Liviana', '7312-1122', TRUE,  '2020-03-15'),
('Héctor', 'Juárez',  'LIC-002-SV', 'Pesada',  '7423-2233', FALSE, '2019-07-20'),
('Raúl',   'Pacheco', 'LIC-003-SV', 'Liviana', '7534-3344', TRUE,  '2021-01-10'),
('Jaime',  'Melara',  'LIC-004-SV', 'Pesada',  '7645-4455', FALSE, '2018-11-05'),
('Oscar',  'Chávez',  'LIC-005-SV', 'Liviana', '7756-5566', TRUE,  '2022-06-18'),
('Pedro',  'Aguilar', 'LIC-006-SV', 'Pesada',  '7867-6677', TRUE,  '2020-09-30');

INSERT INTO PEDIDO (id_cliente, fecha_solicitud, direccion_entrega, tipo_carga, peso_kg, prioridad, estado) VALUES
(1,  '2026-03-01 08:00:00', 'Col. San Benito #12, San Salvador',        'Electrónica',      320.00, 'Alta',  'Entregado'),
(2,  '2026-03-02 09:00:00', 'Av. Roosevelt #45, San Salvador',          'Alimentos',        150.00, 'Media', 'Entregado'),
(3,  '2026-03-03 10:00:00', 'Calle Arce #78, San Salvador',             'Ropa',              80.00, 'Baja',  'Entregado'),
(4,  '2026-03-05 08:30:00', 'Col. Miramonte #90, San Salvador',         'Químicos',         500.00, 'Alta',  'Entregado'),
(5,  '2026-03-07 09:00:00', 'Bo. San Miguelito #22, San Salvador',      'Electrodomésticos',420.00, 'Media', 'En Ruta'),
(6,  '2026-03-08 10:00:00', 'Res. Las Palmas #31, San Miguel',          'Muebles',          700.00, 'Baja',  'En Ruta'),
(7,  '2026-03-10 08:00:00', 'Col. Flor Blanca #14, San Salvador',       'Medicamentos',      60.00, 'Alta',  'Pendiente'),
(8,  '2026-03-11 09:30:00', 'Urbaniz. Altos #55, Santa Tecla',          'Alimentos',        200.00, 'Media', 'Pendiente'),
(9,  '2026-03-12 08:00:00', 'Bo. Candelaria #8, San Salvador',          'Electrónica',      290.00, 'Alta',  'Pendiente'),
(10, '2026-03-13 11:00:00', 'Col. Atlacatl #33, San Salvador',          'Ropa',             110.00, 'Baja',  'Pendiente'),
(1,  '2026-03-14 08:30:00', 'Res. Bello Horizonte, San Salvador',       'Alimentos',        180.00, 'Media', 'Pendiente'),
(3,  '2026-03-15 09:00:00', 'Bo. La Vega #17, San Salvador',            'Ferretería',       650.00, 'Media', 'Pendiente');


INSERT INTO ENVIO (id_pedido, id_vehiculo, id_conductor, id_operador, fecha_asignacion, fecha_entrega_estimada, fecha_entrega_real, ruta_descripcion, estado_envio) VALUES
(1, 1, 1, 1, '2026-03-01 10:00:00', '2026-03-02', '2026-03-02', 'Salida bodega central → Col. San Benito',         'Entregado'),
(2, 2, 2, 3, '2026-03-02 10:30:00', '2026-03-03', '2026-03-03', 'Bodega norte → Av. Roosevelt',                    'Entregado'),
(3, 4, 3, 1, '2026-03-03 11:00:00', '2026-03-04', '2026-03-04', 'Salida central → Calle Arce',                     'Entregado'),
(4, 1, 4, 3, '2026-03-05 09:00:00', '2026-03-06', '2026-03-06', 'Bodega central → Col. Miramonte',                 'Entregado'),
(5, 3, 2, 2, '2026-03-07 10:00:00', '2026-03-08', NULL,         'Bodega norte → Bo. San Miguelito',                'En Ruta'),
(6, 7, 5, 1, '2026-03-08 11:00:00', '2026-03-09', NULL,         'Bodega central → Res. Las Palmas (San Miguel)',   'En Ruta'),
(7, 2, 1, 3, '2026-03-10 09:00:00', '2026-03-11', NULL,         'Bodega norte → Col. Flor Blanca',                 'Asignado'),
(8, 4, 6, 2, '2026-03-11 10:00:00', '2026-03-12', NULL,         'Bodega central → Urbaniz. Altos, Santa Tecla',   'Asignado');

INSERT INTO HISTORIAL_ESTADO (id_envio, estado, fecha_actualizacion, descripcion, registrado_por) VALUES
(1, 'Asignado',   '2026-03-01 10:00:00', 'Vehículo y conductor asignados al pedido.',            'asolano'),
(1, 'En Ruta',    '2026-03-01 11:30:00', 'Conductor salió de bodega central hacia destino.',      'Conductor-Miguel'),
(1, 'Entregado',  '2026-03-02 14:00:00', 'Pedido entregado con éxito. Cliente firmó recibo.',    'Conductor-Miguel'),
(2, 'Asignado',   '2026-03-02 10:30:00', 'Asignación completada por operador.',                  'dmorales'),
(2, 'En Ruta',    '2026-03-02 12:00:00', 'Conductor inició ruta hacia Av. Roosevelt.',            'Conductor-Héctor'),
(2, 'Entregado',  '2026-03-03 10:00:00', 'Entrega exitosa confirmada por conductor.',             'Conductor-Héctor'),
(3, 'Asignado',   '2026-03-03 11:00:00', 'Vehículo 4 y conductor 3 asignados.',                  'asolano'),
(3, 'En Ruta',    '2026-03-03 12:30:00', 'Conductor Raúl inició ruta hacia Calle Arce.',          'Conductor-Raúl'),
(3, 'Entregado',  '2026-03-04 09:45:00', 'Pedido entregado sin novedades.',                       'Conductor-Raúl'),
(5, 'Asignado',   '2026-03-07 10:00:00', 'Camión 3 y conductor 2 asignados. Carga pesada.',       'dmorales'),
(5, 'En Ruta',    '2026-03-07 11:00:00', 'Conductor Héctor salió con carga de electrodomésticos.','Conductor-Héctor'),
(6, 'Asignado',   '2026-03-08 11:00:00', 'Furgoneta 7 asignada para entrega a San Miguel.',       'asolano');




