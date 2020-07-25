using System;
using System.ComponentModel;
// ReSharper disable MemberCanBePrivate.Global

namespace Plugin.Health
{
    [AttributeUsage(AttributeTargets.All)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    sealed class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;

        public PreserveAttribute(bool allMembers, bool conditional)
        {
            AllMembers  = allMembers;
            Conditional = conditional;
        }

        public PreserveAttribute()
        {
        }
    }
}
