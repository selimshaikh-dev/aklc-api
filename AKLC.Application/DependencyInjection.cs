using AKLC.Application.DTOs.Students;
using AKLC.Application.DTOs.Videos;
using AKLC.Application.Interfaces;
using AKLC.Application.Services;
using AKLC.Application.Validators;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace AKLC.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // =========================================
            // STUDENT SERVICE
            // =========================================

            services.AddScoped<
                IStudentService,
                StudentService>();


            // =========================================
            // STUDENT FEE ASSIGNMENT SERVICE
            // =========================================

            services.AddScoped<
                IStudentFeeAssignmentService,
                StudentFeeAssignmentService>();


            // =========================================
            // FEE TYPE SERVICE
            // =========================================

            services.AddScoped<
                IFeeTypeService,
                FeeTypeService>();


            // =========================================
            // VIDEO SERVICE
            // =========================================

            services.AddScoped<
                IVideoService,
                VideoService>();


            // =========================================
            // STUDENT PAYMENT SERVICE
            // =========================================

            services.AddScoped<
                IStudentPaymentService,
                StudentPaymentService>();


            // =========================================
            // ACCOUNT HEAD SERVICE
            // =========================================

            services.AddScoped<
                IAccountHeadService,
                AccountHeadService>();


            // =========================================
            // INCOME TRANSACTION SERVICE
            // =========================================

            services.AddScoped<
                IIncomeTransactionService,
                IncomeTransactionService>();


            // =========================================
            // EXPENSE TRANSACTION SERVICE
            // =========================================

            services.AddScoped<
                IExpenseTransactionService,
                ExpenseTransactionService>();


            // =========================================
            // PAYMENT SCHEDULE SERVICE
            // =========================================

            services.AddScoped<
                IPaymentScheduleService,
                PaymentScheduleService>();


            // =========================================
            // PAYMENT ALLOCATION SERVICE
            // =========================================

            services.AddScoped<
                IPaymentAllocationService,
                PaymentAllocationService>();


            // =========================================
            // ACCOUNT REPORT SERVICE
            // =========================================

            services.AddScoped<
                IAccountReportService,
                AccountReportService>();


            // =========================================
            // STUDENT VALIDATORS
            // =========================================

            services.AddScoped<
                IValidator<CreateStudentRequest>,
                CreateStudentRequestValidator>();

            services.AddScoped<
                IValidator<StudentEducationRequest>,
                StudentEducationRequestValidator>();

            services.AddScoped<
                IValidator<StudentProfessionalExperienceRequest>,
                StudentProfessionalExperienceRequestValidator>();

            services.AddScoped<
                IValidator<StudentLanguageProficiencyRequest>,
                StudentLanguageProficiencyRequestValidator>();


            // =========================================
            // VIDEO VALIDATORS
            // =========================================

            services.AddScoped<
                IValidator<CreateVideoRequest>,
                CreateVideoRequestValidator>();

            services.AddScoped<
                IValidator<UpdateVideoRequest>,
                UpdateVideoRequestValidator>();


            return services;
        }
    }
}