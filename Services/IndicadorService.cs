using System.Data;
using Microsoft.Data.SqlClient;
using Csharpapigenerica.Models;
using Microsoft.Extensions.Configuration;

namespace Csharpapigenerica.Services
{
    public class EntidadGenerica
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }

    public class IndicadorService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public IndicadorService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("LocalDb") ?? throw new InvalidOperationException("Cadena de conexión no encontrada.");
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<IndicadorDto>> ObtenerTodosAsync()
        {
            var lista = new List<IndicadorDto>();
            using var conn = GetConnection();
            var cmd = new SqlCommand("spIndicador", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Accion", "SELECT");

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapearIndicador(reader));
            }
            return lista;
        }

        public async Task<IndicadorDto?> ObtenerPorIdAsync(int id)
        {
            using var conn = GetConnection();
            var cmd = new SqlCommand("spIndicador", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Accion", "SELECT_BY_ID");
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapearIndicador(reader) : null;
        }

        public async Task CrearAsync(IndicadorDto dto)
        {
            using var conn = GetConnection();
            var cmd = CrearComandoConParametros(dto, "INSERT", conn);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ActualizarAsync(IndicadorDto dto)
        {
            if (dto.Id == null) throw new ArgumentException("Id requerido para actualizar");

            using var conn = GetConnection();
            var cmd = CrearComandoConParametros(dto, "UPDATE", conn);
            cmd.Parameters.AddWithValue("@Id", dto.Id);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            using var conn = GetConnection();
            var cmd = new SqlCommand("spIndicador", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Accion", "DELETE");
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public class VariableIndicadorDto
        {
            public int Id { get; set; }
            public string Variable { get; set; } = "";
            public string Indicador { get; set; } = "";
            public double Dato { get; set; }
            public DateTime FechaDato { get; set; }
            public string Usuario { get; set; } = "";
        }

        public async Task<List<VariableIndicadorDto>> ObtenerVariablesPorIndicadorAsync()
        {
            var resultado = new List<VariableIndicadorDto>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("spVariablesPorIndicador", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Accion", "SELECT");

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                resultado.Add(new VariableIndicadorDto
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Variable = reader["Variable"].ToString() ?? "",
                    Indicador = reader["Indicador"].ToString() ?? "",
                    Dato = Convert.ToDouble(reader["dato"]),
                    FechaDato = Convert.ToDateTime(reader["fechadato"]),
                    Usuario = reader["Usuario"].ToString() ?? ""
                });
            }
            return resultado;
        }

        public async Task<List<EntidadGenerica>> ObtenerLiteralesPorArticuloAsync(string fkidArticulo)
        {
            return await EjecutarConsultaGenerica("spObtenerLiteralesPorArticulo", "@fkidArticulo", fkidArticulo);
        }

        public async Task<List<EntidadGenerica>> ObtenerNumeralesPorLiteralAsync(string fkidLiteral)
        {
            return await EjecutarConsultaGenerica("spObtenerNumeralesPorLiteral", "@fkidLiteral", fkidLiteral);
        }

        public async Task<List<EntidadGenerica>> ObtenerParagrafosPorArticuloAsync(string fkidArticulo)
        {
            return await EjecutarConsultaGenerica("spObtenerParagrafosPorArticulo", "@fkidArticulo", fkidArticulo);
        }

        private async Task<List<EntidadGenerica>> EjecutarConsultaGenerica(string procedimiento, string nombreParametro, string valorParametro)
        {
            var resultado = new List<EntidadGenerica>();
            using var conexion = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(procedimiento, conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue(nombreParametro, valorParametro);

            await conexion.OpenAsync();
            using var lector = await comando.ExecuteReaderAsync();
            while (await lector.ReadAsync())
            {
                resultado.Add(new EntidadGenerica
                {
                    Id = lector["id"].ToString() ?? "",
                    Nombre = lector["nombre"].ToString() ?? ""
                });
            }
            return resultado;
        }

        private IndicadorDto MapearIndicador(SqlDataReader reader)
        {
            return new IndicadorDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Codigo = reader.GetString(reader.GetOrdinal("codigo")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Objetivo = reader.GetString(reader.GetOrdinal("objetivo")),
                Alcance = reader.GetString(reader.GetOrdinal("alcance")),
                Formula = reader.GetString(reader.GetOrdinal("formula")),
                FkidTipoIndicador = reader.GetInt32(reader.GetOrdinal("fkidtipoindicador")),
                FkidUnidadMedicion = reader.GetInt32(reader.GetOrdinal("fkidunidadmedicion")),
                Meta = reader.GetString(reader.GetOrdinal("meta")),
                FkidSentido = reader.GetInt32(reader.GetOrdinal("fkidsentido")),
                FkidFrecuencia = reader.GetInt32(reader.GetOrdinal("fkidfrecuencia")),
                FkidArticulo = reader["fkidarticulo"] as string,
                Fkidliteral = reader["fkidliteral"] as string,
                FkidNumeral = reader["fkidnumeral"] as string,
                FkidParagrafo = reader["fkidparagrafo"] as string
            };
        }

        private SqlCommand CrearComandoConParametros(IndicadorDto dto, string accion, SqlConnection conn)
        {
            var cmd = new SqlCommand("spIndicador", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Accion", accion);
            cmd.Parameters.AddWithValue("@Codigo", dto.Codigo);
            cmd.Parameters.AddWithValue("@Nombre", dto.Nombre);
            cmd.Parameters.AddWithValue("@Objetivo", dto.Objetivo);
            cmd.Parameters.AddWithValue("@Alcance", dto.Alcance);
            cmd.Parameters.AddWithValue("@Formula", dto.Formula);
            cmd.Parameters.AddWithValue("@FkidTipoIndicador", dto.FkidTipoIndicador);
            cmd.Parameters.AddWithValue("@FkidUnidadMedicion", dto.FkidUnidadMedicion);
            cmd.Parameters.AddWithValue("@Meta", dto.Meta);
            cmd.Parameters.AddWithValue("@FkidSentido", dto.FkidSentido);
            cmd.Parameters.AddWithValue("@FkidFrecuencia", dto.FkidFrecuencia);
            cmd.Parameters.AddWithValue("@FkidArticulo", (object?)dto.FkidArticulo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fkidliteral", (object?)dto.Fkidliteral ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FkidNumeral", (object?)dto.FkidNumeral ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FkidParagrafo", (object?)dto.FkidParagrafo ?? DBNull.Value);

            return cmd;
        }
    }
}
