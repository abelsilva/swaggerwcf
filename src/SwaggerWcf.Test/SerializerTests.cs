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
 * SerializerTests.cs : Tests for serializer output of methods/models/properties
 */


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;


namespace SwaggerWcf.Test
{
	[TestClass]
	public class SerializerTests
	{
		[TestMethod]
		public void CanWriteCompositeType()
		{
			Serializer serializer = new Serializer(null);
			string model = serializer.WriteType(typeof (SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(model));

			var obj = JObject.Parse(model);

			Assert.AreEqual("SampleService.CompositeType", obj["id"]);

			var props = obj["properties"] as JObject;
			Assert.IsNotNull(props);
			Assert.IsTrue(props.HasValues);
			Assert.AreEqual(7, props.Count);

			Assert.AreEqual(true, props["BoolValue"]["required"]);
			Assert.AreEqual("array", props["ArrayValue"]["type"]);
			Assert.AreEqual("string", props["EnumValue"]["type"]);
			Assert.AreEqual(false, props["EnumValue"]["required"]);
			Assert.AreEqual("integer(16)", props["ShortValue"]["type"]);
		}

		[TestMethod]
		public void CanWriteTypeStack()
		{
			Serializer serializer = new Serializer(new[] {"InternalUse"});
			Stack<Type> typeStack = new Stack<Type>();
			typeStack.Push(typeof (SampleService.CompositeType));

			string models = serializer.WriteDefinitions(typeStack);

			var obj = JObject.Parse(HttpUtility.UrlDecode(models));

			Assert.AreEqual(2, obj.Count);
			Assert.IsNotNull(obj["SampleService.CompositeType"]);
		}

		[TestMethod]
		public void CanWriteContainerProperty()
		{
			Serializer serializer = new Serializer(null);

			string model = serializer.WriteType(typeof (SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(model));

			var obj = JObject.Parse(HttpUtility.UrlDecode(model));

			Assert.AreEqual("SampleService.CompositeType", obj["id"].ToString());

			var container = obj["properties"]["ArrayValue"];
			Assert.IsNotNull(container);
			Assert.AreEqual("array", container["type"]);
			Assert.AreEqual("string", container["items"]["$ref"]);

			var enumProperty = obj["properties"]["EnumValue"];
			Assert.IsNotNull(enumProperty);
			Assert.AreEqual("string", enumProperty["type"]);

			var enumValues = enumProperty["enum"] as JArray;
			Assert.AreEqual(3, enumValues.Count);
			Assert.IsTrue(enumValues.Any(v => v.ToString().Equals("Alpha")));
		}

		[TestMethod]
		public void CanHideHiddenTypes()
		{
			//gets the Secret property when it's tag isn't configured
			var serializerAll = new Serializer(null);

			string modelAll = serializerAll.WriteType(typeof (SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(modelAll));

			var objAll = JObject.Parse(modelAll);
			Assert.IsNotNull(objAll["properties"]["ArrayValue"]);
			Assert.IsNotNull(objAll["properties"]["Secret"]);

			//hides it when it is
			var serializer = new Serializer(new[] {"InternalUse"});

			string model = serializer.WriteType(typeof (SampleService.CompositeType), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(model));

			var obj = JObject.Parse(model);
			Assert.IsNotNull(obj["properties"]["ArrayValue"]);
			Assert.IsNull(obj["properties"]["Secret"]);
		}

		[TestMethod]
		public void CanHideTaggedTypes()
		{
			var typeStack = new Stack<Type>();
			typeStack.Push(typeof (ModelSampleA));
			typeStack.Push(typeof (ModelSampleB));

			//gets the Secret property when it's tag isn't configured
			var serializerAll = new Serializer(null);

			string modelAll = serializerAll.WriteDefinitions(typeStack);
			Assert.IsFalse(string.IsNullOrEmpty(modelAll));

			var objAll = JObject.Parse(modelAll) as JObject;
			Assert.AreEqual(2, objAll.Count);
			Assert.AreEqual(0, typeStack.Count);

			var serializerTags = new Serializer(new[] {"Test"});

			typeStack.Push(typeof (ModelSampleA));
			typeStack.Push(typeof (ModelSampleB));

			string modelHidden = serializerTags.WriteDefinitions(typeStack);
			Assert.IsFalse(string.IsNullOrEmpty(modelHidden));
			var objHidden = JObject.Parse(modelHidden) as JObject;
			Assert.AreEqual(1, objHidden.Count);

		}

		[TestMethod]
		public void CanWriteMemberProperties()
		{
			var serializer = new Serializer(null);
			var type = serializer.WriteType(typeof (ModelSampleA), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(type));
			var obj = JObject.Parse(type);
			var container = obj["properties"]["MyString"];
			Assert.IsNotNull(container);
			Assert.AreEqual("string(10)", container["type"]);
			Assert.AreEqual("my string description", container["description"]);

			var anotherSerializer = new Serializer(null);
			var sampleCModelSerialized = anotherSerializer.WriteType(typeof (ModelSampleC), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(sampleCModelSerialized));
			var sbObj = JObject.Parse(sampleCModelSerialized);
			var sbContainer = sbObj["properties"]["MyString2"];
			Assert.IsNotNull(sbContainer);
			Assert.AreEqual("string", sbContainer["type"]);
		}

		[TestMethod]
		public void CanWriteDataContractName()
		{
			var serializer = new Serializer(null);
			var serialized = serializer.WriteType(typeof (ModelSampleWithDataContractName), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(serialized));
			var jObj = JObject.Parse(serialized);
			Assert.AreEqual("ModelSampleName", jObj["id"]);
		}

		[TestMethod]
		public void CanWriteDataMemberName()
		{
			var serializer = new Serializer(null);
			var serialized = serializer.WriteType(typeof(ModelSampleWithDataContractName), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(serialized));
			var jObj = JObject.Parse(serialized);
			Assert.AreEqual("ModelSampleName", jObj["id"]);
			Assert.IsNotNull(jObj["properties"]["CustomMemberName"]);
		}

		[TestMethod]
		public void CanWriteCustomReturnTypeNameInDataContractWithoutFullName()
		{
			var serializer = new Serializer(null);
			var serialized = serializer.WriteType(typeof (ModelSampleWithDataContractName), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(serialized));
			var jObj = JObject.Parse(serialized);
			Assert.AreEqual("ModelSampleName", jObj["id"]);
			var container = jObj["properties"]["CustomReturnType"];
			Assert.IsNotNull(container);
			Assert.AreEqual("ModelSampleReferenced", container["type"]);
		}

		[TestMethod]
		public void CanWriteArrayTypeNameWithDataContractName()
		{
			var serializer = new Serializer(null);
			var serialized = serializer.WriteType(typeof (ModelSampleC), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(serialized));
			var jObj = JObject.Parse(serialized);
			Assert.IsNotNull(jObj["properties"]["ArrayOfSamples"]);
			var arrayElementTypeValue = jObj["properties"]["ArrayOfSamples"]["items"]["$ref"];
			Assert.AreEqual("ModelSampleName", arrayElementTypeValue);
		}

		[TestMethod]
		public void CanWriteArrayMemberProperties()
		{
			var serializer = new Serializer(null);
			var serialized = serializer.WriteType(typeof(ModelSampleC), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(serialized));
			var jObj = JObject.Parse(serialized);
			Assert.IsNotNull(jObj["properties"]["ArrayOfSamples"]);
			var arrayElementTypeValue = jObj["properties"]["ArrayOfSamples"]["items"]["$ref"];
			Assert.AreEqual("ModelSampleName", arrayElementTypeValue);
			var container = jObj["properties"]["ArrayOfSamples"];
			Assert.IsNotNull(container);
			Assert.AreEqual("my list description", container["description"]);
		}

		[TestMethod]
		public void CanWriteParentChildContractsInCorrectOrder()
		{
			var serializer = new Serializer(null);
			var serialized = serializer.WriteType(typeof(ModelSampleFurtherDerived), new Stack<Type>());
			Assert.IsFalse(string.IsNullOrEmpty(serialized));
			var jObj = JObject.Parse(serialized);
			var properties = jObj["properties"];
			Assert.AreEqual(6, properties.Count());
			Assert.AreEqual("TestBaseString", ((JProperty)properties.ElementAt(0)).Name);
			Assert.AreEqual("TestBaseString2", ((JProperty)properties.ElementAt(1)).Name);
			Assert.AreEqual("TestDerivedString1", ((JProperty)properties.ElementAt(2)).Name);
			Assert.AreEqual("TestDerivedString2", ((JProperty)properties.ElementAt(3)).Name);
			Assert.AreEqual("TestFurtherDerivedString1", ((JProperty)properties.ElementAt(4)).Name);
			Assert.AreEqual("TestFurtherDerivedString2", ((JProperty)properties.ElementAt(5)).Name);
		}


		[DataContract]
		[Tag("Test")]
		internal class ModelSampleA
		{
			[DataMember]
			[MemberProperties(TypeSizeNote = "10", Description = "my string description")]
			public string MyString { get; set; }
		}

		internal class ModelSampleB
		{
			public string MyString { get; set; }
		}

		internal class ModelSampleC
		{
			[DataMember]
			public string MyString2 { get; set; }

			[DataMember]
			[MemberProperties(Description = "my list description")]
			public List<ModelSampleWithDataContractName> ArrayOfSamples { get; set; }
		}

		[DataContract(Name = "ModelSampleName")]
		internal class ModelSampleWithDataContractName
		{
			[DataMember(Name = "CustomMemberName")]
			public string MyString3 { get; set; }

			[DataMember(Name = "CustomReturnType")]
			public ModelSampleReferencedByAnotherDataContract ModelSampleRefd { get; set; }
		}

		[DataContract(Name = "ModelSampleReferenced")]
		internal class ModelSampleReferencedByAnotherDataContract
		{
			[DataMember(Name = "Bar")]
			public string Foo { get; set; }
		}

		[DataContract(Name = "ModelSampleBase")]
		internal class ModelSampleBase
		{
			[DataMember(Name = "TestBaseString")]
			public string TestBaseString { get; set; }

			[DataMember(Name = "TestBaseString2")]
			public string TestBaseString2 { get; set; }
		}

		[DataContract(Name = "ModelSampleFirstDerived")]
		internal class ModelSampleFirstDerived : ModelSampleBase
		{
			[DataMember(Name = "TestDerivedString1")]
			public string TestDerivedString1 { get; set; }

			[DataMember (Name = "TestDerivedString2")]
			public string TestDerivedString2 { get; set; }
		}

		[DataContract(Name = "ModelSampleFurtherDerived")]
		internal class ModelSampleFurtherDerived : ModelSampleFirstDerived
		{
			[DataMember(Name = "TestFurtherDerivedString1")]
			public string TestFurtherDerivedString1 { get; set; }

			[DataMember(Name = "TestFurtherDerivedString2")]
			public string TestFurtherDerivedString2 { get; set; }
		}
	}
	

}
