-- ============================================
--      POBLAMIENTO DE DATOS BASE DE DATOS
-- ============================================

--- SpaReservations

INSERT INTO "SpaReservations" ("Id","UserId", "SucursalId", "SpaTreatmentId", "ReservationDate", "StartTime", "EndTime")
VALUES 
(gen_random_uuid(),'588075e0-7007-4791-b7e0-4af5341d0c31', '2', 'cb6781dd-8e44-4060-a42a-1c585d435468', '2025-07-05', '14:00', '14:40'),
(gen_random_uuid(),'6a742943-93ed-4f0f-ace2-67cffad3c966', '3', '5abfe3cb-5209-4718-9c00-6aee88f8ba1b', '2025-07-06', '12:00', '12:40'),
(gen_random_uuid(),'588075e0-7007-4791-b7e0-4af5341d0c31', '4', '0ee80eae-76da-4aed-96c9-dd26be67ceec', '2025-07-07', '13:00', '13:40'),
(gen_random_uuid(),'6a742943-93ed-4f0f-ace2-67cffad3c966', '5', 'a9bd74a9-9393-4c2a-bb7e-8c239cd6008b', '2025-07-08', '15:00', '15:40');
