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
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.ViewModels
{
    /// <summary>
    /// Represents a view model for user data in the Upendo User Manager module.
    /// This class is used to encapsulate user-related information, including personal details,
    /// account settings, roles, and other metadata required for user management operations.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// This property is used to distinguish and manage individual users
        /// within the Upendo User Manager module.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// This property is required for user authentication and identification.
        /// </summary>
        /// <remarks>
        /// This property is required and represents the user's username.
        /// It is used in various operations such as user authentication, identification, and display.
        /// </remarks>
        [Required]
        public string Username { get; set; }
        
        /// <summary>
        /// Gets or sets the first name of the user.
        /// This property is required for user identification and personalization.
        /// </summary>
        /// <remarks>
        /// This property is required and represents the user's first name.
        /// It is used in various operations such as user creation, editing, and display.
        /// </remarks>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// This property is required for user identification and personalization.
        /// </summary>
        /// <remarks>
        /// This property is required and represents the user's last name.
        /// It is used in various operations such as user creation, editing, and display.
        /// </remarks>
        [Required]  
        public string LastName { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user is a superuser.
        /// Superusers have elevated privileges and can perform administrative tasks
        /// across the entire application, regardless of portal-specific restrictions.
        /// </summary>
        public bool IsSuperUser { get; set; }
        public bool Approved { get; set; }
        public int? AffiliateId { get; set; }
        
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        /// <remarks>
        /// This property is required and represents the user's email address.
        /// It is used in various operations such as user creation, editing, and display.
        /// </remarks>
        [Required]
        public string Email { get; set; }
        
        /// <summary>
        /// Gets or sets the display name of the user.
        /// This property represents the full name or a preferred name
        /// that is displayed in the user interface and other contexts.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Gets or sets the password for the user.
        /// This property is used for user authentication and account management.
        /// </summary>
        /// <remarks>
        /// Ensure that the password is securely handled and stored.
        /// Avoid exposing this property unnecessarily to maintain security.
        /// </remarks>
        public string Password { get; set; }
        
        /// <summary>
        /// Gets or sets the confirmation password entered by the user.
        /// This property is used to verify that the password entered matches the intended password
        /// during operations such as user registration or password change.
        /// </summary>
        public string ConfirmPassword { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user's password should be updated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the password should be updated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property is typically used to determine if the user's password needs to be changed
        /// during user management operations, such as editing user details.
        /// </remarks>
        public bool UpdatePassword { get; set; }
        
        /// <summary>
        /// Gets or sets the last known IP address of the user.
        /// This property is used to track the most recent IP address from which the user accessed the system.
        /// </summary>
        public string LastIpaddress { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user account is marked as deleted.
        /// This property is used to track the logical deletion status of a user account
        /// without permanently removing the user data from the system.
        /// </summary>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether an email should be sent to the user.
        /// This property is typically used during user creation or updates to determine
        /// if a notification email should be dispatched.
        /// </summary>
        public bool SendEmail { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user account is locked out.
        /// </summary>
        /// <remarks>
        /// A locked-out account typically indicates that the user has exceeded the allowed number of failed login attempts
        /// or that the account has been administratively disabled. This property is used to determine if the user is
        /// temporarily or permanently restricted from accessing their account.
        /// </remarks>
        public bool LockedOut { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who created this record.
        /// This property is nullable and can be used to track the origin of the record
        /// for auditing or administrative purposes.
        /// </summary>
        public int? CreatedByUserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the user account was created.
        /// </summary>
        /// <remarks>
        /// This property is nullable, indicating that the creation date may not always be available.
        /// It is typically used to track when the user account was initially registered in the system.
        /// </remarks>
        public DateTime? CreatedOnDate { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who last modified this user record.
        /// </summary>
        /// <remarks>
        /// This property is nullable to account for cases where the modification information
        /// is not available or has not been set.
        /// </remarks>
        public int? LastModifiedByUserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the user was last modified.
        /// </summary>
        /// <remarks>
        /// This property is nullable and will contain a value only if the user has been modified.
        /// It is used to track the most recent update to the user's information.
        /// </remarks>
        public DateTime? LastModifiedOnDate { get; set; }
        
        /// <summary>
        /// Gets or sets the token used for resetting the user's password.
        /// This token is typically generated when a password reset request is initiated
        /// and is used to verify the authenticity of the request.
        /// </summary>
        public Guid? PasswordResetToken { get; set; }
        
        /// <summary>
        /// Gets or sets the expiration date and time for the password reset token.
        /// This property indicates the deadline by which the user must use the password reset token
        /// before it becomes invalid.
        /// </summary>
        public DateTime? PasswordResetExpiration { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the portal associated with the user.
        /// This property is used to distinguish users across different portals
        /// within the Upendo User Manager module.
        /// </summary>
        public int PortalID { get; set; }
        
        /// <summary>
        /// Gets or sets the roles assigned to the user.
        /// </summary>
        /// <remarks>
        /// This property contains an array of role names that the user is associated with.
        /// It is used to manage and display the user's roles within the Upendo User Manager module.
        /// </remarks>
        public string[] UserRoles { get; set; }
        
        /// <summary>
        /// Gets or sets the new role to be assigned to the user.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the new role for the user.
        /// </value>
        /// <remarks>
        /// This property is used to update or assign a new role to the user during user management operations.
        /// </remarks>
        public string NewUserRol { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user is authorized.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is authorized; otherwise, <c>false</c>.
        /// </value>
        public bool Authorized { get; set; }
        
        /// <summary>
        /// Gets or sets the URL or path to the user's avatar image.
        /// This property is used to display the user's profile picture in the Upendo User Manager module.
        /// </summary>
        public string Avatar { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user has agreed to the terms and conditions.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user has agreed to the terms and conditions; otherwise, <c>false</c>.
        /// </value>
        public bool HasAgreedToTerms { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user has administrative privileges.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is an administrator; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdmin { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user has requested account removal.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user has requested account removal; otherwise, <c>false</c>.
        /// </value>
        public bool RequestsRemoval { get; set; }
    }
}