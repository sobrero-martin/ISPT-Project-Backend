using DTO.ENUM;

namespace DTO.DTOs.SchoolYearDTO;

public class InscriptionRequestDTO
{
    public long FileCurriculumId { get; set; }
    public string FileCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int PendingDivisionsCount { get; set; }
    public bool IsCompleted { get; set; }
    public List<InscriptionRequestDivisionDTO> Divisions { get; set; } = new();
}

public class InscriptionRequestDivisionDTO
{
    public long FileDivisionId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public EnumFileDivisionAdmitedStatus Status { get; set; }
}

public class ReviewInscriptionRequestsDTO
{
    public List<InscriptionRequestDecisionDTO> Decisions { get; set; } = new();
}

public class InscriptionRequestDecisionDTO
{
    public long FileDivisionId { get; set; }
    public EnumFileDivisionAdmitedStatus Status { get; set; }
}
