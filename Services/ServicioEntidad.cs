using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace csharpapigenerica.Services
{
    public class ServicioEntidad
    {
        private readonly string _connectionString;

        public ServicioEntidad(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LocalDb")!;
        }
public class EntidadGenerica
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
}
public async Task<List<EntidadGenerica>> ObtenerTodosAsync(string nombreTabla)
{
    var lista = new List<EntidadGenerica>();
    using var connection = new SqlConnection(_connectionString);
    using var command = new SqlCommand($"EXEC sp{nombreTabla}", connection);

    await connection.OpenAsync();
    using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        lista.Add(new EntidadGenerica
        {
            Id = reader["id"].ToString() ?? "",
            Nombre = reader["nombre"].ToString() ?? ""
        });
    }

    return lista;
}

        public async Task<List<Dictionary<string, object>>> ObtenerPorParametroAsync(string proyecto, string tabla, string nombreParametro, string valorParametro)
        {
            var lista = new List<Dictionary<string, object>>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand($"EXEC sp{tabla}Por{nombreParametro} @{nombreParametro}", connection);
            command.Parameters.AddWithValue($"@{nombreParametro}", valorParametro);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var fila = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    fila[reader.GetName(i)] = reader[i];
                }
                lista.Add(fila);
            }

            return lista;
        }
    }
}
