using ElinaTestProject.Converters;
using ElinaTestProject.Interfaces.Admin;
using ElinaTestProject.Interfaces.History;
using ElinaTestProject.Interfaces.Order;
using ElinaTestProject.Interfaces.Queue;
using ElinaTestProject.Interfaces.Reservation;
using ElinaTestProject.Interfaces.Table;
using ElinaTestProject.Interfaces.User;
using ElinaTestProject.Interfaces.WorkShift;
using ElinaTestProject.Models.Admin;
using ElinaTestProject.Models.History;
using ElinaTestProject.Models.Order;
using ElinaTestProject.Models.Queue;
using ElinaTestProject.Models.Reservation;
using ElinaTestProject.Models.Table;
using ElinaTestProject.Models.User;
using ElinaTestProject.Models.WorkShift;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostGreContext.Context;
using Newtonsoft.Json;

namespace ElinaTestProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connectionString = "Server=127.0.0.1;Username=postgres;Password=example;Database=TestDb;Persist Security Info=True";
           // var connectionString = "Server=127.0.0.1;Username=postgres;Password=example;Database=UserMessage;Persist Security Info=True";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            //builder.Services.AddControllers().addn(options =>
            //{
            //    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            //    options.

            //});

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new TimeSpanConverter());
            });
            


            builder.Services.AddDbContext<TestDbContext>(opt => opt.UseNpgsql(connectionString));

            builder.Services.AddScoped<IAdminInterface, AdminRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITableRepository, TableRepository>();
            builder.Services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();
            builder.Services.AddScoped<IQueueRepository, QueueRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IHistoryRepository,HistoryRepository>();

            
            


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
