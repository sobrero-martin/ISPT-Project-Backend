namespace Repositorio.Implementations.Students;

public interface IStudentCourses
{
    Task Inscription();
    Task GetCoursesByStudentId(long studentId);
}