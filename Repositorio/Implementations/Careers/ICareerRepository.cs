using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Careers
{
    public interface ICareerRepository
    {
        Task<ResponseDTO<List<CareerDTO>>> GetFull();
        Task<ResponseDTO<CareerDTO>> GetById(long id);
        Task<ResponseDTO<string>> Put(long id, CareerPostDTO careerPostDTO);
        Task<ResponseDTO<CareerDTO>> Post(CareerPostDTO careerPostDTO);
    }
}
