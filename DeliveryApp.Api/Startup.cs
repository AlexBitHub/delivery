using System.Reflection;
using Api.Filters;
using DeliveryApp.Api.Adapters.BackgroundJobs;
using DeliveryApp.Core.Application.Commands.AssignCourier;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Commands.MoveCouriers;
using DeliveryApp.Core.Application.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.Queries.GetOpenOrders;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Primitives;
using Quartz;
using Api.Formatters;
using Api.OpenApi;
using DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;

namespace DeliveryApp.Api;

public class Startup
{
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();
        var configuration = builder.Build();
        Configuration = configuration;
    }

    /// <summary>
    ///     Конфигурация
    /// </summary>
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Health Checks
        services.AddHealthChecks();

        // Cors
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin(); // Не делайте так в проде!
                });
        });

        // Configuration
        services.Configure<Settings>(options => Configuration.Bind(options));
        var connectionString = Configuration["CONNECTION_STRING"];
        var geoServiceGrpcHost = Configuration["GEO_SERVICE_GRPC_HOST"];
        var messageBrokerHost = Configuration["MESSAGE_BROKER_HOST"];

        services.AddDbContext<ApplicationDbContext>((_, optBuilder) =>
        {
            optBuilder.UseNpgsql(
                connectionString,
                sqlOpt => { sqlOpt.MigrationsAssembly("DeliveryApp.Infrastructure"); });
            optBuilder.EnableSensitiveDataLogging();
        });

        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<ICourierRepository, CourierRepository>();

        // Domain Services
        services.AddTransient<IDispatchService, DispatchService>();

        // MediatR 
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());
        // Commands
        services.AddTransient<IRequestHandler<CreateOrderCommand, bool>, CreateOrderHandler>();
        services.AddTransient<IRequestHandler<MoveCouriersCommand, bool>, MoveCouriersHandler>();
        services.AddTransient<IRequestHandler<AssignCourierCommand, bool>, AssignOrdersHandler>();
        services.AddTransient<IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>>(
            _ => new GetCreatedAndAssignedOrdersHandler(connectionString));
        services.AddTransient<IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>>(
            _ => new GetBusyCouriersHandler(connectionString));

        services.AddMvcCore();
        services.AddEndpointsApiExplorer();

        AddHttpHandlers(services);
        AddSwagger(services);

        services.AddTransient<IGeoClient, Client>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "openapi/{documentName}/openapi.json";
        })
        .UseSwaggerUI(options => 
        {
            options.RoutePrefix = "openapi";
            options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Delivery Service");
            options.RoutePrefix = string.Empty;
            options.SwaggerEndpoint("/openapi-original.json", "Swagger Delivery Service");
        });
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    private void AddHttpHandlers(IServiceCollection services)
    {
        services.AddControllers(options => 
        {
            options.InputFormatters.Insert(0, new InputFormatterStream());
        }).
        AddNewtonsoftJson(options => 
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new StringEnumConverter() 
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });
        });
    }

    private void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options => 
        {
            options.SwaggerDoc("1.0.0", new OpenApiInfo()
            {
                Title = "Delivery Service",
                Description = "Отвечает за учет курьеров, деспетчеризацию заказов, доставку",
                Contact = new OpenApiContact()
                {
                    Name = "Alexey Bulygin",
                    Url = new Uri("https://microarch.ru"),
                    Email = "info@microarch.ru"
                }
            });
            options.CustomSchemaIds(type => type.FriendlyId(true));
            options.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");
            options.DocumentFilter<BasePathFilter>("");
            options.OperationFilter<GeneratePathParamsValidationFilter>();
        });
        services.AddSwaggerGenNewtonsoftSupport();
    }

    private void AddCronJobs(IServiceCollection services)
    {
        services.AddQuartz(configure => 
        {
            var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
            var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));
            // var processOutboxMessageJbKey = new JobKey(nameof())
            configure.AddJob<AssignOrdersJob>(assignOrdersJobKey)
                     .AddTrigger(trigger => 
                                 trigger.ForJob(assignOrdersJobKey)
                                        .WithSimpleSchedule(schedule => 
                                                            schedule.WithIntervalInSeconds(1)
                                                                    .RepeatForever()))
                     .AddJob<MoveCouriersJob>(moveCouriersJobKey)
                     .AddTrigger(trigger => 
                                 trigger.ForJob(moveCouriersJobKey)
                                        .WithSimpleSchedule(schedule =>
                                                            schedule.WithIntervalInSeconds(2)
                                                                    .RepeatForever()));
            
            // configure.UseMicrosoftDependencyInjectionJobFactory();                                                                    
        });   
        services.AddQuartzHostedService();
    }
}