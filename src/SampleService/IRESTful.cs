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
 * IRESTful.cs : Sample service declaration showing multiple methods, with summary and notes attributes
 */

using System;
using SwaggerWcf.Attributes;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace SampleService
{
    [ServiceContract]
    public interface IRESTful
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Data/{value}")]
        string GetData(string value);
        
        [SwaggerWcfTag("InternalUse")]
        [SwaggerWcfTag("InternalUse1")]
        [SwaggerWcfPath(summary: "Does stuff.", description: "I mean, it does some really interesting stuff. Stuff like you wouldn't believe.")]
        [SwaggerWcfResponse(400, "Four hundred error")]
        [SwaggerWcfResponse(200, "OK")]
        [SwaggerWcfResponse(205, "Some error")]
        [SwaggerWcfResponse(404, "Not found")]
        [SwaggerWcfResponse(401, "Something weird happened")]
        [SwaggerWcfResponse(301, "Three O one Something weird happened")]
        [OperationContract]
        [WebInvoke(UriTemplate = "/data", Method = "POST", RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        [WebInvoke(
            UriTemplate = "/Data/{value}?val={anothervalue}&option={optionalvalue}",
            Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string PutData(string value,
            [SwaggerWcfParameter(Required = true, Description = "Yes, you need to include this.")] string anothervalue,
            string optionalvalue,
            string valueWithLengthRequirement);

        [OperationContract]
        [WebGet(UriTemplate = "/List")]
        CompositeType[] GetList();

        [OperationContract]
        [WebInvoke(UriTemplate = "/Data/{value}",
            Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        void Delete(string value);

        [OperationContract]
        [WebInvokeAttribute(UriTemplate = "/Data2/{value}", Method = "POST")]
        [SwaggerWcfPath(summary: "Example for hiding a parameter", description: "The second parameter, object 'bar' is hidden")]
        string HideOneOfTwoParams(string value, [SwaggerWcfHidden]object bar);


        [WebGet(UriTemplate = "/Data2Asynch/{value}")]
        [SwaggerWcfPath(summary: "Example for hiding parameters and overriding return type", description: "Two parameters, AsynchCallback callback and object asyncState are hidden")]
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginServiceAsyncMethod(string value, [SwaggerWcfHidden]AsyncCallback callback, [SwaggerWcfHidden]object asyncState);

        // Note: There is no OperationContractAttribute for the end method.
        CompositeType EndServiceAsyncMethod(IAsyncResult result);

        [WebGet(UriTemplate = "/DisplayContractName")]
        CustomDataContractSample DisplayDataContractNameInsteadOfClassName();

        [WebInvoke(UriTemplate = "/DisplayContractNameForRespAndReq", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        MyRespClass DisplayContractNameForRespAndReqTest(
            [SwaggerWcfParameter(Description = "Request object")]MyReqClass req);

        [WebInvoke(UriTemplate = "/DisplayBaseClassProperties", Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        MySampleFurtherDerived DisplayBaseClassProperties(MySampleFirstDerived mySample);
    }



}
