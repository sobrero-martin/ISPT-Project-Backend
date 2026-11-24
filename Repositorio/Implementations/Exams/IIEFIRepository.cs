using DTO.DTOs.DTO_Response;
using DTO.DTOs.ExamDTO;

namespace Repositorio.Implementations.Exams
{
    public interface IIEFIRepository
    {
        Task<ResponseDTO<List<IEFIDTO>>> GetFull();
        Task<ResponseDTO<IEFIPostDTO>> GetIEFIById(long id);
        Task<ResponseDTO<string>> Post(IEFIPostDTO iefi);
        Task<ResponseDTO<string>> Edit(IEFIPostDTO iefi);
        Task<ResponseDTO<string>> Delete(long id);
        Task<ResponseDTO<List<IEFIDivisionStudentDTO>>> GetStudents(long divisionId, long? studentPersonId = null, bool registerEligible = false);
        Task<ResponseDTO<string>> RegisterStudent(long divisionId, long fileDivisionId);
        Task<ResponseDTO<string>> SaveGrade(long divisionId, long fileDivisionId, int? grade);
    }
}
