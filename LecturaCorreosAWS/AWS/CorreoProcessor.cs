using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using LecturaCorreosAWS.Models;
using MySqlConnector;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LecturaCorreosAWS.AWS;

public class CorreoProcessor
{
    public async Task Handler(SNSEvent snsEvent, ILambdaContext context)
    {
        var record = snsEvent.Records[0].Sns.Message;
        var correo = JsonSerializer.Deserialize<Consultas>(record);

        using (var connection = new MySqlConnection("Server=tu-endpoint.rds.amazonaws.com;Database=CorreosDB;User=root;Password=tu_contraseña;"))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("INSERT INTO correos (remitente, dia, hora, contenido) VALUES (@remitente, @dia, @hora, @contenido)", connection);

            /*command.Parameters.AddWithValue("@remitente", correo.Remitente);
            command.Parameters.AddWithValue("@dia", correo.Dia);
            command.Parameters.AddWithValue("@hora", correo.Hora);
            command.Parameters.AddWithValue("@contenido", correo.Contenido);
*/
            await command.ExecuteNonQueryAsync();
        }
    }
    
}