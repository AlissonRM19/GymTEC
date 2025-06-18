-- ============================================
--      POBLAMIENTO DE DATOS BASE DE DATOS
-- ============================================

--- Tipos de planilla

SELECT table_schema, table_name 
FROM information_schema.tables 
WHERE table_name = 'TiposPlanilla';

INSERT INTO "TiposPlanilla" ("Descripcion")
VALUES ('Mensual'),('Horas'),('Clase')
ON CONFLICT ("Descripcion") DO NOTHING;

SELECT * FROM "TiposPlanilla";