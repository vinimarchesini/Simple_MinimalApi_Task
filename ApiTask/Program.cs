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

app.MapPost("/tasks", async (Task task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{task.Id}", task);
});

app.MapPut("/tasks/{id}", async (int id, Task inputTask, AppDbContext db) =>
{
    if (await db.Tasks.FindAsync(id) is Task task)
    {
        task.Name = inputTask.Name;
        task.IsConcluded = inputTask.IsConcluded;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.MapDelete("/tasks/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Tasks.FindAsync(id) is Task task)
    {
        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return Results.Ok(task);
    }
    return Results.NotFound();
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

