﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using PostGreContext.Context.Configurations;
using PostGreContext.Models;
using System;
using System.Collections.Generic;
#nullable enable

namespace PostGreContext.Context;

public partial class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OrderDb> OrderDbs { get; set; }

    public virtual DbSet<OrderTable> OrderTables { get; set; }

    public virtual DbSet<Qeue> Qeues { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationTable> ReservationTables { get; set; }

    public virtual DbSet<TableDb> TableDbs { get; set; }

    public virtual DbSet<UserDb> UserDbs { get; set; }

    public virtual DbSet<WorkShift> WorkShifts { get; set; }

    public virtual DbSet<WorkShiftUser> WorkShiftUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.OrderDbConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrderTableConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.QeueConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.ReservationConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.ReservationTableConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TableDbConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.UserDbConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.WorkShiftConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.WorkShiftUserConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
