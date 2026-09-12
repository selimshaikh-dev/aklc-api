using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================================
        // AUTHENTICATION
        // =========================================

        public DbSet<User> Users =>
            Set<User>();

        public DbSet<Role> Roles =>
            Set<Role>();


        // =========================================
        // STUDENTS
        // =========================================

        public DbSet<Student> Students =>
            Set<Student>();

        public DbSet<StudentEducationQualification> StudentEducationQualifications =>
            Set<StudentEducationQualification>();

        public DbSet<StudentProfessionalExperience> StudentProfessionalExperiences =>
            Set<StudentProfessionalExperience>();

        public DbSet<StudentLanguageProficiency> StudentLanguageProficiencies =>
            Set<StudentLanguageProficiency>();

        public DbSet<Course> Courses =>
            Set<Course>();

        public DbSet<Batch> Batches =>
            Set<Batch>();

        public DbSet<FeeType> FeeTypes =>
            Set<FeeType>();

        public DbSet<CourseFeeStructure> CourseFeeStructures =>
            Set<CourseFeeStructure>();

        public DbSet<StudentFeeAssignment> StudentFeeAssignments =>
            Set<StudentFeeAssignment>();

        public DbSet<PaymentSchedule> PaymentSchedules =>
            Set<PaymentSchedule>();

        public DbSet<StudentPayment> StudentPayments =>
            Set<StudentPayment>();

        public DbSet<PaymentAllocation> PaymentAllocations =>
            Set<PaymentAllocation>();

        public DbSet<PaymentAttachment> PaymentAttachments =>
            Set<PaymentAttachment>();

        public DbSet<AuditLog> AuditLogs =>
            Set<AuditLog>();


        // =========================================
        // ACCOUNTS
        // =========================================

        public DbSet<AccountHead> AccountHeads =>
            Set<AccountHead>();

        public DbSet<IncomeTransaction> IncomeTransactions =>
            Set<IncomeTransaction>();

        public DbSet<ExpenseTransaction> ExpenseTransactions =>
            Set<ExpenseTransaction>();


        // =========================================
        // MODEL CONFIGURATION
        // =========================================

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}