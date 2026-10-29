using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class FlightshallContext : DbContext
    {
        public FlightshallContext()
        {
        }

        public FlightshallContext(DbContextOptions<FlightshallContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airline> Airlines { get; set; }
        public virtual DbSet<Airport> Airports { get; set; }
        public virtual DbSet<ApiKey> ApiKeys { get; set; }
        public virtual DbSet<BookingFlghtDetail> BookingFlghtDetails { get; set; }
        public virtual DbSet<BookingPaxDetail> BookingPaxDetails { get; set; }
        public virtual DbSet<BookingPaymentRecieved> BookingPaymentRecieveds { get; set; }
        public virtual DbSet<BookingSupplierInfo> BookingSupplierInfos { get; set; }
        public virtual DbSet<BookingTransaction> BookingTransactions { get; set; }
        public virtual DbSet<ControlPermission> ControlPermissions { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeAddress> EmployeeAddresses { get; set; }
        public virtual DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public virtual DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }
        public virtual DbSet<EmployeeEarning> EmployeeEarnings { get; set; }
        public virtual DbSet<EmployeeIncentive> EmployeeIncentives { get; set; }
        public virtual DbSet<EmployeePayroll> EmployeePayrolls { get; set; }
        public virtual DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
        public virtual DbSet<FareMarkUpNew> FareMarkUpNews { get; set; }
        public virtual DbSet<FlightBooking> FlightBookings { get; set; }
        public virtual DbSet<FlightSearchTrack> FlightSearchTracks { get; set; }
        public virtual DbSet<FlightsSearchNew> FlightsSearchNews { get; set; }
        public virtual DbSet<HotelsMetaSearchTrack> HotelsMetaSearchTracks { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<MarkUpNew> MarkUpNews { get; set; }
        public virtual DbSet<MetaSearch> MetaSearches { get; set; }
        public virtual DbSet<MetaSearchTrack> MetaSearchTracks { get; set; }
        public virtual DbSet<OneDflightFareNew> OneDflightFareNews { get; set; }
        public virtual DbSet<OneDflightSagementNew> OneDflightSagementNews { get; set; }
        public virtual DbSet<OneDflightSagementTest> OneDflightSagementTests { get; set; }
        public virtual DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleStatus> RoleStatuses { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierPayment> SupplierPayments { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=3.13.105.185,49729;Database=Flightshall;user id=sa;password=Flightshall@123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Airline>(entity =>
            {
                entity.ToTable("Airline");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Airport>(entity =>
            {
                entity.ToTable("Airport");

                entity.Property(e => e.CityCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CityName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Continent)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CountryName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Longtitude)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StateCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StateName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.HasKey(e => e.KeyId)
                    .HasName("PK__ApiKeys__21F5BE278E10D980");

                entity.Property(e => e.KeyId).HasColumnName("KeyID");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookingFlghtDetail>(entity =>
            {
                entity.HasKey(e => e.FlightSegmentId);

                entity.Property(e => e.FlightSegmentId).HasColumnName("FlightSegmentID");

                entity.Property(e => e.Airline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ArrDate).HasColumnType("datetime");

                entity.Property(e => e.ArrivalDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ArriveOn)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.BoundClass)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Dcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Delind).HasDefaultValueSql("((0))");

                entity.Property(e => e.DepDate).HasColumnType("datetime");

                entity.Property(e => e.DepartDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DepartOn)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightType)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Gcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BookingPaxDetail>(entity =>
            {
                entity.HasKey(e => e.PaxId)
                    .HasName("PK_PaxDetails");

                entity.Property(e => e.PaxId).HasColumnName("PaxID");

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.Delind).HasDefaultValueSql("((0))");

                entity.Property(e => e.Dob)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOB");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PaxTitle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaxType)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BookingPaymentRecieved>(entity =>
            {
                entity.ToTable("BookingPaymentRecieved");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.PamentDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentRecived).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<BookingSupplierInfo>(entity =>
            {
                entity.ToTable("BookingSupplierInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BalanceDueAmt).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BalanceDueDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.FirstDepositPaidAmt).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FirstDepositPaidDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsfullPaid).HasColumnName("ISFullPaid");

                entity.Property(e => e.SecondDepositDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SecondDepositPaidAmt).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SupplerRefrence)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("supplerRefrence");

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BookingTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__BookingT__55433A6BB0968E2B");

                entity.ToTable("BookingTransaction");

                entity.Property(e => e.AssignAgentId).HasColumnName("Assign_AgentId");

                entity.Property(e => e.Comment).IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.AssignAgent)
                    .WithMany(p => p.BookingTransactionAssignAgents)
                    .HasForeignKey(d => d.AssignAgentId)
                    .HasConstraintName("FK__BookingTr__Assig__367C1819");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingTransactions)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK__BookingTr__Booki__37703C52");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.BookingTransactions)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__BookingTr__Statu__3864608B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BookingTransactionUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__BookingTr__UserI__395884C4");
            });

            modelBuilder.Entity<ControlPermission>(entity =>
            {
                entity.HasKey(e => e.Cid);

                entity.ToTable("ControlPermission");

                entity.Property(e => e.Cid).HasColumnName("CID");

                entity.Property(e => e.Marketing)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Platforms)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProviderName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TravolieIsActive).HasColumnName("Travolie_IsActive");

                entity.Property(e => e.TravolieSagePayIsActive).HasColumnName("TravolieSagePay_IsActive");

                entity.Property(e => e.TravolieSitePhone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Travolie_SitePhone");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AadharCard)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.Doj)
                    .HasColumnType("date")
                    .HasColumnName("DOJ");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lwd)
                    .HasColumnType("date")
                    .HasColumnName("LWD");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Pan)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PAN");

                entity.Property(e => e.PseudoName).HasMaxLength(50);
            });

            modelBuilder.Entity<EmployeeAddress>(entity =>
            {
                entity.HasKey(e => e.AddressId);

                entity.ToTable("EmployeeAddress");

                entity.Property(e => e.Address1)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Address2)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddressType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAddresses)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeAddress_Employee");
            });

            modelBuilder.Entity<EmployeeAttendance>(entity =>
            {
                entity.ToTable("EmployeeAttendance");

                entity.Property(e => e.AttendanceDate).HasColumnType("date");

                entity.Property(e => e.LoginTime).HasColumnType("datetime");

                entity.Property(e => e.LogoutTime).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAttendances)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeAttendance_Employee");

                entity.HasOne(d => d.PayrollPeriod)
                    .WithMany(p => p.EmployeeAttendances)
                    .HasForeignKey(d => d.PayrollPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeAttendance_PayrollPeriod");
            });

            modelBuilder.Entity<EmployeeDeduction>(entity =>
            {
                entity.ToTable("EmployeeDeduction");

                entity.Property(e => e.Esi)
                    .HasColumnType("money")
                    .HasColumnName("ESI");

                entity.Property(e => e.Lwp)
                    .HasColumnType("money")
                    .HasColumnName("LWP");

                entity.Property(e => e.Misc).HasColumnType("money");

                entity.Property(e => e.Pf)
                    .HasColumnType("money")
                    .HasColumnName("PF");

                entity.Property(e => e.Tds)
                    .HasColumnType("money")
                    .HasColumnName("TDS");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeDeductions)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeDeduction_Employee");

                entity.HasOne(d => d.Payroll)
                    .WithMany(p => p.EmployeeDeductions)
                    .HasForeignKey(d => d.PayrollId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeDeduction_EmployeePayroll");
            });

            modelBuilder.Entity<EmployeeEarning>(entity =>
            {
                entity.ToTable("EmployeeEarning");

                entity.Property(e => e.Arrears).HasColumnType("money");

                entity.Property(e => e.Basic).HasColumnType("money");

                entity.Property(e => e.Bonus).HasColumnType("money");

                entity.Property(e => e.Da)
                    .HasColumnType("money")
                    .HasColumnName("DA");

                entity.Property(e => e.GrossTotal)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Hra)
                    .HasColumnType("money")
                    .HasColumnName("HRA");

                entity.Property(e => e.Incentive).HasColumnType("money");

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.Property(e => e.SpecialAllowance).HasColumnType("money");

                entity.Property(e => e.Variable).HasColumnType("money");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeEarnings)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeEarning_Employee");

                entity.HasOne(d => d.Payroll)
                    .WithMany(p => p.EmployeeEarnings)
                    .HasForeignKey(d => d.PayrollId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeEarning_EmployeePayroll");
            });

            modelBuilder.Entity<EmployeeIncentive>(entity =>
            {
                entity.ToTable("EmployeeIncentive");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.Incentive).HasColumnType("money");

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeIncentives)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeIncentive_Employee");

                entity.HasOne(d => d.PayrollPeriod)
                    .WithMany(p => p.EmployeeIncentives)
                    .HasForeignKey(d => d.PayrollPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeIncentive_PayrollPeriod");
            });

            modelBuilder.Entity<EmployeePayroll>(entity =>
            {
                entity.ToTable("EmployeePayroll");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Deductions).HasColumnType("money");

                entity.Property(e => e.Earnings).HasColumnType("money");

                entity.Property(e => e.LastModified).HasColumnType("datetime");

                entity.Property(e => e.ReleasedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeePayrolls)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeePayroll_Employee");

                entity.HasOne(d => d.PayrollPeriod)
                    .WithMany(p => p.EmployeePayrolls)
                    .HasForeignKey(d => d.PayrollPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeePayroll_PayrollPeriod");
            });

            modelBuilder.Entity<EmployeeSalary>(entity =>
            {
                entity.ToTable("EmployeeSalary");

                entity.Property(e => e.Basic).HasColumnType("money");

                entity.Property(e => e.Da)
                    .HasColumnType("money")
                    .HasColumnName("DA");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.GrossTotal)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Hra)
                    .HasColumnType("money")
                    .HasColumnName("HRA");

                entity.Property(e => e.Incentive).HasColumnType("money");

                entity.Property(e => e.SpecialAllowance).HasColumnType("money");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Variable).HasColumnType("money");
            });

            modelBuilder.Entity<FareMarkUpNew>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FareMarkUp_New");

                entity.Property(e => e.Airlines)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Classes)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Destination)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.MarkUp).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.MarkUpType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Origin)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaxType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FlightBooking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.AdultFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AdultMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AdutlTax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Agent)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Airline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.AssignDate).HasColumnType("datetime");

                entity.Property(e => e.BaggageCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BaggageSize)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BillAddress1)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.BillAddress2)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BillCity)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BillCountry)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BillPostalCode)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.BillState)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BookingDate).HasColumnType("datetime");

                entity.Property(e => e.BookingNumber).HasMaxLength(100);

                entity.Property(e => e.CardCharge)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CardName)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.CardNumber)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.CardSecCode)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.CardType)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.CardValMonth)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.ChildFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ChildMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ChildTax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DepartDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightClass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightCost).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.FlightDetailsInbound).IsUnicode(false);

                entity.Property(e => e.FlightDetailsOutbound).IsUnicode(false);

                entity.Property(e => e.FlightWay)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FromDeparture)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gross).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.InfantFare).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InfantMarkup).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InfantTax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Initialmco).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.IsStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('A')")
                    .IsFixedLength(true);

                entity.Property(e => e.Isclaimed).HasDefaultValueSql("((0))");

                entity.Property(e => e.Marketing)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MarkupType)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MarkupValue)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Mco)
                    .HasColumnType("money")
                    .HasColumnName("MCO");

                entity.Property(e => e.MercahntFeeAfterCal).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.MerchantFeeInPercent).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Nett).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.PassangerData).IsUnicode(false);

                entity.Property(e => e.PaxFullName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentType)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Profitt).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Provider)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierFee).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SupportCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SupportPlan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToDestination)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TotalMco)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("TotalMCO");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.FlightBookings)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__FlightBoo__Statu__44CA3770");
            });

            modelBuilder.Entity<FlightSearchTrack>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FlightSearch_Track");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DCODE");

                entity.Property(e => e.DepartDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FareTrip)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("GCODE");

                entity.Property(e => e.Marketing)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FlightsSearchNew>(entity =>
            {
                entity.HasKey(e => e.SearchId);

                entity.ToTable("FlightsSearch_New");

                entity.Property(e => e.SearchId).ValueGeneratedNever();

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.SearchResult).IsRequired();
            });

            modelBuilder.Entity<HotelsMetaSearchTrack>(entity =>
            {
                entity.HasKey(e => e.SearchTrackId)
                    .HasName("PK__Hotels.M__69ADE96F7F0EA727");

                entity.ToTable("Hotels.MetaSearch_Track");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DCODE");

                entity.Property(e => e.Destination)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.Marketing)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.LoggedOnDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<MarkUpNew>(entity =>
            {
                entity.ToTable("MarkUp_New");

                entity.Property(e => e.Airline)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.AirlineRestriction)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Channel)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.FareType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FromCity)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FromCityRestriction)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FromCountry)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FromCountryRestriction)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.InBoundRestriction)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.InboundAirline)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.InboundClasses)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.InboundEndDate).HasColumnType("datetime");

                entity.Property(e => e.InboundStartDate).HasColumnType("datetime");

                entity.Property(e => e.JourneyType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.OutBoundRestriction)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.OutboundAirline)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.OutboundClasses)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.OutboundEndDate).HasColumnType("datetime");

                entity.Property(e => e.OutboundStartDate).HasColumnType("datetime");

                entity.Property(e => e.PaxCount)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.PaxType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StopCount)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ToCity)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ToCityRestriction)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ToCountry)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ToCountryRestriction)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MetaSearch>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MetaSearch");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlightId)
                    .HasMaxLength(500)
                    .HasColumnName("FlightID");
            });

            modelBuilder.Entity<MetaSearchTrack>(entity =>
            {
                entity.HasKey(e => e.SearchTrackId)
                    .HasName("PK__MetaSear__69ADE96F3E7BDB06");

                entity.ToTable("MetaSearch_Track");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DCODE");

                entity.Property(e => e.DepartDate).HasColumnType("datetime");

                entity.Property(e => e.FareTrip)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("GCODE");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.Marketing)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReturnDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<OneDflightFareNew>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("OneDFlightFare_New");

                entity.Property(e => e.Airline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DepDateFrom).HasColumnType("date");

                entity.Property(e => e.DepDateTo).HasColumnType("date");

                entity.Property(e => e.Destination)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FareType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightClass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightSegIdx)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FlightSegIDx");

                entity.Property(e => e.FlightWay)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsJc).HasColumnName("IsJC");

                entity.Property(e => e.IsOd).HasColumnName("IsOD");

                entity.Property(e => e.MarkupAdtChd).HasColumnName("Markup_Adt_Chd");

                entity.Property(e => e.MarkupJc).HasColumnName("MarkupJC");

                entity.Property(e => e.MarkupOd).HasColumnName("MarkupOD");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Notes)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Origin)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SellDateTo).HasColumnType("date");

                entity.Property(e => e.TotalFareJc).HasColumnName("TotalFareJC");

                entity.Property(e => e.TotalFareOd).HasColumnName("TotalFareOD");

                entity.Property(e => e.UniqueId)
                    .IsUnicode(false)
                    .HasColumnName("UniqueID");
            });

            modelBuilder.Entity<OneDflightSagementNew>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("OneDFlightSagement_New");

                entity.Property(e => e.Airline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baggage)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CodeShare)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Eft)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EFT");

                entity.Property(e => e.EquipmentType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightClass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightSegIdx)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("FlightSegIDx");

                entity.Property(e => e.FromAirportCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FromTerminal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FromTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToAirportCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToTerminal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OneDflightSagementTest>(entity =>
            {
                entity.ToTable("OneDFlightSagement_test");

                entity.Property(e => e.Airline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baggage)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CodeShare)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Eft)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EFT");

                entity.Property(e => e.EquipmentType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightClass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlightSegIdx)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("FlightSegIDx");

                entity.Property(e => e.FromAirportCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FromTerminal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FromTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToAirportCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToTerminal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PayrollPeriod>(entity =>
            {
                entity.ToTable("PayrollPeriod");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Twd).HasColumnName("TWD");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Role1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Role");
            });

            modelBuilder.Entity<RoleStatus>(entity =>
            {
                entity.ToTable("RoleStatus");

                entity.Property(e => e.RoleStatusId).HasColumnName("RoleStatusID");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.RoleStatuses)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RoleStatu__Statu__45BE5BA9");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.Status1)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("Status");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.Property(e => e.Address1).HasMaxLength(255);

                entity.Property(e => e.Address2).HasMaxLength(255);

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ZIP");
            });

            modelBuilder.Entity<SupplierPayment>(entity =>
            {
                entity.ToTable("SupplierPayment");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DatePaid).HasColumnType("datetime");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.SupplierPayments)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplierPayment_FlightBookings");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierPayments)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplierPayment_Supplier");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SupplierPayments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_SupplierPayment_Users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PageAccess)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleId__498EEC8D");
            });

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserFirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserLastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassword)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserRole)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
