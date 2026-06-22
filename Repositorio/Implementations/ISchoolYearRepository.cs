using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations
{
    public interface ISchoolYearRepository
    {
        Task<ResponseDTO<List<SchoolYearDTO>>> GetFull();
        Task<ResponseDTO<List<SchoolYearPostDTO>>> GetRaw();
        Task<ResponseDTO<SchoolYearPostDTO>> GetById(long id);
        Task<ResponseDTO<SchoolYearPostDTO>> Post(SchoolYearPostDTO schoolYear);
    }
}
