using DTO.DTOs.AuthDTO;
using DTO.DTOs.DTO_Response;

namespace Repositorio.Repository;

public interface IAuthRepository
{
    public Task<ResponseDTO<TokenDTO>> Login(AuthDTO authDTO);
    public Task<ResponseDTO<TokenDTO>> Refresh(TokenDTO tokenDTO);
}