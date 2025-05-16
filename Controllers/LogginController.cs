using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using BCrypt.Net;

namespace Csharpapigenerica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LogginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Clase auxiliar para recibir los datos del login
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Contrasena { get; set; } = string.Empty;
        }

        // Endpoint: POST /api/loggin/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validación básica del request
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Contrasena))
                {
                    return BadRequest("Email y contraseña son obligatorios.");
                }

                // Obtener cadena de conexión usando el nombre que ya tienes configurado en appsettings.json
                var connectionString = _configuration.GetConnectionString("SqlServer");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("SELECT email, contrasena FROM Usuario WHERE email = @email", connection);
                    command.Parameters.AddWithValue("@email", request.Email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return Unauthorized("Credenciales inválidas.");
                        }

                        string emailBD = reader["email"].ToString()!;
                        string contrasenaDb = reader["contrasena"].ToString()!;

                        string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena);
                        Console.WriteLine(contrasenaHash + " " + request.Contrasena);

                        bool esValido = BCrypt.Net.BCrypt.Verify(request.Contrasena, contrasenaDb);


                        // bool esValido = BCrypt.Net.BCrypt.Verify(request.Contrasena, contrasenaHash);

                        if (!esValido)
                        {
                            return Unauthorized("Credenciales inválidas.");
                        }

                        return Ok(new
                        {
                            mensaje = "Login exitoso",
                            usuario = emailBD
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
