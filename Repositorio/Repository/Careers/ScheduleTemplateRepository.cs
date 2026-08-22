using BD;
using BD.Entidades;
using BD.Entities;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Careers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Repositorio.Repository.Careers
{
    public class ScheduleTemplateRepository : IScheduleTemplateRepository
    {
        private readonly AppDbContext context;
        public ScheduleTemplateRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<ScheduleTemplateDTO>>> GetByDivisionTemplateId(long divisionTemplateId)
        {
            try
            {
                var divisionTemplate = await context.Set<DivisionTemplate>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == divisionTemplateId);

                if (divisionTemplate == null)
                {
                    return new ResponseDTO<List<ScheduleTemplateDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plantilla de división no encontrada."
                    };
                }

                var scheduleTemplates = await context.Set<ScheduleTemplate>()
                    .AsNoTracking()
                    .Where(s => s.DivisionTemplateId == divisionTemplateId)
                    .Select(s => new ScheduleTemplateDTO
                    {
                        Id = s.Id,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Day = s.Day
                    })
                    .ToListAsync();

                return new ResponseDTO<List<ScheduleTemplateDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = scheduleTemplates,
                    Message = "Plantillas de división obtenidas exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener divisiones por espacio curricular: {ex.Message}");

                return new ResponseDTO<List<ScheduleTemplateDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las plantillas de división."
                };
            }
        }

        public async Task<ResponseDTO<ScheduleTemplateDTO>> GetById (long id)
        {
            try
            {
                var scheduleTemplate = await context.Set<ScheduleTemplate>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (scheduleTemplate == null)
                {
                    return new ResponseDTO<ScheduleTemplateDTO>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plantilla de horario no encontrada."
                    };
                }
                var scheduleTemplateDTO = new ScheduleTemplateDTO
                {
                    Id = scheduleTemplate.Id,
                    StartTime = scheduleTemplate.StartTime,
                    EndTime = scheduleTemplate.EndTime,
                    Day = scheduleTemplate.Day
                };
                return new ResponseDTO<ScheduleTemplateDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = scheduleTemplateDTO,
                    Message = "Plantilla de horario obtenida exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la plantilla de horario por ID: {ex.Message}");
                return new ResponseDTO<ScheduleTemplateDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener la plantilla de horario."
                };
            }
        }

        public async Task<ResponseDTO<string>> Post(long divisionTemplateId, ScheduleTemplatePostDTO scheduleTemplateDTO)
        {
            try
            {
                var scheduleTemplateEntity = new ScheduleTemplate
                {
                    Id = scheduleTemplateDTO.Id,
                    StartTime = scheduleTemplateDTO.StartTime,
                    EndTime = scheduleTemplateDTO.EndTime,
                    Day = scheduleTemplateDTO.Day,
                    DivisionTemplateId = divisionTemplateId,
                    CreatedBy = scheduleTemplateDTO.CreatedById ?? Guid.Empty
                };

                await context.Set<ScheduleTemplate>().AddAsync(scheduleTemplateEntity);
                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Operación exitosa.",
                    Object = $"¡Plantilla de horario:{scheduleTemplateDTO.Id} creada con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la plantilla de horario: {ex.Message}");

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear la plantilla de horario.",
                    Object = null
                };
            }
        }
    }
}
