using System;

using SwaggerWcf.Models;


namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Describe a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SwaggerWcfPropertyAttribute : Attribute
    {
        /// <summary>
        ///     Describes a property
        /// </summary>
        public SwaggerWcfPropertyAttribute()
        {
        }

        public string Title { get; set; }

        /// <summary>
        ///     Description of this parameter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Determines whether this parameter is mandatory.
        /// </summary>
        public bool Required { get { return _Required.GetValueOrDefault(); } set { _Required = value; } }

        /// <summary>
        ///     Sets the ability to pass empty-valued parameters. This is valid
        ///     only for either query or formData parameters and allows you to
        ///     send a parameter with a name only or an empty value.
        /// </summary>
        //public bool AllowEmptyValue { get { return _AllowEmptyValue ?? false; } set { _AllowEmptyValue = value; } }

        /// <summary>
        ///     Determines the format of the array if type array is used.
        /// </summary>
        //public CollectionFormat? CollectionFormat { get; set; }

        /// <summary>
        ///     Declares the value of the parameter that the server will use if none is provided
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        ///     Illustrate what the value is supposed to be like.
        /// </summary>
        public object Example { get; set; }

        /// <summary>
        ///     Maximum allowed value, as modified by ExclusiveMaximum.
        ///     Must be a valid JSON number, and storable as a decimal.
        /// </summary>
        public string Maximum {
            get { return _Maximum.HasValue ? _Maximum.ToString() : null; }
            set { _Maximum = decimal.Parse(value); }
        }

        /// <summary>
        ///     If true, the instance is valid if it is strictly less than the value of Maximum.
        /// </summary>
        public bool ExclusiveMaximum {
            get { return _ExclusiveMaximum.GetValueOrDefault(); }
            set { _ExclusiveMaximum = value; }
        }

        /// <summary>
        ///     Minimum allowed value, as modified by ExclusiveMinimum
        ///     Must be a valid JSON number, and storable as a decimal.
        /// </summary>
        public string Minimum {
            get { return _Minimum.HasValue ? _Minimum.ToString() : null; }
            set { _Minimum = decimal.Parse(value); }
        }

        /// <summary>
        ///     If true, the instance is valid if it is strictly greatern than the value of Maximum.
        /// </summary>
        public bool ExclusiveMinimum {
            get { return _ExclusiveMinimum.GetValueOrDefault(); }
            set { _ExclusiveMinimum = value; }
        }

        /// <summary>
        ///     Maximum length.
        /// </summary>
        public int MaxLength {
            get { return _MaxLength.GetValueOrDefault(); }
            set { _MaxLength = value; }
        }

        /// <summary>
        ///     Minimum length
        /// </summary>
        public int MinLength {
            get { return _MinLength.GetValueOrDefault(); }
            set { _MinLength = value; }
        }

        /// <summary>
        ///    Should be a valid regular expression, according to the ECMA 262 regular expression dialect.
        ///    A string instance is considered valid if the regular expression matches the instance successfully.
        ///    Regular expressions are not implicitly anchored.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        ///     An array instance is valid against MaxItems if its size
        ///     is less than, or equal to, the value of this keyword.
        /// </summary>
        public int MaxItems {
            get { return _MaxItems.GetValueOrDefault(); }
            set { _MaxItems = value; }
        }

        /// <summary>
        ///     An array instance is valid against MaxItems if its size
        ///     is greater than, or equal to, the value of this keyword.
        /// </summary>
        public int MinItems {
            get { return _MinItems.GetValueOrDefault(); }
            set { _MinItems = value; }
        }

        /// <summary>
        ///     If true, the instance validates successfully if all of its elements are unique.
        /// </summary>
        public bool UniqueItems {
            get { return _UniqueItems.GetValueOrDefault(); }
            set { _UniqueItems = value; }
        }

        /// <summary>
        ///     A numeric instance is valid against "multipleOf" if the result of
        ///     the division of the instance by this keyword's value is an integer.
        /// </summary>
        public decimal MultipleOf {
            get { return _MultipleOf.GetValueOrDefault(); }
            set { _MultipleOf = value; }
        }

        // To be usable as attribute named parameters, parameter types must not be
        // nullable but we need to be able to see whether each value was set or not,
        // so hide a nullable value for each available option.

        internal bool?    _Required         { get; set; }
        internal bool?    _ExclusiveMaximum { get; set; }
        internal decimal? _Minimum          { get; set; }
        internal decimal? _Maximum          { get; set; }
        internal bool?    _ExclusiveMinimum { get; set; }
        internal int?     _MaxLength        { get; set; }
        internal int?     _MinLength        { get; set; }
        internal int?     _MaxItems         { get; set; }
        internal int?     _MinItems         { get; set; }
        internal bool?    _UniqueItems      { get; set; }
        internal decimal? _MultipleOf       { get; set; }
    }
}
