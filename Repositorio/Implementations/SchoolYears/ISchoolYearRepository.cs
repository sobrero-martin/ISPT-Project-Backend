using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.SchoolYears
{
    public interface ISchoolYearRepository
    {
        Task<ResponseDTO<List<SchoolYearDTO>>> GetFull();
        Task<ResponseDTO<List<SchoolYearByGradePostDTO>>> GetRaw();
        Task<ResponseDTO<SchoolYearByGradePostDTO>> GetById(long id);
        Task<ResponseDTO<List<SchoolYearCurriculumDTO>>> GetCurriculumsBySchoolYearId(long schoolYearId);
        /*
        Task<ResponseDTO<SchoolYearPostDTO>> Post(SchoolYearPostDTO schoolYear);*/
        Task<ResponseDTO<SchoolYearByGradePostDTO>> PostByGrade(SchoolYearByGradePostDTO schoolYear);
    }
}
