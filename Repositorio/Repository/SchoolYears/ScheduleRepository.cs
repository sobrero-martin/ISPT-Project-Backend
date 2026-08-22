using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.SchoolYears;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository.SchoolYears
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext context;

        public ScheduleRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<ScheduleDTO>>> GetByDivisionId(long divisionId)
        {
            try
            {
                var schedules = await context.Set<Schedule>()
                    .Where(s => s.DivisionId== divisionId)
                    .Select(s => new ScheduleDTO
                    {
                        Id = s.Id,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Day = s.Day
                    })
                    .ToListAsync();

                return new ResponseDTO<List<ScheduleDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = schedules,
                    Message = "Horarios obtenidos correctamente."

                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los horarios: {ex.Message}");
                return new ResponseDTO<List<ScheduleDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener los horarios."

                };
            }
        }
    }
}
