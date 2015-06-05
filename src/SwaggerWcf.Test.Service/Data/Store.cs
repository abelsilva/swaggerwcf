using System;
using System.Collections.Generic;

namespace SwaggerWcf.Test.Service.Data
{
    public class Store
    {
        static Store()
        {
            Books = new List<Book>();
            Languages = new List<Language>();
            Authors = new List<Author>();

            var langSpanish = new Language
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Spanish"
            };
            var langEnglish = new Language
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "English"
            };
            var langFrench = new Language
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "French"
            };
            var langChinese = new Language
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Chinese"
            };
            Languages.Add(langSpanish);
            Languages.Add(langEnglish);
            Languages.Add(langFrench);
            Languages.Add(langChinese);

            var authorMiguelCervantes = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Miguel de Cervantes"
            };
            var authorCharlesDickens = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Charles Dickens"
            };
            var authorJRRTolkien = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "J. R. R. Tolkien"
            };
            var authorAntoineSaintExupery = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Antoine de Saint-Exupéry"
            };
            var authorJKRowling = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "J. K. Rowling"
            };
            var authorAgathaChristie = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Agatha Christie"
            };
            var authorCaoXueqin = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Cao Xueqin"
            };
            var authorHRiderHaggard = new Author
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "H. Rider Haggard"
            };
            Authors.Add(authorMiguelCervantes);
            Authors.Add(authorCharlesDickens);
            Authors.Add(authorJRRTolkien);
            Authors.Add(authorAntoineSaintExupery);
            Authors.Add(authorJKRowling);
            Authors.Add(authorAgathaChristie);
            Authors.Add(authorCaoXueqin);
            Authors.Add(authorHRiderHaggard);

            var bookDonQuixote = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Don Quixote",
                Author = authorMiguelCervantes,
                FirstPublished = 1605,
                Language = langSpanish
            };
            var bookATaleOfTwoCities = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "A Tale Of Two Cities",
                Author = authorCharlesDickens,
                FirstPublished = 1859,
                Language = langEnglish
            };
            var bookTheLordOfTheRings = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "The Lord of the Rings",
                Author = authorJRRTolkien,
                FirstPublished = 1954,
                Language = langEnglish
            };
            var bookTheHobbit = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "The Hobbit",
                Author = authorJRRTolkien,
                FirstPublished = 1937,
                Language = langEnglish
            };
            var bookLePetitPrince = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Le Petit Prince",
                Author = authorAntoineSaintExupery,
                FirstPublished = 1943,
                Language = langFrench
            };
            var bookHarryPotterAndThePhilosopherStone = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Harry Potter and the Philosopher's Stone",
                Author = authorJKRowling,
                FirstPublished = 1997,
                Language = langEnglish
            };
            var bookAndThenThereWereNone = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "And Then There Were None",
                Author = authorAgathaChristie,
                FirstPublished = 1939,
                Language = langEnglish
            };
            var bookDreamOfTheRedChamber = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "紅樓夢/红楼梦 (Dream of the Red Chamber)",
                Author = authorCaoXueqin,
                FirstPublished = 1754,
                Language = langChinese
            };
            var bookSheAHistoryOfAdventure = new Book
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "She: A History of Adventure",
                Author = authorHRiderHaggard,
                FirstPublished = 1887,
                Language = langEnglish
            };
            Books.Add(bookDonQuixote);
            Books.Add(bookATaleOfTwoCities);
            Books.Add(bookTheLordOfTheRings);
            Books.Add(bookTheHobbit);
            Books.Add(bookLePetitPrince);
            Books.Add(bookHarryPotterAndThePhilosopherStone);
            Books.Add(bookAndThenThereWereNone);
            Books.Add(bookDreamOfTheRedChamber);
            Books.Add(bookSheAHistoryOfAdventure);
        }

        public static readonly List<Book> Books;
        public static readonly List<Language> Languages;
        public static readonly List<Author> Authors;
    }
}
