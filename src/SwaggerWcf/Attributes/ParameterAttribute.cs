using System;

namespace SwaggerWcf.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ParameterAttribute : Attribute
	{
		/// <summary>
		/// Attribute used to specify properties of a DataMeber
		/// </summary>
		/// <param name="description">Description for a DataMember</param>
		/// <param name="required">Define member as required</param>
		public ParameterAttribute(string description = null, bool required = false)
		{
			Description = description;
		    Required = required;
		}
        
		public string Description { get; set; }
		public bool Required { get; set; }
	}
}
