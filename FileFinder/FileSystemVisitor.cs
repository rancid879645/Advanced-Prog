namespace FileFinder
{
    public class FileSystemVisitor
    {
        private readonly Func<string, bool> _filter;
        private readonly string _rootPath;
        private bool _stopSearch= false;

        public FileSystemVisitor(string rootPath, Func<string,bool> filter)
        {
            this._rootPath = rootPath;
            this._filter = filter;
        }

        public event EventHandler SearchStarted;
        public event EventHandler SearchFinished;
        public event EventHandler<IsFoundEventArgs> FileFound;
        public event EventHandler<IsFoundEventArgs> DirectoryFound;
        public event EventHandler FilteredFileFound;
        public event EventHandler FilteredDirectoryFound;

        public IEnumerable<string> Traverse()
        {
            var result = Traverse(_rootPath);
            return result;
        }

        public IEnumerable<string> Traverse(string folder)
        {
            OnSearchStarted();
            foreach (var subFolder in Directory.GetDirectories(folder))
            {
                OnDirectoryFound(subFolder);
                if (_stopSearch)
                    break;
                
                if (_filter == null || _filter(subFolder))
                {
                    OnFilteredDirectoryFound();
                    yield return subFolder;
                }

                foreach (var item in Traverse(subFolder))
                    yield return item;
            }

            _stopSearch = false;
            foreach (var file in Directory.GetFiles(folder))
            {
                OnFileFound(file);
                if (_stopSearch)
                    break;

                if (_filter != null && !_filter(file)) continue;
                OnFilteredFileFound();
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

        private void OnFileFound(string fileName)
        {
            var eventArgs = new IsFoundEventArgs
            {
                Name = fileName
            };
            FileFound?.Invoke(this, eventArgs);
            if (eventArgs.IsAbortSearch)
                _stopSearch = true;
        }

        private void OnDirectoryFound(string directoryName)
        {
            var eventArgs = new IsFoundEventArgs
            {
                Name = directoryName
            };
            DirectoryFound?.Invoke(this, eventArgs);
            if (eventArgs.IsAbortSearch)
                _stopSearch = true;
        }

        private void OnFilteredFileFound()
        {
            FilteredFileFound?.Invoke(this, EventArgs.Empty);
        }

        private void OnFilteredDirectoryFound()
        {
            FilteredDirectoryFound?.Invoke(this, EventArgs.Empty);
        }


    }
}
