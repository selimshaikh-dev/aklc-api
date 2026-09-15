using AKLC.Application.Interfaces;
using AKLC.Infrastructure.Identity;
using AKLC.Infrastructure.Persistence;
using AKLC.Infrastructure.Persistence.Seed;
using AKLC.Infrastructure.Repositories;
using AKLC.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AKLC.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // =========================================
        // CONNECTION STRING
        // =========================================

        var connectionString =
            configuration.GetConnectionString(
                "DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection was not found.");


        // =========================================
        // DATABASE
        // =========================================

        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseSqlServer(
                    connectionString));


        // =========================================
        // PASSWORD HASHING
        // =========================================

        services.AddScoped<
            IPasswordHasher,
            PasswordHasher>();


        // =========================================
        // FILE STORAGE
        // =========================================

        services.AddScoped<
            IFileStorageService,
            FileStorageService>();


        // =========================================
        // VIDEO THUMBNAIL SERVICE
        // =========================================

        services.AddScoped<
            IVideoThumbnailService,
            VideoThumbnailService>();


        // =========================================
        // JWT SETTINGS
        // =========================================

        services.Configure<JwtSettings>(
            configuration.GetSection(
                JwtSettings.SectionName));


        // =========================================
        // JWT TOKEN GENERATOR
        // =========================================

        services.AddScoped<
            IJwtTokenGenerator,
            JwtTokenGenerator>();


        // =========================================
        // AUTHENTICATION SERVICE
        // =========================================

        services.AddScoped<
            IAuthService,
            AuthService>();


        // =========================================
        // STUDENT REPOSITORY
        // =========================================

        services.AddScoped<
            IStudentRepository,
            StudentRepository>();


        // =========================================
        // STUDENT FEE ASSIGNMENT REPOSITORY
        // =========================================

        services.AddScoped<
            IStudentFeeAssignmentRepository,
            StudentFeeAssignmentRepository>();


        // =========================================
        // STUDENT PAYMENT REPOSITORY
        // =========================================

        services.AddScoped<
            IStudentPaymentRepository,
            StudentPaymentRepository>();


        // =========================================
        // PAYMENT ATTACHMENT REPOSITORY
        // =========================================

        services.AddScoped<
            IPaymentAttachmentRepository,
            PaymentAttachmentRepository>();


        // =========================================
        // ACCOUNT HEAD REPOSITORY
        // =========================================

        services.AddScoped<
            IAccountHeadRepository,
            AccountHeadRepository>();


        // =========================================
        // FEE TYPE REPOSITORY
        // =========================================

        services.AddScoped<
            IFeeTypeRepository,
            FeeTypeRepository>();


        // =========================================
        // VIDEO REPOSITORY
        // =========================================

        services.AddScoped<
            IVideoRepository,
            VideoRepository>();


        // =========================================
        // INCOME TRANSACTION REPOSITORY
        // =========================================

        services.AddScoped<
            IIncomeTransactionRepository,
            IncomeTransactionRepository>();


        // =========================================
        // EXPENSE TRANSACTION REPOSITORY
        // =========================================

        services.AddScoped<
            IExpenseTransactionRepository,
            ExpenseTransactionRepository>();


        // =========================================
        // PAYMENT SCHEDULE REPOSITORY
        // =========================================

        services.AddScoped<
            IPaymentScheduleRepository,
            PaymentScheduleRepository>();


        // =========================================
        // PAYMENT ALLOCATION REPOSITORY
        // =========================================

        services.AddScoped<
            IPaymentAllocationRepository,
            PaymentAllocationRepository>();


        // =========================================
        // ACCOUNT REPORT REPOSITORY
        // =========================================

        services.AddScoped<
            IAccountReportRepository,
            AccountReportRepository>();


        // =========================================
        // DATABASE SEEDER
        // =========================================

        services.AddScoped<
            AdminSeeder>();


        return services;
    }
}