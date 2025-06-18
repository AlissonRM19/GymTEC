-- ============================================
--      POBLAMIENTO DE DATOS BASE DE DATOS
-- ============================================

--- Usuarios



-- Instructores
INSERT INTO "Users" ("Id","Cedula","NombreCompleto","Correo","PasswordMd5","Role","SucursalId","TipoPlanillaId","Salario")
VALUES 
  (
    gen_random_uuid(),
    '1-1111-1111',
    'Lucía Pérez',
    'lucia.instructora@gymtec.com',
    md5('instructora123'),
    'Instructor',
    1,
    (SELECT "Id" FROM "TiposPlanilla" WHERE "Descripcion"='Por hora'),
    3500.00
  ),
  (
    gen_random_uuid(),
    '2-2222-2222',
    'Mario Ruiz',
    'mario.instructor@gymtec.com',
    md5('instructor456'),
    'Instructor',
    2,
    (SELECT "Id" FROM "TiposPlanilla" WHERE "Descripcion"='Clase'),
    4200.00
  );

-- Clientes
INSERT INTO "Users" ("Id","Cedula","NombreCompleto","Correo","PasswordMd5","Role","SucursalId")
VALUES 
  (
    gen_random_uuid(),
    '3-3333-3333',
    'Sofía Hernández',
    'sofia.cliente@gymtec.com',
    md5('cliente123'),
    'Cliente',
    1
  ),
  (
    gen_random_uuid(),
    '4-4444-4444',
    'Daniel Salas',
    'daniel.cliente@gymtec.com',
    md5('cliente456'),
    'Cliente',
    2
  );

-- Admin
INSERT INTO "Users" ("Id","Cedula","NombreCompleto","Correo","PasswordMd5","Role","SucursalId","TipoPlanillaId","Salario")
VALUES (
  gen_random_uuid(),
  '5-5555-5555',
  'Andrea Villalobos',
  'admin@gymtec.com',
  md5('adminpass'),
  'Administrador',
  1,
  (SELECT "Id" FROM "TiposPlanilla" WHERE "Descripcion"='Mensual'),
  1500000
);
