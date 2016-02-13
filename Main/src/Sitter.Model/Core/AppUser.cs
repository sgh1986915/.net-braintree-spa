﻿using System;
using System.Collections.Generic;
﻿using Newtonsoft.Json;

namespace MySitterHub.Model.Core
{
    public class AppUser
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string MobilePhone { get; set; }
        
        public string Email { get; set; }
        public UserRole UserRole { get; set; }
        
        /// <summary>
        /// Note, not persisted to database
        /// </summary>
        public string PhotoUrl { get; set; }
        
        /// <summary>
        /// If user has profiel picture in S3
        /// </summary>
        public bool HasProfilePic { get; set; }

        public float TimezoneOffset { get; set; }

        public string FullName()
        {
            return FirstName + " " + LastName;
        }

        public string FirstNameLastInitial()
        {
            return FirstName + " " + (string.IsNullOrEmpty(LastName) ? null : LastName.Substring(0, 1).ToUpper() + ".");
        }

        public DateTime ToLocalTime(DateTime date)
        {
            return date.AddHours(TimezoneOffset);
        }

        public const int NotSignedUpUserId = -103;
        public const int AppServiceUserId = -104;

    }

    public enum UserRole
    {
        Visitor,
        Parent,
        Sitter,
        Admin
    }

    public class AppUserPass
    {
        public int Id { get; set; }

        /// <summary>
        ///     We are duplicating the Email and MobilePhone on this object because every web service call needs to call the
        ///     AuthManager.ValidateToken() which hopefully only needs to call database once.
        /// </summary>
        public string Email { get; set; }

        public string MobilePhone { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        [JsonIgnore]
        public string Token { get; set; }
    }

    public enum CustomerType
    {
        Parent = 0,
        Babysitter = 1,
    }

    public class Sitter
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string ParentMobile { get; set; }
        public string ParentEmail { get; set; }
        
        public List<InviteToSignup> InviteToSignup { get; set; }

        // BsonIgnore
        public AppUser User { get; set; }
    }

    public class Parent
    {
        public int Id { get; set; }
        public List<ParentMySitter> Sitters { get; set; }
        public List<InviteToSignup> InviteToSignup { get; set; }

        // BsonIgnore
        public AppUser User { get; set; }
    }

    public class ParentMySitter
    {
        public int SitterId { get; set; }
        public decimal Rate { get; set; }
        public int SortOrder { get; set; }
    }

    public class InviteToSignup
    {
        public string MobilePhone { get; set; }
        public InvitationState InviteStatus { get; set; }
        public DateTime  InviteDate { get; set; }

        /// <summary>
        /// This is just for the parent to keep track of the name of the sitter
        /// </summary>
        public string InviteNickName { get; set; }
    }

    public class SignupInfo
    {
        public SitterSignupInfo SitterSignupInfo { get; set; }
        public AppUser User { get; set; }
        public string Pass { get; set; }
    }

    public class SitterSignupInfo
    {
        public int Age { get; set; }
        
        //[Required]
        public string ParentMobile { get; set; }
        public string ParentEmail { get; set; }
    }

    //public enum InviteStatus
    //{
    //    InvitePending,
    //    AcceptedInvite,
    //    DeclinedInvite,
    //    InviteCancelled
    //}
}