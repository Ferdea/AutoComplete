using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Autocomplete
{
    internal class AutocompleteTask
    {
        /// <returns>
        /// Возвращает первую фразу словаря, начинающуюся с prefix.
        /// </returns>
        /// <remarks>
        /// Эта функция уже реализована, она заработает, 
        /// как только вы выполните задачу в файле LeftBorderTask
        /// </remarks>
        public static string FindFirstByPrefix(IReadOnlyList<string> phrases, string prefix)
        {
            var index = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) + 1;
            if (index < phrases.Count && phrases[index].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return phrases[index];
            
            return null;
        }

        /// <returns>
        /// Возвращает первые в лексикографическом порядке count (или меньше, если их меньше count) 
        /// элементов словаря, начинающихся с prefix.
        /// </returns>
        /// <remarks>Эта функция должна работать за O(log(n) + count)</remarks>
        public static string[] GetTopByPrefix(IReadOnlyList<string> phrases, string prefix, int count)
        {
            // тут стоит использовать написанный ранее класс LeftBorderTask
            return phrases
                .Skip(LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) + 1)
                .Where(s => s.StartsWith(prefix))
                .Take(count)
                .ToArray();
        }

        /// <returns>
        /// Возвращает количество фраз, начинающихся с заданного префикса
        /// </returns>
        public static int GetCountByPrefix(IReadOnlyList<string> phrases, string prefix)
        {
            // тут стоит использовать написанные ранее классы LeftBorderTask и RightBorderTask
            return RightBorderTask.GetRightBorderIndex(phrases, prefix, -1, phrases.Count) -
                   LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) - 1;
        }
    }

    [TestFixture]
    public class AutocompleteTests
    {
        [Test]
        public void TopByPrefix_IsEmpty_WhenNoPhrases()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { }, new string[] { }, new string[] { }),
                "aboba",
                3
            );
            CollectionAssert.IsEmpty(actualTopWords);
        }

        [Test]
        public void TopByPrefix_AnyPhrases_WhenPrefixIsEmptyString()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "aa", "ab", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                string.Empty,
                3
            );
            CollectionAssert.AreEqual(new string[] { "aa a a", "ab a a", "ba a a" }, actualTopWords);
        }

        [Test]
        public void TopByPrefix_IsEmpty_WhenPrefixLessThanAnyWord()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "ac", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "aa",
                3
            );
            CollectionAssert.IsEmpty(actualTopWords);
        }

        [Test]
        public void TopByPrefix_IsEmpty_WhenPrefixMoreThanAnyWord()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "ac", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "zz",
                3
            );
            CollectionAssert.IsEmpty(actualTopWords);
        }

        [Test]
        public void TopByPrefix_WhenPrefixInPhrases()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "ac", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "ac",
                3
            );
            CollectionAssert.AreEqual(new string[] { "ac a a" }, actualTopWords);
        }

        [Test]
        public void TopByPrefix_WhenPrefixWordsInPhrases()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "ac", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" },
                    new string[] { "a" }),
                "ac",
                3
            );
            CollectionAssert.AreEqual(new string[] { "ac a a", "aca a a", "acb a a" }, actualTopWords);
        }

        [Test]
        public void TopByPrefix_WhenPrefixWordsNotInPhrases()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "ac", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" },
                    new string[] { "a" }),
                "ad",
                3
            );
            CollectionAssert.IsEmpty(actualTopWords);
        }

        [Test]
        public void TopByPrefix_WhenPrefixNotInPhrases()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" },
                    new string[] { "a" }),
                "ac",
                3
            );
            CollectionAssert.AreEqual(new string[] { "aca a a", "acb a a" }, actualTopWords);
        }

        [Test]
        public void TopByPrefix_WhenCountPositiveButLessCountPrefixWords()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" },
                    new string[] { "a" }),
                "ac",
                1
            );
            CollectionAssert.AreEqual(new string[] { "aca a a" }, actualTopWords);
        }

        [Test]
        public void TopByPrefix_WhenCountMoreCountPrefixWords()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" },
                    new string[] { "a" }),
                "ac",
                100
            );
            CollectionAssert.AreEqual(new string[] { "aca a a", "acb a a" }, actualTopWords);
        }

        [Test]
        public void TopByPrefix_IsEmpty_WhenCountEqualZero()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" },
                    new string[] { "a" }),
                "ac",
                0
            );
            CollectionAssert.IsEmpty(actualTopWords);
        }

        [Test]
        public void TopByPrefix_IsEmpty_WhenCountIsNegative()
        {
            var actualTopWords = AutocompleteTask.GetTopByPrefix(
                new Phrases(new string[] { "ab", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" },
                    new string[] { "a" }),
                "ac",
                -5
            );
            CollectionAssert.IsEmpty(actualTopWords);
        }

        [Test]
        public void СountByPrefix_IsZero_WhenNoPhrases()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { }, new string[] { }, new string[] { }),
                "aboba"
            );
            Assert.AreEqual(0, actualCountWords);
        }
        
        [Test]
        public void СountByPrefix_WhenPrefixIsEmptyString()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { "aa", "ab", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                string.Empty
            );
            Assert.AreEqual(5, actualCountWords);
        }

        [Test]
        public void СountByPrefix_IsZero_WhenPrefixLessThanAnyPhrases()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { "ab", "ac", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "aa"
            );
            Assert.AreEqual(0, actualCountWords);
        }
        
        [Test]
        public void СountByPrefix_IsZero_WhenPrefixMoreThanAnyPhrases()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { "ab", "ac", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "zz"
            );
            Assert.AreEqual(0, actualCountWords);
        }
        
        [Test]
        public void СountByPrefix_IsZero_WhenPrefixInPhrases()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { "ab", "ac", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "ac"
            );
            Assert.AreEqual(1, actualCountWords);
        }
        
        [Test]
        public void СountByPrefix_IsZero_WhenPrefixWordsInPhrases()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { "ab", "ac", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "ac"
            );
            Assert.AreEqual(3, actualCountWords);
        }
        
        [Test]
        public void СountByPrefix_IsZero_WhenPrefixWordsNotInPhrases()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { "ab", "ac", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "ad"
            );
            Assert.AreEqual(0, actualCountWords);
        }
        
        [Test]
        public void СountByPrefix_IsZero_WhenPrefixNotInPhrases()
        {
            var actualCountWords = AutocompleteTask.GetCountByPrefix(
                new Phrases(new string[] { "ab", "aca", "acb", "ba", "bc", "gg" }, new string[] { "a" }, new string[] { "a" }),
                "ac"
            );
            Assert.AreEqual(2, actualCountWords);
        }
    }
}
