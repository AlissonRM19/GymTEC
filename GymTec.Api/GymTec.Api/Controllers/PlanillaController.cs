using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GymTec.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTec.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanillaController : ControllerBase
    {
        private readonly GymTecContext _context;

        public PlanillaController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET api/Planilla?fechaInicio=2025-06-01&fechaFin=2025-06-30
        /// Devuelve la planilla en el rango de fechas.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrador")] // o el rol que corresponda
        public async Task<IActionResult> GetPlanilla([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            if (fechaFin < fechaInicio)
                return BadRequest("fechaFin debe ser mayor o igual a fechaInicio.");

            // Obtener la conexión de EF Core
            var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            // Llamada al function: SELECT * FROM sp_generar_planilla(p1, p2)
            cmd.CommandText = "SELECT * FROM sp_generar_planilla(@p0, @p1)";
            cmd.CommandType = CommandType.Text;

            // Parámetros
            var param0 = cmd.CreateParameter();
            param0.ParameterName = "p0";
            param0.DbType = DbType.Date;
            param0.Value = fechaInicio.Date;
            cmd.Parameters.Add(param0);

            var param1 = cmd.CreateParameter();
            param1.ParameterName = "p1";
            param1.DbType = DbType.Date;
            param1.Value = fechaFin.Date;
            cmd.Parameters.Add(param1);

            var lista = new List<object>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new
                {
                    Sucursal = reader.IsDBNull(0) ? null : reader.GetString(0),
                    Cedula = reader.IsDBNull(1) ? null : reader.GetString(1),
                    NombreCompleto = reader.IsDBNull(2) ? null : reader.GetString(2),
                    TipoPlanilla = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Unidades = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4),
                    Monto = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5)
                });
            }

            await conn.CloseAsync();
            return Ok(lista);
        }
    }
}

