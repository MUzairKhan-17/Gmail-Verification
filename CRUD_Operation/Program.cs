using CRUD_Operation.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

builder.Services.AddDbContext<myContext>(row => row.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{Controller=User}/{Action=Index}"
);

app.Run();