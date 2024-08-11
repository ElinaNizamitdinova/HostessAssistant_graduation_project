﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostGreContext.Context;
using PostGreContext.Models;
using System;
using System.Collections.Generic;

#nullable disable

namespace PostGreContext.Context.Configurations
{
    public partial class QeueConfiguration : IEntityTypeConfiguration<Qeue>
    {
        public void Configure(EntityTypeBuilder<Qeue> entity)
        {
            entity.HasKey(e => e.QeueId).HasName("qeue_pkey");

            entity.ToTable("qeue", "queues");

            entity.Property(e => e.QeueId).HasColumnName("qeue_id");
            entity.Property(e => e.EndDateTime).HasColumnName("end_date_time");
            entity.Property(e => e.OrderSequence).HasColumnName("order_sequence");
            entity.Property(e => e.PriorityOrderSequence).HasColumnName("priority_order_sequence");
            entity.Property(e => e.QeueStatusId).HasColumnName("qeue_status_id");
            entity.Property(e => e.StartDateTime).HasColumnName("start_date_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkShiftId).HasColumnName("work_shift_id");

            entity.HasOne(d => d.User).WithMany(p => p.Qeues)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("qeue_user_id_fkey");

            entity.HasOne(d => d.WorkShift).WithMany(p => p.Qeues)
                .HasForeignKey(d => d.WorkShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("qeue_work_shift_id_fkey");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Qeue> entity);
    }
}