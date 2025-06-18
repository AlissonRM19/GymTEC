-- ============================================
--      POBLAMIENTO DE DATOS BASE DE DATOS
-- ============================================

--- ClaseReservations

INSERT INTO "ClaseReservations" ("Id","UserId","ClaseId","ReservationDate")
VALUES 
(gen_random_uuid(), '35ad801b-ab41-4c72-b7e9-26b01b219fbc', '32874d0e-9ef5-44fa-bd1c-5b549f0c6682', '2025-06-04'),
(gen_random_uuid(), 'cd540e17-00d8-4f1c-888e-5f4f9bdabfe2', 'fb70f70c-7cb6-4b79-8e23-83269b083b15', '2025-06-05'),
(gen_random_uuid(), '59569996-e7a4-4db6-95ad-c84778f3a214', '018da079-0eb0-436e-b16a-454f1382e780', '2025-06-06'),
(gen_random_uuid(), '80c13ae7-8880-483f-9d7f-6159f45dc466', 'd0fcdef8-9484-4dee-bb29-149b1edce659', '2025-06-07'),
(gen_random_uuid(), '588075e0-7007-4791-b7e0-4af5341d0c31', '317bc6a9-64f0-4163-9259-360ddc215ad3', '2025-06-08');