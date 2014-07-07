/**
The MIT License (MIT)

Copyright (c) 2013 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KlaudWerk.Security.Claims
{
    /// <summary>
    /// Available Claim Types
    /// </summary>
    [Serializable]
    [DataContract]
    public class ClaimType : IEquatable<ClaimType>
    {
        private const string StrAdministrator = "http://www.klaudwerk.com/claims/is-administrator";
        private const string StrRole = "http://www.klaudwerk.com/claims/role";
        private const string StrGroup = "http://www.klaudwerk.com/claims/group";
        private const string StrAccountId = "http://www.klaudwerk.com/claims/account-id";
        private const string StrAccountMappedId = "http://www.klaudwerk.com/claims/account-mapped-id";
        private const string StrName = "http://www.klaudwerk.com/claims/account-name";
        private const string StrApplication = "http://www.klaudwerk.com/claims/application";
        private const string StrAutentication = "http://www.klaudwerk.com/claims/authentication";
        private const string StrAdRoot = "http://www.klaudwerk.com/claims/ad-root";
        private const string StrAccountType = "http://www.klaudwerk.com/claims/account-type";

        private const string StrLocation = "http://www.klaudwerk.com/claims/location";
        private const string StrDisplayName = "http://www.klaudwerk.com/claims/display-name";
        private const string StrGivenName = "http://www.klaudwerk.com/claims/given-name";
        private const string StrSurname = "http://www.klaudwerk.com/claims/surname";
        private const string StrDepartment = "http://www.klaudwerk.com/claims/department";
        private const string StrCompany = "http://www.klaudwerk.com/claims/company";
        private const string StrJobTitle = "http://www.klaudwerk.com/claims/job-title";
        private const string StrEmail = "http://www.klaudwerk.com/claims/email";
        private const string StrPhone = "http://www.klaudwerk.com/claims/phone";
        private const string StrFax = "http://www.klaudwerk.com/claims/fax";


        private static readonly ClaimType _administrator=new ClaimType(StrAdministrator);
        private static readonly ClaimType _role=new ClaimType(StrRole);
        private static readonly ClaimType _group=new ClaimType(StrGroup);
        private static readonly ClaimType _accountId=new ClaimType(StrAccountId);
        private static readonly ClaimType _accountName=new ClaimType(StrName);
        private static readonly ClaimType _application=new ClaimType(StrApplication);
        private static readonly ClaimType _authentication=new ClaimType(StrAutentication);
        private static readonly ClaimType _adRoot=new ClaimType(StrAdRoot);
        private static readonly ClaimType _mappedId = new ClaimType(StrAccountMappedId);
        private static readonly ClaimType _accType = new ClaimType(StrAccountType);

        private static readonly ClaimType _location=new ClaimType(StrLocation);
        private static readonly ClaimType _displayName=new ClaimType(StrDisplayName);
        private static readonly ClaimType _givenName=new ClaimType(StrGivenName);
        private static readonly ClaimType _surname=new ClaimType(StrSurname);
        private static readonly ClaimType _department=new ClaimType(StrDepartment);
        private static readonly ClaimType _company=new ClaimType(StrCompany);
        private static readonly ClaimType _jobTitle=new ClaimType(StrJobTitle);
        private static readonly ClaimType _email=new ClaimType(StrEmail);
        private static readonly ClaimType _phone=new ClaimType(StrPhone);
        private static readonly ClaimType _fax=new ClaimType(StrFax);

        private static readonly Dictionary<string,Func<ClaimType>> _map=new Dictionary<string, Func<ClaimType>>
                                                                     {
                                                                         {StrAdministrator,()=>_administrator},
                                                                         {StrRole,()=>_role},
                                                                         {StrGroup,()=>_group},
                                                                         {StrAccountId,()=>_accountId},
                                                                         {StrAccountMappedId,()=>_mappedId},
                                                                         {StrName,()=>_accountName},
                                                                         {StrApplication,()=>_application},
                                                                         {StrAutentication,()=>_authentication},
                                                                         {StrAdRoot,()=>_adRoot},
                                                                         {StrLocation,()=>_location},
                                                                         {StrDisplayName,()=>_displayName},
                                                                         {StrGivenName,()=>_givenName},
                                                                         {StrSurname,()=>_surname},
                                                                         {StrDepartment,()=>_department},
                                                                         {StrCompany,()=>_company},
                                                                         {StrJobTitle,()=>_jobTitle},
                                                                         {StrEmail,()=>_email},
                                                                         {StrPhone,()=>_phone},
                                                                         {StrFax,()=>_fax}

                                                                     };
        [DataMember]
        private readonly string _name;

        /// <summary>
        /// Gets the type of the account.
        /// </summary>
        /// <value>The type of the account.</value>
        public static ClaimType AccountType
        {
            get { return _accType; }
        }
        /// <summary>
        /// Gets the administrator.
        /// </summary>
        /// <value>The administrator.</value>
        public static ClaimType Administrator
        {
            get { return _administrator; }
        }

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <value>The role.</value>
        public static ClaimType Role
        {
            get { return _role; }
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>The group.</value>
        public static ClaimType Group
        {
            get { return _group; }
        }

        /// <summary>
        /// Gets the account id.
        /// </summary>
        /// <value>The account id.</value>
        public static ClaimType AccountId
        {
            get { return _accountId; }
        }

        /// <summary>
        /// Gets the mapped id.
        /// </summary>
        /// <value>The mapped id.</value>
        public static ClaimType MappedId
        {
            get { return _mappedId; }
        }

        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        /// <value>The name of the account.</value>
        public static ClaimType AccountName
        {
            get { return _accountName; }
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        public static ClaimType Application
        {
            get { return _application; }
        }

        /// <summary>
        /// Gets the authentication.
        /// </summary>
        /// <value>The authentication.</value>
        public static ClaimType Authentication
        {
            get { return _authentication; }
        }

        /// <summary>
        /// Gets the ad root.
        /// </summary>
        /// <value>The ad root.</value>
        public static ClaimType AdRoot
        {
            get { return _adRoot; }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public static ClaimType Location
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public static ClaimType DisplayName
        {
            get { return _displayName; }
        }

        /// <summary>
        /// Gets the name of the given.
        /// </summary>
        /// <value>The name of the given.</value>
        public static ClaimType GivenName
        {
            get { return _givenName; }
        }

        /// <summary>
        /// Gets the surname.
        /// </summary>
        /// <value>The surname.</value>
        public static ClaimType Surname
        {
            get { return _surname; }
        }

        /// <summary>
        /// Gets the department.
        /// </summary>
        /// <value>The department.</value>
        public static ClaimType Department
        {
            get { return _department; }
        }

        /// <summary>
        /// Gets the company.
        /// </summary>
        /// <value>The company.</value>
        public static ClaimType Company
        {
            get { return _company; }
        }

        /// <summary>
        /// Gets the job title.
        /// </summary>
        /// <value>The job title.</value>
        public static ClaimType JobTitle
        {
            get { return _jobTitle; }
        }

        /// <summary>
        /// Gets the email.
        /// </summary>
        /// <value>The email.</value>
        public static ClaimType Email
        {
            get { return _email; }
        }

        /// <summary>
        /// Gets the phone.
        /// </summary>
        /// <value>The phone.</value>
        public static ClaimType Phone
        {
            get { return _phone; }
        }

        /// <summary>
        /// Gets the fax.
        /// </summary>
        /// <value>The fax.</value>
        public static ClaimType Fax
        {
            get { return _fax; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimType"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ClaimType(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Get an instance of a Claim Type by its name
        /// </summary>
        /// <param name="claimName">Name of the claim.</param>
        /// <returns></returns>
        public static ClaimType Get(string claimName)
        {
            Func<ClaimType> claim;
            if (_map.TryGetValue(claimName, out claim))
                return claim.Invoke();
            throw new ArgumentException(string.Format("Claim type not found:{0}",claimName));
        }
        #region IEquatable implementation
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(ClaimType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ClaimType)) return false;
            return Equals((ClaimType) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public static bool operator ==(ClaimType left, ClaimType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClaimType left, ClaimType right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
