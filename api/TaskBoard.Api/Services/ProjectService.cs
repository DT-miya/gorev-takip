using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data;
using TaskBoard.Api.Interfaces;
using TaskBoard.Api.Exceptions;
using TaskBoard.Api.DTOs.Project;
using TaskBoard.Api.Data.Entities;
using System.ComponentModel;

namespace TaskBoard.Api.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context) {
        _context = context;
    }

    public async Task<List<ProjectResponse>> GetProjectResponsesAsync(int userId)
    {
        var projeler = await _context.Projects
        .Where(p => p.Members.Any(m => m.UserId == userId))
        .Select(p => new ProjectResponse
        {
            Id =p.Id,
            Name =p.Name,
            Description = p.Description,
            MemberCount =p.Members.Count,
            OwnerName = p.Owner.FullName
        })
        .ToListAsync();

        return projeler;
    }
    public async Task<ProjectResponse> GetByIdAsync(int projectId, int userId)
    {
        await EnsureMemberAsync(projectId, userId);

        var proje = await _context.Projects
        .Where(p => p.Id == projectId)
        .Select(p => new ProjectResponse
        {
            Id =p.Id,
            Name =p.Name,
            Description = p.Description,
            MemberCount =p.Members.Count,
            OwnerName = p.Owner.FullName
        })
        .FirstOrDefaultAsync();
        if (proje == null)
        {
            throw new NotFoundException("Proje bulunamadı.");
        } else
        {
            return proje;
        }
        
    }
    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, int userId)
    {
        var project = new Project 
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = userId
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var member = new ProjectMember
    {
        ProjectId = project.Id,
        UserId = userId,
        Role = ProjectRoles.Owner
    };
        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();


        return await GetByIdAsync(project.Id, userId);
    }
    public async Task<ProjectResponse> UpdateAsync(UpdateProjectRequest request, int userId, int projectId)
    {
        await EnsureMemberAsync(projectId, userId);

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        throw new NotFoundException("Proje bulunamadı");

        project.Name = request.Name;
        project.Description = request.Description;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(projectId, userId);
    }
    public async Task DeleteAsync(int projectId, int userId)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            throw new NotFoundException("Proje bulunamadı.");
        }
        if (project.OwnerId != userId)
        {
            throw new ForbiddenException("Bu projeyi sadece sahibi silebilir.");
        }
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
    public async Task EnsureMemberAsync(int projectId, int userId)
    {
        var uyeKontrol = await _context.ProjectMembers
        .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);

        if (!uyeKontrol)
        {
            throw new ForbiddenException("Erişim yetkiniz yok.");
        };
    }

    public async Task<List<MemberDto>> GetMembersAsync(int projectId, int userId)
    {
        await EnsureMemberAsync(projectId, userId);

        var members = await _context.ProjectMembers
        .Where (m => m.ProjectId == projectId)
        .Select(m => new MemberDto
        {
            UserId = m.UserId,
            FullName = m.User.FullName,
            Email = m.User.Email,
            Role = m.Role
        })
        .ToListAsync();

        return members;
    }

    public async Task<List<MemberDto>> AddMemberAsync(int projectId, AddMemberRequest request, int userId)
    {
        await EnsureMemberAsync(projectId, userId);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if ( user == null)
        {
            throw new NotFoundException("Bu e-posta ile kullanıcı bulunamadı.");
        }

        var alreadyMember = await _context.ProjectMembers
        .AnyAsync(m => m.ProjectId == projectId && m.UserId == user.Id);

        if (alreadyMember)
        {
            throw new ConflictException("Bu kullanıcı zaten üye.");
        }

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = user.Id,
            Role = ProjectRoles.Member
        };

        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();

        return await GetMembersAsync(projectId, userId);
    }
}