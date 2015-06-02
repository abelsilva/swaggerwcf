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
 * MapperTests.cs : Tests of model creation via mapper methods.
 */


using System;
using System.IO;
using System.ServiceModel.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SwaggerWcf.Models;
using SwaggerWcf.Attributes;
using SwaggerWcf.Support;

namespace SwaggerWcf.Test
{
	[TestClass]
	public class MapperTests
	{
		[TestMethod]
		public void CanMapCollectionTypes()
		{
			var typeMap = new Stack<Type>();
			Assert.AreEqual("array", Helpers.MapSwaggerType(typeof(List<string>), typeMap));
			Assert.AreEqual("array", Helpers.MapSwaggerType(typeof(int[]), typeMap));

			Assert.AreEqual(0, typeMap.Count);
		}

		[TestMethod]
		public void CanMapOperation()
		{
			var mapper = new Mapper(new List<string> { "SecretThings" });

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			Assert.AreEqual(6, operations.Count());
			Assert.AreEqual("/method/test", operations.First().Item1);
			var operation = operations.First().Item2;

			Assert.AreEqual(3, operation.parameters.Count);

			var uno = operation.parameters.First(p => p.name.Equals("uno"));
			var dos = operation.parameters.First(p => p.name.Equals("dos"));
			var tres = operation.parameters.First(p => p.name.Equals("tRes"));

			Assert.AreEqual("query", uno.paramType);
			Assert.AreEqual(true, uno.required);
			Assert.IsTrue(string.IsNullOrEmpty(uno.description));

			Assert.AreEqual("query", dos.paramType);
			Assert.AreEqual(true, dos.required);
			Assert.AreEqual("integer(32)", dos.type);

			Assert.AreEqual("query", tres.paramType);
			Assert.AreEqual(false, tres.required);
			Assert.AreEqual("The third option.", tres.description);
			Assert.AreEqual("string(22)", tres.type);
		}

		[TestMethod]
		public void CanMapOperationUriWithRequiredParams()
		{
			var mapper = new Mapper(new List<string> { "SecretThings" }, true, false);
			
			Assert.IsTrue(mapper.ShowRequiredQueryParamsInHeader);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			Assert.AreEqual("/method/test?uno={uno}&dos={dos}", operations.First().Item1);
		}

		[TestMethod]
		public void CanMarkOperationSummaryIfTagged()
		{
			var mapper = new Mapper(null, false, true);
			Assert.IsTrue(mapper.MarkTagged);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			var operation = operations.First(o => o.Item1.Equals("/keepitsecret")).Item2;
			Assert.AreEqual("Secret method***", operation.summary);
		}

		[TestMethod]
		public void CanMapResponseCodes()
		{
			var mapper = new Mapper(null);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			var operation = operations.First(o => o.Item1.Equals("/keepitsecret")).Item2;

			Assert.AreEqual(7, operation.errorResponses.Count());
			Assert.AreEqual("OK", operation.errorResponses[0].message);
			Assert.AreEqual(200, operation.errorResponses[0].code);
		}


		[TestMethod]public void CanSortResponseCodes()
		{
			var mapper = new Mapper(null);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			var operation = operations.First(o => o.Item1.Equals("/keepitsecret")).Item2;

			Assert.AreEqual(7, operation.errorResponses.Count());

			var lastIndex = operation.errorResponses.Count() - 1;
			for (int i = 0; i < lastIndex; i++)
			{
				if (i == (lastIndex - 1))
					break;
				Assert.IsTrue(operation.errorResponses[i].code < operation.errorResponses[i + 1].code);
			}
		}


		[TestMethod]
		public void CanMapContentTypes()
		{
			var mapper = new Mapper(null);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());

			var operation = operations.First(o => o.Item1.Equals("/keepitsecret")).Item2;
			Assert.AreEqual(1, operation.produces.Count);
			Assert.AreEqual("application/xml", operation.produces[0]);

