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
 * CompositeType.cs : Sample class demonstrating a complex model with tagged properties and multiple property types.
 */

using SwaggerWcf.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SampleService
{
    [DataContract]
    [Description("CompositeType Description")]
    [SwaggerWcfTag("InternalUse")]
    public class CompositeType
    {
        public CompositeType()
        {
            BoolValue = true;
            ArrayValue = new List<string>() { "Foo", "Bar", "Baz" };
        }

        [DataMember]
        [Description("Whatever you do don't set this to")]
        public bool BoolValue
        { get; set; }

        [SwaggerWcfHidden]
        [DataMember]
        public string StringValue
        { get; set; }

        [SwaggerWcfTag("InternalUse")]
        [DataMember(EmitDefaultValue = false)]
        public SecretObject Secret
        { get; set; }

        [DataMember]
        public List<string> ArrayValue
        { get; set; }

        [DataMember]
        public EnumType? EnumValue
        { get; set; }

        [DataMember]
        public short ShortValue
        { get; set; }

        [DataMember(IsRequired = true)]
        [Description("Description text")]
        public string StringValueWithLengthRestriction
        { get; set; }

        [DataMember]
        [Description("List description text")]
        public List<SubContractSample> SampleList
        { get; set; }
    }

    public enum EnumType
    {
        Alpha,
        Beta,
        Gamma
    }

    [DataContract]
    public class SecretObject
    {
        [DataMember]
        public string SecretData
        { get; set; }
    }

    [DataContract(Name = "MyDataContractName")]
    public class CustomDataContractSample
    {
        [DataMember(Name = "Foo")]
        public string SampleString
        { get; set; }

        [DataMember(Name = "Bar")]
        public SubContractSample SampleMemberTwo
        { get; set; }
    }

    [DataContract(Name = "SubContractClassUserDefinedName")]
    public class SubContractSample
    {
        [DataMember(Name = "SubContractUserDefinedDataMemberName")]
        public string SubContractSampleString
        { get; set; }
    }

    [DataContract(Name = "MyRequest")]
    public class MyReqClass
    {
        [DataMember(Name = "MyRequesetMember")]
        public string MyRespString
        { get; set; }
    }

    [DataContract(Name = "MyResonse")]
    public class MyRespClass
    {
        [DataMember(Name = "MyResponseMember")]
        public string MyReqString
        { get; set; }
    }

    [DataContract(Name = "MySampleBase")]
    public class MySampleBase
    {
        [DataMember(Name = "TestBaseString")]
        public string TestBaseString
        { get; set; }

        [DataMember(Name = "TestBaseString2")]
        public string TestBaseString2
        { get; set; }
    }

    [DataContract(Name = "MySampleFirstDerived")]
    public class MySampleFirstDerived : MySampleBase
    {
        [DataMember(Name = "TestDerivedString1")]
        public string TestDerivedString1
        { get; set; }

        [DataMember(Name = "TestDerivedString2")]
        public string TestDerivedString2
        { get; set; }
    }

    [DataContract(Name = "ModelSampleFurtherDerived")]
    public class MySampleFurtherDerived : MySampleFirstDerived
    {
        [DataMember(Name = "TestFurtherDerivedString1")]
        public string TestFurtherDerivedString1
        { get; set; }

        [DataMember(Name = "TestFurtherDerivedString2")]
        public string TestFurtherDerivedString2
        { get; set; }
    }
}