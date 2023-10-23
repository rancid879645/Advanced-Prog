namespace FileFinder
{
    public class FileSystemVisitor
    {
        private readonly Func<string, bool> filter;
        private readonly string _rootPath;
        public delegate void SearchStartedEventHandler(object sender, EventArgs e);
        public delegate void SearchFinishedEventHandler(object sender, EventArgs e);

        public FileSystemVisitor(string rootPath, Func<string,bool> filter)
        {
            this._rootPath = rootPath;
            this.filter = filter;
        }

        public event SearchStartedEventHandler SearchStarted;
        public event SearchFinishedEventHandler SearchFinished;

        public IEnumerable<string> FindFile()
        {
            OnSearchStarted();
            var result =FindFile(_rootPath);
            OnSearchFinished();
            return result;
        }

        public IEnumerable<string> FindFile(string folder)
        {
            foreach (var subFolder in Directory.GetDirectories(folder))
            {
                if (filter == null || filter(subFolder))
                    yield return subFolder;

                foreach (var item in FindFile(subFolder))
                    yield return item;
            }

            foreach (var file in Directory.GetFiles(folder))
            {
                if (filter== null || filter(file))
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
