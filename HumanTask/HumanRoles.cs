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

namespace HumanTask
{
    /// <summary>
    /// The enumeration defines Generic Human roles
    /// 
    /// Generic human roles define what a person or group of people
    //  resulting from a people query can do with tasks and notifications.
    //
    /// </summary>
    [Serializable]
    public class HumanRoles : IEquatable<HumanRoles>
    {
        private static readonly HumanRoles _initiator;
        private static readonly HumanRoles _stakeholder;
        private static readonly HumanRoles _potentialOwner;
        private static readonly HumanRoles _actualOwner;
        private static readonly HumanRoles _businessAdmin;
        private static readonly HumanRoles _excludedOwner;
        private static readonly HumanRoles _recepient;
        private static readonly HumanRoles _potentialDelegee;
        // Task initiator is the person who creates the task instance
        public static HumanRoles Initiator { get { return _initiator; } }
        // Stakeholders are the people ultimately responsible for the oversight and outcome of the task instance.
        public static HumanRoles Stakeholder { get { return _stakeholder; } }
        //  The people who receive the task so that they can claim and complete it. 
        //  A potential owner becomes the actual owner of a task by  explicitly claiming it.
         public static HumanRoles PotentialOwner { get { return _potentialOwner; } }
        //  The person who performs the task.
        //  A task has exactly one actual owner
         public static HumanRoles ActualOwner{ get { return _actualOwner; } }
        //   
        // The people who can play the same role as task stakeholders
        //
         public static HumanRoles BusinessAdministrator { get { return _businessAdmin; } }
        //
        // Excluded owners may not become an actual or potential owner and thus they may not reserve or
        // start the task. They can only doing statistic or information operations.
        //
         public static HumanRoles ExcludedOwner { get { return _excludedOwner; } }
        //
        //  The people who receive the notification, such as happens when a deadline is missed or when a milestone is reached.
        //
         public static HumanRoles Recepient{ get { return _recepient; } }
        //
        //  A person or group of people who can be delegate to perform the task
        //
         public static HumanRoles PotentialDelegatee { get { return _potentialDelegee; } }

        private readonly string _name;
        private static readonly Dictionary<string,HumanRoles> _map=new Dictionary<string, HumanRoles>();
        /// <summary>
        /// Initializes the <see cref="HumanRoles"/> class.
        /// </summary>
        static HumanRoles()
        {
            _initiator = new HumanRoles("Initiator");
            _stakeholder = new HumanRoles("Stakeholder");
            _potentialOwner = new HumanRoles("PotentialOwner");
            _actualOwner = new HumanRoles("ActualOwner");
            _businessAdmin = new HumanRoles("BusinessAdministrator");
            _excludedOwner = new HumanRoles("ExcludedOwner");
            _recepient = new HumanRoles("Recepient");
            _potentialDelegee = new HumanRoles("PotentialDelegee");
            _map["Initiator"] = _initiator;
            _map["Stakeholder"] = _stakeholder;
            _map["PotentialOwner"]=_potentialOwner;
            _map["ActualOwner"]=_actualOwner;
            _map["BusinessAdministrator"]=_businessAdmin;
            _map["ExcludedOwner"]=_excludedOwner;
            _map["Recepient"]=_recepient;
            _map["PotentialDelegee"]=_potentialDelegee;
        }

        /// <summary>
        /// Gets the role by its name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public static HumanRoles GetByName(string roleName)
        {
            HumanRoles role;
            if(!_map.TryGetValue(roleName,out role))
                throw new ArgumentException(string.Format("Invalid role name: {0}",roleName));
            return role;
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HumanRoles"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected HumanRoles(string name)
        {
            _name = name;
        }
        
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string  ToString()
        {
            return _name;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HumanTask.HumanRoles"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(HumanRoles role)
        {
            return role == null ? null : role.Name;
        }

        #region IEquatable implementation
        public bool Equals(HumanRoles other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(HumanRoles))
            {
                return  typeof(string)==obj.GetType()? ((string)obj)== _name: false;
            }
            return Equals((HumanRoles) obj);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public static bool operator ==(HumanRoles left, HumanRoles right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HumanRoles left, HumanRoles right)
        {
            return !Equals(left, right);
        }
#endregion

    }
}
