﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TriggerSheets.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QSEntities1 : DbContext
    {
        public QSEntities1()
            : base("name=QSEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Answers_tbl> Answers_tbl { get; set; }
        public virtual DbSet<Questions_tbl> Questions_tbl { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Triggers_tbl> Triggers_tbl { get; set; }
        public virtual DbSet<User_Line> User_Line { get; set; }
        public virtual DbSet<Summary_tbl> Summary_tbl { get; set; }
    }
}
