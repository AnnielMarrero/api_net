using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Application.DTOs;
using TaskManagement.Api.Application.Exceptions;
using TaskManagement.Api.Application.Services;
using TaskManagement.Api.Domain.Enums;
using TaskManagement.Api.Infrastructure.Persistence;
using TaskManagement.Api.Infrastructure.Persistence.Repositories;
using TaskStatus = TaskManagement.Api.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests;

public sealed class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsAndReturnsTask()
    {
        await using var fixture = await TestFixture.CreateAsync();

        var created = await fixture.Service.CreateAsync(CreateRequest("Prepare report", "Ana"));
        var found = await fixture.Service.GetByIdAsync(created.Id);

        Assert.Equal("Prepare report", found.Title);
        Assert.Equal("Ana", found.Responsible);
        Assert.Equal(TaskStatus.Pending, found.Status);
    }

    [Fact]
    public async Task GetPagedAsync_FiltersByStatusAndResponsibleAndPaginates()
    {
        await using var fixture = await TestFixture.CreateAsync();

        await fixture.Service.CreateAsync(CreateRequest("First", "Ana", TaskStatus.Pending));
        await fixture.Service.CreateAsync(CreateRequest("Second", "Ana", TaskStatus.Pending));
        await fixture.Service.CreateAsync(CreateRequest("Third", "Luis", TaskStatus.Completed));

        var result = await fixture.Service.GetPagedAsync(new TaskQueryParameters
        {
            Status = TaskStatus.Pending,
            Responsible = "Ana",
            Page = 2,
            PageSize = 1
        });

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Single(result.Items);
        Assert.Equal("Second", result.Items.Single().Title);
    }

    [Fact]
    public async Task UpdateAsync_ChangesTaskFields()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var created = await fixture.Service.CreateAsync(CreateRequest("Draft", "Ana"));

        await fixture.Service.UpdateAsync(created.Id, new UpdateTaskRequest(
            "Published",
            "Final version",
            TaskStatus.Completed,
            "Luis",
            UtcDate(2026, 12, 1)));

        var updated = await fixture.Service.GetByIdAsync(created.Id);
        Assert.Equal("Published", updated.Title);
        Assert.Equal(TaskStatus.Completed, updated.Status);
        Assert.Equal("Luis", updated.Responsible);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTaskAndGetThrowsNotFound()
    {
        await using var fixture = await TestFixture.CreateAsync();
        var created = await fixture.Service.CreateAsync(CreateRequest("Remove me", "Ana"));

        await fixture.Service.DeleteAsync(created.Id);

        await Assert.ThrowsAsync<TaskNotFoundException>(() => fixture.Service.GetByIdAsync(created.Id));
    }

    private static CreateTaskRequest CreateRequest(
        string title,
        string responsible,
        TaskStatus status = TaskStatus.Pending) =>
        new(title, "Description", status, responsible, UtcDate(2026, 12, 31));

    private static DateTime UtcDate(int year, int month, int day) =>
        new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    private sealed class TestFixture : IAsyncDisposable
    {
        private readonly SqliteConnection connection;
        private readonly ApplicationDbContext dbContext;

        private TestFixture(SqliteConnection connection, ApplicationDbContext dbContext)
        {
            this.connection = connection;
            this.dbContext = dbContext;
            Service = new TaskService(new TaskRepository(dbContext), new UnitOfWork(dbContext));
        }

        public TaskService Service { get; }

        public static async Task<TestFixture> CreateAsync()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            return new TestFixture(connection, dbContext);
        }

        public async ValueTask DisposeAsync()
        {
            await dbContext.DisposeAsync();
            await connection.DisposeAsync();
        }
    }
}
