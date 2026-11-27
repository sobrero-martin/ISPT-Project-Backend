using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;

namespace Repositorio.Implementations.Assistance;

public interface IAttendanceRepository
{
    Task<ResponseDTO<List<AttendanceDayDTO>>> GetDays(long divisionId);
    Task<ResponseDTO<AttendanceDayDTO>> CreateToday(long divisionId, DateTime date);
    Task<ResponseDTO<string>> DeleteDay(long attendanceDayId);
    Task<ResponseDTO<List<DailyAttendanceDTO>>> GetDaily(long attendanceDayId);
    Task<ResponseDTO<string>> UpdateDaily(long attendanceDayId, List<AttendanceUpdateDTO> attendances);
}
