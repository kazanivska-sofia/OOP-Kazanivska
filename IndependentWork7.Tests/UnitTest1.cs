using System;
using System.Collections.Generic;
using Xunit;
using IndependentWork7;

namespace IndependentWork7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldReturnAddedValue()
        {
            // Arrange
            var cache = new Cache<string, int>(3);

            // Act
            cache.Add("one", 1);
            cache.Add("two", 2);

            // Assert
            Assert.Equal(1, cache.Get("one"));
            Assert.Equal(2, cache.Get("two"));
        }

        [Fact]
        public void Count_ShouldReflectCurrentNumberOfItems()
        {
            // Arrange
            var cache = new Cache<int, string>(3);
            Assert.Equal(0, cache.Count);

            // Act
            cache.Add(1, "A");
            cache.Add(2, "B");

            // Assert
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrueForExistingAndFalseForMissingKey()
        {
            // Arrange
            var cache = new Cache<string, string>(2);
            cache.Add("key1", "val1");

            // Assert
            Assert.True(cache.ContainsKey("key1"));
            Assert.False(cache.ContainsKey("key2"));
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var cache = new Cache<string, string>(2);
            cache.Add("key1", "val1");

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => cache.Get("missing_key"));
        }

        [Fact]
        public void Add_WhenCapacityExceeded_ShouldEvictOldestItem_FIFO()
        {
            // Arrange
            var cache = new Cache<string, string>(capacity: 2);
            cache.Add("first", "1");
            cache.Add("second", "2");

            // Act: додаємо третій елемент при ємності 2 -> "first" має видалитися
            cache.Add("third", "3");

            // Assert
            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey("first"));
            Assert.True(cache.ContainsKey("second"));
            Assert.True(cache.ContainsKey("third"));
            Assert.Throws<KeyNotFoundException>(() => cache.Get("first"));
        }

        [Fact]
        public void Add_ExistingKey_ShouldUpdateValueWithoutEvicting()
        {
            // Arrange
            var cache = new Cache<string, string>(capacity: 2);
            cache.Add("k1", "old");
            cache.Add("k2", "val2");

            // Act
            cache.Add("k1", "new");

            // Assert
            Assert.Equal(2, cache.Count);
            Assert.Equal("new", cache.Get("k1"));
            Assert.True(cache.ContainsKey("k2"));
        }
    }
}
