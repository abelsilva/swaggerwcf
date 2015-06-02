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
 * ApiDeclarationTests.cs : High level integration tests for high level description of sample services.
 */


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SwaggerWcf.Test
{
	[TestClass]
	public class ApiDeclarationTests
	{
		AppDomain _TestDomain;

		[TestInitialize]
		public void Init()
		{
			AppDomainSetup testDomainSetup = new AppDomainSetup()
			{
				ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
			};

			_TestDomain = AppDomain.CreateDomain("SampleServiceDomain",
				null,
				testDomainSetup);
			_TestDomain.Load("SampleService");
		}

		[TestCleanup]
		public void Cleanup()
		{
			AppDomain.Unload(_TestDomain);
			_TestDomain = null;
		}

		[TestMethod]
		public void CanGetSampleDeclaration()
		{
			Scanner scanner = new Scanner();

			Stream stream = scanner.GetServiceDetails(_TestDomain, new Uri("http://mockhost"), "v1/rest");
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			Assert.IsFalse(string.IsNullOrEmpty(str));

			var obj = JObject.Parse(str);
			Assert.AreEqual("2.0", obj["swagger"]);
			Assert.AreEqual("1.0.0.0", obj["apiVersion"]);
			Assert.AreEqual("http://mockhost", obj["basePath"]);
			Assert.AreEqual("/v1/rest", obj["resourcePath"]);
			Assert.IsTrue(obj["apis"].HasValues);

			var api = obj["apis"][0];
			Assert.AreEqual("/v1/rest/data", api["path"]);
		}
	}
}
