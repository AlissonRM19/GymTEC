-- ============================================
--      POBLAMIENTO DE DATOS BASE DE DATOS
-- ============================================

--- Sucursales

INSERT INTO "Sucursales" ("Id","Nombre","Provincia","Canton","Distrito","Direccion","Telefono")
VALUES 
  (
    '3',
    'GymTEC Cartago',
    'Cartago',
    'Central',
	'Oriental',
	'Av. Central, 100m este del TEC',
    '2550-1234'
  ),
  (
    '4',
    'GymTEC San José',
    'San José',
    'Montes de Oca',
	'Sabanilla',
	'De la rotonda 200m norte',
    '2280-5678'
  ),
  (
    '5',
    'GymTEC Alajuela',
    'Alajuela',
    'Central',
	'San José',
	'Contiguo a City Mall',
    '2430-7890'
  );

SELECT * FROM "Sucursales"
  