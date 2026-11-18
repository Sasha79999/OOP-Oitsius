using System;
using System.Collections.Generic;
using Xunit;

namespace IndependentWork7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_ShouldStoreValue()
        {
            Cache<string, string> cache = new Cache<string, string>(3);

            cache.Add("a", "1");

            Assert.True(cache.ContainsKey("a"));
        }

        [Fact]
        public void Get_ShouldReturnValueByKey()
        {
            Cache<string, string> cache = new Cache<string, string>(3);
            cache.Add("a", "1");
            cache.Add("b", "2");

            Assert.Equal("2", cache.Get("b"));
        }

        [Fact]
        public void Add_WhenFull_ShouldRemoveOldest()
        {
            Cache<string, string> cache = new Cache<string, string>(2);
            cache.Add("a", "1");
            cache.Add("b", "2");

            cache.Add("c", "3");

            Assert.False(cache.ContainsKey("a"));
        }

        [Fact]
        public void Add_WhenFull_ShouldKeepNewestItems()
        {
            Cache<string, string> cache = new Cache<string, string>(2);
            cache.Add("a", "1");
            cache.Add("b", "2");

            cache.Add("c", "3");

            Assert.True(cache.ContainsKey("b"));
            Assert.True(cache.ContainsKey("c"));
        }

        [Fact]
        public void Get_MissingKey_ShouldThrowKeyNotFoundException()
        {
            Cache<string, string> cache = new Cache<string, string>(2);

            Assert.Throws<KeyNotFoundException>(() => cache.Get("x"));
        }

        [Fact]
        public void ContainsKey_ExistingKey_ShouldReturnTrue()
        {
            Cache<string, string> cache = new Cache<string, string>(2);
            cache.Add("a", "1");

            Assert.True(cache.ContainsKey("a"));
        }

        [Fact]
        public void ContainsKey_MissingKey_ShouldReturnFalse()
        {
            Cache<string, string> cache = new Cache<string, string>(2);

            Assert.False(cache.ContainsKey("x"));
        }

        [Fact]
        public void Count_NewCache_ShouldBeZero()
        {
            Cache<string, string> cache = new Cache<string, string>(2);

            Assert.Equal(0, cache.Count);
        }

        [Fact]
        public void Count_ShouldNotExceedCapacity()
        {
            Cache<string, string> cache = new Cache<string, string>(2);
            cache.Add("a", "1");
            cache.Add("b", "2");
            cache.Add("c", "3");
            cache.Add("d", "4");

            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void Add_ExistingKey_ShouldUpdateValueWithoutRemovingOthers()
        {
            Cache<string, string> cache = new Cache<string, string>(2);
            cache.Add("a", "1");
            cache.Add("b", "2");

            cache.Add("a", "10");

            Assert.Equal("10", cache.Get("a"));
            Assert.True(cache.ContainsKey("b"));
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void Constructor_InvalidCapacity_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Cache<string, string>(0));
        }
    }
}