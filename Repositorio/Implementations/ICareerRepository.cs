using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository
{
    public interface ICareerRepository
    {
        Task<ResponseDTO<List<CareerDTO>>> GetFull();
        Task<ResponseDTO<CareerDTO>> GetById(long id);
        Task<ResponseDTO<string>> Put(long id, Career carrera);
        Task<ResponseDTO<CareerDTO>> Post(Career carrera);
    }
}
