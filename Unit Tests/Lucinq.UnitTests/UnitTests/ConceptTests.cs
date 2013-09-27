using System;
using System.Diagnostics;
using System.IO;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NUnit.Framework;

namespace Lucinq.UnitTests.UnitTests
{
    [TestFixture]
    public class ConceptTests
    {
        [Test]
        public void OpeningClosingObjects()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Opening FS Dir");
            FSDirectory fileSystemDirectory = FSDirectory.Open(new DirectoryInfo(GeneralConstants.Paths.BBCIndex));
            WriteTime(stopwatch);
            Console.WriteLine("Opening Ram Dir");
            RAMDirectory ramDirectory = new RAMDirectory(fileSystemDirectory);
            WriteTime(stopwatch);
            ramDirectory.Dispose();
            WriteTime(stopwatch);
            Console.WriteLine("Disposed Ram Dir");
            fileSystemDirectory.Dispose();
            WriteTime(stopwatch);
            Console.WriteLine("Disposed FS Dir");
            stopwatch.Stop();
        }

        [Test]
        public void OpenCloseOpenClose()
        {
            OpeningClosingObjects();
            OpeningClosingObjects();
            OpeningClosingObjects();
            OpeningClosingObjects();
            OpeningClosingObjects();
            OpeningClosingObjects();
            OpeningClosingObjects();
            OpeningClosingObjects();
            OpeningClosingObjects();
        }

        private void WriteTime(Stopwatch stopwatch)
        {
            Console.Write(stopwatch.ElapsedTicks + " - ");
            Console.Write(stopwatch.ElapsedMilliseconds + "\r\n");
        }
    }
}
