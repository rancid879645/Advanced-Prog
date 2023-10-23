namespace FileFinder
{
    public class FileSystemVisitor
    {
        private readonly string _rootPath;
        public delegate void SearchStartedEventHandler(object sender, EventArgs e);
        public delegate void SearchFinishedEventHandler(object sender, EventArgs e);

        public FileSystemVisitor(string rootPath)
        {
            this._rootPath = rootPath;
        }

        public event SearchStartedEventHandler SearchStarted;
        public event SearchFinishedEventHandler SearchFinished;

        public IEnumerable<string> FindFile(string fileName = "")
        {
            var fullTree = GetFullTree();
            OnSearchStarted();
            foreach (var directory in fullTree.Where(directory => directory.ToUpper().Contains(fileName.ToUpper())))
            {
                yield return directory;
            }
            OnSearchFinished();
        }

        public void PrintTree(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                Console.WriteLine(directory);
            }
        }

        public List<string> GetFullTree()
        {
            return Directory.GetFileSystemEntries(_rootPath, "*", SearchOption.AllDirectories).ToList();
        }

        protected virtual void OnSearchStarted()
        {
            SearchStarted?.Invoke(this,EventArgs.Empty);
        }

        protected virtual void OnSearchFinished()
        {
            SearchFinished?.Invoke(this,EventArgs.Empty);
        }



    }
}
