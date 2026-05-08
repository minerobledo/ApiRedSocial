using Aplication.DTO.InputDto.Login.reponceProfileAndUser;
using Aplication.DTO.InputDto.Register;
using Aplication.DTO.OutputDto.Profile;
using Aplication.DTO.Users;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Aplication.ResponPattern;
using AutoMapper;
using Domain.Entities;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Jobs;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Resend;
using System.Reflection;
using System.Text;




namespace Infrastructure
{
    public static class ServicesExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JWT");

            // 🔐 Configuración de JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!))
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // Solo aplica para rutas como /chatHub
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

            // 🚀 Configuración de Firebase
            ConfigureFirebase(services);

            // 🗂️ Configuración de AutoMapper
            ConfigureAutoMapper(services);

            ConfigureQuartz(services);

            ConfigureReSend(services,configuration);

            // 📦 Inyección de dependencias
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFirebaseMessagingRepository, FirebaseMessagingRepository>();
            
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IFirebaseMessagingRepository, FirebaseMessagingRepository>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRefresTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IContestRespository, ContestRepositoy>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IEventRepocitory, EventRepocitory>();
            services.AddScoped<IStatisticsRepocitory , StatisticsRepocitory>();
            services.AddScoped<IAdminRepocitory,  AdminRepocitory>();
            services.AddScoped<IAdminChatRepository, AdminChatRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IQuartzJobService ,QuartzJobService>();
            services.AddScoped<IImageService, ImageService>();
            // Agregar SignalR
            services.AddSignalR();
            services.AddScoped<IBackapService, BackapService>();
            // Inyección del ChatHub y Repositorios relacionados con el Chat
            services.AddScoped<IChatRepository, ChatRepocitory>(); // Implementación de acceso a datos del chat
            services.AddScoped<IMessageRepository, MessageRepository>(); // Repositorio para guardar mensajes
        }

        private static void ConfigureFirebase(IServiceCollection services)
        {
            GoogleCredential credential;
            var credentialsJson = $@"
            {{
                ""type"": ""{Environment.GetEnvironmentVariable("type")}"",
                ""project_id"": ""{Environment.GetEnvironmentVariable("project_id")}"",
                ""private_key_id"": ""{Environment.GetEnvironmentVariable("private_key_id")}"",
                ""private_key"": ""{Environment.GetEnvironmentVariable("private_key")}"",
                ""client_email"": ""{Environment.GetEnvironmentVariable("client_email")}"",
                ""client_id"": ""{Environment.GetEnvironmentVariable("FIREBASE_CLIENT_ID")}"",
                ""auth_uri"": ""{Environment.GetEnvironmentVariable("auth_uri")}"",
                ""token_uri"": ""{Environment.GetEnvironmentVariable("token_uri")}"",
                ""auth_provider_x509_cert_url"": ""{Environment.GetEnvironmentVariable("auth_provider_x509_cert_url")}"",
                ""client_x509_cert_url"": ""{Environment.GetEnvironmentVariable("client_x509_cert_url")}"",
                ""universe_domain"": ""{Environment.GetEnvironmentVariable("universe_domain")}""
            }}";

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("type")))
            {
                
                // Cargar las credenciales desde el JSON generado a partir de las variables de entorno
                credential = GoogleCredential.FromJson(credentialsJson);
            }else
            {
                //en caso contrario de que se ejecute en mi pc para debuging lee desde es archivo JSON en la carpeta raiz
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-credentials.json");
                Console.WriteLine(path);
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                credential = GoogleCredential.FromFile(path);
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }

            var builder = new FirestoreClientBuilder
            {
                Credential = credential
            };

            var firestoreClient = builder.Build();

            // Registrar FirestoreDb en los servicios
            services.AddSingleton(provider =>
            {
                return FirestoreDb.Create("proyectox-f9e00", client: firestoreClient);
            });
        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<RegisterProfileDto, Domain.Entities.Profile>();
                mc.CreateMap<Domain.Entities.Profile, LoginProfileResponce>();
                mc.CreateMap<DeviceTokenDto, DeviceToken>();
                mc.CreateMap<Domain.Entities.Profile, ProfileShortDto>();
                mc.CreateMap<Domain.Entities.Profile, ProfileLongDto>();
                mc.CreateMap<Domain.Entities.Profile, ProfileForAdmin>();
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
        private static void ConfigureReSend(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ResendClientOptions>(o =>
            {
                // Leé tu token desde configuración/variables de entorno
                o.ApiToken = configuration["Resend:ApiToken"];
            });
            services.AddHttpClient<ResendClient>();
            services.AddTransient<IResend, ResendClient>();

            // Tu servicio
            services.AddScoped<IEmailService, EmailService>();
        }
        private static void ConfigureQuartz(IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                
                var jobKey = new JobKey("DeleteGlobalNotifications7DaysOld");

                // Registrar la tarea en Quartz
                q.AddJob<DeleteGlovalNotificationJob>(opts => opts.WithIdentity(jobKey));

                // Definir el cronograma de ejecución (ejemplo: cada día a las 00:30 AM)
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("Trigger-ConquestPosesor")
                    .WithCronSchedule("0 30 0 * * ?")); // 00:30 AM todos los días
            });
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }
        
    }
}
