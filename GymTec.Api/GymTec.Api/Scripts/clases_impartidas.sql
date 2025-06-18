-- ============================================
--      POBLAMIENTO DE DATOS BASE DE DATOS
-- ============================================

--- clases_impartidas

SELECT * FROM "clases_impartidas";

INSERT INTO "clases_impartidas" ("Id","UserId","Fecha","Detalle")
VALUES 
(1, 'cd540e17-00d8-4f1c-888e-5f4f9bdabfe2', '2025-07-01', 'Clase de Zumba con grupo intermedio'),
(2, '59569996-e7a4-4db6-95ad-c84778f3a214', '2025-07-02', 'Clase de Yoga avanzada'),
(3, 'cd540e17-00d8-4f1c-888e-5f4f9bdabfe2', '2025-07-03', 'Pilates para principiantes'),
(4, '59569996-e7a4-4db6-95ad-c84778f3a214', '2025-07-04', 'Natación para niños'),
(5, '80c13ae7-8880-483f-9d7f-6159f45dc466', '2025-07-08', 'Indoor Cycling para adultos');
