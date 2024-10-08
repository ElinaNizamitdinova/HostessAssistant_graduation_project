﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace PostGreContext.Models;

public partial class OrderDb
{
    public int OrderId { get; set; }

    public int WorkShiftId { get; set; }

    public int UserId { get; set; }

    public int? ReservationId { get; set; }

    public int OrderStatusId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public bool? IsPriority { get; set; }

    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    public virtual Reservation? Reservation { get; set; }

    public virtual UserDb User { get; set; } = null!;

    public virtual WorkShift WorkShift { get; set; } = null!;
}