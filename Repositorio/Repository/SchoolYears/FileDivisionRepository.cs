using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Repositorio.Implementations.SchoolYears;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository.SchoolYears
{
    public class FileDivisionRepository : IFileDivisionRepository
    {
        private readonly AppDbContext context;

        public FileDivisionRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<FileDivisionDTO>>> GetByDivisionId(long divisionId)
        {
            try
            {
                var fileDivisions = context.Set<FileDivision>()
                    .Where(fd => fd.DivisionId == divisionId)
                    .Select(fd => new FileDivisionDTO
                    {
                        FileCode = fd.FileCurriculum.File.Code,
                        StudentName = fd.FileCurriculum.File.Student.Lastname + ", " + fd.FileCurriculum.File.Student.Firstname,
                        FileDivisionStatus = fd.FileDivisionStatus,
                        FileDivisionObservations = fd.FileDivisionObservations
                    })
                    .ToList();


                return new ResponseDTO<List<FileDivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = fileDivisions,
                    Message = "File divisions retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los legajos_divisiones: {ex.Message}");

                return new ResponseDTO<List<FileDivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Error al obtener las inscripciones de estudiantes."
                };
            }
        }

        public async Task<ResponseDTO<string>> Post(StudentFileDivisionPostDTO studentFileDivisionDTO)
        {
            try
            {
                var fileDivisionEntity = new FileDivision
                {
                    FileCurriculumId = studentFileDivisionDTO.FileId,
                    DivisionId = studentFileDivisionDTO.DivisionId,
                    FileDivisionStatus = "Activo",
                    FileDivisionObservations = null,
                    CreatedBy = studentFileDivisionDTO.CreatedBy ?? Guid.Empty,
                };
                context.Set<FileDivision>().Add(fileDivisionEntity);
                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = null,
                    Message = "File division created successfully."    
                };
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error al crear la inscripción de estudiante: {ex.Message}");

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Error al crear la inscripción de estudiante."
                };
            }
        }
    }
}
