namespace PRGTrainer.Core.Tests
{
    using System.Collections.Generic;
    using Model.ReferenceBook;
    using Moq;
    using NUnit.Framework;
    using ReferenceBookReaders;
    using ReferenceBookStorage;

    [TestFixture]
    public class ReferenceBookStorageTests
    {
        private static readonly List<ReferenceBookPart> ReferenceBook = new List<ReferenceBookPart>
        {
            new ReferenceBookPart
            {
                Identifier = 1, ParentIdentifier = 0, Name = "Root 1",
                SubParts = new List<ReferenceBookPart>
                {
                    new ReferenceBookPart
                    {
                        Identifier = 3, ParentIdentifier = 1, SubParts = new List<ReferenceBookPart>(),
                        Content = @"Part 1 content", Name = @"Part 1"
                    },

                    new ReferenceBookPart
                    {
                        Identifier = 4, ParentIdentifier = 1, SubParts = new List<ReferenceBookPart>(),
                        Content = @"Part 2 content", Name = @"Part 2"
                    }
                }
            },

            new ReferenceBookPart
            {
                Identifier = 2, ParentIdentifier = 0, Name = @"Root 2", Content = @"Content 1",
                SubParts = new List<ReferenceBookPart>()
            }
        };

        private Mock<IReferenceBookReader> _readerMock;
        private IReferenceBookStorage _referenceBookStorage;

        [SetUp]
        public void SetUp()
        {
            _readerMock = new Mock<IReferenceBookReader>();
            _readerMock.Setup(c => c.Read()).Returns(ReferenceBook);

            _referenceBookStorage = new ReferenceBookStorage(_readerMock.Object);
        }

        [Test, Description(@"Проверка разворачивания дерева справочника.")]
        public void ExpandTreeTest()
        {
            _referenceBookStorage.FillStorage();
            Assert.That(_referenceBookStorage.GetPartById(1).Name, Is.EqualTo("Root 1"));
            Assert.That(_referenceBookStorage.GetPartById(2).Name, Is.EqualTo("Root 2"));
            Assert.That(_referenceBookStorage.GetPartById(3).Name, Is.EqualTo("Part 1"));
            Assert.That(_referenceBookStorage.GetPartById(4).Name, Is.EqualTo("Part 2"));
        }
    }
}