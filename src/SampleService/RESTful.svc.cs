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
 * RESTful.svc.cs : Sample service implementation
 */

using System.Threading;
using System.Web.Services.Description;
using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SampleService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [SwaggerWcf("/v1/rest")]
    public class RESTful : IRESTful
    {
		 public string GetData(string value)
		 {
			 return string.Format("You entered: {0}", value);
		 }

		 [Description("A detailed explanation of the fabulous things this method can do for you.")]
		 public CompositeType GetDataUsingDataContract(CompositeType composite)
		 {
			 if (composite == null)
			 {
				 throw new ArgumentNullException("composite");
			 }
			 if (composite.BoolValue)
			 {
				 composite.StringValue += "Suffix";
				 composite.Secret = new SecretObject { SecretData = "Boo!" };
			 }
			 return composite;
		 }

		 public CompositeType GetDataUsingDataContract(string composite)
		 {
			 return new CompositeType
			 {
				 BoolValue = false,
				 StringValue = "foobar"
			 };
		 }


		 public string PutData(string value, string anothervalue, string optionalvalue, string valueWithLengthRequirement)
		 {
			 return value + anothervalue + optionalvalue + valueWithLengthRequirement;
		 }

		 public CompositeType[] GetList()
		 {
			 return new[]
		   {
		 	  new CompositeType { StringValue="Stuff" },
		 	  new CompositeType { Secret=new SecretObject { SecretData="WHAT?!" } }
		   };
		 }

		 public void Delete(string value)
		 {

		 }

		 public string HideOneOfTwoParams(string foo, object bar)
		 {
			 return foo;
		 }

		 public IAsyncResult BeginServiceAsyncMethod(string msg, AsyncCallback callback, object asyncState)
		 {
			 return null;
		 }

		 public CompositeType EndServiceAsyncMethod(IAsyncResult result)
		 {
			 return new CompositeType();
		 }

	    public CustomDataContractSample DisplayDataContractNameInsteadOfClassName()
	    {
		    return new CustomDataContractSample();
	    }

	    public MyRespClass DisplayContractNameForRespAndReqTest(MyReqClass req)
	    {
			 return new MyRespClass();
	    }

	    public MySampleFurtherDerived DisplayBaseClassProperties(MySampleFirstDerived mySample)
	    {
		    return new MySampleFurtherDerived();
	    }
    }
}
