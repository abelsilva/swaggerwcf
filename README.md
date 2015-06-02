SwaggerWcf
==========

Swagger implementation for WCF REST services.

## Getting Started

You'll need two things - a "discovery" service that lets Swagger know what services are present, and an implemented service you want to make discoverable.

### Scanner
To create the discovery service, just add ```[Swaggerator.Scanner]``` to your RouteTable in Global.asax, like so: 
```
RouteTable.Routes.Add(new ServiceRoute("api-docs", new WebServiceHostFactory(), typeof(Swaggerator.Scanner)));
```
The standard for Swagger services is to live under /api-docs, but you can put it wherever you like.

### Swaggerizing
To make your service visible to the discovery service, add the ```[Swaggerated]``` tag to a WCF service.
```
[Swaggerator.Attributes.Swaggerated("/rest","A RESTful WCF Service")]
public class RESTful : IRESTful
```
Note this is the service implementation, not the DataContract inferface. WCF allows for multiple implementations of a single contract, so you need to add the ```[Swaggerated]``` attribute to a specific implementation for Swaggerator to know exactly what it going on. Other attributes like ```[OperationSummary]``` or ```[ResponseCode]``` can be applied to either implementation or declaration.

The first argument is required - it tells Swagger where your service is actually hosted within your project.

The second argument is optional. If you leave it off, Swaggerator will look for a ```[Description]``` annotation. If it can't find a description, the text will just be left blank.

### Being Selective
If you have aspects of your service you'd rather not advertise, the ```[Hidden]``` attribute is your friend. For instance, if you have a method that's only available to internal users, and your Swagger docs will be exposed to external users, just add ```[Hidden]``` to the method declaration, either in the DataContract (to apply to any implementation), or in a specific implementation.

You can add the ```[Hidden]``` attribute to classes to keep them out of the ```models``` declaration. Note, if public methods use that type as a parameter or return type, the type name will still be visible there. Users just won't be able to see the types format.

Lastly, you can add the ```[Hidden]``` attribute to a specific property of a type. The rest of the type will still be returned in the ```models``` section.

If you want more finegrained control of display things, you can use the ```[Tag()]``` attribute to identify something, and then specify in configuration whether those things are visible or not. For instance, you could tag some special methods as "InternalUse", and then add the following to your production web.config:

```
  <configSections>
    <section name="swagger" type="Swaggerator.Configuration.SwaggerSection, Swaggerator" />
  </configSections>

  <swagger>
    <tags>
      <tag name="InternalUse" visible="false" />
    </tags>
  </swagger>
```

Meanwhile on your dev server, you'd set the visible property of the "InternalUse" tag to true, or leave it out of the config entirely. Note this does not prevent users accessing those methods if they aren't properly secured - it only prevents documentation for them being generated.

### Optional Settings
If you want to the method headers to show required query params, set "ShowRequiredQueryParamsInHeader" to true. If you want to show which methods are tagged, set "MarkTaggedMethods" to true (SwaggerWcf will apend "***" at the end of summary text).

```
  <swagger>
    <tags>
      <tag name="InternalUse" visible="false" />
    </tags>
    <settings>
      <setting name="ShowRequiredQueryParamsInHeader" value="true"/>
      <setting name="MarkTaggedMethods" value="true"/>
    </settings>
  </swagger>
```

### Markup

There are some other attributes that may come in handy as well, to add details to your documentation or to override default assumptions Swaggerator makes.

```[OperationSummary]``` lets you set a summary string for a service method. In swagger-ui, these are displayed alongside the method url, before the method details have been expanded.

```[OperationNotes]``` lets you set a longer string describing a service method. In swagger-ui these are displayed after clicking to expand a method.

```[ResponseCode]``` lets you enumerate the various codes your method may return. An optional string gives details, i.e. "409 - Username is already taken"

```[ParameterSettings]``` let you specifiy exactly how a method parameter works. By default swagger assumes all path parameters are required strings, all body parameters are required and typed, and all query parameters are optional string. Using parameter settings you can specify that a given query param is required, or that a path parameter should be an integer, etc.

```[OverrideReturnType]``` lets you override the return type of a method. It is useful for documenting asynchronous methods.

```[MemberProperties]``` let you specify data type size information and description for a DataMember. For example, if you specify DataTypeNote property with "10" for a member that retunrs a string, it will display as "string(10)."

```[Accepts]``` and ```[Produces]``` let you define the content-types your method will work with. By default Swaggerator will assume xml & json for all methods, but this will let you narrow to one specific format, or call out a streamed format like "image/jpg".

Attributes applied to the method implementation will override attributes applied to the method declaration.

### That's it!

Now get a Swagger-compliant tool, like swagger-ui, and point it at your newly swaggerized WCF service. By default, you'll point it at \<yourserver\>/api-docs.json, but if you modified the route in the first step above, make the appropriate adjustments. Happy swaggerizing!

### License


Copyright (c) 2014 Digimarc Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
