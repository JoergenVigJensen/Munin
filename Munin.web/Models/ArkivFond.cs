//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ArkivFond
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ArkivFond()
        {
            this.Arkivalie = new HashSet<Arkivalie>();
        }
    
        public int ArkivfondID { get; set; }
        public string ArkivNr { get; set; }
        public string ArkivNavn { get; set; }
        public string ArkivDef { get; set; }
        public string ArkivType { get; set; }
        public Nullable<double> StiftetDay { get; set; }
        public Nullable<double> StiftetMonth { get; set; }
        public Nullable<double> StiftetYear { get; set; }
        public Nullable<double> AfsluttetDay { get; set; }
        public Nullable<double> AfsluttetMonth { get; set; }
        public Nullable<double> AfsluttetYear { get; set; }
        public string Adresse { get; set; }
        public string Postnr { get; set; }
        public string By { get; set; }
        public string Note { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Arkivalie> Arkivalie { get; set; }
    }
}