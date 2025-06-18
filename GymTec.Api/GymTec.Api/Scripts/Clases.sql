-- ============================================
--      POBLAMIENTO DE DATOS BASE DE DATOS
-- ============================================

--- Clases

INSERT INTO "Clases" ("Id","Tipo","IsGrupal","Capacidad","Fecha","HoraInicio","HoraFin","InstructorId")
VALUES 
(gen_random_uuid(), 'Zumba', TRUE, 25, '2025-06-04', '08:00', '09:00', 'cd540e17-00d8-4f1c-888e-5f4f9bdabfe2'),
(gen_random_uuid(), 'Yoga', TRUE, 20, '2025-06-05', '09:30', '10:30', '59569996-e7a4-4db6-95ad-c84778f3a214'),
(gen_random_uuid(), 'Pilates', TRUE, 15, '2025-06-06', '11:00', '12:00', 'cd540e17-00d8-4f1c-888e-5f4f9bdabfe2'),
(gen_random_uuid(), 'Natación', FALSE, 5, '2025-06-07', '07:00', '08:00', '59569996-e7a4-4db6-95ad-c84778f3a214'),
(gen_random_uuid(), 'Indoor Cycling', TRUE, 30, '2025-06-08', '17:00', '18:00', '80c13ae7-8880-483f-9d7f-6159f45dc466');

SELECT * FROM "Clases";