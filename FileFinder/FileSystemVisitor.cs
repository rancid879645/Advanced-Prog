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
        public event EventHandler<IsFoundArgs> FileFound;

        public IEnumerable<string> Traverse()
        {
            var result = Traverse(_rootPath);
            return result;
        }

        public IEnumerable<string> Traverse(string folder)
        {
            foreach (var subFolder in Directory.GetDirectories(folder))
            {
                if (_filter == null || _filter(subFolder))
                    yield return subFolder;

                foreach (var item in Traverse(subFolder))
                    yield return item;
            }

            foreach (var file in Directory.GetFiles(folder))
            {
                if (_stopSearch)
                    break;

                if (_filter != null && !_filter(file)) continue;
                OnFileFound(file);
                yield return file;


            }
        }

        public void PrintTree(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                Console.WriteLine($"    {directory}");
            }
        }

        public void OnSearchStarted()
        {
            SearchStarted?.Invoke(this,EventArgs.Empty);
        }

        public void OnSearchFinished()
        {
            SearchFinished?.Invoke(this,EventArgs.Empty);
        }

        public void OnFileFound(string fileName)
        {
            var eventArgs = new IsFoundArgs();
            eventArgs.Name = fileName;
            FileFound?.Invoke(this, eventArgs);
            if (eventArgs.IsAbortSearch)
                _stopSearch = true;
        }



    }
}
