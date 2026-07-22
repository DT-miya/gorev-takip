using BCrypt.Net;
using TaskBoard.Api.Data.Entities;

namespace TaskBoard.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        // Daha önce seed atıldıysa tekrar ekleme
        if (context.Users.Any())
            return;

        //user
        var ali = new User
        {
            FullName = "Ali Yılmaz",
            Email = "ali@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
        };

        var ayse = new User
        {
            FullName = "Ayşe Demir",
            Email = "ayse@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
        };

        context.Users.AddRange(ali, ayse);
        context.SaveChanges();

        // proje
        var project = new Project
        {
            Name = "Staj Projesi",
            Description = "Görev takip uygulaması",
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
            Name = "Yapılacak",
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


        // görevler
        context.Tasks.AddRange(

    new TaskItem
    {
        ColumnId = todo.Id,
        Title = "Login ekranını hazırla",
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
        Title = "JWT Token oluştur",
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
        Title = "Entity oluştur",
        AssigneeId = ayse.Id,
        Priority = TaskPriorities.Low,
        Order = 1
    },

    new TaskItem
    {
        ColumnId = done.Id,
        Title = "Proje yapısı kuruldu",
        AssigneeId = ali.Id,
        Priority = TaskPriorities.Low,
        Order = 2
    }

);


        context.SaveChanges();
    }
}