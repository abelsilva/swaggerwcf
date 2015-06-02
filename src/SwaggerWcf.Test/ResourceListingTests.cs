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
 * ResourceListingTests.cs : Integration tests for high level discovery and listing of sample services.
 */


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SwaggerWcf.Test
{
	[TestClass]
	public class ResourceListingTests
	{
		AppDomain _EmptyDomain;
		AppDomain _TestDomain;

		[TestInitialize]
		public void Init()
		{
			_EmptyDomain = AppDomain.CreateDomain("EmptyDomain");
			//_EmptyDomain = AppDomain.CurrentDomain;

			//this doesn't seem to work as expected - if this is run concurrently with the EmptyResourceList test using CurrentDomain, things break
			AppDomainSetup testDomainSetup = new AppDomainSetup();
			testDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			_TestDomain = AppDomain.CreateDomain("SampleServiceDomain",
				null,
				testDomainSetup);
			_TestDomain.Load("SampleService");
		}

		[TestCleanup]
		public void Cleanup()
		{
			AppDomain.Unload(_EmptyDomain);
			AppDomain.Unload(_TestDomain);
			_TestDomain = null;
			_EmptyDomain = null;
		}

		[TestMethod]
		public void CanGetEmptyResourceList()
		{
			Scanner scanner = new Scanner();

			Stream stream = scanner.GetServices(_EmptyDomain);
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			Assert.IsFalse(string.IsNullOrEmpty(str));

			var obj = JObject.Parse(str);
			Assert.AreEqual("2.0", obj["swagger"]);
			Assert.AreEqual("No Swaggerized assemblies.", obj["apiVersion"]);
			Assert.IsNull(obj["basePath"]);
			Assert.IsFalse(obj["apis"].HasValues);
		}

		[TestMethod]
		public void CanGetSampleResourceList()
		{
			Scanner scanner = new Scanner();


			Stream stream = scanner.GetServices(_TestDomain);
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			Assert.IsFalse(string.IsNullOrEmpty(str));

			var obj = JObject.Parse(str);
			Assert.AreEqual("2.0", obj["swagger"]);
			Assert.AreEqual("1.0.0.0", obj["apiVersion"]);
			Assert.IsTrue(obj["apis"].HasValues);

			var api1 = obj["apis"].Children().FirstOrDefault(o => o["path"].Value<string>().Equals("/v1/rest"));
			Assert.IsNotNull(api1);
			Assert.AreEqual("A RESTful WCF Service", api1["description"]);

			var api2 = obj["apis"].Children().FirstOrDefault(o => o["path"].Value<string>().Equals("/SecondaryService.svc"));
			Assert.IsNotNull(api2);
			Assert.AreEqual("Another endpoint", api2["description"]);
		}
	}
}
