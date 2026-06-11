using BD.Entidades;
using DTO.DTOs.CareerDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations
{
    public interface ICorrelativeRepository
    {
        Task<bool> Exists(long subjectId1, long subjectId2);
        Task<CorrelativeDTO> Post(Correlative correlative);
        Task<bool> Delete(long id);
    }
}
