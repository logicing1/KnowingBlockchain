using MudBlazor;

namespace GroupKnowledgeClient.Services.Default
{
    public class BreadcrumbService : IBreadcrumbService
    {
        private readonly Stack<BreadcrumbItem> _breadcrumbs = new();

        public event Action? BreadcrumbsChanged;

        public List<BreadcrumbItem> Crumbs => _breadcrumbs.Reverse().ToList();

        public void Push(string name, string page)
        {
            var crumb = new BreadcrumbItem(name, page);
            _breadcrumbs.Push(crumb);
            NotifyChanged();
        }

        public void Pop()
        {
            _breadcrumbs.Pop();
            NotifyChanged();
        }

        private void NotifyChanged() => BreadcrumbsChanged?.Invoke();


    }
}
