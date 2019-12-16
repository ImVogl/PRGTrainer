namespace PRGTrainer.Core.Tests
{
    using System.IO;
    using System.Linq;
    using System.Xml.Schema;
    using NUnit.Framework;
    using ReferenceBookReaders;

    [TestFixture]
    public class ReferenceBookReaderTests
    {

        private static readonly string RootFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestItems", @"ReferenceBook");
        private readonly IReferenceBookReader _reader = new FileReferenceBookReader();

        [Test, Description(@"Чтение корректного файла со справочником.")]
        public void CorrectReferenceBookReadTest()
        {
            CopyTargetBase(@"Correct");
            var referenceBook = _reader.Read().ToList();

            Assert.That(referenceBook, Is.Not.Null);
            Assert.That(referenceBook.Count, Is.EqualTo(2));
            Assert.That(referenceBook[0].Name, Is.EqualTo(@"Первый раздел"));
            Assert.That(referenceBook[0].Content, Is.Null);
            Assert.That(referenceBook[0].SubParts, Is.Not.Null);
            Assert.That(referenceBook[0].SubParts, Is.Not.Empty);
            Assert.That(referenceBook[0].SubParts.Count(), Is.EqualTo(2));
            Assert.That(referenceBook[0].ParentIdentifier, Is.EqualTo(0));

            var firstSubParts = referenceBook[0].SubParts.ToList();
            Assert.That(firstSubParts[0], Is.Not.Null);
            Assert.That(firstSubParts[0].Name, Is.EqualTo(@"Первый подраздел"));
            Assert.That(firstSubParts[0].Content, Is.EqualTo(@"Содержимое раздела."));
            Assert.That(firstSubParts[0].SubParts, Is.Empty);
            Assert.That(firstSubParts[0].ParentIdentifier, Is.EqualTo(referenceBook[0].Identifier));

            Assert.That(firstSubParts[1], Is.Not.Null);
            Assert.That(firstSubParts[1].Name, Is.EqualTo(@"Второй подраздел"));
            Assert.That(firstSubParts[1].Content, Is.Null);
            Assert.That(firstSubParts[1].SubParts, Is.Not.Null);
            Assert.That(firstSubParts[1].SubParts, Is.Not.Empty);
            Assert.That(firstSubParts[1].SubParts.Count(), Is.EqualTo(1));
            Assert.That(firstSubParts[1].ParentIdentifier, Is.EqualTo(referenceBook[0].Identifier));

            var firstSubSubParts = firstSubParts[1].SubParts.ToList();
            Assert.That(firstSubSubParts[0].Content, Is.Not.Null);
            Assert.That(firstSubSubParts[0].Content, Is.EqualTo(@"Содержимое параграфа."));
            Assert.That(firstSubSubParts[0].FilePath, Is.Null);
            Assert.That(firstSubSubParts[0].Name, Is.Not.Null);
            Assert.That(firstSubSubParts[0].Name, Is.EqualTo(@"Первый параграф"));
            Assert.That(firstSubSubParts[0].ParentIdentifier, Is.EqualTo(firstSubParts[1].Identifier));

            Assert.That(referenceBook[1].Name, Is.EqualTo(@"Второй раздел"));
            Assert.That(referenceBook[1].Content, Is.EqualTo(@"Содержимое корневого раздела."));
            Assert.That(referenceBook[1].SubParts, Is.Not.Null);
            Assert.That(referenceBook[1].SubParts, Is.Not.Empty);
            Assert.That(referenceBook[1].SubParts.Count(), Is.EqualTo(1));
            Assert.That(referenceBook[1].ParentIdentifier, Is.EqualTo(0));

            var secondSubParts = referenceBook[1].SubParts.ToList();
            Assert.That(secondSubParts[0].Name, Is.EqualTo(@"Первый подраздел"));
            Assert.That(secondSubParts[0].Content, Is.Not.Null);
            Assert.That(secondSubParts[0].Content, Is.EqualTo(@"Содержимое раздела."));
            Assert.That(secondSubParts[0].FilePath, Is.Not.Null);
            Assert.That(secondSubParts[0].FilePath, Is.EqualTo(Path.Combine(RootFolder, @"Correct\ManualCount.png")));
            Assert.That(secondSubParts[0].SubParts, Is.Not.Null);
            Assert.That(secondSubParts[0].SubParts, Is.Empty);
            Assert.That(secondSubParts[0].ParentIdentifier, Is.EqualTo(referenceBook[1].Identifier));
        }

        [Test, Description(@"Раздел не имеет содержимого.")]
        public void EmptyPartTest()
        {
            CopyTargetBase(@"EmptyPart");
            Assert.Throws(typeof(XmlSchemaException), () => _reader.Read().ToList());
        }

        [Test, Description(@"Атрибут Name отсутствует.")]
        public void NoNameAttributeTest()
        {
            CopyTargetBase(@"NoNameAttribute");
            Assert.Throws(typeof(XmlSchemaException), () => _reader.Read().ToList());
        }

        [Test, Description(@"Атрибут Name пустой.")]
        public void EmptyNameAttribute()
        {
            CopyTargetBase(@"EmptyNameAttribute");
            Assert.Throws(typeof(XmlSchemaException), () => _reader.Read().ToList());
        }

        /// <summary>
        /// Копирование файла со справочником.
        /// </summary>
        /// <param name="sourceFolderName">Промежуточная папка.</param>
        private void CopyTargetBase(string sourceFolderName)
        {
            var targetPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"ReferenceBook.xml");
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            var sourcePath = Path.Combine(RootFolder, sourceFolderName, @"ReferenceBook.xml");
            File.Copy(sourcePath, targetPath);
        }
    }
}