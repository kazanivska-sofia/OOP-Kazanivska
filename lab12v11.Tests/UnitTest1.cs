using System;
using System.Collections.Generic;
using lab12v11;
using Xunit;

namespace lab12v11.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public void Add_ShouldAddMovieSuccessfully()
        {
            var repo = new InMemoryRepository<Movie>();
            var movie = new Movie { Id = 1, Title = "Interstellar", Director = "Nolan", ReleaseYear = 2014 };

            repo.Add(movie);

            Assert.Equal(1, repo.GetById(1).Id);
        }

        [Fact]
        public void GetById_ExistingId_ReturnsCorrectMovie()
        {
            var repo = new InMemoryRepository<Movie>();
            repo.Add(new Movie { Id = 10, Title = "Dune", Director = "Villeneuve", ReleaseYear = 2021 });

            var result = repo.GetById(10);

            Assert.NotNull(result);
            Assert.Equal("Dune", result.Title);
        }

        [Fact]
        public void GetById_NonExistingId_ThrowsException()
        {
            var repo = new InMemoryRepository<Movie>();

            Action act = () => repo.GetById(999);

            Assert.Throws<KeyNotFoundException>(act);
        }

        [Fact]
        public void Update_ExistingMovie_UpdatesMovieData()
        {
            var repo = new InMemoryRepository<Movie>();
            repo.Add(new Movie { Id = 1, Title = "Old Title", Director = "Dir", ReleaseYear = 2000 });

            repo.Update(new Movie { Id = 1, Title = "New Title", Director = "Dir", ReleaseYear = 2000 });

            Assert.Equal("New Title", repo.GetById(1).Title);
        }

        [Fact]
        public void Delete_ExistingMovie_RemovesMovie()
        {
            var repo = new InMemoryRepository<Movie>();
            repo.Add(new Movie { Id = 1, Title = "Title", Director = "Dir", ReleaseYear = 2000 });

            repo.Delete(1);

            Action act = () => repo.GetById(1);

            Assert.Throws<KeyNotFoundException>(act);
        }
    }
}
