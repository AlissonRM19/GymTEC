INSERT INTO "Users" ("Id", "Cedula", "NombreCompleto", "Correo", "PasswordMd5", "Role")
VALUES (
  gen_random_uuid(),
  '1-2345-6789',
  'Admin GymTEC',
  'admin@gymtec.local',
  md5('password123'),
  0  -- valor entero para Administrador
);

INSERT INTO "SpaTreatments" ("Id", "Name", "IsDefault")
VALUES 
  (gen_random_uuid(), 'Masaje relajante', TRUE),
  (gen_random_uuid(), 'masaje descarga muscular', TRUE),
  (gen_random_uuid(), 'sauna', TRUE),
  (gen_random_uuid(), 'baños a vapor', TRUE)
ON CONFLICT ("Name") DO UPDATE SET "IsDefault" = EXCLUDED."IsDefault";

INSERT INTO "Sucursales" ("Nombre", "Provincia", "Canton", "Distrito", "Direccion", "Telefono")
VALUES
  ('Sucursal Central', 'San José', 'San José', 'Carmen', 'Av. Central 123', '8888-0001'),
  ('Sucursal Oeste', 'San José', 'Esparza', 'Barranca', 'Ruta 1 Km 45', '8888-0002');
