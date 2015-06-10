SwaggerWcf ![nuget status](http://img.shields.io/nuget/v/SwaggerWcf.svg?style=flat)
==========

Generates [Swagger](http://swagger.io/) (2.0) for WCF services and also provides [swagger-ui](https://github.com/swagger-api/swagger-ui).

With an API described in Swagger you can use multiple Swagger tools like client generators, see [swagger-codegen](https://github.com/swagger-api/swagger-codegen) for more details.

This project has started as a fork from [superstator/Swaggeratr](https://github.com/superstator/Swaggeratr) to implement version 2.0 of Swagger.

## Getting Started

### Step 1: Install SwaggerWcf package

```

Install-Package SwaggerWcf

```

### Step 2: Configure WCF routes in `Global.asax`

Add the route in the `Application_Start` method

```csharp

protected void Application_Start(object sender, EventArgs e)
{
    // [.......]
    
    RouteTable.Routes.Add(new ServiceRoute("api-docs", new WebServiceHostFactory(), typeof(SwaggerWcfEndpoint)));
}

```

### Step 3: Optionaly configure WCF response auto types

Add the following to you `Web.config`.
This will allow the WCF service to accept requests and send replies based on the `Content-Type` headers.

```xml

<system.serviceModel>
  <behaviors>
    <endpointBehaviors>
      <behavior>
        <webHttp automaticFormatSelectionEnabled="true" />
      </behavior>
    </endpointBehaviors>
    <!-- [.......] -->
  </behaviors>
</system.serviceModel>
  
```

### Step 4: Configure WCF services general information

Add the following to you `Web.config` and change the values accordingly

```xml

<configSections>
  <section name="swaggerwcf" type="SwaggerWcf.Configuration.SwaggerWcfSection, SwaggerWcf" />
</configSections>

<swaggerwcf>
  <tags>
    <tag name="LowPerformance" visible="false" />
  </tags>
  <settings>
    <setting name="InfoDescription" value="Sample Service to test SwaggerWCF" />
    <setting name="InfoVersion" value="0.0.1" />
    <setting name="InfoTermsOfService" value="Terms of Service" />
    <setting name="InfoTitle" value="SampleService" />
    <setting name="InfoContactName" value="Abel Silva" />
    <setting name="InfoContactUrl" value="http://github.com/abelsilva" />
    <setting name="InfoContactEmail" value="no@e.mail" />
    <setting name="InfoLicenseUrl" value="https://github.com/abelsilva/SwaggerWCF/blob/master/LICENSE" />
    <setting name="InfoLicenseName" value="Apache License" />
  </settings>
</swaggerwcf>

```

Notes:
* make sure the `configSections` block is the first child of `configuration`
* `tags` will be described further down

### Step 5: Decorate WCF services interfaces

For each method, configure the `WebInvoke` or `WebGet` attribute, and add a `SwaggerWcfPath` attribute.

```csharp

[ServiceContract]
public interface IStore
{
    [SwaggerWcfPath("Create book", "Create a book on the store")]
    [WebInvoke(UriTemplate = "/books",
        BodyStyle = WebMessageBodyStyle.Bare,
        Method = "POST",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
    [OperationContract]
    Book CreateBook(Book value);
    
    // [.......]
}

```

### Step 6: Decorate WCF services class

Add the `SwaggerWcf` attribute to the class providing the base path for the service (the same as used in step 2).
Optinally, for each method, add the `SwaggerWcfTag` to categorize the method and the `SwaggerWcfResponse` for each possible response from the service.

```csharp

[SwaggerWcf("/v1/rest")]
public class BookStore : IStore
{
    [SwaggerWcfTag("Books")]
    [SwaggerWcfResponse(HttpStatusCode.Created, "Book created, value in the response body with id updated")]
    [SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", true)]
    [SwaggerWcfResponse(HttpStatusCode.InternalServerError,
        "Internal error (can be forced using ERROR_500 as book title)", true)]
    public Book CreateBook(Book value)
    {
        // [.......]
    }
    
    // [.......]
}

```

### Step 7: Optionaly decorate data types used in WCF services

```csharp

[DataContract(Name = "book")]
[Description("Book with title, first publish date, author and language")]
[SwaggerWcfDefinition(ExternalDocsUrl = "http://en.wikipedia.org/wiki/Book", ExternalDocsDescription = "Description of a book")]
public class Book
{
    [DataMember(Name = "id")]
    [Description("Book ID")]
    public string Id { get; set; }

    // [.......]
}

```

## Attributes

| Attribute              | Used in                                    | Description                   | Options                                                                                             |
| ---------------------- |------------------------------------------- | ----------------------------- | --------------------------------------------------------------------------------------------------- |
| `SwaggerWcf`           | `Class`, `Interface`                       | Enable parsing WCF service    | `ServicePath`                                                                                       |
| `SwaggerWcfHidden`     | `Class`, `Method`, `Property`, `Parameter` | Hide element from Swagger     |                                                                                                     |
| `SwaggerWcfTag`        | `Class`, `Method`, `Property`, `Parameter` | Add a tag to an element       | `TagName`, `HideFromSpec`                                                                           |
| `SwaggerWcfPath`       | `Method`                                   | Configure a method in Swagger | `Summary`, `Description`, `OperationId`, `ExternalDocsDescription`, `ExternalDocsUrl`, `Deprecated` |
| `SwaggerWcfParameter`  | `Parameter`                                | Configure method parameters   | `Required`, `Description`                                                                           |
| `SwaggerWcfResponse`   | `Method`                                   | Configure method return value | `Code`, `Description`, `EmptyResponseOverride`, `Headers`                                           |
| `SwaggerWcfDefinition` | `Class`                                    | Configure a data type         | `ExternalDocsDescription`, `ExternalDocsUrl`                                                        |

## Tags

Tags are used to create categories in Swagger UI.

In SwaggerWcf they can also be used to hide elements from the Swagger output using the `Web.config`

Using the configuration from step 4, any elements with the tag `LowPerformance` will be hidden from Swagger.

When a `SwaggerWcfTag` is added to an element, it may be configured with `HideFromSpec`.
This will prevent this tag to be displayed in the Swagger output.

## TODO

* Add some options to configuration in `Web.config`
* Tests

## How to Improve It

Fork this project [abelsilva/swaggerwcf](https://github.com/abelsilva/swaggerwcf) and submit pull requests.
