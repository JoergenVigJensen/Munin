﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Munin.web.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ILABNewEntities2 : DbContext
    {
        public ILABNewEntities2()
            : base("name=ILABNew2Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Arkivalie> Arkivalie { get; set; }
        public virtual DbSet<ArkivFond> ArkivFond { get; set; }
        public virtual DbSet<Aviser> Aviser { get; set; }
        public virtual DbSet<Bibliotek> Bibliotek { get; set; }
        public virtual DbSet<Billeder> Billeder { get; set; }
        public virtual DbSet<Giver> Giver { get; set; }
        public virtual DbSet<Journaler> Journaler { get; set; }
        public virtual DbSet<Kirkebog> Kirkebog { get; set; }
        public virtual DbSet<Kort> Kort { get; set; }
        public virtual DbSet<Matrikler> Matrikler { get; set; }
        public virtual DbSet<Sekvenser> Sekvenser { get; set; }
        public virtual DbSet<Udklip> udklip { get; set; }
        public virtual DbSet<UserTbl> UserTbl { get; set; }
    }
}
