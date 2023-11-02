namespace FileFinder
{
    public sealed class FileSystemVisitor
    {
        private readonly Func<string, bool> _filter;

        private readonly string _rootPath;

        public FileSystemVisitor(string rootPath, Func<string,bool> filter)
        {
            _rootPath = rootPath;
            _filter = filter;
        }   

        public event EventHandler? SearchStarted;

        public event EventHandler? SearchFinished;

        public event EventHandler<IsFoundEventArgs>? FileFound;

        public event EventHandler<IsFoundEventArgs>? DirectoryFound;

        public event EventHandler<IsFilteredEventArgs>? FilteredFileFound;

        public event EventHandler<IsFilteredEventArgs>? FilteredDirectoryFound;

        public IEnumerable<string> Traverse() => Traverse(_rootPath);

        public IEnumerable<string> Traverse(string folder)
        {
            OnSearchStarted();

            foreach (var subFolder in Directory.GetDirectories(folder))
            {
                OnDirectoryFound(subFolder, out var abortSearch);

                if (abortSearch)
                {
                    break;
                }
                
                if (_filter == null || _filter(subFolder))
                {
                    OnFilteredDirectoryFound(subFolder, out var excludeFromResult);

                    if (excludeFromResult)
                    {
                        continue;
                    }

                    yield return subFolder;
                }

                foreach (var item in Traverse(subFolder))
                {
                    yield return item;
                }
            }

            foreach (var file in Directory.GetFiles(folder))
            {
                OnFileFound(file, out var abortSearch);

                if (abortSearch)
                {
                    break;
                }

                if (_filter != null && !_filter(file))
                {
                    continue;
                }

                OnFilteredFileFound(file, out var excludeFromResult);

                if (excludeFromResult)
                {
                    continue;
                }

                yield return file;
            }

            OnSearchFinished();
        }

        public void PrintTree(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                Console.WriteLine($"    {directory}");
            }
        }

        private void OnSearchStarted()
        {
            SearchStarted?.Invoke(this,EventArgs.Empty);
        }

        private void OnSearchFinished()
        {
            SearchFinished?.Invoke(this,EventArgs.Empty);
        }

        private void OnFileFound(string fileName, out bool abortSearch)
        {
            var eventArgs = new IsFoundEventArgs
            {
                Name = fileName
            };

            FileFound?.Invoke(this, eventArgs);

            abortSearch = eventArgs.IsAbortSearch;
        }

        private void OnDirectoryFound(string directoryName, out bool abortSearch)
        {
            var eventArgs = new IsFoundEventArgs
            {
                Name = directoryName
            };

            DirectoryFound?.Invoke(this, eventArgs);

            abortSearch = eventArgs.IsAbortSearch;
        }

        private void OnFilteredFileFound(string fileName, out bool excludeFromResult)
        {
            var eventArgs = new IsFilteredEventArgs
            {
                Name = fileName
            };

            FilteredFileFound?.Invoke(this, eventArgs);

            excludeFromResult = eventArgs.IsExcludeFromResult;
        }

        private void OnFilteredDirectoryFound(string directoryName, out bool excludeFromResult)
        {
            var eventArgs = new IsFilteredEventArgs
            {
                Name = directoryName
            };

            FilteredDirectoryFound?.Invoke(this, eventArgs);

            excludeFromResult = eventArgs.IsExcludeFromResult;
        }
    }
}
