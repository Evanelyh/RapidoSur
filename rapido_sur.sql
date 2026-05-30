CREATE TABLE cliente (
    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL,
    apellido VARCHAR(60) NOT NULL,
    telefono VARCHAR(15),
    correo VARCHAR(100),
    direccion VARCHAR(200),
    fecha_registro DATE NOT NULL DEFAULT (CURRENT_DATE)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE pedido (
    id_pedido INT AUTO_INCREMENT PRIMARY KEY,
    id_cliente INT NOT NULL,
    fecha_solicitud DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    direccion_entrega VARCHAR(200) NOT NULL,
    tipo_carga VARCHAR(80),
    peso_kg DECIMAL(8,2) NOT NULL,
    prioridad VARCHAR(20) DEFAULT 'Media',
    estado VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
    observaciones TEXT,
    CONSTRAINT fk_pedido_cliente FOREIGN KEY (id_cliente) 
        REFERENCES cliente(id_cliente) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE vehiculo (
    id_vehiculo INT AUTO_INCREMENT PRIMARY KEY,
    placa VARCHAR(15) UNIQUE NOT NULL,
    tipo VARCHAR(50) NOT NULL,
    capacidad_carga_kg DECIMAL(8,2) NOT NULL,
    estado_operativo VARCHAR(30) NOT NULL DEFAULT 'Disponible',
    marca VARCHAR(50),
    modelo VARCHAR(50),
    anio INT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE conductor (
    id_conductor INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL,
    apellido VARCHAR(60) NOT NULL,
    num_licencia VARCHAR(20) UNIQUE NOT NULL,
    tipo_licencia VARCHAR(10),
    telefono VARCHAR(15),
    disponible BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_ingreso DATE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE operador (
    id_operador INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL,
    apellido VARCHAR(60) NOT NULL,
    usuario VARCHAR(50) UNIQUE NOT NULL,
    clave VARCHAR(255) NOT NULL,
    rol VARCHAR(30) NOT NULL DEFAULT 'Operador',
    correo VARCHAR(100)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE envio (
    id_envio INT AUTO_INCREMENT PRIMARY KEY,
    id_pedido INT NOT NULL,
    id_vehiculo INT NOT NULL,
    id_conductor INT NOT NULL,
    id_operador INT NOT NULL,
    fecha_asignacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_entrega_estimada DATE,
    fecha_entrega_real DATE NULL,
    ruta_descripcion TEXT,
    estado_envio VARCHAR(30) NOT NULL DEFAULT 'Asignado',
    CONSTRAINT fk_envio_pedido FOREIGN KEY (id_pedido) 
        REFERENCES pedido(id_pedido) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_envio_vehiculo FOREIGN KEY (id_vehiculo) 
        REFERENCES vehiculo(id_vehiculo) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_envio_conductor FOREIGN KEY (id_conductor) 
        REFERENCES conductor(id_conductor) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_envio_operador FOREIGN KEY (id_operador) 
        REFERENCES operador(id_operador) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE historial_estado (
    id_historial INT AUTO_INCREMENT PRIMARY KEY,
    id_envio INT NOT NULL,
    estado VARCHAR(30) NOT NULL,
    fecha_actualizacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    descripcion TEXT,
    registrado_por VARCHAR(60),
    CONSTRAINT fk_historial_envio FOREIGN KEY (id_envio) 
        REFERENCES envio(id_envio) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO cliente (id_cliente, nombre, apellido, telefono, correo, direccion, fecha_registro) VALUES
(1, 'Carlos', 'Martínez', '7823-1100', 'cmartinez@gmail.com', 'Colonia Escalón, San Salvador', '2026-01-10'),
(2, 'Laura', 'Pérez', '6712-2200', 'lperez@hotmail.com', 'Residencial Santa Elena, Antiguo Cuscatlán', '2026-01-12'),
(3, 'Roberto', 'Gómez', '7934-3300', 'rgomez@yahoo.com', 'Barrio San Jacinto, San Salvador', '2026-01-15'),
(4, 'Ana', 'López', '6645-4400', 'alopez@gmail.com', 'Col. Jardines de Guadalupe, San Salvador', '2026-01-20'),
(5, 'Mario', 'Hernández', '7756-5500', 'mhernandez@outlook.com', 'Urbanización Madre Selva, Soyapango', '2026-02-01'),
(6, 'Sofía', 'Ramos', '7867-6600', 'sramos@gmail.com', 'Residencial Los Héroes, San Miguel', '2026-02-05'),
(7, 'Jorge', 'Torres', '6978-7700', 'jtorres@gmail.com', 'Col. La Rábida, Santa Ana', '2026-02-10'),
(8, 'Carmen', 'Flores', '7089-8800', 'cflores@yahoo.com', 'Barrio El Centro, Usulután', '2026-02-15'),
(9, 'Esteban', 'Cruz', '7190-9900', 'ecruz@gmail.com', 'Col. Quiñónez, San Salvador', '2026-02-20'),
(10, 'Patricia', 'Vásquez', '7201-0011', 'pvasquez@gmail.com', 'Residencial Altavista, Antiguo Cuscatlán', '2026-02-25');

INSERT INTO vehiculo (id_vehiculo, placa, tipo, capacidad_carga_kg, estado_operativo, marca, modelo, anio) VALUES
(1, 'P-123-ABC', 'Camión', 5000.00, 'Disponible', 'Toyota', 'Dyna', 2019),
(2, 'P-456-DEF', 'Furgoneta', 2000.00, 'Disponible', 'Hyundai', 'H100', 2020),
(3, 'P-789-GHI', 'Camión', 8000.00, 'En Ruta', 'Isuzu', 'Forward', 2018),
(4, 'P-321-JKL', 'Furgoneta', 1500.00, 'Disponible', 'Kia', 'K2700', 2021),
(5, 'P-654-MNO', 'Camioneta', 1000.00, 'Disponible', 'Nissan', 'Frontier', 2022),
(6, 'P-987-PQR', 'Camión', 6000.00, 'En Mantenimiento', 'Mercedes-Benz', 'Atego', 2017),
(7, 'P-111-STU', 'Furgoneta', 2500.00, 'En Ruta', 'Ford', 'Transit', 2020),
(8, 'P-222-VWX', 'Camioneta', 1200.00, 'Disponible', 'Toyota', 'Hilux', 2023);

INSERT INTO conductor (id_conductor, nombre, apellido, num_licencia, tipo_licencia, telefono, disponible, fecha_ingreso) VALUES
(1, 'Miguel', 'Argueta', 'LIC-001-SV', 'Liviana', '7312-1122', TRUE, '2020-03-15'),
(2, 'Héctor', 'Juárez', 'LIC-002-SV', 'Pesada', '7423-2233', FALSE, '2019-07-20'),
(3, 'Raúl', 'Pacheco', 'LIC-003-SV', 'Liviana', '7534-3344', TRUE, '2021-01-10'),
(4, 'Jaime', 'Melara', 'LIC-004-SV', 'Pesada', '7645-4455', TRUE, '2018-11-05'),
(5, 'Oscar', 'Chávez', 'LIC-005-SV', 'Liviana', '7756-5566', FALSE, '2022-06-18'),
(6, 'Pedro', 'Aguilar', 'LIC-006-SV', 'Pesada', '7867-6677', TRUE, '2020-09-30');

INSERT INTO operador (id_operador, nombre, apellido, usuario, clave, rol, correo) VALUES
(1, 'Andrea', 'Solano', 'asolano', 'asolano123', 'Operador', 'asolano@rapidosur.com'),
(2, 'Luis', 'Contreras', 'lcontreras', 'lcontreras123', 'Administrador', 'lcontreras@rapidosur.com'),
(3, 'Diana', 'Morales', 'dmorales', 'dmorales123', 'Operador', 'dmorales@rapidosur.com'),
(4, 'René', 'Escobar', 'rescobar', 'rescobar123', 'Administrador', 'rescobar@rapidosur.com');

INSERT INTO pedido (id_pedido, id_cliente, fecha_solicitud, direccion_entrega, tipo_carga, peso_kg, prioridad, estado, observaciones) VALUES
(1, 1, '2026-03-01 08:30:00', 'Col. San Benito #12, San Salvador', 'Electrónica', 320.00, 'Alta', 'Entregado', 'Entregar en horario de oficina'),
(2, 2, '2026-03-02 09:15:00', 'Av. Roosevelt #45, San Miguel', 'Alimentos', 150.00, 'Media', 'Entregado', 'Carga refrigerada'),
(3, 3, '2026-03-03 10:00:00', 'Calle Arce #78, San Salvador', 'Ropa', 80.00, 'Entregado', 'Entregado', 'Ninguna'),
(4, 4, '2026-03-05 11:45:00', 'Col. Miramonte #90, San Salvador', 'Químicos', 500.00, 'Alta', 'Entregado', 'Manejar con extremo cuidado. Frágil'),
(5, 5, '2026-03-07 08:10:00', 'Bo. San Miguelito #22, Santa Ana', 'Electrodomésticos', 420.00, 'Media', 'En Ruta', 'Llamar antes de llegar'),
(6, 6, '2026-03-08 09:00:00', 'Res. Las Palmas #31, La Libertad', 'Muebles', 700.00, 'Baja', 'En Ruta', 'Carga pesada'),
(7, 7, '2026-03-10 14:20:00', 'Col. Flor Blanca #14, San Salvador', 'Medicamentos', 60.00, 'Alta', 'Asignado', 'Prioridad médica urgente'),
(8, 8, '2026-03-11 15:30:00', 'Urbaniz. Altos #55, Santa Tecla', 'Alimentos', 200.00, 'Media', 'Asignado', 'Entregar por la tarde'),
(9, 9, '2026-03-12 11:00:00', 'Bo. Candelaria #8, San Salvador', 'Electrónica', 290.00, 'Alta', 'Pendiente', 'Equipos delicados'),
(10, 10, '2026-03-13 13:40:00', 'Col. Atlacatl #33, San Salvador', 'Ropa', 110.00, 'Baja', 'Pendiente', 'Bolsas selladas'),
(11, 1, '2026-03-14 10:15:00', 'Res. Bello Horizonte, San Salvador', 'Alimentos', 180.00, 'Media', 'Pendiente', 'Cajas de abarrotes'),
(12, 3, '2026-03-15 16:00:00', 'Bo. La Vega #17, San Salvador', 'Ferretería', 650.00, 'Media', 'Pendiente', 'Herramientas de construcción');

INSERT INTO envio (id_envio, id_pedido, id_vehiculo, id_conductor, id_operador, fecha_asignacion, fecha_entrega_estimada, fecha_entrega_real, ruta_descripcion, estado_envio) VALUES
(1, 1, 1, 1, 1, '2026-03-01 10:00:00', '2026-03-02', '2026-03-02', 'Ruta San Salvador Centro - Colonia San Benito', 'Entregado'),
(2, 2, 2, 3, 2, '2026-03-02 10:30:00', '2026-03-03', '2026-03-03', 'Ruta Carretera de Oro - Carretera Panamericana hacia San Miguel', 'Entregado'),
(3, 3, 4, 4, 1, '2026-03-03 11:00:00', '2026-03-04', '2026-03-04', 'Ruta Centro Histórico San Salvador - Calle Arce', 'Entregado'),
(4, 4, 1, 6, 2, '2026-03-05 12:00:00', '2026-03-06', '2026-03-06', 'Ruta Boulevard Los Héroes - Colonia Miramonte', 'Entregado'),
(5, 5, 3, 2, 2, '2026-03-07 09:15:00', '2026-03-08', NULL, 'Carretera Panamericana Occidente hacia Santa Ana', 'En Ruta'),
(6, 6, 7, 5, 1, '2026-03-08 10:00:00', '2026-03-09', NULL, 'Carretera al Puerto de La Libertad - Residencial Las Palmas', 'En Ruta'),
(7, 7, 2, 1, 2, '2026-03-10 15:00:00', '2026-03-11', NULL, 'Ruta Escalón - Flor Blanca San Salvador', 'Asignado'),
(8, 8, 4, 6, 2, '2026-03-11 16:00:00', '2026-03-12', NULL, 'Carretera Panamericana - Santa Tecla Altos', 'Asignado');

INSERT INTO historial_estado (id_historial, id_envio, estado, fecha_actualizacion, descripcion, registrado_por) VALUES
(1, 1, 'Pendiente', '2026-03-01 08:30:00', 'Pedido recibido en el sistema web', 'Cliente'),
(2, 1, 'Asignado', '2026-03-01 10:00:00', 'Vehículo Dyna (P-123-ABC) y Conductor Miguel Argueta asignados', 'Andrea Solano'),
(3, 1, 'En Ruta', '2026-03-01 11:30:00', 'Conductor salió con la carga desde el centro logístico', 'Miguel Argueta'),
(4, 1, 'Entregado', '2026-03-02 14:00:00', 'Carga entregada en perfecto estado. Firma del cliente capturada', 'Miguel Argueta'),
(5, 2, 'Pendiente', '2026-03-02 09:15:00', 'Pedido de alimentos registrado', 'Cliente'),
(6, 2, 'Asignado', '2026-03-02 10:30:00', 'Asignación de despacho a furgoneta Hyundai (P-456-DEF) y conductor Raúl Pacheco', 'Luis Contreras'),
(7, 2, 'En Ruta', '2026-03-02 12:00:00', 'Carga en tránsito por carretera Panamericana', 'Raúl Pacheco'),
(8, 2, 'Entregado', '2026-03-03 10:00:00', 'Entrega final exitosa en Av. Roosevelt San Miguel', 'Raúl Pacheco'),
(9, 5, 'Pendiente', '2026-03-07 08:10:00', 'Pedido de electrodomésticos ingresado', 'Cliente'),
(10, 5, 'Asignado', '2026-03-07 09:15:00', 'Operador asignó Camión Isuzu (P-789-GHI) y conductor Héctor Juárez', 'Luis Contreras'),
(11, 5, 'En Ruta', '2026-03-07 10:00:00', 'Conductor inició el trayecto hacia Santa Ana', 'Héctor Juárez'),
(12, 6, 'Pendiente', '2026-03-08 09:00:00', 'Pedido de muebles registrado en sucursal', 'Andrea Solano');
