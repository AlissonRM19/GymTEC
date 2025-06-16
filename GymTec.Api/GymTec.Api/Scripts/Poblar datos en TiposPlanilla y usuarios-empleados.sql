INSERT INTO "TiposPlanilla" ("Descripcion")
VALUES ('Mensual'),('Horas'),('Clase')
ON CONFLICT ("Descripcion") DO NOTHING;

INSERT INTO "Users" ("Id","Cedula","NombreCompleto","Correo","PasswordMd5","Role","SucursalId","TipoPlanillaId","Salario")
VALUES (
  gen_random_uuid(),
  '1-3000-3000',
  'Empleado Mensual',
  'empleado@gym.local',
  md5('pass'),
  'Instructor',
  1,
  (SELECT "Id" FROM "TiposPlanilla" WHERE "Descripcion"='Mensual'),
  2000.00
);