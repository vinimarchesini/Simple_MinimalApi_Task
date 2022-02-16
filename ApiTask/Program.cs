using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseInMemoryDatabase("DBTasks"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/tasks", async (AppDbContext db) => await db.Tasks.ToListAsync());

app.MapGet("/tasks/{id}", async (int id, AppDbContext db) =>
 await db.Tasks.FindAsync(id) is Task task ? Results.Ok(task) : Results.NotFound());

app.MapGet("/tasks/concludeds", async (AppDbContext db) =>
 await db.Tasks
 .Where(x => x.IsConcluded)
 .ToListAsync());

app.MapPut("/tasks/{id}", async (int id, Task inputTask, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();
    task.Name = inputTask.Name;
    task.IsConcluded = inputTask.IsConcluded;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();
    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.Ok(task);
});

app.MapPost("/tasks", async (Task task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{task.Id}", task);
});

app.Run();

class Task
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsConcluded { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Task> Tasks => Set<Task>();

}