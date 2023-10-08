﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Why.Models;

namespace Why.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Post> Posts { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Post>().HasIndex(p => p.Date);
    }
}
