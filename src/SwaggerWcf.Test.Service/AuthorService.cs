using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SwaggerWcf.Test.Service
{
    [SwaggerWcf("/v1/authors")]
    [SwaggerWcfTag("Authors")]
    public class AuthorService : BaseService<Author>, IAuthorService
    {
        [SwaggerWcfTag("GenericAuthor")]
        public override Author Get(string id)
        {
            return new Author { Id = id };
        }

        [SwaggerWcfTag("GenericAuthor")]
        public override Author Create(Author item)
        {
            return item;
        }

        [SwaggerWcfTag("GenericAuthor")]
        public override Author Update(string id, Author item)
        {
            return new Author { Id = id };
        }

        [SwaggerWcfTag("GenericAuthor")]
        public override Author Delete(string id)
        {
            return new Author { Id = id };
        }
    }
}