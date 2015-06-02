using System;

namespace SwaggerWcf.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class ParameterSettingsAttribute : Attribute
	{
		/// <summary>
		/// Overrides default behavior for a method parameter.
		/// </summary>
		/// <param name="isRequired">Is the parameter required or optional for this method. Defaults to false (not required).</param>
		/// <param name="description">Descriptive text.</param>
		/// <param name="hidden">Should be parameter be hidden for this method? Defaults to false.</param>
		public ParameterSettingsAttribute(bool isRequired = false, string description = null, bool hidden = false)
		{
			IsRequired = isRequired;
			Description = description;
			Hidden = hidden;
		}

		public bool IsRequired { get; set; }
		public string Description { get; set; }
		public bool Hidden { get; set; }
	}
}
