﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace regMVC.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class inventarioEntitiesDBA : DbContext
    {
        public inventarioEntitiesDBA()
            : base("name=inventarioEntitiesDBA")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<bodega> bodega { get; set; }
        public virtual DbSet<mostrador> mostrador { get; set; }
        public virtual DbSet<productos> productos { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<economia> economia { get; set; }
        public virtual DbSet<caja> caja { get; set; }
    }
}
