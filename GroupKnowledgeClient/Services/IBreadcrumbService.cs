using MudBlazor;

namespace GroupKnowledgeClient.Services
{
    public interface IBreadcrumbService
    {
        List<BreadcrumbItem> Crumbs { get; }

        event Action? BreadcrumbsChanged;

        void Push(string name, string page);

        void Pop();
    }
}