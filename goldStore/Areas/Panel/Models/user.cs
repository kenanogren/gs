//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace goldStore.Areas.Panel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class user
    {
        public int userId { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string rePassword { get; set; }
        public string phone { get; set; }
        public string activationCode { get; set; }
        public string resetCode { get; set; }
        public string hostName { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<int> loginAttempt { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public Nullable<System.DateTime> loginTime { get; set; }
        public Nullable<bool> isMailVerified { get; set; }
        public Nullable<int> roleId { get; set; }
    
        public virtual role role { get; set; }
    }
}
