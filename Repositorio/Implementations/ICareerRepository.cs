using BD.Entidades;
using DTO.DTOs.CareerDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository
{
    public interface ICareerRepository
    {
        Task<List<CareerDTO>> GetFull();
        Task<CareerDTO> GetById(long id);
        Task<bool> Put(long id, Career carrera);
        Task<CareerDTO> Post(Career carrera);
    }
}
