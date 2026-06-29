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
        Task<ResponseDTO<bool>> Exists(long subjectId1, long subjectId2);
        Task<ResponseDTO<CorrelativeDTO>> Post(Correlative correlative);
        Task<ResponseDTO<bool>> Delete(long subjectId, long correlativeId);
        Task<ResponseDTO<bool>> SaveChanges (long subjectId, List<CorrelativeChangeDTO> changes);
    }
}
