using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace LecturaCorreosAWS.Services;

public class CorreoService
{
    public void LeerCorreos()
    {
        using (var client = new ImapClient())
        {
            client.Connect("imap.gmail.com", 993, true);
            client.Authenticate("agonzalez.mex@grupoei.com.mx", "13wolfiinXP");

            var folder = client.GetFolder("Desarrollo");
            folder.Open(FolderAccess.ReadOnly);

            var correos = folder.Search(SearchQuery.All);

            foreach (var correo in correos)
            {
                var message = folder.GetMessage(correo);
                
                var dateTime = message.Date;
                var fecha = dateTime.Date.ToString("dd/MM/yyyy");
                var hora = string.Format("{0:hh\\:mm\\:ss}", dateTime.TimeOfDay);
                
                var cliente = message.From.Mailboxes.First().Address;
                var contenido = message.TextBody ?? "Correo sin contenido.";

                Console.WriteLine($"Remitente: {cliente}");
                Console.WriteLine($"Fecha: {fecha}");
                Console.WriteLine($"Contenido: {contenido}");
                
                EnviarEventoAWS(cliente, fecha, hora, contenido);

            }
            
            client.Disconnect(true);
        }
    }
    
    public void EnviarEventoAWS(string remitente, string fecha, string hora, string contenido)
    {
        var snsClient = new AmazonSimpleNotificationServiceClient();
        var publishRequest = new PublishRequest
        {
            TopicArn = "arn:aws:sns:us-east-1:970547342167:CorreoEventos",
            Message = $"{{ \"Remitente\": \"{remitente}\", \"Fecha\": \"{fecha}\", \"Contenido\": \"{contenido}\" }}"
        };
        snsClient.PublishAsync(publishRequest).Wait();
    }
}