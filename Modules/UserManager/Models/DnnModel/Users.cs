/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES 
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.Models.DnnModel
{
    /// <summary>
    /// Represents a user entity within the DNN (DotNetNuke) framework.
    /// </summary>
    /// <remarks>
    /// This class is a partial class designed to encapsulate user-related data, including personal details,
    /// account status, and associated roles. It is primarily used within the Upendo User Manager module.
    /// </remarks>
    public partial class Users
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Users"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the default state of a <see cref="Users"/> object, 
        /// including initializing the <see cref="UserRoles"/> collection to an empty set.
        /// </remarks>
        public Users()
        {
            UserRoles = new HashSet<UserRoles>();
        }
        
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        /// <remarks>
        /// This property serves as the primary key for the <see cref="Users"/> entity.
        /// It uniquely identifies a user within the DNN framework and is used in various
        /// operations, such as user management and data retrieval.
        /// </remarks>
        [Key]
        public int UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the username associated with the user.
        /// </summary>
        /// <remarks>
        /// The username serves as a unique identifier for the user within the DNN (DotNetNuke) framework.
        /// It is used for authentication, user management, and display purposes.
        /// </remarks>
        public string Username { get; set; }
        
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        /// <remarks>
        /// This property represents the user's given name and is used for display purposes,
        /// personalization, and identification within the Upendo User Manager module.
        /// </remarks>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the user's last name.
        /// </value>
        /// <remarks>
        /// This property is used to store the family name or surname of a user. 
        /// It is a required field in user-related operations within the Upendo User Manager module.
        /// </remarks>
        public string LastName { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user has superuser privileges within the DNN (DotNetNuke) framework.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is a superuser; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Superusers have elevated permissions and can perform administrative tasks that regular users cannot.
        /// </remarks>
        public bool IsSuperUser { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the affiliate associated with the user.
        /// </summary>
        /// <remarks>
        /// This property represents the optional affiliate ID linked to the user. 
        /// It is used to track affiliate relationships within the DNN framework.
        /// </remarks>
        public int? AffiliateId { get; set; }
        
        /// <summary>
        /// Gets or sets the email address associated with the user.
        /// </summary>
        /// <remarks>
        /// The email address is a required field and is used for user identification, communication,
        /// and account-related operations within the Upendo User Manager module.
        /// </remarks>
        public string Email { get; set; }
        
        /// <summary>
        /// Gets or sets the display name of the user.
        /// </summary>
        /// <remarks>
        /// The display name is a user-friendly representation of the user's identity, 
        /// often combining the first and last names or a custom name chosen by the user.
        /// It is used in various contexts, such as displaying user information in the UI 
        /// or searching for users.
        /// </remarks>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user's password should be updated.
        /// </summary>
        /// <remarks>
        /// This property is used to determine if a password update operation is required for the user.
        /// It is typically utilized in scenarios where password changes are enforced or requested.
        /// </remarks>
        public bool UpdatePassword { get; set; }
        
        /// <summary>
        /// Gets or sets the last known IP address of the user.
        /// </summary>
        /// <remarks>
        /// This property stores the most recent IP address from which the user accessed the system.
        /// It can be used for auditing, security, or logging purposes.
        /// </remarks>
        public string LastIpaddress { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user account is marked as deleted.
        /// </summary>
        /// <remarks>
        /// This property is used to determine if the user account is logically deleted within the system.
        /// A value of <c>true</c> indicates that the account is deleted, while <c>false</c> means it is active.
        /// </remarks>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user is approved.
        /// </summary>
        /// <remarks>
        /// This property reflects the approval status of the user within the system.
        /// A value of <c>true</c> indicates that the user is approved, while <c>false</c> indicates otherwise.
        /// </remarks>
        public bool Approved { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who created this record.
        /// </summary>
        /// <remarks>
        /// This property holds the user ID of the creator of the record. 
        /// It is nullable to account for scenarios where the creator's information 
        /// might not be available or applicable.
        /// </remarks>
        public int? CreatedByUserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the user account was created.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime"/> value representing the creation date and time of the user account, 
        /// or <c>null</c> if the creation date is not specified.
        /// </value>
        /// <remarks>
        /// This property is used to track when the user account was initially created within the system.
        /// </remarks>
        public DateTime? CreatedOnDate { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who last modified this record.
        /// </summary>
        /// <remarks>
        /// This property holds the user ID of the individual responsible for the most recent modification
        /// of the user record. It is nullable to accommodate scenarios where the modification details
        /// are unavailable or not applicable.
        /// </remarks>
        public int? LastModifiedByUserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the user was last modified.
        /// </summary>
        /// <remarks>
        /// This property indicates the most recent modification timestamp for the user entity.
        /// It is typically updated whenever changes are made to the user's details.
        /// </remarks>
        public DateTime? LastModifiedOnDate { get; set; }
        
        /// <summary>
        /// Gets or sets the token used for resetting the user's password.
        /// </summary>
        /// <remarks>
        /// This property stores a unique identifier (GUID) that is generated when a password reset request is initiated.
        /// It is used to validate the user's identity during the password reset process.
        /// </remarks>
        public Guid? PasswordResetToken { get; set; }
        
        /// <summary>
        /// Gets or sets the expiration date and time for the password reset token.
        /// </summary>
        /// <remarks>
        /// This property indicates the deadline by which a user must use their password reset token.
        /// If the token is not used before this date and time, it will become invalid.
        /// </remarks>
        public DateTime? PasswordResetExpiration { get; set; }
       
        /// <summary>
        /// Gets or sets the collection of roles associated with the user.
        /// </summary>
        /// <remarks>
        /// This property represents the many-to-many relationship between users and roles within the DNN framework.
        /// Each <see cref="UserRoles"/> object in the collection links the user to a specific role, 
        /// including additional metadata such as effective dates, expiry dates, and status.
        /// </remarks>
        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}
