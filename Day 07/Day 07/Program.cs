using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace Day_07
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fileSystem = new FileSystem();
            fileSystem.Read("input.txt");

            var accumulationVisitor = new AccumulateSizeOfFolderVisitor(100000);
            fileSystem.TraverseAllFiles(accumulationVisitor);
            Console.WriteLine($"Solution Part 1: {accumulationVisitor.AccumulatedSize}");

            var bestMatchVisitor = new FindSmallestFolderAboveSizeVisitor(30000000 - (70000000 - fileSystem.TotalSize));
            fileSystem.TraverseAllFiles(bestMatchVisitor);
            Console.WriteLine($"Solution Part 2: {bestMatchVisitor.bestMatch}");
            var result = 0;
        }
        
    }

    internal class FileSystem
    {
        public int TotalSize => rootFolder.TotalSize;

        private readonly FileSystemFolder rootFolder = new FileSystemFolder()
        {
            Name = "/",
            Parent = null
        };

        public void Read(string inputFileName)
        {
            var currentFolder = rootFolder;
            var lines = File.ReadAllLines(inputFileName);
            foreach (var line in lines.Select(x => new InputLine(x)))
            {
                if (line is { _tokens: ["$", "cd", "/"] })
                {
                    currentFolder = rootFolder;
                    continue;
                }

                if (line is { _tokens: ["$", "cd", ".."] })
                {
                    currentFolder = currentFolder.Parent;
                    continue;
                }


                if (line is { _tokens: ["$", "cd", var subFolder] })
                {
                    currentFolder = currentFolder.SubFolders.Find(x => x.Name == subFolder);
                }

                if (line is { _tokens: ["dir", var folder] })
                {
                    currentFolder.SubFolders.Add(new FileSystemFolder() { Name = folder, Parent = currentFolder });
                }

                if (line is { _tokens: [var size, var fileName] } && int.TryParse(size, out var intSize))
                {
                    currentFolder.SubFiles.Add(new FileSystemFile()
                    {
                        Name = fileName,
                        TotalSize = intSize
                    });
                }
            }

            this.UpdateFolderSize(rootFolder);
        }

        private void UpdateFolderSize(FileSystemFolder folder)
        {
            folder.FolderSize = folder.SubFiles.Select(x => x.TotalSize).Sum();

            foreach (var subFolder in folder.SubFolders)
            {
                UpdateFolderSize(subFolder);
            }

            folder.TotalSize = folder.SubFolders.Select(x => x.TotalSize).Sum() + folder.FolderSize;
        }

        public void TraverseAllFiles(IFolderVisitor visitor)
        {
            TraverseAllFiles(visitor, rootFolder);
        }
        
        private void TraverseAllFiles(IFolderVisitor visitor, FileSystemFolder subFolder)
        {
            visitor.Visit(subFolder);
            foreach (var fileSystemFolder in subFolder.SubFolders)
            {
                TraverseAllFiles(visitor, fileSystemFolder);
            }
        }

    }

    internal interface IFolderVisitor
    {
        void Visit(FileSystemFolder folder);
    }

    internal class AccumulateSizeOfFolderVisitor : IFolderVisitor
    {
        public int AccumulatedSize = 0;
        private readonly int _maxSize;

        public AccumulateSizeOfFolderVisitor(int maxSize)
        {
            _maxSize = maxSize;
        }

        public void Visit(FileSystemFolder folder)
        {
            if (folder.TotalSize <= _maxSize)
            {
                AccumulatedSize += folder.TotalSize;
            }
        }
    }

    internal class FindSmallestFolderAboveSizeVisitor : IFolderVisitor
    {
        private readonly int _minRequiredSpace;
        public int bestMatch = int.MaxValue;

        public FindSmallestFolderAboveSizeVisitor(int minRequiredSpace)
        {
            _minRequiredSpace = minRequiredSpace;
        }

        public void Visit(FileSystemFolder folder)
        {
            if (folder.TotalSize >= _minRequiredSpace)
            {
                bestMatch = Math.Min(bestMatch, folder.TotalSize);
            }
        }
    }
    
    internal class InputLine
    {
        private readonly string _line;
        public readonly IReadOnlyList<string> _tokens;

        public InputLine(string line)
        {
            _line = line;
            _tokens = line.Split(" ");
        }

    }
    
    internal class FileSystemFolder
    {
        public FileSystemFolder Parent { get; init; }

        public string Name { get; init; }

        public int TotalSize { get; set; }

        public int FolderSize { get; set; }

        public List<FileSystemFolder> SubFolders { get; } = new();
        public List<FileSystemFile> SubFiles { get; } = new();
    }

    internal class FileSystemFile
    {
        public string Name { get; init; }

        public int TotalSize { get; set; }
    }
}