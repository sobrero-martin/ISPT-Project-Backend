using DTO.DTOs.Documentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Students;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/file-documentation")]
public class FileDocumentationController : ControllerBase
{
    private readonly IFileDocsRepository fileDocsRepository;

    public FileDocumentationController(IFileDocsRepository fileDocsRepository)
    {
        this.fileDocsRepository = fileDocsRepository;
    }

    [HttpGet("{fileId:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<ActionResult> GetAllDocumentsByFileId(long fileId)
    {
        var response = await fileDocsRepository.GetAllDocumentsByFileId(fileId);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpGet("{fileId:long}/reload")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<ActionResult> ReloadDocumentsDeliverableByYear(long fileId)
    {
        var response = await fileDocsRepository.ReloadDocumentsDeliverableByYear(fileId);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpPut]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<ActionResult> ChangeDocumentsStatus(FileDocumentation documentation)
    {
        var response = await fileDocsRepository.ChangeDocumentsStatus(documentation);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpPost("row")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<ActionResult> AddYearInDeliverableDocumentsPerYear(AddDeliverableDocYearDTO DTO)
    {
        var response = await fileDocsRepository.AddYearInDeliverableDocumentsPerYear(DTO);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpDelete("row/{Id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<ActionResult> DeleteYearInDeliverableDocumentsPerYear(long Id)
    {
        var response = await fileDocsRepository.DeleteYearInDeliverableDocumentsPerYear(Id);
        return StatusCode((int)response.StatusCode, response);
    }
}