using DTO.DTOs.StudentsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Students;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/files")]
public class FileController : ControllerBase
{
    private readonly IFileRepository fileRepository;
    
    public FileController(IFileRepository fileRepository)
    {
        this.fileRepository = fileRepository;
    }
    
    [HttpGet("{id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetFilesByStudendId(long id)
    {
        var res = await fileRepository.getFilesByStudentId(id);
        return StatusCode((int)res.StatusCode, res);
    }
    
    
    [HttpGet("file/{id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetFileById(long id)
    {
        var res = await fileRepository.getFileById(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> CreateFile(FilePostDTO filePostDTO)
    {
        var res = await fileRepository.createFile(filePostDTO);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpPut]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> UpdateFile(FilePostDTO filePutDTO)
    {
        var res = await fileRepository.updateFile(filePutDTO);
        return StatusCode((int)res.StatusCode, res);
    }
}