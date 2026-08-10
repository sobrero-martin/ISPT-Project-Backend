using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Careers
{
    public interface ICorrelativeRepository
    {
        Task<ResponseDTO<bool>> SaveChanges (long subjectId, List<CorrelativeChangeDTO> changes);
    }
}
