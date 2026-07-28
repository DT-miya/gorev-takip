using System.Threading.Tasks;
using TaskBoard.Api.DTOs.Admin;

namespace TaskBoard.Api.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(CreateActivityLogDto dto);
}