-- ==============================
-- Trigger B3: proteger tratamientos por defecto
-- ==============================

-- 1. Crear o reemplazar la función trigger
CREATE OR REPLACE FUNCTION fn_protect_default_spa_treatment()
RETURNS trigger AS
$$
BEGIN
    -- En DELETE, OLD representa la fila que se pretende eliminar
    IF TG_OP = 'DELETE' THEN
        IF OLD."IsDefault" THEN
            RAISE EXCEPTION 'No se puede eliminar un tratamiento por defecto: %', OLD."Name";
        END IF;
        RETURN OLD;
    END IF;

    -- En UPDATE, NEW contiene valores propuestos, OLD los valores actuales
    IF TG_OP = 'UPDATE' THEN
        IF OLD."IsDefault" THEN
            RAISE EXCEPTION 'No se puede modificar un tratamiento por defecto: %', OLD."Name";
        END IF;
        RETURN NEW;
    END IF;

    -- Para cualquier otro caso (no debería ocurrir), no alterar
    RETURN NULL;
END;
$$
LANGUAGE plpgsql SECURITY DEFINER;

-- 2. Crear/eliminar trigger asociado
DROP TRIGGER IF EXISTS trg_protect_default_spa_treatment ON "SpaTreatments";
CREATE TRIGGER trg_protect_default_spa_treatment
    BEFORE UPDATE OR DELETE
    ON "SpaTreatments"
    FOR EACH ROW
    EXECUTE FUNCTION fn_protect_default_spa_treatment();

--Prueba: Debe lanzar excepción: “No se puede modificar un tratamiento por defecto: ...”
UPDATE "SpaTreatments" SET "Name" = 'X' WHERE "IsDefault" = TRUE;

-- Debe lanzar excepción similar.
DELETE FROM "SpaTreatments" WHERE "IsDefault" = TRUE;
