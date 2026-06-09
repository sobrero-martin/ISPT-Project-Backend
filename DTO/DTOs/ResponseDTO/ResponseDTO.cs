using System.Net;

namespace DTO.DTOs.DTO_Response;

public class ResponseDTO<T>
{
    // StatusCode: Código de respuesta del servidor
    public HttpStatusCode StatusCode { get; set; }
    
    // Object: Respuesta que envia el servidor
    public T Object { get; set; }
    
    // Message: Mensaje que envia siempre el servidor junto al código
    public string Message { get; set; }
}