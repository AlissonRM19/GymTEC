-- ==============================
-- Stored Procedure B2: sp_generar_planilla(rango de fechas)
-- ==============================
-- Este procedure devuelve para cada empleado con TipoPlanilla en Mensual/Horas/Clase:
--   sucursal: nombre de sucursal (o 'Sin Sucursal')
--   cedula: Users.Cedula
--   nombre_completo: Users.NombreCompleto
--   tipo_planilla: descripcion del tipo
--   unidades: NULL para Mensual; total de horas (numeric) para Horas; conteo (numeric) para Clase
--   monto: Salario para Mensual; horas_totales * Salario para Horas; conteo_clases * Salario para Clase
-- Filtra registros de horas_trabajadas y clases_impartidas según Fecha entre los parámetros.

CREATE OR REPLACE FUNCTION sp_generar_planilla(
    p_fecha_inicio DATE,
    p_fecha_fin DATE
)
RETURNS TABLE(
    sucursal VARCHAR,
    cedula VARCHAR,
    nombre_completo VARCHAR,
    tipo_planilla VARCHAR,
    unidades NUMERIC,
    monto NUMERIC
)
LANGUAGE plpgsql AS
$$
BEGIN
    RETURN QUERY

    -- Planilla Mensual: incluimos todos los usuarios cuyo TipoPlanilla = 'Mensual'.
    -- Para mensual, interpretamos que el salario se paga completo si el mes del rango coincide,
    -- o simplemente siempre devolvemos Salario; se pueden filtrar por mes externo al llamar.
    SELECT
        COALESCE(su."Nombre", 'Sin Sucursal') AS sucursal,
        u."Cedula" AS cedula,
        u."NombreCompleto" AS nombre_completo,
        tp."Descripcion" AS tipo_planilla,
        NULL::NUMERIC AS unidades,
        u."Salario" AS monto
    FROM "Users" u
    JOIN "TiposPlanilla" tp ON u."TipoPlanillaId" = tp."Id"
    LEFT JOIN "Sucursales" su ON u."SucursalId" = su."Id"
    WHERE tp."Descripcion" = 'Mensual'
      -- Opcional: filtrar sólo si el rango abarca un mes concreto:
      -- AND date_trunc('month', p_fecha_inicio) = date_trunc('month', CURRENT_DATE)
      -- o condiciones similares según reglas de negocio
    UNION ALL

    -- Planilla por Horas
    SELECT
        COALESCE(su."Nombre", 'Sin Sucursal') AS sucursal,
        u."Cedula",
        u."NombreCompleto",
        tp."Descripcion" AS tipo_planilla,
        SUM(EXTRACT(EPOCH FROM (ht."HoraSalida" - ht."HoraEntrada"))/3600.0)::NUMERIC(12,2) AS unidades,
        (SUM(EXTRACT(EPOCH FROM (ht."HoraSalida" - ht."HoraEntrada"))/3600.0) * u."Salario")::NUMERIC(12,2) AS monto
    FROM "Users" u
    JOIN "TiposPlanilla" tp ON u."TipoPlanillaId" = tp."Id"
    JOIN horas_trabajadas ht ON ht."UserId" = u."Id"
    LEFT JOIN "Sucursales" su ON u."SucursalId" = su."Id"
    WHERE tp."Descripcion" = 'Horas'
      AND ht."Fecha"::DATE BETWEEN p_fecha_inicio AND p_fecha_fin
    GROUP BY su."Nombre", u."Cedula", u."NombreCompleto", tp."Descripcion", u."Salario"

    UNION ALL

    -- Planilla por Clase
    SELECT
        COALESCE(su."Nombre", 'Sin Sucursal') AS sucursal,
        u."Cedula",
        u."NombreCompleto",
        tp."Descripcion" AS tipo_planilla,
        COUNT(ci.*)::NUMERIC AS unidades,
        (COUNT(ci.*) * u."Salario")::NUMERIC(12,2) AS monto
    FROM "Users" u
    JOIN "TiposPlanilla" tp ON u."TipoPlanillaId" = tp."Id"
    JOIN clases_impartidas ci ON ci."UserId" = u."Id"
    LEFT JOIN "Sucursales" su ON u."SucursalId" = su."Id"
    WHERE tp."Descripcion" = 'Clase'
      AND ci."Fecha"::DATE BETWEEN p_fecha_inicio AND p_fecha_fin
    GROUP BY su."Nombre", u."Cedula", u."NombreCompleto", tp."Descripcion", u."Salario"

    ORDER BY sucursal, nombre_completo;
END;
$$;

--Prubas: 
SELECT * FROM sp_generar_planilla('2025-06-01', '2025-06-30');
