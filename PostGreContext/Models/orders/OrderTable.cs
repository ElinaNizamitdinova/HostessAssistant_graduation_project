﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace PostGreContext.Models;

public partial class OrderTable
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? TableId { get; set; }

    public virtual OrderDb? Order { get; set; }

    public virtual TableDb? Table { get; set; }
}