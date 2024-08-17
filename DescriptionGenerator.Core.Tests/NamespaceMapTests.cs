using FluentAssertions;

namespace DescriptionGenerator.Core.Tests
{
    public class NamespaceMapTests
    {
        [Fact]
        public void Generate_ShouldGenerateMap_FromDictionary()
        {
            var dictMap = new Dictionary<string, string>() { { "test", "not_test" }, {"ok", "not_ok" } };

            var namespaceMap = new NamespaceMap();
            namespaceMap.Generate(dictMap);

            foreach ( var item in dictMap ) 
            {
                namespaceMap.TryGetValue(item.Key, out var value).Should().BeTrue();
                item.Value.Should().Be(value);
            }

            dictMap.Count.Should().Be(namespaceMap.Value.Count());
        }

        [Fact]
        public void TryGetValue_ShouldReturnFalse_WhenItemIsNotPresentInMap()
        {
            var namespaceMap = new NamespaceMap();
            namespaceMap.SetValue("ok", "not_ok");

            namespaceMap.TryGetValue("not_a_valid_key", out var value).Should().BeFalse();
            value.Should().BeNullOrEmpty();
        }

        [Fact]
        public void TryGetValue_ShouldReturnTrueAndValue_WhenItemIsPresentInMap()
        {
            var namespaceMap = new NamespaceMap();
            namespaceMap.SetValue("ok", "not_ok");

            namespaceMap.TryGetValue("ok", out var value).Should().BeTrue();
            value.Should().Be("not_ok");
        }

        [Fact]
        public void SetValue_ShouldAddNewValue_WhenKeyIsNotPresentInMap()
        {
            var namespaceMap = new NamespaceMap();

            namespaceMap.SetValue("ok", "not_ok");

            namespaceMap.TryGetValue("ok", out var value).Should().BeTrue();
            value.Should().Be("not_ok");
            namespaceMap.Value.Count.Should().Be(1);
        }

        [Fact]
        public void SetValue_ShouldOverrideValue_WhenKeyIsPresentInMap()
        {
            var namespaceMap = new NamespaceMap();
            namespaceMap.SetValue("ok", "new_ok");

            namespaceMap.SetValue("ok", "new_ok");

            namespaceMap.TryGetValue("ok", out var value).Should().BeTrue();
            value.Should().Be("new_ok");
            namespaceMap.Value.Count.Should().Be(1);
        }

        [Theory]
        [InlineData("namespace","new_namespace","namespace.folder.type_namespace", "new_namespace.folder.type_namespace")]
        [InlineData("namespace", "new_namespace", "namespace.folder.type", "new_namespace.folder.type")]
        [InlineData("namespace", "new_namespace", "not_namespace.folder.type", "not_namespace.folder.type")]
        [InlineData("namespace.folder.type", "new_namespace.folder.type", "namespace", "namespace")]
        public void SetValue_ShouldOverrideUpdateAllValues_WhenNewValueIsAPartOfTheirNamespace(string partValue, string newPartValue, string itemValue, string newItemValue)
        {
            var namespaceMap = new NamespaceMap();
            namespaceMap.SetValue(partValue, partValue);
            namespaceMap.SetValue(itemValue, itemValue);

            namespaceMap.SetValue(partValue, newPartValue);

            namespaceMap.TryGetValue(partValue, out string value).Should().BeTrue();
            value.Should().Be(newPartValue);
            namespaceMap.TryGetValue(itemValue, out value).Should().BeTrue();
            value.Should().Be(newItemValue);
            namespaceMap.Value.Count.Should().Be(2);
        }
    }
}