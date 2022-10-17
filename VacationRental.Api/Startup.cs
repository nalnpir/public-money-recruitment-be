using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Services.Services;
using VacationRental.Services.Services.Contracts;
using VacationRental.Services.Validators;

namespace VacationRental.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerGen(opts => opts.SwaggerDoc("v1", new OpenApiInfo() { Title = "Vacation rental information", Version = "v1" }));

        services.AddSingleton<IDictionary<int, Rental>>(new Dictionary<int, Rental>());
        services.AddSingleton<IDictionary<int, Booking>>(new Dictionary<int, Booking>());
        services.AddSingleton<IDictionary<int, PreparationDays>>(new Dictionary<int, PreparationDays>());
        services.AddScoped<CommonValidator>();
        services.AddSingleton<BookingValidator>();
        services.AddSingleton<RentalValidator>();

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IPreparationDaysRepository, PreparationDaysRepository>();
        services.AddScoped<IRentalService, RentalService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IBookingService, BookingService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "VacationRental v1"));

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

    }
}