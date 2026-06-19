using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations
{
    public interface ICurriculumRepository
    {
        Task<ResponseDTO<List<CurriculumDTO>>> GetByCareer(long careerId);
        Task<ResponseDTO<CurriculumDTO>> GetById(long id);
        Task<ResponseDTO<string>> Put(long id, Curriculum curriculum);
        Task<ResponseDTO<CurriculumDTO>> Post(Curriculum curriculum);
    }
}
