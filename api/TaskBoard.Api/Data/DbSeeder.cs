using BCrypt.Net;
using TaskBoard.Api.Data.Entities;

namespace TaskBoard.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        // Daha �nce seed at�ld�ysa tekrar ekleme
        if (context.Users.Any())
            return;

        //user
        var ali = new User
        {
            FullName = "Ali Y�lmaz",
            Email = "ali@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
        };

        var ayse = new User
        {
            FullName = "Ay�e Demir",
            Email = "ayse@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
        };

        context.Users.AddRange(ali, ayse);
        context.SaveChanges();

        // proje
        var project = new Project
        {
            Name = "Staj Projesi",
            Description = "G�rev takip uygulamas�",
            OwnerId = ali.Id
        };

        context.Projects.Add(project);
        context.SaveChanges();

        // �yeler
        context.ProjectMembers.AddRange(

    new ProjectMember
    {
        ProjectId = project.Id,
        UserId = ali.Id,
        Role = ProjectRoles.Owner
    },

    new ProjectMember
    {
        ProjectId = project.Id,
        UserId = ayse.Id,
        Role = ProjectRoles.Member
    }

);

       

        // kolonlar
        var todo = new BoardColumn
        {
            ProjectId = project.Id,
            Name = "Yap�lacak",
            Order = 1
        };

        var doing = new BoardColumn
        {
            ProjectId = project.Id,
            Name = "Devam Ediyor",
            Order = 2
        };

        var done = new BoardColumn
        {
            ProjectId = project.Id,
            Name = "Bitti",
            Order = 3
        };

        context.BoardColumns.AddRange(todo, doing, done);
        context.SaveChanges();


        // g�revler
        context.Tasks.AddRange(

    new TaskItem
    {
        ColumnId = todo.Id,
        Title = "Login ekran�n� haz�rla",
        AssigneeId = ali.Id,
        Priority = TaskPriorities.High,
        Order = 1
    },

    new TaskItem
    {
        ColumnId = todo.Id,
        Title = "Register endpointi",
        AssigneeId = ali.Id,
        Priority = TaskPriorities.Medium,
        Order = 2
    },

    new TaskItem
    {
        ColumnId = doing.Id,
        Title = "JWT Token olu�tur",
        AssigneeId = ali.Id,
        Priority = TaskPriorities.High,
        Order = 1
    },

    new TaskItem
    {
        ColumnId = doing.Id,
        Title = "Angular Login",
        AssigneeId = ayse.Id,
        Priority = TaskPriorities.Medium,
        Order = 2
    },

    new TaskItem
    {
        ColumnId = done.Id,
        Title = "Entity olu�tur",
        AssigneeId = ayse.Id,
        Priority = TaskPriorities.Low,
        Order = 1
    },

    new TaskItem
    {
        ColumnId = done.Id,
        Title = "Proje yap�s� kuruldu",
        AssigneeId = ali.Id,
        Priority = TaskPriorities.Low,
        Order = 2
    }

);


        context.SaveChanges();
    }
}