			var operation2 = operations.First(o => o.Item1.Equals("/method/test")).Item2;
			Assert.AreEqual(2, operation2.produces.Count);
			Assert.IsTrue(operation2.produces.Contains("application/xml"));
			Assert.IsTrue(operation2.produces.Contains("application/json"));
		}

		[TestMethod]
		public void CanMapNotesAndSummary()
		{
			var mapper = new Mapper(null);

			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());
			var operation = operations.First(o => o.Item1.Equals("/method/test")).Item2;

			Assert.AreEqual("Short format", operation.summary);
			Assert.AreEqual("Long format", operation.notes);
		}
		
		[TestMethod]
		public void CanMapVoidOperation()
		{
			var mapper = new Mapper(null);
			var map = typeof (MapTest).GetInterfaceMap(typeof (IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());
			var operation = operations.First(o => o.Item1.Equals("/voidtest")).Item2;
			Assert.IsNotNull(operation);
			Assert.AreEqual("None",operation.type);
		}

		[TestMethod]
		public void CanMapBuiltInParams()
		{
			var mapper = new Mapper(null);
			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());
			var operation = operations.First(o => o.Item1.Equals("/voidtest")).Item2;

			Assert.AreEqual(17, operation.parameters.Count);
			Assert.AreEqual("boolean", GetTypeFromParamList("bl", operation.parameters));
			Assert.AreEqual("integer(8)", GetTypeFromParamList("bt", operation.parameters));
			Assert.AreEqual("integer(8, signed)", GetTypeFromParamList("sbt", operation.parameters));
			Assert.AreEqual("character", GetTypeFromParamList("ch", operation.parameters));
			Assert.AreEqual("decimal", GetTypeFromParamList("dm", operation.parameters));
			Assert.AreEqual("double", GetTypeFromParamList("db", operation.parameters));
			Assert.AreEqual("float", GetTypeFromParamList("fl", operation.parameters));
			Assert.AreEqual("integer(32)", GetTypeFromParamList("it", operation.parameters));
			Assert.AreEqual("integer(32, unsigned)", GetTypeFromParamList("uit", operation.parameters));
			Assert.AreEqual("integer(64)", GetTypeFromParamList("lg", operation.parameters));
			Assert.AreEqual("integer(64, unsigned)", GetTypeFromParamList("ulg", operation.parameters));
			Assert.AreEqual("integer(16)", GetTypeFromParamList("st", operation.parameters));
			Assert.AreEqual("integer(16, unsigned)", GetTypeFromParamList("ust", operation.parameters));
			Assert.AreEqual("string", GetTypeFromParamList("str", operation.parameters));
			Assert.AreEqual("Date", GetTypeFromParamList("dt", operation.parameters));
			Assert.AreEqual("string", GetTypeFromParamList("guid", operation.parameters));
			Assert.AreEqual("stream", GetTypeFromParamList("stream", operation.parameters));
			
		}

		[TestMethod]
		public void CanHideParameter()
		{
			var mapper = new Mapper(null);
			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var operations = mapper.GetOperations(map, new Stack<Type>());
			var operation = operations.First(o => o.Item1.Equals("/hideparamtest")).Item2;
			Assert.AreEqual(1, operation.parameters.Count);
			Assert.AreEqual("foo", operation.parameters[0].name);
			Assert.AreEqual("integer(32)", operation.parameters[0].type);
		}

		[TestMethod]
		public void CanOverrideReturnType()
		{
			var mapper = new Mapper(null);
			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var realReturnType = map.InterfaceMethods.First(m => m.Name == "OverrideReturnTypeTest");
			Assert.IsNotNull(realReturnType);
			Assert.AreEqual(typeof(int), realReturnType.ReturnType);
			var operations = mapper.GetOperations(map, new Stack<Type>());
			var operation = operations.First(o => o.Item1.Equals("/overridereturntype")).Item2;
			Assert.AreEqual("string", operation.type);
		}

		[TestMethod]
		public void CanMapDataTypeAsDataContractName()
		{
			var mapper = new Mapper(null);
			var map = typeof (MapTest).GetInterfaceMap(typeof (IMapTest));
			var testOperations = mapper.GetOperations(map, new Stack<Type>());
			var operation = testOperations.First(o => o.Item1.Equals("/customtypetest")).Item2;
			Assert.AreEqual("MyDataContractName", operation.type);
		}

		[TestMethod]
		public void CanOverrideParameterTypeWithDataContractName()
		{
			var mapper = new Mapper(null);
			var map = typeof(MapTest).GetInterfaceMap(typeof(IMapTest));
			var testOperations = mapper.GetOperations(map, new Stack<Type>());
			var operation = testOperations.First(o => o.Item1.Equals("/overrideparamtypeascustomtypetest")).Item2;
			Assert.AreEqual(1, operation.parameters.Count);
			Assert.AreEqual("MyRequest", GetTypeFromParamList("req", operation.parameters));
		}


		private string GetTypeFromParamList(string name, List<Parameter> parameters)
		{
			return (parameters.First(p => p.name.Equals(name))).type;
		}

		interface IMapTest
		{
			[Operation("Short format"), Operation("Long format")]
			[WebGet(UriTemplate = "/method/test?uno={uno}&dos={dos}&tRes={thRee}")]
			int Method(
				[ParameterSettings(IsRequired = true)]string uno,
				[ParameterSettings(IsRequired = true)]string dos,
				[ParameterSettings(Description = "The third option."]string thRee);

			[Tag("SecretThings")]
			[Response(500, "Just because.")]
			[Response(400, "Four hundred error")]
			[Response(200, "OK")]
			[Response(205, "Some error")]
			[Response(404, "Not found")]
			[Response(401, "Something weird happened")]
			[Response(301, "Three O one Something weird happened")]
			[Produces(ContentType = "application/xml")]
			[Operation("Secret method")]
			[WebGet(UriTemplate = "/keepitsecret")]
			int SecretMethod();

			[WebInvoke(Method = "DELETE", UriTemplate = "/voidtest")]
			void VoidMethod(
				bool bl,
				byte bt,
				sbyte sbt,
				char ch,
				decimal dm,
				double db,
				float fl,
				int it,
				uint uit,
				long lg,
				ulong ulg,
				short st,
				ushort ust,
				string str,
				DateTime dt,
				Guid guid,
				Stream stream);

			[WebGet(UriTemplate = "/hideparamtest")]
			int HideParamTest(int foo, [ParameterSettings(Hidden = true)]string bar);

			[WebGet(UriTemplate = "/overridereturntype")]
			[OverrideReturnType(typeof(string))]
			int OverrideReturnTypeTest();

			[WebGet(UriTemplate = "/customtypetest")]
			SampleService.CustomDataContractSample CustomTypeTest();

			[WebInvoke(UriTemplate = "/overrideparamtypeascustomtypetest", Method = "POST")]
			SampleService.MyRespClass OverrideParamTypeAsCustomType(string req);

		}

		class MapTest : IMapTest
		{
			public int Method(string uno, [ParameterSettings(IsRequired = true)]string dos, string tres) { throw new NotImplementedException(); }

			public int SecretMethod() { throw new NotImplementedException(); }

			public void VoidMethod(
				bool bl,
				byte bt,
				sbyte sbt,
				char ch,
				decimal dm,
				double db,
				float fl,
				int it,
				uint uit,
				long lg,
				ulong ulg,
				short st,
				ushort ust,
				string str,
				DateTime dt,
				Guid guid,
				Stream stream)
			{
				throw new NotImplementedException();
			}

			public int HideParamTest(int foo, string bar) { throw new NotImplementedException();}

			public int OverrideReturnTypeTest() { throw  new NotImplementedException();}

			public SampleService.CustomDataContractSample CustomTypeTest(){throw new NotImplementedException();}

			public SampleService.MyRespClass OverrideParamTypeAsCustomType(string req) { throw new NotImplementedException();}
		}
	}
}
