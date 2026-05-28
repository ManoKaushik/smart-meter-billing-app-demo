using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using SmartMeterWeb.Data.Entities;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SmartMeterWeb.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<TodRule> TodRules { get; set; }
        public DbSet<TariffSlab> TariffSlabs { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Arrear> Arrears { get; set; }
        public DbSet<TariffDetails> TariffDetails { get; set; }
        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<CustomerCareMessage> CustomerCareMessages { get; set; }
        public DbSet<SolarMeterReading> SolarMeterReadings { get; set; }

        public DbSet<CustomerCareReply> CustomerCareReplies { get; set; }


        //To update database after making any changes in the classes, run these

        // Add-Migration <Update migration name>
        // Update-Database

        //in package manager console


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasSequence<int>("MeterSeq")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.Entity<Meter>(entity =>
            {
                entity.Property(m => m.MeterSerialNo)
                    .HasDefaultValueSql("'SM' || LPAD(nextval('\"MeterSeq\"')::text, 5, '0')");
            });

            modelBuilder.Entity<OrgUnit>()
                .ToTable(t => t.HasCheckConstraint("CK_Type", "\"Type\" IN ('Zone','Substation','Feeder','DTR')"));

            modelBuilder.Entity<OrgUnit>()
                .HasIndex(l => l.Type);

            modelBuilder.Entity<Tariff>()
                .ToTable(t => t.HasCheckConstraint("CK_Tariff_Dates", "\"EffectiveTo\" IS NULL OR \"EffectiveFrom\" < \"EffectiveTo\""));

            modelBuilder.Entity<Tariff>()
                .ToTable(t => t.HasCheckConstraint("CK_Tariff_BaseRate", "\"BaseRate\" > 0"));

            modelBuilder.Entity<TodRule>()
                .ToTable(t => t.HasCheckConstraint("CK_TodRule_Time", "\"EndTime\" > \"StartTime\""));

            modelBuilder.Entity<TodRule>()
                .ToTable(t => t.HasCheckConstraint("CK_TodRule_Rate", "\"RatePerKwh\" > 0"));

            modelBuilder.Entity<TodRule>()
                .HasIndex(l => l.Name);

            modelBuilder.Entity<TariffSlab>()
                .ToTable(t => t.HasCheckConstraint("CK_TariffSlab_Range", "\"FromKwh\" >= 0 AND \"ToKwh\" > \"FromKwh\""));

            modelBuilder.Entity<TariffSlab>()
                .ToTable(t => t.HasCheckConstraint("CK_TariffSlab_Rate", "\"RatePerKwh\" > 0"));

            modelBuilder.Entity<Consumer>()
                .ToTable(t => t.HasCheckConstraint("CK_Consumer_timestamp", "\"UpdatedAt\" IS NULL OR \"UpdatedAt\" > \"CreatedAt\""));

            modelBuilder.Entity<Consumer>()
                .HasIndex(l => l.Name);

            modelBuilder.Entity<Meter>()
                .ToTable(t => t.HasCheckConstraint("CK_Meter_Status", "\"Status\" IN ('Active','Inactive','Decommissioned')"));

            modelBuilder.Entity<Arrear>()
                .ToTable(t => t.HasCheckConstraint("CK_Arrear_Type", "\"ArrearType\" IN ('Overdue','Penalty','Interest')"));

            modelBuilder.Entity<Arrear>()
                .ToTable(t => t.HasCheckConstraint("CK_Arrear_Amount", "\"Amount\" >= 0"));

            modelBuilder.Entity<Arrear>()
                .ToTable(t => t.HasCheckConstraint("CK_Arrear_PaidStatus", "\"PaidStatus\" IN ('Paid','Unpaid','Partially Paid')"));

            modelBuilder.Entity<Billing>()
                .Property(i => i.TotalAmount)
                .HasComputedColumnSql("\"BaseAmount\" + \"TaxAmount\"", stored: true);

            modelBuilder.Entity<Billing>()
                .ToTable(t => t.HasCheckConstraint("CK_Billings_PaidStatus", "\"PaymentStatus\" IN ('Paid','Unpaid','Overdue','Cancelled','Partially-Paid')"));

            modelBuilder.Entity<Billing>().HasData(
    new Billing
    {
        BillId = 1,
        ConsumerId = 1,
        MeterId = "GN24A00187",
        BillingPeriodStart = new DateOnly(2025, 8, 1),
        BillingPeriodEnd = new DateOnly(2025, 9, 1),
        TotalUnitsConsumed = 175.3,
        BaseAmount = 901.80,
        TaxAmount = 90.18,
        GeneratedAt = new DateTime(2025, 9, 1, 5, 0, 0, DateTimeKind.Utc),
        DueDate = new DateOnly(2025, 9, 15),
        PaidDate = new DateTime(2025, 9, 10, 10, 30, 0, DateTimeKind.Utc),
        PaymentStatus = "Paid",
        DisconnectionDate = null
    },
    new Billing
    {
        BillId = 2,
        ConsumerId = 2,
        MeterId = "LT24C00255",
        BillingPeriodStart = new DateOnly(2025, 8, 1),
        BillingPeriodEnd = new DateOnly(2025, 9, 1),
        TotalUnitsConsumed = 910.5,
        BaseAmount = 7296.12,
        TaxAmount = 875.53,
        GeneratedAt = new DateTime(2025, 9, 1, 5, 0, 0, DateTimeKind.Utc),
        DueDate = new DateOnly(2025, 9, 15),
        PaidDate = new DateTime(2025, 9, 8, 14, 0, 0, DateTimeKind.Utc),
        PaymentStatus = "Paid",
        DisconnectionDate = null
    },
    new Billing
    {
        BillId = 3,
        ConsumerId = 3,
        MeterId = "GN24A00193",
        BillingPeriodStart = new DateOnly(2025, 8, 1),
        BillingPeriodEnd = new DateOnly(2025, 9, 1),
        TotalUnitsConsumed = 225.2,
        BaseAmount = 1264.20,
        TaxAmount = 126.42,
        GeneratedAt = new DateTime(2025, 9, 1, 5, 0, 0, DateTimeKind.Utc),
        DueDate = new DateOnly(2025, 9, 15),
        PaidDate = new DateTime(2025, 9, 12, 11, 45, 0, DateTimeKind.Utc),
        PaymentStatus = "Paid",
        DisconnectionDate = null
    },
    new Billing
    {
        BillId = 4,
        ConsumerId = 4,
        MeterId = "HP24E00301",
        BillingPeriodStart = new DateOnly(2025, 8, 1),
        BillingPeriodEnd = new DateOnly(2025, 9, 1),
        TotalUnitsConsumed = 154.5,
        BaseAmount = 966.75,
        TaxAmount = 77.34,
        GeneratedAt = new DateTime(2025, 9, 1, 5, 0, 0, DateTimeKind.Utc),
        DueDate = new DateOnly(2025, 9, 15),
        PaidDate = new DateTime(2025, 9, 7, 16, 20, 0, DateTimeKind.Utc),
        PaymentStatus = "Paid",
        DisconnectionDate = null
    },
    new Billing
    {
        BillId = 5,
        ConsumerId = 5,
        MeterId = "LT24I00419",
        BillingPeriodStart = new DateOnly(2025, 8, 1),
        BillingPeriodEnd = new DateOnly(2025, 9, 1),
        TotalUnitsConsumed = 12221.3,
        BaseAmount = 121655.60,
        TaxAmount = 18248.34,
        GeneratedAt = new DateTime(2025, 9, 1, 5, 0, 0, DateTimeKind.Utc),
        DueDate = new DateOnly(2025, 9, 15),
        PaidDate = new DateTime(2025, 9, 5, 9, 15, 0, DateTimeKind.Utc),
        PaymentStatus = "Paid",
        DisconnectionDate = null
    }
);



            modelBuilder.Entity<OrgUnit>().HasData(
                new OrgUnit
                {
                    OrgUnitId = 1,
                    Type = "Zone",
                    Name = "North Zone",
                    ParentId = null
                },
                new OrgUnit
                {
                    OrgUnitId = 2,
                    Type = "Substation",
                    Name = "Maplewood Substation",
                    ParentId = 1
                },
                new OrgUnit
                {
                    OrgUnitId = 3,
                    Type = "Feeder",
                    Name = "Feeder F-11",
                    ParentId = 2
                },
                new OrgUnit
                {
                    OrgUnitId = 4,
                    Type = "Feeder",
                    Name = "Feeder F-12",
                    ParentId = 2
                },
                new OrgUnit
                {
                    OrgUnitId = 5,
                    Type = "DTR",
                    Name = "DTR-11045",
                    ParentId = 3
                },
                new OrgUnit
                {
                    OrgUnitId = 6,
                    Type = "DTR",
                    Name = "DTR-12001",
                    ParentId = 4
                }

            );
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    UserName = "admin",
                    PasswordHash = "$2a$11$nG48 / pPMTnCvehLK.ebbXeEXvw5XqJwPLkHPsH5Cuc2466gvIrP8C",   //Admin123
                    DisplayName = "System Administrator",
                    Email = "admin@smartmeter.com",
                    Phone = "9876543210",
                    IsActive = true
                },
                new User
                {
                    UserId = 2,
                    UserName = "ganesh",
                    PasswordHash = "$2a$11$nlUMkcxzYytmlcZWNuDQ0enFXhKDKD9gAHtr45pHlKA.eaPJ9Vi/.",  //gnhes
                    DisplayName = "Ganesh",
                    Email = "ganesh@smartmeter.com",
                    Phone = "9123456780",
                    IsActive = true
                },
                new User
                {
                    UserId = 3,
                    UserName = "support1",
                    PasswordHash = "$2a$11$6EK8mbMOJbJLh0adk1hiQeWeij8lW3OeEeOHyB4/aRRp4Tv1nxCfS",   //support101
                    DisplayName = "Support Engineer",
                    Email = "support1@smartmeter.com",
                    Phone = "9998887770",
                    IsActive = true
                },
                new User
                {
                    UserId = 4,
                    UserName = "auditor",
                    PasswordHash = "$2a$11$U1yXqZUuWV3AoF8Nv78NlePvk7.65UckxVZQ3Nn3qoSdtjpkL9Nn6",    //audit101
                    DisplayName = "Energy Auditor",
                    Email = "auditor@smartmeter.com",
                    Phone = "9988776655",
                    IsActive = true
                },
                new User
                {
                    UserId = 5,
                    UserName = "viewer",
                    PasswordHash = "$2a$11$4A4WI6j0RrYKDbl6ZAmhze6JNH..fZL98NexIsYTAR/Jk9JtFoZv6",       //viewer101
                    DisplayName = "Viewer Account",
                    Email = "viewer@smartmeter.com",
                    Phone = "9876501234",
                    IsActive = true
                }
            );
            modelBuilder.Entity<Tariff>().HasData(
    new Tariff
    {
        TariffId = 1,
        Name = "Residential Basic",
        EffectiveFrom = new DateOnly(2024, 1, 1),
        EffectiveTo = null,
        BaseRate = 90,
        TaxRate = 0.10
    },
    new Tariff
    {
        TariffId = 2,
        Name = "Commercial Standard",
        EffectiveFrom = new DateOnly(2024, 1, 1),
        EffectiveTo = null,
        BaseRate = 150,
        TaxRate = 0.12
    },
    new Tariff
    {
        TariffId = 3,
        Name = "Industrial High-Load",
        EffectiveFrom = new DateOnly(2024, 1, 1),
        EffectiveTo = null,
        BaseRate = 700,
        TaxRate = 0.15
    },
    new Tariff
    {
        TariffId = 4,
        Name = "Agricultural Subsidized",
        EffectiveFrom = new DateOnly(2024, 1, 1),
        EffectiveTo = null,
        BaseRate = 60,
        TaxRate = 0.05
    },
    new Tariff
    {
        TariffId = 5,
        Name = "EV Charging Tariff",
        EffectiveFrom = new DateOnly(2024, 6, 1),
        EffectiveTo = null,
        BaseRate = 120,
        TaxRate = 0.08
    }
);

            modelBuilder.Entity<Consumer>().HasData(
    new Consumer
    {
        ConsumerId = 1,
        Name = "Priya Sharma",
        Phone = "9876543211",
        Email = "priya.s@example.com",
        OrgUnitId = 5, // DTR-11045
        TariffId = 1,  // Residential Basic
        Status = "Active",
        PasswordHash = "$2a$11$pXHOt/8COqUisMbyPjx7Euy2y3myYS9AukeD6jNo91NtYXmKy.k4W" // consumer123
    },
    new Consumer
    {
        ConsumerId = 2,
        Name = "Rohan Kumar",
        Phone = "9876543212",
        Email = "rohan.k@example.com",
        OrgUnitId = 5, // DTR-11045
        TariffId = 2,  // Commercial Standard
        Status = "Active",
        PasswordHash = "$2a$11$pXHOt/8COqUisMbyPjx7Euy2y3myYS9AukeD6jNo91NtYXmKy.k4W" // consumer123
    },
    new Consumer
    {
        ConsumerId = 3,
        Name = "Vikram Singh",
        Phone = "9876543213",
        Email = "vikram.s@example.com",
        OrgUnitId = 6, // DTR-12001
        TariffId = 1,  // Residential Basic
        Status = "Active",
        PasswordHash = "$2a$11$pXHOt/8COqUisMbyPjx7Euy2y3myYS9AukeD6jNo91NtYXmKy.k4W" // consumer123
    },
    new Consumer
    {
        ConsumerId = 4,
        Name = "Anjali Devi",
        Phone = "9876543214",
        Email = "anjali.d@example.com",
        OrgUnitId = 6, // DTR-12001
        TariffId = 5,  // EV Charging Tariff
        Status = "Active",
        PasswordHash = "$2a$11$pXHOt/8COqUisMbyPjx7Euy2y3myYS9AukeD6jNo91NtYXmKy.k4W" // consumer123
    },
    new Consumer
    {
        ConsumerId = 5,
        Name = "Gupta Industries",
        Phone = "9876543215",
        Email = "contact@guptaindustries.com",
        OrgUnitId = 5, // DTR-11045
        TariffId = 3,  // Industrial High-Load
        Status = "Active",
        PasswordHash = "$2a$11$pXHOt/8COqUisMbyPjx7Euy2y3myYS9AukeD6jNo91NtYXmKy.k4W" // consumer123
    }
);

            modelBuilder.Entity<TariffSlab>().HasData(
    // Slabs for TariffId = 1 (Residential Basic)
    new TariffSlab { TariffSlabId = 1, TariffId = 1, FromKwh = 0, ToKwh = 100, RatePerKwh = 4.50 },
    new TariffSlab { TariffSlabId = 2, TariffId = 1, FromKwh = 100.000001, ToKwh = 200, RatePerKwh = 6.00 },
    new TariffSlab { TariffSlabId = 3, TariffId = 1, FromKwh = 200.000001, ToKwh = 500, RatePerKwh = 8.50 },

    // Slabs for TariffId = 2 (Commercial Standard)
    new TariffSlab { TariffSlabId = 4, TariffId = 2, FromKwh = 0, ToKwh = 500, RatePerKwh = 7.00 },
    new TariffSlab { TariffSlabId = 5, TariffId = 2, FromKwh = 500.000001, ToKwh = 1000, RatePerKwh = 9.25 },
    new TariffSlab { TariffSlabId = 6, TariffId = 2, FromKwh = 1000.000001, ToKwh = 5000, RatePerKwh = 11.00 },

    // Slabs for TariffId = 3 (Industrial High-Load)
    new TariffSlab { TariffSlabId = 7, TariffId = 3, FromKwh = 0, ToKwh = 10000, RatePerKwh = 9.50 },
    new TariffSlab { TariffSlabId = 8, TariffId = 3, FromKwh = 10000.000001, ToKwh = 100000, RatePerKwh = 12.00 },

    // Slabs for TariffId = 4 (Agricultural Subsidized)
    new TariffSlab { TariffSlabId = 9, TariffId = 4, FromKwh = 0, ToKwh = 300, RatePerKwh = 2.50 },
    new TariffSlab { TariffSlabId = 10, TariffId = 4, FromKwh = 300.000001, ToKwh = 1000, RatePerKwh = 3.50 },

    // Slabs for TariffId = 5 (EV Charging Tariff)
    new TariffSlab { TariffSlabId = 11, TariffId = 5, FromKwh = 0, ToKwh = 50, RatePerKwh = 5.75 },
    new TariffSlab { TariffSlabId = 12, TariffId = 5, FromKwh = 50.000001, ToKwh = 200, RatePerKwh = 6.50 }
);

            modelBuilder.Entity<TodRule>().HasData(
    // TOD Rules for TariffId = 2 (Commercial Standard)
    new TodRule 
    { 
        TodRuleId = 1, 
        TariffId = 2, 
        Name = "Peak Hours", 
        StartTime = new TimeOnly(18, 0, 0), // 6:00 PM
        EndTime = new TimeOnly(22, 0, 0),   // 10:00 PM
        RatePerKwh = 10.50 
    },
    // SPLIT RULE: Off-Peak from 22:00 to 06:00 is now two parts
    new TodRule 
    { 
        TodRuleId = 2, 
        TariffId = 2, 
        Name = "Off-Peak Hours (Evening)", 
        StartTime = new TimeOnly(22, 0, 0),   // 10:00 PM
        EndTime = new TimeOnly(23, 59, 59),   // 11:59:59 PM
        RatePerKwh = 5.50 
    },
    new TodRule 
    { 
        TodRuleId = 8, // New unique ID
        TariffId = 2, 
        Name = "Off-Peak Hours (Morning)", 
        StartTime = new TimeOnly(0, 0, 0),    // 12:00:00 AM
        EndTime = new TimeOnly(6, 0, 0),      // 6:00 AM
        RatePerKwh = 5.50 
    },
    new TodRule 
    { 
        TodRuleId = 3, 
        TariffId = 2, 
        Name = "Standard Hours", 
        StartTime = new TimeOnly(6, 0, 0),  // 6:00 AM
        EndTime = new TimeOnly(18, 0, 0),   // 6:00 PM
        RatePerKwh = 7.50 
    },

    // TOD Rules for TariffId = 3 (Industrial High-Load)
    new TodRule 
    { 
        TodRuleId = 4, 
        TariffId = 3, 
        Name = "Industrial Peak", 
        StartTime = new TimeOnly(17, 0, 0), // 5:00 PM
        EndTime = new TimeOnly(23, 0, 0),   // 11:00 PM
        RatePerKwh = 13.00 
    },
    // SPLIT RULE: Industrial Off-Peak from 23:00 to 05:00 is now two parts
    new TodRule 
    { 
        TodRuleId = 5, 
        TariffId = 3, 
        Name = "Industrial Off-Peak (Evening)", 
        StartTime = new TimeOnly(23, 0, 0),   // 11:00 PM
        EndTime = new TimeOnly(23, 59, 59),   // 11:59:59 PM
        RatePerKwh = 7.00 
    },
    new TodRule
    {
        TodRuleId = 9, // New unique ID
        TariffId = 3,
        Name = "Industrial Off-Peak (Morning)",
        StartTime = new TimeOnly(0, 0, 0),    // 12:00:00 AM
        EndTime = new TimeOnly(5, 0, 0),      // 5:00 AM
        RatePerKwh = 7.00
    },

    // TOD Rules for TariffId = 5 (EV Charging Tariff)
    // SPLIT RULE: EV Super Off-Peak from 23:00 to 05:00 is now two parts
    new TodRule 
    { 
        TodRuleId = 6, 
        TariffId = 5, 
        Name = "EV Super Off-Peak (Evening)", 
        StartTime = new TimeOnly(23, 0, 0),   // 11:00 PM
        EndTime = new TimeOnly(23, 59, 59),   // 11:59:59 PM
        RatePerKwh = 4.50 
    },
    new TodRule
    {
        TodRuleId = 10, // New unique ID
        TariffId = 5,
        Name = "EV Super Off-Peak (Morning)",
        StartTime = new TimeOnly(0, 0, 0),    // 12:00:00 AM
        EndTime = new TimeOnly(5, 0, 0),      // 5:00 AM
        RatePerKwh = 4.50
    },
    new TodRule 
    { 
        TodRuleId = 7, 
        TariffId = 5, 
        Name = "EV Peak", 
        StartTime = new TimeOnly(19, 0, 0), // 7:00 PM
        EndTime = new TimeOnly(22, 0, 0),   // 10:00 PM
        RatePerKwh = 8.00 
    }
);
            modelBuilder.Entity<TariffDetails>().HasData(
    // Mappings for TariffSlabs (IDs 1 through 12)
    new TariffDetails { TariffDetailsId = 1, TariffSlabId = 1, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 2, TariffSlabId = 2, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 3, TariffSlabId = 3, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 4, TariffSlabId = 4, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 5, TariffSlabId = 5, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 6, TariffSlabId = 6, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 7, TariffSlabId = 7, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 8, TariffSlabId = 8, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 9, TariffSlabId = 9, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 10, TariffSlabId = 10, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 11, TariffSlabId = 11, TodRuleId = null },
    new TariffDetails { TariffDetailsId = 12, TariffSlabId = 12, TodRuleId = null },

    // Mappings for TodRules (IDs 1 through 10)
    new TariffDetails { TariffDetailsId = 13, TariffSlabId = null, TodRuleId = 1 },
    new TariffDetails { TariffDetailsId = 14, TariffSlabId = null, TodRuleId = 2 },
    new TariffDetails { TariffDetailsId = 15, TariffSlabId = null, TodRuleId = 3 },
    new TariffDetails { TariffDetailsId = 16, TariffSlabId = null, TodRuleId = 4 },
    new TariffDetails { TariffDetailsId = 17, TariffSlabId = null, TodRuleId = 5 },
    new TariffDetails { TariffDetailsId = 18, TariffSlabId = null, TodRuleId = 6 },
    new TariffDetails { TariffDetailsId = 19, TariffSlabId = null, TodRuleId = 7 },
    new TariffDetails { TariffDetailsId = 20, TariffSlabId = null, TodRuleId = 8 },
    new TariffDetails { TariffDetailsId = 21, TariffSlabId = null, TodRuleId = 9 },
    new TariffDetails { TariffDetailsId = 22, TariffSlabId = null, TodRuleId = 10 }
);
            modelBuilder.Entity<Meter>().HasData(
    new Meter
    {
        MeterSerialNo = "GN24A00187",
        IpAddress = "192.168.1.101",
        ICCID = "899110234567890123A",
        IMSI = "404112345678901",
        Manufacturer = "Genus Power",
        Firmware = "v2.1.3",
        Category = "Single Phase",
        InstallTsUtc = new DateTimeOffset(2024, 2, 15, 10, 30, 0, TimeSpan.Zero),
        Status = "Active",
        ConsumerId = 1
    },
    new Meter
    {
        MeterSerialNo = "LT24C00255",
        IpAddress = "192.168.1.102",
        ICCID = "899110234567890123B",
        IMSI = "404112345678902",
        Manufacturer = "L&T Electrical",
        Firmware = "v4.5.1",
        Category = "Three Phase",
        InstallTsUtc = new DateTimeOffset(2024, 3, 20, 11, 0, 0, TimeSpan.Zero),
        Status = "Active",
        ConsumerId = 2
    },
    new Meter
    {
        MeterSerialNo = "GN24A00193",
        IpAddress = "192.168.2.101",
        ICCID = "899110234567890123C",
        IMSI = "404112345678903",
        Manufacturer = "Genus Power",
        Firmware = "v2.1.5",
        Category = "Single Phase",
        InstallTsUtc = new DateTimeOffset(2024, 4, 1, 14, 0, 0, TimeSpan.Zero),
        Status = "Active",
        ConsumerId = 3
    },
    new Meter
    {
        MeterSerialNo = "HP24E00301",
        IpAddress = "192.168.2.102",
        ICCID = "899110234567890123D",
        IMSI = "404112345678904",
        Manufacturer = "HPL Electric",
        Firmware = "v1.8.0",
        Category = "Single Phase EV",
        InstallTsUtc = new DateTimeOffset(2024, 6, 5, 9, 45, 0, TimeSpan.Zero),
        Status = "Active",
        ConsumerId = 4
    },
    new Meter
    {
        MeterSerialNo = "LT24I00419",
        IpAddress = "192.168.1.103",
        ICCID = "899110234567890123E",
        IMSI = "404112345678905",
        Manufacturer = "L&T Electrical",
        Firmware = "v5.0.2-beta",
        Category = "Three Phase CT",
        InstallTsUtc = new DateTimeOffset(2024, 1, 25, 15, 10, 0, TimeSpan.Zero),
        Status = "Active",
        ConsumerId = 5
    },
    new Meter
    {
        MeterSerialNo = "SC23S00078",
        IpAddress = "10.0.0.10",
        ICCID = "899110234567890123F",
        IMSI = "404112345678906",
        Manufacturer = "Secure Meters",
        Firmware = "v3.3.1",
        Category = "Three Phase",
        InstallTsUtc = new DateTimeOffset(2023, 11, 10, 12, 0, 0, TimeSpan.Zero),
        Status = "Inactive", // This meter is in inventory, not assigned
        ConsumerId = null
    }
);

            modelBuilder.Entity<MeterReading>().HasData(
    // Readings for Meter 1 (GN24A00187 - Residential)

    new MeterReading
    {
        Id = 1,
        MeterId = "GN24A00187",
        ReadingDateTime = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 231.5,
        Current = 5.2,
        PowerFactor = 0.98,
        EnergyConsumed = 1540.5
    },
    new MeterReading
    {
        Id = 2,
        MeterId = "GN24A00187",
        ReadingDateTime = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 229.8,
        Current = 5.5,
        PowerFactor = 0.99,
        EnergyConsumed = 1715.8
    },
    new MeterReading
    {
        Id = 3,
        MeterId = "GN24A00187",
        ReadingDateTime = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 230.1,
        Current = 5.3,
        PowerFactor = 0.98,
        EnergyConsumed = 1898.2
    },

    // Readings for Meter 2 (LT24C00255 - Commercial)
    new MeterReading
    {
        Id = 4,
        MeterId = "LT24C00255",
        ReadingDateTime = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 235.2,
        Current = 15.8,
        PowerFactor = 0.95,
        EnergyConsumed = 8550.2
    },
    new MeterReading
    {
        Id = 5,
        MeterId = "LT24C00255",
        ReadingDateTime = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 234.9,
        Current = 16.1,
        PowerFactor = 0.96,
        EnergyConsumed = 9460.7
    },
    new MeterReading
    {
        Id = 6,
        MeterId = "LT24C00255",
        ReadingDateTime = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 235.5,
        Current = 15.9,
        PowerFactor = 0.95,
        EnergyConsumed = 10395.1
    },

    // Readings for Meter 3 (GN24A00193 - Residential)
    new MeterReading
    {
        Id = 7,
        MeterId = "GN24A00193",
        ReadingDateTime = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 228.7,
        Current = 6.1,
        PowerFactor = 0.99,
        EnergyConsumed = 1210.3
    },
    new MeterReading
    {
        Id = 8,
        MeterId = "GN24A00193",
        ReadingDateTime = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 229.1,
        Current = 6.4,
        PowerFactor = 0.99,
        EnergyConsumed = 1435.5
    },
    new MeterReading
    {
        Id = 9,
        MeterId = "GN24A00193",
        ReadingDateTime = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 228.9,
        Current = 6.2,
        PowerFactor = 0.98,
        EnergyConsumed = 1655.9
    },

    // Readings for Meter 4 (HP24E00301 - EV Charging)
    new MeterReading
    {
        Id = 10,
        MeterId = "HP24E00301",
        ReadingDateTime = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 230.5,
        Current = 4.5,
        PowerFactor = 0.99,
        EnergyConsumed = 525.6
    },
    new MeterReading
    {
        Id = 11,
        MeterId = "HP24E00301",
        ReadingDateTime = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 231.2,
        Current = 4.8,
        PowerFactor = 0.99,
        EnergyConsumed = 680.1
    },
    new MeterReading
    {
        Id = 12,
        MeterId = "HP24E00301",
        ReadingDateTime = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 230.8,
        Current = 4.6,
        PowerFactor = 0.99,
        EnergyConsumed = 840.3
    },

    // Readings for Meter 5 (LT24I00419 - Industrial)
    new MeterReading
    {
        Id = 13,
        MeterId = "LT24I00419",
        ReadingDateTime = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 414.5,
        Current = 55.2,
        PowerFactor = 0.92,
        EnergyConsumed = 156234.8
    },
    new MeterReading
    {
        Id = 14,
        MeterId = "LT24I00419",
        ReadingDateTime = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 415.1,
        Current = 58.1,
        PowerFactor = 0.93,
        EnergyConsumed = 168456.1
    },
    new MeterReading
    {
        Id = 15,
        MeterId = "LT24I00419",
        ReadingDateTime = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
        Voltage = 413.9,
        Current = 56.5,
        PowerFactor = 0.92,
        EnergyConsumed = 181050.4
    }

);
        }
    }
}
