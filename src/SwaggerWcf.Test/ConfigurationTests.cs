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
 * ConfigurationTests.cs : Tests for configuration parsing.
 */


using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Linq;

namespace SwaggerWcf.Test
{
	[TestClass]
	public class ConfigurationTests
	{
		[TestMethod]
		public void CanGetSettingsFromConfig()
		{
			Scanner scanner = new Scanner();

			Assert.IsNotNull(scanner.HiddenTags);
			Assert.IsTrue(scanner.HiddenTags.Count() == 1);
			Assert.IsTrue(scanner.HiddenTags.Contains("Foo"));
			Assert.IsFalse(scanner.HiddenTags.Contains("Bar"));
		}

		[TestMethod]
		public void CanReadAppConfig()
		{
			var swaggersettings = (Configuration.SwaggerSection)ConfigurationManager.GetSection("swagger");

			Assert.IsNotNull(swaggersettings);
			Assert.IsTrue(swaggersettings.Tags.Count == 2);
			Assert.IsTrue(swaggersettings.Tags.OfType<Configuration.TagElement>().Count(t => t.Name.Equals("Foo")) == 1);
			Assert.IsTrue(swaggersettings.Tags.OfType<Configuration.TagElement>().Count(t => t.Name.Equals("Bar")) == 1);
		}
	}
}
