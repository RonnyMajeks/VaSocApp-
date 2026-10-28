using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VaSocApp_System.Models;

public partial class VaSocDbContext : DbContext
{
    public VaSocDbContext()
    {
    }

    public VaSocDbContext(DbContextOptions<VaSocDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<Budget> Budgets { get; set; }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<Chairperson> Chairpersons { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Contribution> Contributions { get; set; }

    public virtual DbSet<Culture> Cultures { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Itadmin> Itadmins { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Meeting> Meetings { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Rsvp> Rsvps { get; set; }

    public virtual DbSet<Secretary> Secretaries { get; set; }

    public virtual DbSet<Society> Societies { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentAffair> StudentAffairs { get; set; }

    public virtual DbSet<Treasurer> Treasurers { get; set; }

    public virtual DbSet<University> Universities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=VarsitySocietyDB;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId).HasName("PK__ANNOUNCE__9D4530646D2D41C9");

            entity.ToTable("ANNOUNCEMENTS");

            entity.Property(e => e.AnnouncementId).HasColumnName("ANNOUNCEMENT_ID");
            entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");
            entity.Property(e => e.DatePosted)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_ANNOUNCEMENTS_DATE_POSTED")
                .HasColumnName("DATE_POSTED");
            entity.Property(e => e.Message)
                .IsUnicode(false)
                .HasColumnName("MESSAGE");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TITLE");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ANNOUNCEMENTS_PERSONS");

            entity.HasOne(d => d.Society).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ANNOUNCEMENTS_SOCIETIES");
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.BudgetId).HasName("PK__BUDGETS__8AD874622862E0E4");

            entity.ToTable("BUDGETS");

            entity.Property(e => e.BudgetId).HasColumnName("BUDGET_ID");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACADEMIC_YEAR");
            entity.Property(e => e.AmountAllocated)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("AMOUNT_ALLOCATED");
            entity.Property(e => e.DateAllocated).HasColumnName("DATE_ALLOCATED");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");

            entity.HasOne(d => d.Society).WithMany(p => p.Budgets)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_BUDGETS_SOCIETIES");
        });

        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__CAMPUSES__F39BBB94A5D0D191");

            entity.ToTable("CAMPUSES");

            entity.Property(e => e.CampusId).HasColumnName("CAMPUS_ID");
            entity.Property(e => e.CampusName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CAMPUS_NAME");
            entity.Property(e => e.LocationId).HasColumnName("LOCATION_ID");
            entity.Property(e => e.Univid).HasColumnName("UNIVID");

            entity.HasOne(d => d.Location).WithMany(p => p.Campuses)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_CAMPUSES_LOCATIONS");

            entity.HasOne(d => d.Univ).WithMany(p => p.Campuses)
                .HasForeignKey(d => d.Univid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CAMPUSES_UNIVERSITIES");
        });

        modelBuilder.Entity<Chairperson>(entity =>
        {
            entity.HasKey(e => e.ChairpersonId).HasName("PK__CHAIRPER__0E7E0F5D0B418507");

            entity.ToTable("CHAIRPERSONS");

            entity.Property(e => e.ChairpersonId).HasColumnName("CHAIRPERSON_ID");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");

            entity.HasOne(d => d.Person).WithMany(p => p.Chairpeople)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CHAIRPERSONS_PERSONS");

            entity.HasOne(d => d.Society).WithMany(p => p.Chairpeople)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CHAIRPERSONS_SOCIETIES");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__CITIES__6E64DFEAA5065006");

            entity.ToTable("CITIES");

            entity.Property(e => e.CityId).HasColumnName("CITY_ID");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CITY_NAME");
            entity.Property(e => e.Provid).HasColumnName("PROVID");

            entity.HasOne(d => d.Prov).WithMany(p => p.Cities)
                .HasForeignKey(d => d.Provid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CITIES_PROVINCES");
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.HasKey(e => e.ContiId).HasName("PK__CONTRIBU__A1670001683DC804");

            entity.ToTable("CONTRIBUTIONS");

            entity.Property(e => e.ContiId).HasColumnName("CONTI_ID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");

            entity.HasOne(d => d.Person).WithMany(p => p.Contributions)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CONTRIBUTIONS_PERSONS");

            entity.HasOne(d => d.Society).WithMany(p => p.Contributions)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CONTRIBUTIONS_SOCIETIES");
        });

        modelBuilder.Entity<Culture>(entity =>
        {
            entity.HasKey(e => e.Cid).HasName("PK__CULTURES__C1F8DC59D23F8866");

            entity.ToTable("CULTURES");

            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.CultureName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CULTURE_NAME");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__EVENTS__241A510C2EF8A8BD");

            entity.ToTable("EVENTS");

            entity.Property(e => e.EventId).HasColumnName("EVENT_ID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.EventDate).HasColumnName("EVENT_DATE");
            entity.Property(e => e.EventTime)
                .HasPrecision(0)
                .HasColumnName("EVENT_TIME");
            entity.Property(e => e.IsMembersOnly).HasColumnName("IS_MEMBERS_ONLY");
            entity.Property(e => e.LocationId).HasColumnName("LOCATION_ID");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TITLE");
            entity.Property(e => e.Venue)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("VENUE");

            entity.HasOne(d => d.Location).WithMany(p => p.Events)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_EVENTS_LOCATIONS");

            entity.HasOne(d => d.Society).WithMany(p => p.Events)
                .HasForeignKey(d => d.SocietyId)
                .HasConstraintName("FK_EVENTS_SOCIETIES");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__EXPENSES__1D650492F92D7AC6");

            entity.ToTable("EXPENSES");

            entity.Property(e => e.ExpenseId).HasColumnName("EXPENSE_ID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.DateRecorded).HasColumnName("DATE_RECORDED");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");

            entity.HasOne(d => d.Society).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EXPENSES_SOCIETIES");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__FACULTIE__6755FF5BD13B45BC");

            entity.ToTable("FACULTIES");

            entity.HasIndex(e => e.FacultyName, "UQ__FACULTIE__5106EC22C0E66926").IsUnique();

            entity.Property(e => e.FacultyId).HasColumnName("FACULTY_ID");
            entity.Property(e => e.FacultyName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FACULTY_NAME");
            entity.Property(e => e.Univid).HasColumnName("UNIVID");

            entity.HasOne(d => d.Univ).WithMany(p => p.Faculties)
                .HasForeignKey(d => d.Univid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_FACULTIES_UNIVERSITIES");
        });

        modelBuilder.Entity<Itadmin>(entity =>
        {
            entity.HasKey(e => e.ItadminId).HasName("PK__ITADMINS__F12FD3772FFD92C2");

            entity.ToTable("ITADMINS");

            entity.HasIndex(e => e.PersonId, "UQ__ITADMINS__79BC545F74C6AA5B").IsUnique();

            entity.Property(e => e.ItadminId).HasColumnName("ITADMIN_ID");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");

            entity.HasOne(d => d.Person).WithOne(p => p.Itadmin)
                .HasForeignKey<Itadmin>(d => d.PersonId)
                .HasConstraintName("FK_ITADMINS_PERSONS");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__LOCATION__D2263B8E02A2FA36");

            entity.ToTable("LOCATIONS");

            entity.Property(e => e.LocationId).HasColumnName("LOCATION_ID");
            entity.Property(e => e.CityId).HasColumnName("CITY_ID");
            entity.Property(e => e.MapUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MAP_URL");

            entity.HasOne(d => d.City).WithMany(p => p.Locations)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_LOCATIONS_CITIES");
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.HasKey(e => e.Meetid).HasName("PK__MEETINGS__DE27102DEEB9C14F");

            entity.ToTable("MEETINGS");

            entity.Property(e => e.Meetid).HasColumnName("MEETID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.MeetingTime)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MEETING_TIME");
            entity.Property(e => e.Schedule).HasColumnName("SCHEDULE");
            entity.Property(e => e.SecretaryId).HasColumnName("SECRETARY_ID");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TITLE");

            entity.HasOne(d => d.Secretary).WithMany(p => p.Meetings)
                .HasForeignKey(d => d.SecretaryId)
                .HasConstraintName("FK_MEETINGS_SECRETARIES");

            entity.HasOne(d => d.Society).WithMany(p => p.Meetings)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MEETINGS_SOCIETIES");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PK__MEMBERSH__4019620781A1C7DC");

            entity.ToTable("MEMBERSHIPS");

            entity.HasIndex(e => new { e.StudentId, e.SocietyId }, "UQ_MEMBERSHIPS_STUDENT_SOCIETY").IsUnique();

            entity.Property(e => e.MembershipId).HasColumnName("MEMBERSHIP_ID");
            entity.Property(e => e.ReasonForJoining)
                .IsUnicode(false)
                .HasColumnName("REASON_FOR_JOINING");
            entity.Property(e => e.RequestDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_MEMBERSHIPS_REQUEST_DATE")
                .HasColumnName("REQUEST_DATE");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING", "DF_MEMBERSHIPS_STATUS")
                .HasColumnName("STATUS");
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");

            entity.HasOne(d => d.Society).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MEMBERSHIPS_SOCIETIES");

            entity.HasOne(d => d.Student).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MEMBERSHIPS_STUDENTS");
        });

        modelBuilder.Entity<PasswordResetOtp>(entity =>
        {
            entity.HasKey(e => e.OtpId).HasName("PK__PASSWORD__40A54F5301B3F682");

            entity.ToTable("PASSWORD_RESET_OTPS");

            entity.Property(e => e.OtpId).HasColumnName("OTP_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_PASSWORD_RESET_OTPS_CREATED_AT")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.ExpiresAt)
                .HasPrecision(0)
                .HasColumnName("EXPIRES_AT");
            entity.Property(e => e.IsUsed).HasColumnName("IS_USED");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("OTP_CODE");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");

            entity.HasOne(d => d.Person).WithMany(p => p.PasswordResetOtps)
                .HasForeignKey(d => d.PersonId)
                .HasConstraintName("FK_PASSWORD_RESET_OTPS_PERSONS");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__PERSONS__79BC545E1668338E");

            entity.ToTable("PERSONS");

            entity.HasIndex(e => e.Email, "UQ__PERSONS__161CF724E1791174").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__PERSONS__B15BE12E67B218E6").IsUnique();

            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_PERSONS_CREATED_AT")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FIRST_NAME");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true, "DF_PERSONS_IS_ACTIVE")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LAST_NAME");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD_HASH");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("USER_TYPE");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Provid).HasName("PK__PROVINCE__FC5A0A01F5CC1093");

