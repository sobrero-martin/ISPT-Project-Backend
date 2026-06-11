using BD.Entidades;
using DTO.DTOs.CareerDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations
{
    public interface ICurriculumRepository
    {
        Task<List<CurriculumDTO>> GetByCareer(long careerId);
        Task<CurriculumDTO> GetById(long id);
        Task<bool> Put(long id, Curriculum curriculum);
        Task<CurriculumDTO> Post(Curriculum curriculum);
    }
}
