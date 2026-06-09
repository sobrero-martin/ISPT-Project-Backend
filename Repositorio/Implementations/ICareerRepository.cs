using BD.Entidades;
using DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository
{
    public interface ICareerRepository
    {
        Task<List<CareerDTO>> GetFull();
        Task<bool> Put(long id, Career carrera);
        Task<CareerDTO> Post(Career carrera);
    }
}
