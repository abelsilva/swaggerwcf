/*
 * Copyright (c) 2014 Digimarc Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * TagAttribute.cs : Attribute to tag a method/model/property for configuration options.
 * 
 */

using System;

namespace SwaggerWcf.Attributes
{
	/// <summary>
	/// Identifies a class/method/etc for swagger so that visibility can be controlled via configuration.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
	public class TagAttribute : Attribute
	{
		public TagAttribute(string name)
		{
			TagName = name;
		}

		public string TagName { get; set; }
	}
}