            entity.ToTable("PROVINCES");

            entity.Property(e => e.Provid).HasColumnName("PROVID");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PROVINCE_NAME");
        });

        modelBuilder.Entity<Rsvp>(entity =>
        {
            entity.HasKey(e => e.RsvpId).HasName("PK__RSVPS__A42F8B46FFFC4F33");

            entity.ToTable("RSVPS");

            entity.HasIndex(e => new { e.EventId, e.PersonId }, "UQ_RSVPS_EVENT_PERSON").IsUnique();

            entity.Property(e => e.RsvpId).HasColumnName("RSVP_ID");
            entity.Property(e => e.EventId).HasColumnName("EVENT_ID");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.UserStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("NOT_GOING", "DF_RSVPS_USER_STATUS")
                .HasColumnName("USER_STATUS");

            entity.HasOne(d => d.Event).WithMany(p => p.Rsvps)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_RSVPS_EVENTS");

            entity.HasOne(d => d.Person).WithMany(p => p.Rsvps)
                .HasForeignKey(d => d.PersonId)
                .HasConstraintName("FK_RSVPS_PERSONS");
        });

        modelBuilder.Entity<Secretary>(entity =>
        {
            entity.HasKey(e => e.SecretaryId).HasName("PK__SECRETAR__E1595AC5B2A57BF3");

            entity.ToTable("SECRETARIES");

            entity.Property(e => e.SecretaryId).HasColumnName("SECRETARY_ID");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");

            entity.HasOne(d => d.Person).WithMany(p => p.Secretaries)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SECRETARIES_PERSONS");

            entity.HasOne(d => d.Society).WithMany(p => p.Secretaries)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SECRETARIES_SOCIETIES");
        });

        modelBuilder.Entity<Society>(entity =>
        {
            entity.HasKey(e => e.SocietyId).HasName("PK__SOCIETIE__B6B5B681D7C1EF48");

            entity.ToTable("SOCIETIES");

            entity.HasIndex(e => new { e.SocietyName, e.CampusId }, "UQ_SOCIETIES_NAME_CAMPUS").IsUnique();

            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");
            entity.Property(e => e.CampusId).HasColumnName("CAMPUS_ID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.SocietyName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SOCIETY_NAME");

            entity.HasOne(d => d.Campus).WithMany(p => p.Societies)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SOCIETIES_CAMPUSES");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__STUDENTS__E69FE77BDFB98049");

            entity.ToTable("STUDENTS");

            entity.HasIndex(e => e.StudentNumber, "UQ__STUDENTS__1500A8134232A334").IsUnique();

            entity.HasIndex(e => e.PersonId, "UQ__STUDENTS__79BC545F167F5135").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
            entity.Property(e => e.CampusId).HasColumnName("CAMPUS_ID");
            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.FacultyId).HasColumnName("FACULTY_ID");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.StudentNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STUDENT_NUMBER");
            entity.Property(e => e.YearOfStudy).HasColumnName("YEAR_OF_STUDY");

            entity.HasOne(d => d.Campus).WithMany(p => p.Students)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK_STUDENTS_CAMPUSES");

            entity.HasOne(d => d.CidNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Cid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_STUDENTS_CULTURES");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Students)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK_STUDENTS_FACULTIES");

            entity.HasOne(d => d.Person).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.PersonId)
                .HasConstraintName("FK_STUDENTS_PERSONS");
        });

        modelBuilder.Entity<StudentAffair>(entity =>
        {
            entity.HasKey(e => e.AffairId).HasName("PK__STUDENT___FC1B3A69DB6751B3");

            entity.ToTable("STUDENT_AFFAIRS");

            entity.HasIndex(e => e.PersonId, "UQ__STUDENT___79BC545F6D15D20C").IsUnique();

            entity.Property(e => e.AffairId).HasColumnName("AFFAIR_ID");
            entity.Property(e => e.CampusId).HasColumnName("CAMPUS_ID");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.Univid).HasColumnName("UNIVID");

            entity.HasOne(d => d.Campus).WithMany(p => p.StudentAffairs)
                .HasForeignKey(d => d.CampusId)
                .HasConstraintName("FK_STUDENT_AFFAIRS_CAMPUSES");

            entity.HasOne(d => d.Person).WithOne(p => p.StudentAffair)
                .HasForeignKey<StudentAffair>(d => d.PersonId)
                .HasConstraintName("FK_STUDENT_AFFAIRS_PERSONS");

            entity.HasOne(d => d.Univ).WithMany(p => p.StudentAffairs)
                .HasForeignKey(d => d.Univid)
                .HasConstraintName("FK_STUDENT_AFFAIRS_UNIVERSITIES");
        });

        modelBuilder.Entity<Treasurer>(entity =>
        {
            entity.HasKey(e => e.TreasurerId).HasName("PK__TREASURE__F6A3DFA8C6124A77");

            entity.ToTable("TREASURERS");

            entity.Property(e => e.TreasurerId).HasColumnName("TREASURER_ID");
            entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");
            entity.Property(e => e.SocietyId).HasColumnName("SOCIETY_ID");

            entity.HasOne(d => d.Person).WithMany(p => p.Treasurers)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TREASURERS_PERSONS");

            entity.HasOne(d => d.Society).WithMany(p => p.Treasurers)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TREASURERS_SOCIETIES");
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.HasKey(e => e.Univid).HasName("PK__UNIVERSI__4B2B3913AD606D9E");

            entity.ToTable("UNIVERSITIES");

            entity.Property(e => e.Univid).HasColumnName("UNIVID");
            entity.Property(e => e.StaffDomain)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STAFF_DOMAIN");
            entity.Property(e => e.StudentDomain)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STUDENT_DOMAIN");
            entity.Property(e => e.Uniname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UNINAME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
